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
    public partial class ConfigurationControl : UI.UserControlBase
    {
        public ConfigurationControl()
        {
            InitializeComponent();
            this.UseFlowLayout = false;
        }

        public bool IsEnabled
        {
            get
            {
                return this.checkBoxEnable.Checked;
            }

            set
            {
                if (this.checkBoxEnable.Checked == value)
                    return;

                if (this.InvokeRequired)
                {
                    this.Invoke((c, v) => c.IsEnabled = v, value);
                    return;
                }

                this.checkBoxEnable.Checked = value;
                GeoTransformer.GeocachingService.LiveClient.OnIsEnabledChanged();
            }
        }

        public string AccessToken
        {
            get;
            set;
        }

        public GeoTransformer.GeocachingService.GetUserProfileResponse UserProfile
        {
            get;
            internal set;
        }

        private void checkBoxEnable_CheckedChanged(object sender, EventArgs e)
        {
            this.linkAuthenticate.Enabled = this.labelUsername.Enabled = this.labelUsernameValue.Enabled = this.checkBoxEnable.Checked;

            if (checkBoxEnable.Checked)
            {
                if (string.IsNullOrEmpty(this.AccessToken))
                    this.OpenAuthenticationForm();
                else
                {
                    GeoTransformer.GeocachingService.LiveClient.OnIsEnabledChanged();
                    this.labelUsernameValue.Font = new Font(this.labelUsernameValue.Font, FontStyle.Italic);
                    this.labelUsernameValue.Text = "retrieving...";
                    this.pictureBoxAvatar.ImageLocation = null;
                    System.Threading.ThreadPool.QueueUserWorkItem(this.RetrieveUserName);
                }
            }
            else
            {
                GeoTransformer.GeocachingService.LiveClient.OnIsEnabledChanged();
            }
        }

        private void Disable()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(this.Disable);
                return;
            }

            this.AccessToken = null;
            this.labelUsernameValue.Text = "not authenticated";
            this.labelUsernameValue.Font = new Font(this.labelUsernameValue.Font, FontStyle.Italic);
            this.pictureBoxAvatar.ImageLocation = null;
            this.IsEnabled = false;
        }

        /// <summary>
        /// Called when it is not possible to connect to the service to retrieve user profile.
        /// API is not disabled as the network connectivity might be restored later.
        /// </summary>
        private void UnableToConnect()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(this.UnableToConnect);
                return;
            }

            this.labelUsernameValue.Font = new System.Drawing.Font(this.labelUsernameValue.Font, FontStyle.Italic);
            this.labelUsernameValue.Text = "no Internet access";
            this.pictureBoxAvatar.ImageLocation = null;
        }

        /// <summary>
        /// Opens the modal form that displays the OAuth page for Live API.
        /// </summary>
        private void OpenAuthenticationForm()
        {
            this.UserProfile = null;

            var dlg = new AuthenticationForm();
            var result = dlg.ShowDialog();
            if (result != DialogResult.OK || string.IsNullOrEmpty(dlg.AccessToken))
            {
                this.Disable();
                return;
            }

            this.AccessToken = dlg.AccessToken;
            GeoTransformer.GeocachingService.LiveClient.OnIsEnabledChanged();
            this.labelUsernameValue.Font = new Font(this.labelUsernameValue.Font, FontStyle.Italic);
            this.labelUsernameValue.Text = "retrieving...";
            System.Threading.ThreadPool.QueueUserWorkItem(this.RetrieveUserName);
        }

        private void RetrieveUserName(object state)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                this.UnableToConnect();
                return;
            }

            using (var service = GeoTransformer.GeocachingService.LiveClient.CreateClientProxy())
            {
                try
                {
                    var resp = service.GetYourUserProfile(true, true, true, true, true, true);

                    // if we did connect but the service returned an error message then most probably it is the access token being invalid
                    if (resp.Status.StatusCode != 0)
                    {
                        MessageBox.Show("Error while contacting Live API to retrieve your profile. The use of the API will be disabled." + Environment.NewLine + Environment.NewLine + resp.Status.StatusMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Disable();
                        return;
                    }
                    this.UserProfile = resp;
                    this.labelUsernameValue.Invoke(o =>
                        {
                            o.Font = new System.Drawing.Font(this.labelUsernameValue.Font, FontStyle.Regular);
                            o.Text = resp.Profile.User.UserName + " (" + resp.Profile.User.MemberType.MemberTypeName + ")";
                            this.pictureBoxAvatar.ImageLocation = resp.Profile.User.AvatarUrl;
                        });
                }
                catch (System.TimeoutException)
                {
                    this.UnableToConnect();
                }
                catch (Exception ex)
                {
#if DEBUG
                    // only show the message in debug mode. in production it is not helpful that such a window pops up at the start the application.
                    MessageBox.Show("Unable to retrieve the user profile using Live API. The Live API will not be disabled but might not function properly." + Environment.NewLine + Environment.NewLine + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                    this.UnableToConnect();
                }
            }
        }

        private void linkAuthenticate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.OpenAuthenticationForm();
        }
    }
}
