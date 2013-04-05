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

namespace GeoTransformer.Transformers.PocketQueryDownload
{
    internal partial class PocketQueryDownloadOptions : UI.UserControlBase
    {
        public PocketQueryDownloadOptions()
        {
            InitializeComponent();
        }

        public bool DownloadEnabled 
        {
            get 
            { 
                return this.checkBoxEnablePocketQuery.Checked; 
            } 

            internal set
            {
                this.checkBoxEnablePocketQuery.Checked = value;
            }
        }

        /// <summary>
        /// Gets the value that determines if the pocket query download has to be performed using full data download instead of classic ZIP file.
        /// </summary>
        public bool DownloadFullData { get { return this.chkDownloadFullData.Checked; } }

        public Dictionary<Guid, string> CheckedQueries
        {
            get
            {
                return this.comboBoxPocketQuery.CheckedItems.Cast<Tuple<Guid, string>>().ToDictionary(o => o.Item1, o => o.Item2);
            }
        }

        public byte[] SerializeSettings()
        {
            using (var ms = new System.IO.MemoryStream())
            using (var writer = new System.IO.BinaryWriter(ms, Encoding.UTF8))
            {
                writer.Write(this.checkBoxEnablePocketQuery.Checked);
                writer.Write(this.comboBoxPocketQuery.Items.Count);
                int i = 0;
                foreach (Tuple<Guid, string> o in this.comboBoxPocketQuery.Items)
                {
                    writer.Write(o.Item1.ToByteArray().Length);
                    writer.Write(o.Item1.ToByteArray());
                    writer.Write(o.Item2);
                    writer.Write(this.comboBoxPocketQuery.GetItemChecked(i));
                    i++;
                }

                // configuration version
                writer.Write((byte)2);
                writer.Write(this.chkDownloadFullData.Checked);

                writer.Flush();
                return ms.ToArray();
            }
        }

        public void DeserializeSettings(byte[] settings)
        {
            if (settings == null || settings.Length == 0)
                return;

            try
            {
                this.comboBoxPocketQuery.Items.Clear();
                using (var ms = new System.IO.MemoryStream(settings))
                using (var reader = new System.IO.BinaryReader(ms, Encoding.UTF8))
                {
                    var enabled = reader.ReadBoolean();
                    var itemCount = reader.ReadInt32();
                    for (int i = 0; i < itemCount; i++)
                    {
                        this.comboBoxPocketQuery.Items.Add(Tuple.Create(new Guid(reader.ReadBytes(reader.ReadInt32())), reader.ReadString()), reader.ReadBoolean());
                    }

                    this.comboBoxPocketQuery.Text = this.comboBoxPocketQuery.CheckedItems.Cast<Tuple<Guid, string>>().Select(o => o.Item2).DefaultIfEmpty(string.Empty).Aggregate((a, b) => a + ", " + b);

                    this.checkBoxEnablePocketQuery.Checked = enabled;

                    // set the defaults for when loading older versions of configuration data
                    this.chkDownloadFullData.Checked = false;

                    // continuing reading at this point will result in exception if the configuration version is 1
                    var version = reader.ReadByte();
                    this.chkDownloadFullData.Checked = reader.ReadBoolean();
                }
            }
            catch
            {
            }
        }

        internal void SetPocketQueryList(IEnumerable<GeocachingService.PQData> data, bool enableIfOnlyOne = true)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (this.comboBoxPocketQuery.InvokeRequired)
            {
                this.comboBoxPocketQuery.Invoke(() => this.SetPocketQueryList(data));
                return;
            }

            var oldGuids = this.comboBoxPocketQuery.CheckedItems.Cast<Tuple<Guid, string>>().Select(o => o.Item1).ToArray();
            var oldTitles = this.comboBoxPocketQuery.CheckedItems.Cast<Tuple<Guid, string>>().Select(o => o.Item2).ToArray();
            this.comboBoxPocketQuery.Items.Clear();

            foreach (var pq in data)
            {
                this.comboBoxPocketQuery.Items.Add(Tuple.Create(pq.GUID, pq.Name), oldGuids.Contains(pq.GUID) || oldTitles.Contains(pq.Name) || (enableIfOnlyOne && oldGuids.Length == 0 && data.Count() == 1));
            }

            this.comboBoxPocketQuery.Text = this.comboBoxPocketQuery.CheckedItems.Cast<Tuple<Guid, string>>().Select(o => o.Item2).DefaultIfEmpty(string.Empty).Aggregate((a, b) => a + ", " + b);
        }

        internal void SetPocketQueryCheckedState(Guid pocketQueryGuid, bool isChecked)
        {
            if (this.comboBoxPocketQuery.InvokeRequired)
            {
                this.comboBoxPocketQuery.Invoke(() => this.SetPocketQueryCheckedState(pocketQueryGuid, isChecked));
                return;
            }

            for (int i = 0; i < this.comboBoxPocketQuery.Items.Count; i++)
            {
                var item = (Tuple<Guid, string>)this.comboBoxPocketQuery.Items[i];
                if (item.Item1 == pocketQueryGuid)
                {
                    this.comboBoxPocketQuery.SetItemChecked(i, isChecked);
                    break;
                }
            }

            this.comboBoxPocketQuery.Text = this.comboBoxPocketQuery.CheckedItems.Cast<Tuple<Guid, string>>().Select(o => o.Item2).DefaultIfEmpty(string.Empty).Aggregate((a, b) => a + ", " + b);
        }

        private void RefreshPocketQueryList()
        {
            if (!GeocachingService.LiveClient.IsEnabled)
            {
                MessageBox.Show("Automatic pocket query download requires geocaching.com Live API. Please enable the usage of API first.");
                this.checkBoxEnablePocketQuery.Enabled = false;
                return;
            }

            using (var service = GeocachingService.LiveClient.CreateClientProxy())
            {
                var r = service.GetPocketQueryList(service.AccessToken);
                if (r.Status.StatusCode != 0)
                {
                    MessageBox.Show("Unable to load the pocket queries from Live API." + Environment.NewLine + Environment.NewLine + r.Status.StatusMessage);
                    this.checkBoxEnablePocketQuery.Checked = false;
                    return;
                }
                
                if (r.PocketQueryList.Length == 0)
                {
                    MessageBox.Show("You currently do not have any pocket queries available for download. Please use the geocaching.com website to create and run a query.");
                    this.checkBoxEnablePocketQuery.Checked = false;
                    return;
                }

                this.SetPocketQueryList(r.PocketQueryList);
            }
        }

        private void checkBoxEnablePocketQuery_CheckedChanged(object sender, EventArgs e)
        {
            this.labelPocketQuery.Enabled = this.comboBoxPocketQuery.Enabled = this.linkRefresh.Enabled = this.checkBoxEnablePocketQuery.Checked;

            if (this.checkBoxEnablePocketQuery.Checked && this.comboBoxPocketQuery.Items.Count == 0)
                this.RefreshPocketQueryList();
        }

        private void linkRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.RefreshPocketQueryList();
        }
    }
}
