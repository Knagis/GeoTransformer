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

namespace GeoTransformer.Transformers.LoadFromLiveApi
{
    /// <summary>
    /// Configuration control for <see cref="LoadFromLiveApi"/>.
    /// </summary>
    internal partial class ConfigurationControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationControl"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ConfigurationControl(byte[] configuration)
        {
            InitializeComponent();

            GeocachingService.LiveClient.IsEnabledChanged += (a, b) => { this.EnableControls(GeocachingService.LiveClient.IsEnabled); };
            this.EnableControls(GeocachingService.LiveClient.IsEnabled);

            this.listView.SuspendLayout();
            this.listView.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Title", "Title") { MinimumWidth = 100 });
            this.listView.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Query", "Description") { FillsFreeSpace = true });
            this.listView.Columns.AddRange(this.listView.AllColumns.ToArray());

            // load the queries from the configuration
            this.DeserializeConfiguration(configuration);

            this.listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            this.listView.ResumeLayout();
        }

        /// <summary>
        /// Gets the extension instance.
        /// </summary>
        private LoadFromLiveApi Extension
        {
            get
            {
                return Extensions.ExtensionLoader.RetrieveExtensions<LoadFromLiveApi>().First();
            }
        }

        private void EnableControls(bool enable)
        {
            this.toolStripLabelDisabled.Visible = !enable;
            this.toolStripSeparatorDisabled.Visible = !enable;
            this.btnCreate.Enabled = enable;
            bool smthSelected = this.listView.SelectedIndex != -1;
            this.btnModify.Enabled = smthSelected;
            this.btnDelete.Enabled = smthSelected;
            this.listView.Enabled = enable;
        }

        /// <summary>
        /// Deserializes the configuration.
        /// </summary>
        private void DeserializeConfiguration(byte[] configuration)
        {
            if (configuration == null || configuration.Length == 0)
                return;

            using (var stream = new System.IO.MemoryStream(configuration))
            using (var reader = new System.IO.BinaryReader(stream))
            {
                List<Query> data = new List<Query>();

                for (var count = reader.ReadInt32(); count > 0; count--)
                    data.Add(Query.Deserialize(reader));

                this.Queries = data;
            }

        }

        /// <summary>
        /// Gets or sets the queries that are currently displayed in the control.
        /// </summary>
        internal IEnumerable<Query> Queries
        {
            get 
            {
                var obj = this.listView.Objects;

                if (obj == null)
                    return new Query[0]; 

                return obj.OfType<Query>();
            }
            set { this.listView.SetObjects(value, true); }
        }

        /// <summary>
        /// Serializes the configuration as entered by the user.
        /// </summary>
        /// <returns>Serialized configuration.</returns>
        public byte[] SerializeConfiguration()
        {
            using (var stream = new System.IO.MemoryStream())
            using (var writer = new System.IO.BinaryWriter(stream))
            {
                var data = this.Queries.ToList();
                writer.Write(data.Count);
                foreach (var d in data)
                    d.Serialize(writer);

                writer.Flush();
                return stream.ToArray();
            }
        }

        #region [ Button handlers ]
        private void btnCreate_Click(object sender, EventArgs e)
        {
            var f = new FilterEditor();
            f.Show();
            f.FormClosed += this.CreateCompleted;
        }

        void CreateCompleted(object sender, FormClosedEventArgs e)
        {
            var form = (FilterEditor)sender;
            if (form.DialogResult != DialogResult.OK)
                return;

            this.listView.AddObject(form.Query);
            this.listView.SelectObject(form.Query);
            this.listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            var query = (Query)this.listView.SelectedObject;
            var f = new FilterEditor(query);
            f.Show();
            f.FormClosed += this.ModifyCompleted;
        }

        void ModifyCompleted(object sender, FormClosedEventArgs e)
        {
            var form = (FilterEditor)sender;
            if (form.DialogResult != DialogResult.OK)
                return;

            this.listView.RefreshObject(form.Query);
            this.listView.SelectObject(form.Query);
            this.listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var query = (Query)this.listView.SelectedObject;
            if (query == null)
                return;

            var confirm = MessageBox.Show("Are you sure you want to delete the query '" + query.Title + "'?" + Environment.NewLine + Environment.NewLine + "This operation cannot be undone!", "Delete query", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (confirm != DialogResult.Yes)
                return;

            var files = System.IO.Directory.EnumerateFiles(this.Extension.LocalStoragePath, query.Id.ToString() + "*.*", System.IO.SearchOption.TopDirectoryOnly);
            foreach (var f in files.ToList())
            {
                try
                {
                    System.IO.File.Delete(f);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to delete '" + f + "'." + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.listView.RemoveObject(query);
        }

        #endregion

        private void listView_SelectionChanged(object sender, EventArgs e)
        {
            bool smthSelected = this.listView.SelectedIndex != -1;
            this.btnModify.Enabled = smthSelected;
            this.btnDelete.Enabled = smthSelected;
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listView.SelectedIndex != -1)
                this.btnModify_Click(sender, e);
        }

        private void listView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && this.listView.SelectedIndex != -1)
            {
                this.btnModify_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}
