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
using GeoTransformer.Extensions;

namespace GeoTransformer.Modules
{
    public partial class CacheViewers : UserControl
    {
        /// <summary>
        /// Gets the form that the container control is assigned to.
        /// </summary>
        /// <returns>The <see cref="T:System.Windows.Forms.Form"/> that the container control is assigned to. This property will return null if the control is hosted inside of Internet Explorer or in another hosting context where there is no parent form. </returns>
        private new MainForm ParentForm
        {
            get { return (MainForm)base.ParentForm; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewers"/> class.
        /// </summary>
        public CacheViewers()
        {
            InitializeComponent();

            this.InitializeViewers();
        }

        private List<Tuple<ICacheViewer, Control, ToolStripButton>> _cacheViewers = new List<Tuple<ICacheViewer, Control, ToolStripButton>>();
        private System.Xml.Linq.XElement _currentSelection;

        private void InitializeViewers()
        {
            foreach (var instance in Extensions.ExtensionLoader.RetrieveExtensions<ICacheViewer>()
                                                               .OrderBy(o => o is Viewers.CacheEditor.CacheEditor ? 0 : 1)
                                                               .ThenBy(o => o.ButtonText))
            {
                var btn = new ToolStripButton(instance.ButtonText, instance.ButtonImage);
                btn.TextImageRelation = TextImageRelation.ImageAboveText;
                btn.Tag = this._cacheViewers.Count;

                var cond = instance as Extensions.IConditional;
                if (cond != null && !cond.IsEnabled)
                    btn.Enabled = false;
                else if (!instance.IsEnabled(null))
                    btn.Enabled = false;

                this._cacheViewers.Add(Tuple.Create<ICacheViewer, Control, ToolStripButton>(instance, null, btn));
                this.toolStripViewers.Items.Add(btn);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var firstButton = this.toolStripViewers.Items.OfType<ToolStripButton>().FirstOrDefault(o => o.Tag != null);
            firstButton.PerformClick();
        }

        private void toolStripViewers_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var btn = e.ClickedItem as ToolStripButton;
            if (btn == null || btn.Checked)
                return;

            var current = this.toolStripViewers.Items.OfType<ToolStripButton>().FirstOrDefault(o => o.Checked);
            if (current != null)
            {           
                this._cacheViewers[(int)current.Tag].Item2.Visible = false;
                current.Checked = false;
            }

            btn.Checked = true;

            var tag = (int)btn.Tag;

            var viewer = this._cacheViewers[tag];

            if (viewer.Item2 == null)
            {
                this._cacheViewers[tag] = viewer = Tuple.Create(viewer.Item1, viewer.Item1.Initialize(), btn);
                viewer.Item2.Dock = DockStyle.Fill;
                viewer.Item2.Visible = false;
                this.Controls.Add(viewer.Item2);
                viewer.Item2.BringToFront(); // so that the Dock works properly.
            }

            //viewer.Item2.Visible = true;
            this.DisplayCache(this._currentSelection);
        }

        public void DisplayCache(System.Xml.Linq.XElement cache)
        {
            this._currentSelection = cache;

            foreach (var v in this._cacheViewers)
            {
                var enabled = true;
                var cond = v.Item1 as Extensions.IConditional;
                if (cond != null)
                    enabled &= cond.IsEnabled;

                enabled &= v.Item1.IsEnabled(cache);

                v.Item3.Enabled = enabled;

                if (v.Item3.Checked)
                {
                    if (!enabled)
                    {
                        v.Item2.Visible = false;
                        continue;
                    }

                    v.Item2.Visible = true;
                    v.Item1.DisplayCache(cache);
                }
            }
        }
    }
}
