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
using System.Threading.Tasks;

namespace GeoTransformer
{
    /// <summary>
    /// The form that executes transformers and displays the progress while doing so.
    /// </summary>
    public partial class TransformProgress : Form
    {
        private List<ITransformer> _transformers;
        private Transformers.TransformerOptions _options;
        private Form _parentWindow;

        private System.Threading.Thread _worker;
        private bool _workerShouldCancel;
        private bool _performingAutoClose;

        private Dictionary<int, TransformerMessageDisplay> _messageDisplays = new Dictionary<int, TransformerMessageDisplay>();
        private Dictionary<int, PictureBox> _pictureBoxes = new Dictionary<int, PictureBox>();

        /// <summary>
        /// Queue of operations that are pending because the form is not visible.
        /// </summary>
        /// <remarks>
        /// In order to enhance performance, no status update actions are executed while the form is minimized.
        /// </remarks>
        private System.Collections.Concurrent.ConcurrentQueue<Action> _pendingOperations = new System.Collections.Concurrent.ConcurrentQueue<Action>();

        /// <summary>
        /// Gets a value indicating whether the worker process should cancel itself.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the worker process should cancel; otherwise, <c>false</c>.
        /// </value>
        internal bool WorkerShouldCancel
        {
            get { return this._workerShouldCancel; }
        }

        /// <summary>
        /// Returns the <see cref="TransformerMessageDisplay"/> control for the given task.
        /// </summary>
        internal TransformerMessageDisplay GetMessageDisplay(int taskIndex)
        {
            return this._messageDisplays[taskIndex];
        }

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

            this._transformers = transformers.ToList();
            this._options = options;

            var pendingRemove = new List<ITransformer>();
            pendingRemove.AddRange(this._transformers.OfType<Extensions.IRelatedConditional>().Where(o => !o.IsEnabled(this._transformers)).Cast<ITransformer>());
            pendingRemove.AddRange(this._transformers.OfType<Extensions.IConditional>().Where(o => !o.IsEnabled).Cast<ITransformer>());
            pendingRemove.ForEach(o => this._transformers.Remove(o));

            this._transformers.Sort((a, b) => Comparer<int>.Default.Compare((int)a.ExecutionOrder, (int)b.ExecutionOrder));

            this.statusTable.RowCount = this._transformers.Count + 1;
            int i = 0;
            foreach (var tr in this._transformers)
                this.StatusTableCreateRow(i++, tr);
        }

        /// <summary>
        /// Initializes the row in the status table for the given transformer.
        /// </summary>
        /// <param name="taskIndex">The number of the transformator in the execution list.</param>
        /// <param name="transformer">The transformer instance.</param>
        private void StatusTableCreateRow(int taskIndex, Extensions.ITransformer transformer)
        {
            // +1 because of the header
            var row = taskIndex + 1;

            this.statusTable.RowStyles.Add(new RowStyle(SizeType.AutoSize, 0));

            var icon = new PictureBox() 
            { 
                Size = new Size(24, 24), 
                Margin = new Padding(0), 
                Image = Properties.Resources.Pending 
            };
            this.statusTable.Controls.Add(icon);
            this.statusTable.SetRow(icon, row);
            this.statusTable.SetColumn(icon, 0);

            var titleLabel = new Label()
            {
                Text = transformer.Title,
                Padding = new System.Windows.Forms.Padding(3, 4, 3, 3),
                Dock = DockStyle.Fill,
                AutoSize = true,
                TextAlign = ContentAlignment.TopLeft
            };
            this.statusTable.Controls.Add(titleLabel);
            this.statusTable.SetRow(titleLabel, row);
            this.statusTable.SetColumn(titleLabel, 1);

            var messages = new TransformerMessageDisplay();
            this.statusTable.Controls.Add(messages);
            this.statusTable.SetRow(messages, row);
            this.statusTable.SetColumn(messages, 2);

            this._messageDisplays[taskIndex] = messages;
            this._pictureBoxes[taskIndex] = icon;
        }

        /// <summary>
        /// Starts the execution of the transformers.
        /// </summary>
        /// <param name="parentWindow">If the execution is started without the form being visible, set this parameter.</param>
        public void StartExecution(Form parentWindow = null)
        {
            this._parentWindow = parentWindow;

            if (this._worker != null)
                return;

            this._worker = new System.Threading.Thread(WorkerEntry);
            this._worker.Name = "Transformer runner";
            this._worker.SetApartmentState(System.Threading.ApartmentState.STA);
            this._worker.Start();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            this.ResizeForm(true);
            this.BringToFront();
            base.OnLoad(e);
            this.StartExecution();
        }

