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

namespace GeoTransformer.Transformers.LoadGpxTautai
{
    internal partial class GpxTautaiOptions : UI.UserControlBase
    {
        public GpxTautaiOptions()
        {
            InitializeComponent();

            this.comboBoxGpxTautaiDifficultyMin.SelectedItem = "1";
            this.comboBoxGpxTautaiDifficultyMax.SelectedItem = "5";
            this.comboBoxGpxTautaiTerrainMin.SelectedItem = "1";
            this.comboBoxGpxTautaiTerrainMax.SelectedItem = "5";

            this.comboBoxGpxTautaiCacheTypes.Items.Add(Tuple.Create("trad", "Traditional cache"), true);
            this.comboBoxGpxTautaiCacheTypes.Items.Add(Tuple.Create("myst", "Unknown cache"), true);
            this.comboBoxGpxTautaiCacheTypes.Items.Add(Tuple.Create("multi", "Multi-cache"), true);
            this.comboBoxGpxTautaiCacheTypes.Items.Add(Tuple.Create("earth", "Earthcache"), true);
            this.comboBoxGpxTautaiCacheTypes.Items.Add(Tuple.Create("lett", "Letterbox-hybrid cache"), true);
            this.comboBoxGpxTautaiCacheTypes.Items.Add(Tuple.Create("wherigo", "Whereigo cache"), true);
            this.comboBoxGpxTautaiCacheTypes.Items.Add(Tuple.Create("virt", "Virtual cache"), true);
            this.comboBoxGpxTautaiCacheTypes.Items.Add(Tuple.Create("event", "Event cache"), true);
            this.comboBoxGpxTautaiCacheTypes.Items.Add(Tuple.Create("lfevent", "Lost and found event cache"), true);
            this.comboBoxGpxTautaiCacheTypes.Items.Add(Tuple.Create("citoevent", "CITO event cache"), true);

            this.comboBoxGpxTautaiCacheTypes.Text = this.comboBoxGpxTautaiCacheTypes.CheckedItems.OfType<Tuple<string, string>>().Select(o => o.Item2).Aggregate((a, b) => a + ", " + b);
        }

        public bool GpxTautaiEnabled { get { return this.checkBoxEnableGpxTautai.Checked; } }

        public byte[] SerializeSettings()
        {
            if (this.InvokeRequired)
                return (byte[])this.Invoke(new Func<byte[]>(this.SerializeSettings));

            using (var ms = new System.IO.MemoryStream())
            using (var writer = new System.IO.BinaryWriter(ms, Encoding.UTF8))
            {
                writer.Write(this.checkBoxEnableGpxTautai.Checked);
                writer.Write(this.comboBoxGpxTautaiCacheTypes.CheckedItems.Count);
                foreach (var x in this.comboBoxGpxTautaiCacheTypes.CheckedItems.OfType<Tuple<string, string>>().Select(o => o.Item1))
                    writer.Write(x);
                writer.Write((string)this.comboBoxGpxTautaiDifficultyMin.SelectedItem);
                writer.Write((string)this.comboBoxGpxTautaiDifficultyMax.SelectedItem);
                writer.Write((string)this.comboBoxGpxTautaiTerrainMin.SelectedItem);
                writer.Write((string)this.comboBoxGpxTautaiTerrainMax.SelectedItem);
                writer.Write(this.checkBoxGpxTautaiDownloadActive.Checked);
                writer.Write(this.checkBoxGpxTautaiDownloadDisabled.Checked);
                writer.Write(this.checkBoxGpxTautaiDownloadArchived.Checked);
                writer.Write(this.textBoxGpxTautaiUserName.Text);
                writer.Write(this.checkBoxGpxTautaiIncludeLogs.Checked);
                writer.Write(int.Parse(this.textBoxGpxTautaiMaxLogs.Text));

                writer.Flush();
                return ms.ToArray();
            }
        }

