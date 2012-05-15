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

namespace GeoTransformer.Transformers.MarkSolved
{
    internal partial class EditorControl : UI.UserControlBase
    {
        public EditorControl()
        {
            InitializeComponent();

            this.checkBox.CheckedChanged += this.checkBox_CheckedChanged;
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            var handler = this.Events[ValueChangedEvent] as EventHandler;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private static object ValueChangedEvent = new object();

        /// <summary>
        /// Occurs when the user changes the value of the checkbox.
        /// </summary>
        public event EventHandler ValueChanged
        {
            add { this.Events.AddHandler(ValueChangedEvent, value); }
            remove { this.Events.RemoveHandler(ValueChangedEvent, value); }
        }

        /// <summary>
        /// Gets or sets a value for this editor (the checkbox).
        /// </summary>
        /// <value>
        ///   <c>true</c> if the checkbox is checked; otherwise, <c>false</c>.
        /// </value>
        public bool Value
        {
            get
            {
                return this.checkBox.Checked;
            }

            set
            {
                this.checkBox.Checked = value;
            }
        }
    }
}