        /// <summary>
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // resizing can reduce the height of the status table (since the labels can be wrapped before thus taking up more space)
            this.MaximumSize = new Size(1000, this.statusTable.Height + this.toolStrip.Height + this.panel.Padding.Top + this.panel.Padding.Bottom + this.Height - this.ClientSize.Height);
        }

        /// <summary>
        /// Resizes the form according to the contents.
        /// </summary>
        /// <param name="canGrow">if set to <c>true</c> the form can be grown in size. Setting this to <c>false</c> will ensure that if the user resized the form himself, it will not grow again</param>
        private void ResizeForm(bool canGrow)
        {
            if (!this.Visible)
                return;

            bool resize = canGrow || (this.Size.Height == this.MaximumSize.Height);

            this.MinimumSize = new Size(600, 300);
            this.MaximumSize = new Size(1000, this.statusTable.Height + this.toolStrip.Height + this.panel.Padding.Top + this.panel.Padding.Bottom + this.Height - this.ClientSize.Height);

            if (resize)
                this.ClientSize = new Size(this.ClientSize.Width, Math.Min(Screen.FromControl(this).WorkingArea.Height * 75 / 100, this.statusTable.Height + this.toolStrip.Height + this.panel.Padding.Top + this.panel.Padding.Bottom));
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
                }
            }
        }

        /// <summary>
        /// Reports the progress to the user interface.
        /// </summary>
        /// <param name="taskIndex">Index of the task that reports the progress.</param>
        /// <param name="initial">The initial value.</param>
        /// <param name="current">The current value. Automatically forced to be between <paramref name="initial"/> and <paramref name="maximum"/>.</param>
        /// <param name="maximum">The maximum value. Must be greater than or equal to <paramref name="initial"/>.</param>
        /// <param name="skipSupported">Specifies if the transformer supports skipping the long running operation.</param>
        /// <remarks>
        /// The method does not do anything if the form is not visible.
        /// </remarks>
        internal void ReportProgress(int taskIndex, decimal initial, decimal current, decimal maximum, bool skipSupported)
        {
            // ignore progress updates completely while hidden.
            // when the form is shown either a fresh progress update will be executed or the task will have completed.
            if (!this.Visible)
                return;

            if (this.InvokeRequired)
            {
                this.Invoke(t => t.ReportProgress(taskIndex, initial, current, maximum, skipSupported));
                return;
            }

            var msg = this._messageDisplays[taskIndex];
            msg.ShowProgress((int)initial, (int)current, (int)maximum, skipSupported);
            this.ResizeForm(false);
        }

        /// <summary>
        /// Updates the user interface to notify that the execution of a task has been completed.
        /// </summary>
        /// <param name="taskIndex">Index of the task.</param>
        private void ReportDone(int taskIndex)
        {
            if (!this.Visible)
            {
                this._pendingOperations.Enqueue(() => this.ReportDone(taskIndex));
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(t => t.ReportDone(taskIndex));
                return;
            }

            var msg = this._messageDisplays[taskIndex];
            var icon = this._pictureBoxes[taskIndex];

            msg.TaskFinished();

            switch(msg.WorstSeverity)
            {
                case Transformers.StatusSeverity.Error:
                case Transformers.StatusSeverity.FatalError:
                    icon.Image = Properties.Resources.Error;
                    break;

                case Transformers.StatusSeverity.Warning:
                    icon.Image = Properties.Resources.Warning;
                    break;

                default:
                    icon.Image = Properties.Resources.Done;
                    break;
            }

            this.ResizeForm(false);
        }

        internal void ReportStatus(int taskIndex, Transformers.StatusSeverity severity, string message)
        {
            if (!this.Visible)
            {
                this._pendingOperations.Enqueue(() => this.ReportStatus(taskIndex, severity, message));

                if (severity > Transformers.StatusSeverity.Information)
                {
                    this.AllowAutomaticClose = false;
                    this._parentWindow.Invoke(o => this.Show(o));
                }

                return;
            }
            
            if (this.InvokeRequired)
            {
                this.Invoke(t => t.ReportStatus(taskIndex, severity, message));
                return;
            }

            var disp = this._messageDisplays[taskIndex];
            disp.ShowMessage(severity, message);
            
            this.ResizeForm(false);
        }

        private void WorkerEntry()
        {
            var data = new List<Gpx.GpxDocument>();

            int i = 0;
            foreach (var task in this._transformers)
            {
                var context = new ExecutionContext(i, this, this._options);

                try
                {
                    task.Process(data, context);
                }
                catch (Transformers.TransformerCancelledException ex)
                {
                    if (context.ExpectedException == ex)
                        context.ExpectedException = null;

                    this.ReportStatus(i, ex.TransformationStopping ? Transformers.StatusSeverity.FatalError : Transformers.StatusSeverity.Error, ex.Message);

                    if (ex.TransformationStopping)
                        break;
                }
                catch (Exception ex)
                {
                    this.ReportStatus(i, Transformers.StatusSeverity.FatalError, ex.Message + Environment.NewLine + ex.ToString());

                    var result = this.Invoke(t => MessageBox.Show(t, "The current step finished with a fatal error. It is possible to ignore it but it may cause unexpected behavior or corrupted data." + Environment.NewLine + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine + "Do you want to ignore it and continue with the next step?", "Transformer error", MessageBoxButtons.YesNo, MessageBoxIcon.Error));
                    if (result == System.Windows.Forms.DialogResult.No)
                        break;
                }
                finally
                {
                    if (context.ExpectedException != null)
                        this.ReportStatus(i, Transformers.StatusSeverity.FatalError, "The transformer has suppressed a TransformerCancelledException that is not allowed to be suppressed: " + context.ExpectedException.Message);

                    this.ReportDone(i);
                }

                i++;
            }

            this.Invoke(() => this.toolStripClose.Text = "Close");

            if (this.AllowAutomaticClose)
            {
                if (!this.Visible)
                {
                    this.OnFormClosed(new FormClosedEventArgs(CloseReason.None));
                }
                else
                {
                    this._performingAutoClose = true;
                    this.BeginInvoke(o => o.Close());
                }
            }
        }

        private void TransformProgress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                this.Close();
        }

        private void TransformProgress_VisibleChanged(object sender, EventArgs e)
        {
            var queue = this._pendingOperations;

#if DEBUG
            // safety check for development - in theory the queue would not be used anymore, but to make sure we don't lose any messages, it is set to null (better to get an exception).
            this._pendingOperations = null;
#endif

            this.SuspendLayout();

            Action act;
            while (queue.TryDequeue(out act))
                act();

            this.ResumeLayout(true);
        }
    }
}
