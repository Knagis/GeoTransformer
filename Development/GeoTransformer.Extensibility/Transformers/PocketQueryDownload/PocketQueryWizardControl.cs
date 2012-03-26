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
    public partial class PocketQueryWizardControl : UserControl
    {
        private PocketQueryDownloadOptions _configControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="PocketQueryWizardControl"/> class.
        /// </summary>
        public PocketQueryWizardControl()
        {
            InitializeComponent();

            var ext = GeoTransformer.Extensions.ExtensionLoader.RetrieveExtensions<PocketQueryDownload>().FirstOrDefault();

            this._configControl = ext.ConfigurationControl;
        }

        public int PocketQueryCount
        {
            get
            {
                return this.listView.Objects.OfType<GeocachingService.PQData>().Count();
            }
        }

        public int SelectedPocketQueryCount
        {
            get
            {
                return this.listView.CheckedObjects.OfType<GeocachingService.PQData>().Count();
            }
        }

        public void LoadPocketQueries(GeocachingService.LiveClient service)
        {
            this.listView.Clear();
            var data = service.GetPocketQueryList(service.AccessToken);

            if (data.Status.StatusCode != 0)
            {
                MessageBox.Show("Unable to load the pocket query list." + Environment.NewLine + Environment.NewLine + data.Status.StatusMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.listView.SuspendLayout();
            var cw = this.listView.ClientSize.Width;
            this.listView.AllColumns.Add(new BrightIdeasSoftware.OLVColumn() { Text = string.Empty, Width = cw * 1 / 10 });
            this.listView.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Name", "Name") { Width = cw * 9 / 20 });
            this.listView.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Caches", "PQCount") { Width = cw * 3 / 20 });
            this.listView.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Last generated", "DateLastGenerated") { Width = cw * 3 / 10});
            this.listView.Columns.AddRange(this.listView.AllColumns.ToArray());

            this.listView.SetObjects(data.PocketQueryList);

            if (data.PocketQueryList.Length == 0)
            {
                this.listView.EmptyListMsg = "You do not have any pocket queries currently generated. Please run a query on geocaching.com and then try again.";
            }

            this._configControl.SetPocketQueryList(data.PocketQueryList, false);

            var guids = data.PocketQueryList.Select(o => o.GUID).ToList();
            foreach (var prevSelected in this._configControl.CheckedQueries)
            {
                var i = guids.IndexOf(prevSelected.Key);
                if (i != -1)
                    this.listView.CheckObject(data.PocketQueryList[i]);
            }

            this.listView.ResumeLayout(true);
        }

        private void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var obj = this.listView.Objects.OfType<GeocachingService.PQData>().ElementAt(e.Item.Index);

            this._configControl.SetPocketQueryCheckedState(obj.GUID, e.Item.Checked);

            this._configControl.DownloadEnabled = this._configControl.CheckedQueries.Count > 0;
        }
    }
}
