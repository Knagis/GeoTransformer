/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Transformers.UpdateCoordinates
{
    public class UpdateCoordinates : TransformerBase, Extensions.IEditor
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Update coordinates"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.Process; }
        }

        /// <summary>
        /// Processes the specified input document.
        /// </summary>
        public override void Process(System.Xml.Linq.XDocument xml)
        {
            foreach (var wpt in xml.Root.WaypointElements("wpt"))
            {
                var configElement = wpt.ExtensionElement(typeof(UpdateCoordinates));
                if (configElement == null)
                    continue;

                // the control sets the values, let's rely on it to parse it as well
                var coords = UI.CoordinateEditor.ReadXmlConfiguration(configElement);
                if (!coords.HasValue)
                    continue;

                configElement.SetAttributeValue("originalLatitude", wpt.GetAttributeValue("lat"));
                configElement.SetAttributeValue("originalLongitude", wpt.GetAttributeValue("lon"));

                wpt.SetAttributeValue("lat", coords.Value.Latitude);
                wpt.SetAttributeValue("lon", coords.Value.Longitude);
            }
        }

        #region [ IEditor ]

        private EditorControl _editorControl;

        public System.Windows.Forms.Control CreateControl()
        {
            this._editorControl = new EditorControl();
            return this._editorControl;
        }

        public void BindControl(System.Xml.Linq.XElement data)
        {
            this._editorControl.BoundElement = data;
        }

        #endregion

    }
}
