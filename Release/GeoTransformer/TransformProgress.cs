/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoTransformer.Extensions;

namespace GeoTransformer
{
    /// <summary>
    /// The form that executes transformers and displays the progress while doing so.
    /// </summary>
    public partial class TransformProgress : Form
    {
        private class StatusMessage
        {
            public int TaskIndex;
            public string Message;
            public bool Complete;
            public bool Error;
        }

        private delegate void StatusReportDelegate(StatusMessage message);

        private List<ITransformer> _transformers;
        private ITransformer _currentTransformer;
        private Transformers.TransformerOptions _options;

        private System.Threading.Thread _worker;
        private bool _workerShouldCancel;
        private bool _performingAutoClose;
        private bool _gotAnError;

        /// <summary>
        /// Gets or sets a value indicating whether the form is allowed to automatically close in case everything completed successfully. Default is <c>false</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the automatic closing is allowed; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAutomaticClose { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformProgress"/> class.
        /// </summary>
        /// <param name="transformers">The transformers that will be executed.</param>
        /// <param name="options">The options that will be passed to the transformers.</param>
        public TransformProgress(IEnumerable<ITransformer> transformers, Transformers.TransformerOptions options)
        {
            InitializeComponent();

            this.imageList.Images.Add("Done", Properties.Resources.Done);
            this.imageList.Images.Add("Error", Properties.Resources.Error);
            this.imageList.Images.Add("Pending", Properties.Resources.Pending);

            this._options = options;
            this._transformers = transformers.ToList();

            var pendingRemove = new List<ITransformer>();
            pendingRemove.AddRange(this._transformers.OfType<Extensions.IRelatedConditional>().Where(o => !o.IsEnabled(this._transformers)).Cast<ITransformer>());
            pendingRemove.AddRange(this._transformers.OfType<Extensions.IConditional>().Where(o => !o.IsEnabled).Cast<ITransformer>());
            pendingRemove.ForEach(o => this._transformers.Remove(o));

            this._transformers.Sort((a, b) => Comparer<int>.Default.Compare((int)a.ExecutionOrder, (int)b.ExecutionOrder));

            foreach (var tr in this._transformers)
            {
                var i = new ListViewItem(tr.Title, "Pending");
                i.SubItems.Add("");
                this.listView.Items.Add(i);
            }

            var lastItemRect = this.listView.GetItemRect(this.listView.Items.Count - 1);
            lastItemRect = this.listView.RectangleToScreen(lastItemRect);
            var wantedHeight = lastItemRect.Y + lastItemRect.Height - 2;
            var actualHeight = this.listView.ClientSize.Height;
            this.Height += wantedHeight - actualHeight;

            this._worker = new System.Threading.Thread(WorkerEntry);
            this._worker.SetApartmentState(System.Threading.ApartmentState.STA);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this._worker.Start();
        }

        private void toolStripClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TransformProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._worker.IsAlive && !this._performingAutoClose)
            {
                e.Cancel = true;
                if (this._workerShouldCancel)
                    return;

                DialogResult result = System.Windows.Forms.DialogResult.Yes;
                
                // if this is the background operation, then just close the form without complaining, otherwise ask the user
                if (!this.AllowAutomaticClose)
                    result = MessageBox.Show("Do you want to cancel the operation?", "Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    if (!this._worker.IsAlive)
                        e.Cancel = false;

                    this._workerShouldCancel = true;
                    this._currentTransformer.CancelExecution();
                }
            }
        }

        private void ReportProgress(StatusMessage message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((t, m) => t.ReportProgress(m), message);
                return;
            }

            var item = this.listView.Items[message.TaskIndex];

            if (message.Error)
            {
                this._gotAnError = true;
                item.ImageKey = "Error";

                if (this.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Normal;
            }
            else if (message.Complete)
                item.ImageKey = "Done";

            if (message.Message != null)
                item.SubItems[1].Text = message.Message;
        }

        private void WorkerEntry()
        {
            var i = 0;
            IList<System.Xml.Linq.XDocument> data = new List<System.Xml.Linq.XDocument>();
            foreach (var task in this._transformers)
            {
                if (this._workerShouldCancel)
                {
                    this.ReportProgress(new StatusMessage() { TaskIndex = i, Error = true, Message = "User cancelled" });
                    return;
                }

                bool clearMessage = true;
                this._currentTransformer = task;
                this.ReportProgress(new StatusMessage() { TaskIndex = i, Message = "Processing..." });

                var requiresPopup = task as ITransformerWithPopup;
                if (requiresPopup != null)
                    requiresPopup.ParentWindow = this;

                EventHandler<Transformers.StatusMessageEventArgs> handler = (sender, args) => 
                { 
                    clearMessage = false; 
                    if (args.IsError) 
                        throw new Exception(args.Message); 
                    this.ReportProgress(new StatusMessage() { TaskIndex = i, Message = args.Message });
                };

                try
                {
                    task.StatusUpdate += handler;
                    task.Process(data, this._options);
                    this.ReportProgress(new StatusMessage() { TaskIndex = i, Complete = true, Message = clearMessage ? "" : null });
                }
                catch (Exception ex)
                {
                    this.ReportProgress(new StatusMessage() { TaskIndex = i, Error = true, Message = ex.Message });
                    this.Invoke(new Action(() => this.toolStripClose.Text = "Close"));
                    return;
                }
                finally
                {
                    task.StatusUpdate -= handler;
                    if (requiresPopup != null)
                        requiresPopup.ParentWindow = null;
                }

                i++;
            }

            this.Invoke(new Action(() => this.toolStripClose.Text = "Close"));

            if (!this._gotAnError && this.AllowAutomaticClose)
            {
                this._performingAutoClose = true;
                this.BeginInvoke(o => o.Close());
            }
        }

        private void TransformProgress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                this.Close();
        }
    }
}
