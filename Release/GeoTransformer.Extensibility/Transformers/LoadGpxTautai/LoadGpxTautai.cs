/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GeoTransformer.Extensions;

namespace GeoTransformer.Transformers.LoadGpxTautai
{
    public class LoadGpxTautai : TransformerBase, ITransformerWithPopup, IConfigurable, ILocalStorage
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Loading LV caches from GPX Tautai"; }
        }

        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.LoadSources; }
        }

        /// <summary>
        /// Requests the captcha to be entered by the user.
        /// </summary>
        /// <param name="script">The captcha HTML script.</param>
        /// <returns>Key-value pair of the captcha input fields.</returns>
        private Dictionary<string, string> RequestCaptcha(string script)
        {
            if (this.ParentWindow.InvokeRequired)
                return this.ParentWindow.Invoke(s => this.RequestCaptcha(s), script);

            var form = new CaptchaForm(script);
            var result = form.ShowDialog(this.ParentWindow);
            if (result != System.Windows.Forms.DialogResult.OK)
                this.ReportStatus(StatusSeverity.Error, "User cancelled from CAPTCHA window");

            return form.FieldValues;
        }

        /// <summary>
        /// Sends the initial request.
        /// </summary>
        /// <returns>The HTML code for Captcha entry</returns>
        private string SendInitialRequest()
        {
            var req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("http://stats.geoforums.lv/tools/gpxtautai.php");
            req.CookieContainer = new System.Net.CookieContainer();
            req.CookieContainer.Add(new System.Net.Cookie("settings", "act=1|dis=0|arc=0|difmin=1|difmax=5|termin=1|termax=5|trad=0|myst=0|multi=0|event=0|lfevent=1|citoevent=0|lett=0|virt=0|wherigo=0|earth=0|skipr=0|skip=|lonmin=20.96|lonmax=28.24|latmin=55.67|latmax=58.086|logitext=0|logmaxtext=5|attribs=0", "/", "stats.geoforums.lv"));

            using (var resp = (System.Net.HttpWebResponse)req.GetResponse())
            using (var stream = resp.GetResponseStream())
            using (var reader = new System.IO.StreamReader(stream))
            {
                var captchaScript = reader.ReadToEnd();
                captchaScript = captchaScript.Substring(captchaScript.IndexOf("<script type=\"text/javascript\" src=\"http://api.recaptcha.net/"));
                captchaScript = captchaScript.Substring(0, captchaScript.IndexOf("</script>") + 9);
                return captchaScript;
            }
        }

        private void AddFilterFields(Dictionary<string, string> fields)
        {
            this.Configuration.AddFilterFields(fields);
            fields.Add("latmin", "55.67");
            fields.Add("latmax", "58.086");
            fields.Add("lonmin", "20.96");
            fields.Add("lonmax", "28.24");
            fields.Add("gen", "Ģenerēt GPX");
            fields.Add("sortcol", "date");
            fields.Add("sortord", "desc");
        }

        /// <summary>
        /// Downloads the GPX from the site and saves it to a local copy.
        /// </summary>
        /// <param name="fields">The fields for the HTTP post request.</param>
        /// <returns>The path to the GPX file</returns>
        private string DownloadGpx(Dictionary<string, string> fields)
        {
            this.ReportStatus("Preparing request...");

            var req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("http://stats.geoforums.lv/tools/gpxtautai.php");
            req.Timeout = 300000; // 5 minutes
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            using (var stream = req.GetRequestStream())
            using (var writer = new System.IO.StreamWriter(stream))
            {
                bool first = true;
                foreach (var f in fields)
                {
                    if (!first) writer.Write("&");
                    first = false;
                    writer.Write(System.Uri.EscapeUriString(f.Key));
                    writer.Write("=");
                    writer.Write(System.Uri.EscapeUriString(f.Value));
                }
            }

            this.ReportStatus("Downloading the GPX data. This may take a while...");

            using (var resp = req.GetResponse())
            using (var stream = resp.GetResponseStream())
            using (var output = System.IO.File.OpenWrite(System.IO.Path.Combine(this.LocalStoragePath, "GpxTautai.zip.temp")))
            {
                int i = 0;
                byte[] buffer = new byte[4096];
                do
                {
                    i = stream.Read(buffer, 0, buffer.Length);
                    output.Write(buffer, 0, i);
                } while (i > 0);
            }

            this.ReportStatus("Download complete. Loading the data...");

            return System.IO.Path.Combine(this.LocalStoragePath, "GpxTautai.zip.temp");

        }

        /// <summary>
        /// Loads the data ands adds new documents to <paramref name="documents"/>.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            string file = null;
            string cache = System.IO.Path.Combine(this.LocalStoragePath, "GpxTautai.zip.cache");
            string cacheKey = System.IO.Path.Combine(this.LocalStoragePath, "GpxTautai.zip.cache.key");
            bool useCache = false;
            bool userOnlyLocalStorage = (options & TransformerOptions.UseLocalStorage) == TransformerOptions.UseLocalStorage;

            if (System.IO.File.Exists(cache) && System.IO.File.Exists(cacheKey))
            {
                if (userOnlyLocalStorage)
                {
                    file = cache;
                    useCache = true;
                }
                else if (DateTime.Now.Subtract(System.IO.File.GetLastWriteTime(cache)).TotalMinutes < 10)
                {
                    var oldkey = System.IO.File.ReadAllBytes(cacheKey);
                    var newkey = this.Configuration.SerializeSettings();
                    if (oldkey.SequenceEqual(newkey))
                    {
                        // lets use the cached copy in this case
                        file = cache;
                        useCache = true;
                    }
                }
            }

            if (userOnlyLocalStorage && !useCache)
            {
                this.ReportStatus(StatusSeverity.Warning, "Cached copy is not available.");
                return;
            }

            if (!useCache)
            {
                try
                {
                    var captcha = this.SendInitialRequest();
                    this.TerminateExecutionIfNeeded();
                    var fields = this.RequestCaptcha(captcha);
                    this.TerminateExecutionIfNeeded();
                    this.AddFilterFields(fields);
                    file = this.DownloadGpx(fields);
                    this.TerminateExecutionIfNeeded();
                }
                catch (Exception ex)
                {
                    if (!System.IO.File.Exists(cache))
                        this.ReportStatus(StatusSeverity.Error, "Unable to download the GPX data: " + ex.Message);
                    else
                        this.ReportStatus(StatusSeverity.Warning, "Using the cached copy because unable to download a new copy: " + ex.Message);

                    file = cache;
                    useCache = true;
                }
            }

            int wptCount = 0;
            try
            {
                foreach (var gpx in Gpx.Loader.Zip(file))
                {
                    wptCount += gpx.Waypoints.Count(o => o.Geocache.IsDefined());
                    documents.Add(gpx);
                }

                if (useCache)
                    this.ReportStatus("Using file downloaded {0:0.0} minutes ago - {1} caches loaded.", DateTime.Now.Subtract(System.IO.File.GetLastWriteTime(cache)).TotalMinutes, wptCount);
                else
                    this.ReportStatus("Download complete - " + wptCount + " caches loaded.");
            }
            catch
            {
                if (useCache)
                {
                    // this is an attempt to recover from a broken file in the cache.
                    System.IO.File.Delete(cache);
                    this.Process(documents, options);
                    return;
                }

                this.ReportStatus(StatusSeverity.Error, "Could not download valid GPX file. Please try again.");
            }

            if (!useCache)
            {
                System.IO.File.Delete(cache);
                System.IO.File.Move(file, cache);
                System.IO.File.WriteAllBytes(cacheKey, this.Configuration.SerializeSettings());
            }
        }

        #region [ IConfigurable ]

        private GpxTautaiOptions Configuration;

        /// <summary>
        /// Initializes the transformer with the specified current configuration (can be <c>null</c> if the transformer is initialized for the very first time) and
        /// returns the configuration UI control.
        /// </summary>
        /// <param name="currentConfiguration">The current configuration.</param>
        /// <returns>
        /// The configuration UI control.
        /// </returns>
        public System.Windows.Forms.Control Initialize(byte[] currentConfiguration)
        {
            this.Configuration = new GpxTautaiOptions();
            this.Configuration.DeserializeSettings(currentConfiguration);
            return this.Configuration;
        }

        /// <summary>
        /// Retrieves the configuration from the transformer's configuration UI control.
        /// </summary>
        /// <returns>
        /// The serialized configuration data.
        /// </returns>
        public byte[] SerializeConfiguration()
        {
            return this.Configuration.SerializeSettings();
        }

        /// <summary>
        /// Gets a value indicating whether the user has enabled this transformer.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this transformer is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return this.Configuration.GpxTautaiEnabled; }
        }

        Category IHasCategory.Category { get { return new Category(Category.GeocacheSources.ConfigurationOrder, "Geocache source - GPX Tautai"); } }

        #endregion

        #region [ ILocalStorage ]

        /// <summary>
        /// Gets or sets the local storage path where the extension can store its cache if needed. The value is set by the main engine once the extension instance is created.
        /// </summary>
        public string LocalStoragePath
        {
            get;
            set;
        }

        #endregion

        #region [ ITransformerWithPopup ]

        /// <summary>
        /// Sets the parent window that should be used if the transformer has to show a dialog user interface.
        /// </summary>
        public System.Windows.Forms.Form ParentWindow
        {
            get;
            set;
        }

        #endregion
    }
}
