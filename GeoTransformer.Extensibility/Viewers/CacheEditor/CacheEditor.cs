/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Viewers.CacheEditor
{
    public class CacheEditor : Extensions.ICacheViewer
    {
        public System.Drawing.Image ButtonImage
        {
            get { return Resources.Edit; }
        }

        public string ButtonText
        {
            get { return "Editor"; }
        }

        private EditorControl _container;
        private List<Tuple<Extensions.IEditor, System.Windows.Forms.Control>> _editors = new List<Tuple<Extensions.IEditor,System.Windows.Forms.Control>>();

        public System.Windows.Forms.Control Initialize()
        {
            this._container = new EditorControl(this);

            foreach (var ext in Extensions.ExtensionLoader.RetrieveExtensions<Extensions.IEditor>())
            {
                var c = ext.CreateControl();
                this._container.flowLayoutPanel.Controls.Add(c);

                // workaround for a bug in flow layout panel
                this._container.flowLayoutPanel.Controls.Add(new System.Windows.Forms.Control() { Width = 0, Height = 0 });

                this._editors.Add(Tuple.Create(ext, c));
            }

            return this._container;
        }

        /// <summary>
        /// Called to display the cache details in the UI control.
        /// </summary>
        /// <param name="data">The GPX element containing the cache information.</param>
        public void DisplayCache(System.Xml.Linq.XElement data)
        {
            this._container.BindElement(data);

            this._container.flowLayoutPanel.Enabled = data != null;

            foreach (var editor in this._editors)
            {
                if (data == null)
                    editor.Item1.BindControl(null);
                else
                {
                    var elem = data.Element(XmlExtensions.CreateExtensionName(editor.Item1.GetType()));
                    if (elem == null)
                        data.Add(elem = new System.Xml.Linq.XElement(XmlExtensions.CreateExtensionName(editor.Item1.GetType())));
                    editor.Item1.BindControl(elem);
                }
            }
        }

        /// <summary>
        /// Called by the main application to determine if the viewer is enabled for the particular waypoint.
        /// </summary>
        /// <param name="data">The GPX element containing the cache information (can be <c>null</c>).</param>
        /// <returns>
        ///   <c>true</c> if the viewer is enabled for the specified waypoint; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled(System.Xml.Linq.XElement data)
        {
            return true;
        }
    }
}
