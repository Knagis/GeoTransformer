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

namespace GeoTransformer.Transformers.Translator
{
    internal partial class ConfigurationControl : UI.UserControlBase
    {
        public ConfigurationControl()
        {
            InitializeComponent();
        }

        internal ConfigurationData Data
        {
            get
            {
                var d = new ConfigurationData();
                d.IsEnabled = this.checkBoxEnableTranslator.Checked;
                if (this.LanguageCodes != null)
                {
                    d.IgnoreLanguages = this.comboBoxIgnoreLanguages.CheckedIndices.Cast<int>().Select(o => this.LanguageCodes[o]).ToArray();
                    d.LanguageCodes = this.LanguageCodes;
                    d.LanguageNames = this.LanguageNames;
                    d.TargetLanguage = this.LanguageCodes[this.comboBoxTargetLanguage.Invoke(o => o.SelectedIndex)];
                }
                d.TranslateDescriptions = this.checkBoxTranslateDescriptions.Checked;
                d.TranslateHints = this.checkBoxTranslateHints.Checked;
                d.TranslateLogs = this.checkBoxTranslateLogs.Checked;
                d.ClientId = this.txtClientId.Text;
                d.ClientSecret = this.txtClientSecret.Text;
                return d;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (this.LanguageCodes == null && value.LanguageCodes != null)
                {
                    this.LanguageCodes = value.LanguageCodes.ToArray();
                }
                if (this.LanguageNames == null && value.LanguageNames != null)
                {
                    this.LanguageNames = value.LanguageNames.ToArray();
                    this.comboBoxIgnoreLanguages.Items.AddRange(this.LanguageNames);
                    this.comboBoxTargetLanguage.Items.AddRange(this.LanguageNames);
                }

                if (this.LanguageCodes != null)
                {
                    this.comboBoxTargetLanguage.SelectedIndex = Array.IndexOf(this.LanguageCodes, !this.LanguageCodes.Contains(value.TargetLanguage) ? "en" : value.TargetLanguage);

                    foreach (var i in value.IgnoreLanguages)
                        this.comboBoxIgnoreLanguages.SetItemChecked(Array.IndexOf(this.LanguageCodes, i), true);
                }

                this.checkBoxEnableTranslator.Checked = value.IsEnabled;
                this.checkBoxTranslateDescriptions.Checked = value.TranslateDescriptions;
                this.checkBoxTranslateHints.Checked = value.TranslateHints;
                this.checkBoxTranslateLogs.Checked = value.TranslateLogs;

                this.txtClientId.Text = value.ClientId;
                this.txtClientSecret.Text = value.ClientSecret;
            }
        }

        private string[] LanguageCodes;
        private string[] LanguageNames;

        private void checkBoxEnableTranslator_CheckedChanged(object sender, EventArgs e)
        {
            var x = this.checkBoxEnableTranslator.Checked;

            if (x && this.LanguageNames == null)
            {
                using (var service = Translator.CreateServiceWithoutAuthentication())
                {
                    try
                    {
                        this.LanguageCodes = service.GetLanguagesForTranslate(BingApiKeys.TranslatorKey);
                        this.LanguageNames = service.GetLanguageNames(BingApiKeys.TranslatorKey, "en", this.LanguageCodes);

                        this.comboBoxTargetLanguage.Items.AddRange(this.LanguageNames);
                        this.comboBoxTargetLanguage.SelectedIndex = Array.IndexOf(this.LanguageCodes, "en");
                        this.comboBoxIgnoreLanguages.Items.AddRange(this.LanguageNames);
                        this.comboBoxIgnoreLanguages.SetItemChecked(this.comboBoxTargetLanguage.SelectedIndex, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to retrieve languages from Bing Translator:" + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        this.checkBoxEnableTranslator.Checked = false;
                        return;
                    }
                }
            }

            this.labelTargetLanguage.Enabled = x;
            this.labelIgnoreLanguages.Enabled = x;
            this.labelTranslateOptions.Enabled = x;
            this.comboBoxIgnoreLanguages.Enabled = x;
            this.comboBoxTargetLanguage.Enabled = x;
            this.checkBoxTranslateDescriptions.Enabled = x;
            this.checkBoxTranslateHints.Enabled = x;
            this.checkBoxTranslateLogs.Enabled = x;
            this.linkLabelClientID.Enabled = x;
            this.txtClientId.Enabled = x;
            this.labelClientSecret.Enabled = x;
            this.txtClientSecret.Enabled = x;
        }

        private void linkLabelClientID_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://knagis.miga.lv/blog/post.aspx?id=328e9f29-6d16-4dc1-9716-5935b7832f2a");
        }

    }
}