        public void DeserializeSettings(byte[] settings)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<byte[]>(this.DeserializeSettings), settings);
                return;
            }

            if (settings == null || settings.Length == 0)
                return;

            try
            {
                using (var ms = new System.IO.MemoryStream(settings))
                using (var reader = new System.IO.BinaryReader(ms, Encoding.UTF8))
                {
                    this.checkBoxEnableGpxTautai.Checked = reader.ReadBoolean();
                    for (int i = 0; i < this.comboBoxGpxTautaiCacheTypes.Items.Count; i++) this.comboBoxGpxTautaiCacheTypes.SetItemChecked(i, false);
                    var keys = this.comboBoxGpxTautaiCacheTypes.Items.OfType<Tuple<string, string>>().Select(o => o.Item1).ToList();
                    var cnt = reader.ReadInt32();
                    for (int i = 0; i < cnt; i++)
                        this.comboBoxGpxTautaiCacheTypes.SetItemChecked(keys.IndexOf(reader.ReadString()), true);
                    this.comboBoxGpxTautaiDifficultyMin.SelectedItem = reader.ReadString();
                    this.comboBoxGpxTautaiDifficultyMax.SelectedItem = reader.ReadString();
                    this.comboBoxGpxTautaiTerrainMin.SelectedItem = reader.ReadString();
                    this.comboBoxGpxTautaiTerrainMax.SelectedItem = reader.ReadString();

                    this.checkBoxGpxTautaiDownloadActive.Checked = reader.ReadBoolean();
                    this.checkBoxGpxTautaiDownloadDisabled.Checked = reader.ReadBoolean();
                    this.checkBoxGpxTautaiDownloadArchived.Checked = reader.ReadBoolean();
                    this.textBoxGpxTautaiUserName.Text = reader.ReadString();
                    this.checkBoxGpxTautaiIncludeLogs.Checked = reader.ReadBoolean();
                    this.textBoxGpxTautaiMaxLogs.Text = reader.ReadInt32().ToString();
                }
            }
            catch
            {
            }
        }

        private void checkBoxEnableGpxTautai_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control c in this.checkBoxEnableGpxTautai.Parent.Controls)
            {
                if (c != this.checkBoxEnableGpxTautai && c.Name.Contains("GpxTautai"))
                    c.Enabled = this.checkBoxEnableGpxTautai.Checked;
            }

            checkBoxGpxTautaiIncludeLogs_CheckedChanged(sender, e);
        }

        private void checkBoxGpxTautaiIncludeLogs_CheckedChanged(object sender, EventArgs e)
        {
            this.textBoxGpxTautaiMaxLogs.Enabled = this.checkBoxGpxTautaiIncludeLogs.Checked && this.checkBoxGpxTautaiIncludeLogs.Enabled;
        }

        internal void AddFilterFields(Dictionary<string, string> fields)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Dictionary<string, string>>(this.AddFilterFields), fields);
                return;
            }

            if (this.checkBoxGpxTautaiDownloadActive.Checked) fields.Add("act", "1");
            if (this.checkBoxGpxTautaiDownloadDisabled.Checked) fields.Add("dis", "1");
            if (this.checkBoxGpxTautaiDownloadArchived.Checked) fields.Add("arc", "1");

            fields.Add("difmin", (string)this.comboBoxGpxTautaiDifficultyMin.SelectedItem);
            fields.Add("difmax", (string)this.comboBoxGpxTautaiDifficultyMax.SelectedItem);
            fields.Add("termin", (string)this.comboBoxGpxTautaiTerrainMin.SelectedItem);
            fields.Add("termax", (string)this.comboBoxGpxTautaiTerrainMax.SelectedItem);
            
            foreach (var t in this.comboBoxGpxTautaiCacheTypes.CheckedItems.OfType<Tuple<string, string>>().Select(o => o.Item1))
                fields.Add(t, "1");

            fields.Add("skip", this.textBoxGpxTautaiUserName.Text);
            
            if (this.checkBoxGpxTautaiIncludeLogs.Checked) fields.Add("logi", "1");
            fields.Add("logmax", this.textBoxGpxTautaiMaxLogs.Text.Trim());
        }

        private void textBoxGpxTautaiMaxLogs_Validating(object sender, CancelEventArgs e)
        {
            var v = int.Parse(this.textBoxGpxTautaiMaxLogs.Text);
            if (v < 1 || v > 20)
            {
                e.Cancel = true;
                MessageBox.Show("Please enter a value between 1 and 20.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
    }
}
