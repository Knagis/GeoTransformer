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

namespace GeoTransformer.Transformers.UpdateCoordinates
{
    public partial class EditorControl : UI.UserControlBase
    {
        public EditorControl()
        {
            InitializeComponent();
        }

        public System.Xml.Linq.XElement BoundElement 
        {
            get
            {
                return this.coordinateEditor1.BoundElement;
            }

            set
            {
                this.coordinateEditor1.BoundElement = value;
            }
        }
    }
}
