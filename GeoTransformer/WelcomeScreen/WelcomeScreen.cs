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

namespace GeoTransformer.WelcomeScreen
{
    public partial class WelcomeScreen : Form
    {
        private class TablessControl : TabControl
        {
            protected override void WndProc(ref Message m) 
            {     
                // Hide tabs by trapping the TCM_ADJUSTRECT message
                if (m.Msg == 0x1328 && !this.DesignMode) 
                    m.Result = (IntPtr)1;     
                else 
                    base.WndProc(ref m);   
            } 
        }

        private MainForm _parentForm;

        public WelcomeScreen(MainForm parentForm)
        {
            InitializeComponent();

            this._parentForm = parentForm;

            this.SuspendLayout();

            this.checkBoxDoNotShowWelcomeScreen.Checked = DoNotShowWelcomeScreen;

            foreach (Control tab in this.tabControl1.Controls)
            {
                tab.BackColor = this.BackColor;
            }

            using (var icon = new System.Drawing.Icon(parentForm.Icon, this.pictureBox1.Size))
            {
                this.pictureBox5.Image =
                this.pictureBox3.Image =
                this.pictureBox2.Image = 
                this.pictureBox1.Image = 
                    icon.ToBitmap();
            }

            this.richTextBox1.Rtf = ReadRichTextResource("Screen1");
            this.richTextBox2.Rtf = ReadRichTextResource("Screen2");
            this.richTextBox3.Rtf = ReadRichTextResource("Screen3");
            this.richTextBox4.Rtf = ReadRichTextResource("Screen4");
            this.richTextBox5.Rtf = ReadRichTextResource("Screen5");

            this.ResumeLayout();
        }

        private static System.Reflection.Assembly currentAssembly = System.Reflection.Assembly.GetCallingAssembly();
        private static string ReadRichTextResource(string name)
        {
            using (var stream = currentAssembly.GetManifestResourceStream("GeoTransformer.WelcomeScreen." + name + ".rtf"))
            using (var reader = new System.IO.StreamReader(stream))
                return reader.ReadToEnd();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == this.tabControl1.TabCount - 1)
            {
                this.Close();
                new System.Threading.Thread((a) => this._parentForm.listViewers.LoadListViewerData((bool)a)).Start(true);

                this._parentForm.OpenCacheTab();
                this._parentForm.listViewers.OpenSpecificViewer(typeof(Viewers.BingMaps.BingMapsList));
                return;
            }

            if (this.tabControl1.SelectedIndex == 3)
            {
                if (this.pocketQueryWizardControl.Visible && this.pocketQueryWizardControl.PocketQueryCount > 0 && this.pocketQueryWizardControl.SelectedPocketQueryCount == 0)
                {
                    var res = MessageBox.Show("You have not selected any pocket queries. Do you want to go back to select some?", "Pocket queries", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (res == System.Windows.Forms.DialogResult.Yes)
                        return;
                }
            }

            this.buttonPrevious.Enabled = true;
            this.tabControl1.SelectedIndex++;
            if (this.tabControl1.SelectedIndex == this.tabControl1.TabCount - 1)
            {
                this.buttonNext.Text = "Finish";
                this.checkBoxDoNotShowWelcomeScreen.Checked = true;
            }
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            this.buttonNext.Text = "Next";
            this.tabControl1.SelectedIndex--;
            if (this.tabControl1.SelectedIndex == 0)
                this.buttonPrevious.Enabled = false;
        }

        private void wizardControl1_UserAuthenticated(object sender, EventArgs e)
        {
            try
            {
                using (var service = GeocachingService.LiveClient.CreateClientProxy())
                {
                    var profile = service.GetYourUserProfileCached();

                    if (string.Equals(profile.Profile.User.MemberType.MemberTypeName, "Basic", StringComparison.OrdinalIgnoreCase))
                    {
                        this.richTextBox4Basic.Rtf = ReadRichTextResource("Screen4Basic");
                        this.richTextBox4Basic.Visible = true;
                    }
                    else
                    {
                        this.pocketQueryWizardControl.Visible = true;
                        this.pocketQueryWizardControl.LoadPocketQueries(service);
                    }
                }
            }
            catch
            {
            }
        }

        private void richTextBox4Basic_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        public static bool DoNotShowWelcomeScreen { get; set; }
        private void checkBoxDoNotShowWelcomeScreen_CheckedChanged(object sender, EventArgs e)
        {
            DoNotShowWelcomeScreen = this.checkBoxDoNotShowWelcomeScreen.Checked;
        }
    }
}
