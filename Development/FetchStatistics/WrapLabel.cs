/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace FetchStatistics
{
    public partial class WrapLabel : Label
    {
        #region  Public Constructors

        public WrapLabel()
        {
            this.AutoSize = false;
        }

        #endregion

        #region  Protected Overridden Methods

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.FitToContents();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.FitToContents();
        }

        #endregion

        #region  Protected Virtual Methods

        protected virtual void FitToContents()
        {
            var size = this.GetPreferredSize(new System.Drawing.Size(this.Width, 0));
            this.Height = size.Height;
        }

        #endregion  Protected Virtual Methods

        #region  Public Properties

        [DefaultValue(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
        }

        #endregion  Public Properties
    }
}
