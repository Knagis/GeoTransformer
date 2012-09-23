namespace FieldNoteManager
{
    partial class Window
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.colLogTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCacheCode = new System.Windows.Forms.DataGridViewLinkColumn();
            this.colCacheTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colLogText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPublish = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnLoad = new System.Windows.Forms.ToolStripButton();
            this.btnPublish = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGrid
            // 
            this.dataGrid.AllowUserToAddRows = false;
            this.dataGrid.AllowUserToDeleteRows = false;
            this.dataGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLogTime,
            this.colCacheCode,
            this.colCacheTitle,
            this.colLogType,
            this.colLogText,
            this.colPublish,
            this.colResult});
            this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGrid.Location = new System.Drawing.Point(0, 27);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.RowTemplate.Height = 24;
            this.dataGrid.Size = new System.Drawing.Size(637, 285);
            this.dataGrid.TabIndex = 0;
            this.dataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellContentClick);
            this.dataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellValueChanged);
            this.dataGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGrid_EditingControlShowing);
            // 
            // colLogTime
            // 
            this.colLogTime.DataPropertyName = "LogTime";
            this.colLogTime.HeaderText = "Log time";
            this.colLogTime.Name = "colLogTime";
            this.colLogTime.ReadOnly = true;
            // 
            // colCacheCode
            // 
            this.colCacheCode.DataPropertyName = "CacheCode";
            this.colCacheCode.HeaderText = "Cache code";
            this.colCacheCode.Name = "colCacheCode";
            this.colCacheCode.ReadOnly = true;
            this.colCacheCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colCacheCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colCacheTitle
            // 
            this.colCacheTitle.DataPropertyName = "CacheTitle";
            this.colCacheTitle.HeaderText = "Cache title";
            this.colCacheTitle.Name = "colCacheTitle";
            this.colCacheTitle.ReadOnly = true;
            // 
            // colLogType
            // 
            this.colLogType.DataPropertyName = "LogType";
            this.colLogType.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colLogType.HeaderText = "Log type";
            this.colLogType.Items.AddRange(new object[] {
            "Found it",
            "Didn\'t find it",
            "Needs repair"});
            this.colLogType.Name = "colLogType";
            this.colLogType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colLogType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colLogText
            // 
            this.colLogText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colLogText.DataPropertyName = "Text";
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colLogText.DefaultCellStyle = dataGridViewCellStyle1;
            this.colLogText.HeaderText = "Log text";
            this.colLogText.Name = "colLogText";
            // 
            // colPublish
            // 
            this.colPublish.DataPropertyName = "ShouldPublish";
            this.colPublish.HeaderText = "Publish?";
            this.colPublish.Name = "colPublish";
            // 
            // colResult
            // 
            this.colResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colResult.DataPropertyName = "Result";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colResult.DefaultCellStyle = dataGridViewCellStyle2;
            this.colResult.HeaderText = "Result";
            this.colResult.Name = "colResult";
            this.colResult.ReadOnly = true;
            this.colResult.Visible = false;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoad,
            this.btnPublish});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(637, 27);
            this.toolStrip.TabIndex = 1;
            // 
            // btnLoad
            // 
            this.btnLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(148, 24);
            this.btnLoad.Text = "Load visits from GPS";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnPublish
            // 
            this.btnPublish.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPublish.Image = ((System.Drawing.Image)(resources.GetObject("btnPublish.Image")));
            this.btnPublish.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPublish.Name = "btnPublish";
            this.btnPublish.Size = new System.Drawing.Size(92, 24);
            this.btnPublish.Text = "Publish logs";
            this.btnPublish.ToolTipText = "Publish the logs to geocaching.com.";
            this.btnPublish.Click += new System.EventHandler(this.btnPublish_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 312);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(637, 25);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(100, 20);
            this.toolStripStatusLabel.Text = "Import a file...";
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGrid);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip);
            this.Name = "Window";
            this.Size = new System.Drawing.Size(637, 337);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnLoad;
        private System.Windows.Forms.ToolStripButton btnPublish;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLogTime;
        private System.Windows.Forms.DataGridViewLinkColumn colCacheCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCacheTitle;
        private System.Windows.Forms.DataGridViewComboBoxColumn colLogType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLogText;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colPublish;
        private System.Windows.Forms.DataGridViewTextBoxColumn colResult;
    }
}
