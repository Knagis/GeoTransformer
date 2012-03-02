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

namespace GeoTransformer.UI
{
    /// <summary>
    /// User control that enforces simple margin related settings so that the configuration controls are
    /// aligned in the same style when added to the form. The control enforces flow layout, if more than
    /// one row of controls are required, add a container as the root child control.
    /// </summary>
    public partial class UserControlBase : UserControl
    {
        private static System.Windows.Forms.Layout.LayoutEngine _flowLayoutEngine;
        private static System.Windows.Forms.Layout.LayoutEngine FlowLayoutEngine
        {
            get
            {
                if (_flowLayoutEngine == null)
                    using (var x = new FlowLayoutPanel())
                        _flowLayoutEngine = x.LayoutEngine;
                return _flowLayoutEngine;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserControlBase"/> class.
        /// </summary>
        public UserControlBase()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control uses flow layout.
        /// </summary>
        protected bool UseFlowLayout { get; set; }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.ControlAdded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.ControlEventArgs"/> that contains the event data.</param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (e.Control is Panel)
                e.Control.Margin = new Padding(0);
        }

        /// <summary>
        /// Gets a cached instance of the control's layout engine.
        /// </summary>
        /// <returns>The <see cref="T:System.Windows.Forms.Layout.LayoutEngine"/> for the control's contents.</returns>
        public override System.Windows.Forms.Layout.LayoutEngine LayoutEngine
        {
            get
            {
                return this.UseFlowLayout ? FlowLayoutEngine : base.LayoutEngine;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MinimumSize = new System.Drawing.Size(30, 10);
            this.Name = "ConfigurationControlBase";
            this.Margin = new Padding(0);
            this.ResumeLayout(false);
        }
    }
}
