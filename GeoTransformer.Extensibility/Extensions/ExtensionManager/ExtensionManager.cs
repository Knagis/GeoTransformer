/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Extensions.ExtensionManager
{
    /// <summary>
    /// Extension that allows the user to download and install additional extensions from the web.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ExtensionManager : ITopLevelTabPage
    {
        /// <summary>
        /// Holds the WebBrowser control that displays the extension manager.
        /// </summary>
        private System.Windows.Forms.WebBrowser _browser;

        /// <summary>
        /// Notifies that the document template has finished loading.
        /// </summary>
        private System.Threading.ManualResetEventSlim _documentLoaded = new System.Threading.ManualResetEventSlim(false);

        /// <summary>
        /// The extensions available for download.
        /// </summary>
        private static List<ExtensionData> _extensions;

        /// <summary>
        /// The synchronization target.
        /// </summary>
        private static object _syncRoot = new object();

        /// <summary>
        /// Gets a collection with all available extensions and their most recent versions.
        /// </summary>
        public static System.Collections.ObjectModel.ReadOnlyCollection<ExtensionData> Extensions
        {
            get
            {
                if (_extensions == null)
                    LoadExtensions();

                return _extensions.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the image to be displayed on the button.
        /// </summary>
        /// <value>The tab page image.</value>
        public System.Drawing.Image TabPageImage
        {
            get { return Resources.Icon; }
        }

        /// <summary>
        /// Gets the text to be displayed on the button that opens the tab page.
        /// </summary>
        /// <value>The tab page title.</value>
        public string TabPageTitle
        {
            get { return "Extensions"; }
        }

        /// <summary>
        /// Creates the control that will be displayed in the tab page. The method is called only once and after that the control is reused.
        /// </summary>
        /// <returns>An initialized control that is displayed in the tab page.</returns>
        public System.Windows.Forms.Control Initialize()
        {
            new System.Threading.Thread(LoadAndDisplayExtensions).Start();

            var browser = new System.Windows.Forms.WebBrowser();
            browser.AllowNavigation = false;
            browser.IsWebBrowserContextMenuEnabled = false;
            browser.WebBrowserShortcutsEnabled = false;
            browser.ObjectForScripting = this;
            browser.DocumentCompleted += (a, e) => { this._documentLoaded.Set(); };
            browser.DocumentText = Resources.Template;
            return this._browser = browser;
        }

        public bool CancelUninstallExtension(int id)
        {
            var ex = _extensions[id];

            var path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Extensions", ex.AssemblyName);
            if (!System.IO.Directory.Exists(path))
            {
                System.Windows.Forms.MessageBox.Show("Unable to find the extension folder.", "Extension uninstall", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            var filepath = System.IO.Path.Combine(path, "pending_uninstall");
            System.IO.File.Delete(filepath);

            return true;
        }

        public bool UninstallExtension(int id)
        {
            var ex = _extensions[id];

            var path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Extensions", ex.AssemblyName);
            if (!System.IO.Directory.Exists(path))
            {
                System.Windows.Forms.MessageBox.Show("Unable to find the extension folder.", "Extension uninstall", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            var filepath = System.IO.Path.Combine(path, "pending_uninstall");
            System.IO.File.WriteAllText(filepath, string.Empty);

            System.Windows.Forms.MessageBox.Show("The extension is marked for removal. It will be uninstalled when you restart GeoTransformer.", "Extension uninstall", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

#warning The uninstall process currently does not remove data from ExtensionData folder.

            return true;
        }

        public bool InstallExtension(int id)
        {
            var ex = _extensions[id];

            var path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Extensions", ex.AssemblyName);
            if (System.IO.Directory.Exists(path))
            {
                var clean = System.Windows.Forms.MessageBox.Show("The extension is already installed or the previous installation failed." + Environment.NewLine + Environment.NewLine + "Do you want to try to delete the existing files and install again?", "Extension install", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button2);
                if (clean != System.Windows.Forms.DialogResult.Yes)
                    return false;

                try
                {
                    System.IO.Directory.Delete(path, true);
                }
                catch (System.IO.IOException iex)
                {
                    System.Windows.Forms.MessageBox.Show("Unable to delete the extension folder (" + iex.Message + "). It will be removed when you restart GeoTransformer.", "Extension install", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    System.IO.File.WriteAllText(System.IO.Path.Combine(path, "pending_uninstall"), string.Empty);
                    return false;
                }
            }

            System.IO.Directory.CreateDirectory(path);

            new System.Threading.Thread(() => this.InstallExtensionWorker(id, ex)).Start();

            return true;
        }

        private void InstallExtensionWorker(int id, ExtensionData extension)
        {
            var path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Extensions", extension.AssemblyName);
            try
            {
                var archive = System.IO.Path.Combine(path, "extension.zip");

                using (var wc = new System.Net.WebClient())
                    wc.DownloadFile(extension.DownloadUri, archive);

                new ICSharpCode.SharpZipLib.Zip.FastZip().ExtractZip(archive, path, null);

                System.IO.File.Delete(archive);

                this.ExecuteScript("installComplete(" + id.ToString(System.Globalization.CultureInfo.InvariantCulture) + ");");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable to install the extension " + extension.Title + "." + Environment.NewLine + Environment.NewLine + ex.Message, "Extension install", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                try
                {
                    System.IO.Directory.Delete(path, true);
                }
                catch (System.IO.IOException)
                {
                    System.IO.File.WriteAllText(System.IO.Path.Combine(path, "pending_uninstall"), string.Empty);
                }
            }
        }

        private static void LoadExtensions()
        {
            if (_extensions != null)
                return;

            lock (_syncRoot)
            {
                if (_extensions != null)
                    return;

                var xdoc = System.Xml.Linq.XDocument.Load("http://knagis.miga.lv/GeoTransformer/Extensions/extensions.xml");
                var d = new List<ExtensionData>();

                foreach (var e in xdoc.Root.Elements())
                {
                    d.Add(new ExtensionData(e));
                }

                _extensions = d;
            }
        }

        private void LoadAndDisplayExtensions()
        {
            try
            {
                LoadExtensions();

                int i = 0;
                foreach (var x in _extensions)
                    DisplayExtension(i++, x.Title, x.Description, x.IsInstalled);

                this.ExecuteScript("document.getElementById('loading').removeNode(true);");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while retrieving extension data:" + Environment.NewLine + Environment.NewLine + ex.ToString(), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Executes the given JavaScript code on the embedded browser. Performs invoke on the UI thread if needed.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        private void ExecuteScript(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
                return;

            this._documentLoaded.Wait();

            if (this._browser.InvokeRequired)
            {
                this._browser.Invoke(() => this.ExecuteScript(script));
                return;
            }

            var element = this._browser.Document.CreateElement("script");
            element.SetAttribute("type", "text/javascript");
            element.SetAttribute("text", script);
            this._browser.Document.Body.InsertAdjacentElement(System.Windows.Forms.HtmlElementInsertionOrientation.BeforeEnd, element);
        }

        private void DisplayExtension(int id, string title, string description, bool isInstalled)
        {
            var sb = new StringBuilder();
            sb.Append("displayExtension(");
            sb.Append(id.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(",\"");
            JavaScriptStringEncode(sb, title);
            sb.Append("\",\"");
            JavaScriptStringEncode(sb, description);
            sb.Append("\",");
            sb.Append(isInstalled.ToString().ToLowerInvariant());
            sb.Append(");");

            this.ExecuteScript(sb.ToString());
        }

        #region [ Code copied from HttpEncode class with ILSpy ]

        protected static void JavaScriptStringEncode(StringBuilder stringBuilder, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            int startIndex = 0;
            int num = 0;
            int i = 0;
            while (i < value.Length)
            {
                char c = value[i];
                if (c == '\r' || c == '\t' || c == '"' || c == '\'' || c == '<' || c == '>' || c == '\\' || c == '\n' || c == '\b' || c == '\f' || c < ' ' || c == '&')
                {
                    if (num > 0)
                    {
                        stringBuilder.Append(value, startIndex, num);
                    }
                    startIndex = i + 1;
                    num = 0;
                }
                char c2 = c;
                if (c2 <= '"')
                {
                    switch (c2)
                    {
                        case '\b':
                            stringBuilder.Append("\\b");
                            break;
                        case '\t':
                            stringBuilder.Append("\\t");
                            break;
                        case '\n':
                            stringBuilder.Append("\\n");
                            break;
                        case '\v':
                            goto IL_188;
                        case '\f':
                            stringBuilder.Append("\\f");
                            break;
                        case '\r':
                            stringBuilder.Append("\\r");
                            break;
                        default:
                            if (c2 != '"')
                            {
                                goto IL_188;
                            }
                            stringBuilder.Append("\\\"");
                            break;
                    }
                }
                else
                {
                    switch (c2)
                    {
                        case '&':
                            AppendCharAsUnicodeJavaScript(stringBuilder, c);
                            goto IL_19C;
                            num++;
                            goto IL_19C;
                        case '\'':
                            break;
                        default:
                            switch (c2)
                            {
                                case '<':
                                case '>':
                                    break;
                                case '=':
                                    goto IL_188;
                                default:
                                    if (c2 != '\\')
                                    {
                                        goto IL_188;
                                    }
                                    stringBuilder.Append("\\\\");
                                    goto IL_19C;
                            }
                            break;
                    }
                    AppendCharAsUnicodeJavaScript(stringBuilder, c);
                }
            IL_19C:
                i++;
                continue;
            IL_188:
                if (c < ' ')
                {
                    AppendCharAsUnicodeJavaScript(stringBuilder, c);
                    goto IL_19C;
                }
                num++;
                goto IL_19C;
            }
            if (num > 0)
            {
                stringBuilder.Append(value, startIndex, num);
            }
        }
        private static void AppendCharAsUnicodeJavaScript(StringBuilder builder, char c)
        {
            builder.Append("\\u");
            int num = (int)c;
            builder.Append(num.ToString("x4", System.Globalization.CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
