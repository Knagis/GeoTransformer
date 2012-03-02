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

namespace GeoTransformer.Extensions.GeocachingService
{
    internal partial class AuthenticationForm : Form
    {
        public AuthenticationForm()
        {
            if (_parseQueryString == null)
            {
                var p1 = System.Linq.Expressions.Expression.Parameter(typeof(string), "query");
                var t = Type.GetType("System.UriTemplateHelpers, " + typeof(System.UriTemplate).Assembly.FullName, true);
                var c1 = System.Linq.Expressions.Expression.Call(t, "ParseQueryString", Type.EmptyTypes, p1);
                var l1 = System.Linq.Expressions.Expression.Lambda<Func<string, System.Collections.Specialized.NameValueCollection>>(c1, p1);
                _parseQueryString = l1.Compile();
            }

            InitializeComponent();
        }

        private static Func<string, System.Collections.Specialized.NameValueCollection> _parseQueryString;

        /// <summary>
        /// Gets the access token (is set after the user successfully authenticates).
        /// </summary>
        public string AccessToken
        {
            get;
            private set;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            this.webBrowser.Navigate(Configuration.AuthenticationAddress);
            this.webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
        }

        void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var uri = e.Url;

            if (uri.GetLeftPart(UriPartial.Path).EndsWith(Configuration.AuthenticationAddress, StringComparison.OrdinalIgnoreCase))
            {
                var qs = _parseQueryString(uri.Query);
                if (string.IsNullOrEmpty(qs["accessToken"]))
                {
                    MessageBox.Show("Unable to authenticate with the Live API.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    return;
                }
                else
                {
                    this.AccessToken = qs["accessToken"];
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }

                this.Close();
                return;
            }

            this.webBrowser.PreviewKeyDown += new PreviewKeyDownEventHandler(webBrowser_PreviewKeyDown);
        }

        void webBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 27)
                this.toolStripClose.PerformClick();
        }

        private void toolStripClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void Form_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
                this.toolStripClose.PerformClick();
        }
    }
}
