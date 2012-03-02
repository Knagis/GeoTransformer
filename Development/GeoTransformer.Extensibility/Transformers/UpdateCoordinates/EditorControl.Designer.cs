namespace GeoTransformer.Transformers.UpdateCoordinates
{
    partial class EditorControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.singletonTooltip = new GeoTransformer.UI.SingletonTooltip();
            this.coordinateEditor1 = new GeoTransformer.UI.CoordinateEditor();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Updated coordinates:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.singletonTooltip.SetToolTip(this.label1, "Enter the corrected coordinates for the cache. These will usually\r\nbe the coordin" +
        "ates you obtained by solving a mystery cache or\r\na step in a multi-cache.");
            // 
            // coordinateEditor1
            // 
            this.coordinateEditor1.BackColor = System.Drawing.Color.Transparent;
            this.coordinateEditor1.BoundElement = null;
            this.coordinateEditor1.Coordinates = null;
            this.coordinateEditor1.Location = new System.Drawing.Point(159, 3);
            this.coordinateEditor1.Name = "coordinateEditor1";
            this.coordinateEditor1.Size = new System.Drawing.Size(200, 20);
            this.coordinateEditor1.TabIndex = 2;
            // 
            // EditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.coordinateEditor1);
            this.Name = "EditorControl";
            this.Size = new System.Drawing.Size(362, 26);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private UI.SingletonTooltip singletonTooltip;
        private UI.CoordinateEditor coordinateEditor1;
    }
}
