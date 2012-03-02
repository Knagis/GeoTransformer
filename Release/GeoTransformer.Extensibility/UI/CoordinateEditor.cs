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
using GeoTransformer.Coordinates;

namespace GeoTransformer.UI
{
    /// <summary>
    /// A control that enables the user to enter coordinates. The control performs parsing from different formats and provides simple access to the entered value.
    /// </summary>
    public partial class CoordinateEditor : UserControl
    {
        private System.Xml.Linq.XElement _boundElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateEditor"/> class.
        /// </summary>
        public CoordinateEditor()
        {
            InitializeComponent();
        }

        private string _watermark;
        /// <summary>
        /// Gets or sets the watermark text to be displayd on the input element.
        /// </summary>
        /// <value>
        /// The watermark text.
        /// </value>
        public string WatermarkText
        {
            get { return this._watermark; }
            set 
            { 
                this._watermark = value;
                this.textBox.CharacterCasing = string.IsNullOrEmpty(value) ? CharacterCasing.Upper : CharacterCasing.Normal;
                this.textBox.SetWatermark(value);
            }
        }

        /// <summary>
        /// Gets or sets the currently displayed coordinates.
        /// </summary>
        /// <value>
        /// The current coordinates. If the control is displaying an error, <c>null</c> is returned.
        /// </value>
        public Wgs84Point? Coordinates
        {
            get
            {
                return Wgs84Point.TryParse(this.textBox.Text);
            }

            set
            {
                if (Nullable.Equals(this.Coordinates, value))
                    return;

                this.textBox.Text = value == null ? string.Empty : value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the input control is read only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the control is read only; otherwise, <c>false</c>.
        /// </value>
        public bool ReadOnly
        {
            get
            {
                return this.textBox.ReadOnly;
            }

            set
            {
                this.textBox.ReadOnly = value;
            }
        }

        public static object CoordinatesChangedEvent = new object();

        /// <summary>
        /// Occurs when the user changes the coordinates.
        /// </summary>
        public event EventHandler CoordinatesChanged
        {
            add { this.Events.AddHandler(CoordinatesChangedEvent, value); }
            remove { this.Events.RemoveHandler(CoordinatesChangedEvent, value); }
        }

        public System.Xml.Linq.XElement BoundElement
        {
            get
            {
                return this._boundElement;
            }

            set
            {
                if (this._boundElement == value)
                    return;

                this._boundElement = value;
                this.ReadXElement(value);
            }
        }

        private void UpdateXElement(System.Xml.Linq.XElement elem)
        {
            if (elem == null)
                return;

            var coords = this.Coordinates;
            var lat = coords.HasValue ? coords.Value.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) : null;
            var lon = coords.HasValue ? coords.Value.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) : null;

            if (!string.Equals(elem.GetAttributeValue("latitude"), lat, StringComparison.Ordinal))
                elem.SetAttributeValue("latitude", lat);
            if (!string.Equals(elem.GetAttributeValue("longitude"), lon, StringComparison.Ordinal))
                elem.SetAttributeValue("longitude", lon);

            if (!coords.HasValue && !string.IsNullOrWhiteSpace(this.textBox.Text) && !string.Equals(elem.GetAttributeValue("textValue"), this.textBox.Text, StringComparison.Ordinal))
                elem.SetAttributeValue("textValue", this.textBox.Text);
        }

        private void ReadXElement(System.Xml.Linq.XElement elem)
        {
            if (elem == null)
            {
                this.Coordinates = null;
                return;
            }

            decimal lat, lon;
            if (!decimal.TryParse(elem.GetAttributeValue("latitude"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lat)
                || !decimal.TryParse(elem.GetAttributeValue("longitude"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lon))
            {
                this.textBox.Text = elem.GetAttributeValue("textValue");
                return;
            }

            this.textBox.Text = new Coordinates.Wgs84Point(lat, lon).ToString();
        }

        /// <summary>
        /// Reads the coordinates from the XML configuration element and returns a parsed object or <c>null</c> if the element does not contain coordinates.
        /// </summary>
        /// <param name="elem">The element that contains the coordinates.</param>
        public static Coordinates.Wgs84Point? ReadXmlConfiguration(System.Xml.Linq.XElement elem)
        {
            if (elem == null)
                return null;

            decimal lat, lon;
            if (!decimal.TryParse(elem.GetAttributeValue("latitude"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lat)
                || !decimal.TryParse(elem.GetAttributeValue("longitude"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lon))
                return null;

            return new Coordinates.Wgs84Point(lat, lon);
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBox.Text))
            {
                this.errorProvider.SetError(this.textBox, null);
                this.textBox.Width = this.ClientSize.Width;
            }
            else
            {
                try
                {
                    var c = new Coordinates.Wgs84Point(this.textBox.Text);
                    this.errorProvider.SetError(this.textBox, null);
                    this.textBox.Width = this.ClientSize.Width;
                    this.textBox.Text = c.ToString();
                }
                catch (Exception ex)
                {
                    this.errorProvider.SetError(this.textBox, ex.Message);
                    this.textBox.Width = this.ClientSize.Width - this.errorProvider.Icon.Width - this.errorProvider.GetIconPadding(this.textBox);
                }
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (this._boundElement != null)
                this.UpdateXElement(this._boundElement);

            var handler = this.Events[CoordinatesChangedEvent] as EventHandler;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
