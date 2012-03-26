/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoTransformer.Transformers.LoadGpxTautai
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class CaptchaForm : Form
    {
        public Dictionary<string, string> FieldValues { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptchaForm"/> class.
        /// </summary>
        /// <param name="htmlScript">The HTML script that will be displayed on the form.</param>
        public CaptchaForm(string htmlScript)
        {
            this.FieldValues = new Dictionary<string, string>();

            InitializeComponent();

            var bc = this.BackColor.R.ToString("X2") + this.BackColor.G.ToString("X2") + this.BackColor.B.ToString("X2");
            this.webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
            this.webBrowser.ObjectForScripting = this;
            this.webBrowser.DocumentText = "<html><head><style type=\"text/css\">body { background-color: #" + bc + " }</style></head><body onkeypress=\"window.external.BrowserKeyPress(event.keyCode)\">" + htmlScript + "</body></html>";
        }

        void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.webBrowser.Focus();
            var x = this.webBrowser.Document.GetElementsByTagName("input").Cast<HtmlElement>().FirstOrDefault(o => !string.Equals(o.GetAttribute("type"), "hidden", StringComparison.OrdinalIgnoreCase));
            if (x != null)
                x.Focus();
        }

        public void BrowserKeyPress(int keyCode)
        {
            this.CaptchaForm_KeyPress(this.webBrowser, new KeyPressEventArgs((char)keyCode));
        }

        private void toolStripClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void CaptchaForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                this.toolStripOK.PerformClick();
            }
            else if (e.KeyChar == (char)27)
            {
                this.toolStripClose.PerformClick();
            }
        }

        private void toolStripOK_Click(object sender, EventArgs e)
        {
            var inputs = this.webBrowser.Document.GetElementsByTagName("input").Cast<HtmlElement>();
            foreach (var i in inputs)
                this.FieldValues[i.GetAttribute("name")] = i.GetAttribute("value");

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
