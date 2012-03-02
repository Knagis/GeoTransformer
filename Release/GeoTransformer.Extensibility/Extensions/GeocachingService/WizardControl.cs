/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoTransformer.Extensions.GeocachingService
{
    public partial class WizardControl : UserControl
    {
        private ConfigurationControl _configControl;

        public WizardControl()
        {
            InitializeComponent();

            try
            {
                var ext = GeoTransformer.Extensions.ExtensionLoader.RetrieveExtensions<Extensions.GeocachingService.GeocachingService>().FirstOrDefault();

                this._configControl = ext.ConfigurationControl;

                if (GeoTransformer.GeocachingService.LiveClient.IsEnabled)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(this.RetrieveProfile);
                }
            }
            catch
            {
            }
        }

        private void OpenAuthenticationForm()
        {
            var dlg = new AuthenticationForm();
            var result = dlg.ShowDialog();
            if (result != DialogResult.OK || string.IsNullOrEmpty(dlg.AccessToken))
            {
                this.Disable();
                return;
            }

            this._configControl.AccessToken = dlg.AccessToken;
            System.Threading.ThreadPool.QueueUserWorkItem(this.RetrieveProfile);
        }

        private void RetrieveProfile(object state)
        {
            this.Invoke(() =>
            {
                this.buttonAuthenticate.Text = "Retrieving profile...";
                this.buttonAuthenticate.Enabled = false;
            });
            try
            {
                using (var service = GeoTransformer.GeocachingService.LiveClient.CreateClientProxy())
                {
                    var resp = service.GetYourUserProfile(true, true, true, true, true, true);
                    this._configControl.UserProfile = resp;
                    this.Invoke(() =>
                    {
                        this.pictureBoxStatusBar.ImageLocation = "http://img.geocaching.com/stats/img.aspx?txt=Live+API+connected&uid=" + resp.Profile.User.PublicGuid.ToString("D");
                        this.buttonAuthenticate.Text = "Authenticated";
                        this.buttonAuthenticate.Enabled = false;

                        this._configControl.IsEnabled = true;
                        this.OnUserAuthenticated();
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to retrieve the user profile using Live API. The use of the API will be disabled." + Environment.NewLine + Environment.NewLine + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Disable();
                return;
            }
        }

        private void Disable()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(this.Disable);
                return;
            }

            this.buttonAuthenticate.Text = "Authenticate using geocaching.com Live API";
            this.buttonAuthenticate.Enabled = true;
            this.pictureBoxStatusBar.ImageLocation = null;
        }

        private void buttonAuthenticate_Click(object sender, EventArgs e)
        {
            this.OpenAuthenticationForm();
        }

        private static object UserAuthenticatedEvent = new object();
        public event EventHandler UserAuthenticated
        {
            add { this.Events.AddHandler(UserAuthenticatedEvent, value); }
            remove { this.Events.RemoveHandler(UserAuthenticatedEvent, value); }
        }

        protected void OnUserAuthenticated(EventArgs args = null)
        {
            var handler = this.Events[UserAuthenticatedEvent] as EventHandler;
            if (handler == null)
                return;

            handler(this, args ?? EventArgs.Empty);
        }
    }
}
