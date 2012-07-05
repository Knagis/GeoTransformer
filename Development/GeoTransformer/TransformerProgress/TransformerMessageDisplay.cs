/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace GeoTransformer
{
    /// <summary>
    /// A control for displaying messages in the <see cref="TransformProgress"/> form.
    /// </summary>
    internal class TransformerMessageDisplay : FlowLayoutPanel
    {
        /// <summary>
        /// Gets the level of the most severe message printed.
        /// </summary>
        public Transformers.StatusSeverity WorstSeverity { get; private set; }

        private Label infoLabel;

        private ProgressBar progressBar;

        private Button skipButton;

        private Panel progressPanel;

        private static object SkipRequestedEvent = new object();

        /// <summary>
        /// Occurs when the user requests that the current long running operation is skipped.
        /// </summary>
        internal event EventHandler SkipRequested
        {
            add { this.Events.AddHandler(SkipRequestedEvent, value); }
            remove { this.Events.RemoveHandler(SkipRequestedEvent, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformerMessageDisplay"/> class.
        /// </summary>
        public TransformerMessageDisplay()
        {
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Dock = DockStyle.Fill;

            this.Margin = new Padding(0, 3, 0, 0);
        }

        /// <summary>
        /// Hides the progress bar that is no longer needed since the operation has finished.
        /// </summary>
        public void ProgressFinished()
        {
            if (this.progressPanel != null)
                this.progressPanel.Visible = false;
        }

        /// <summary>
        /// Cleans up the interface from elements that are not needed once the task has completed.
        /// </summary>
        public void TaskFinished()
        {
            if (this.infoLabel != null)
            {
                if (string.IsNullOrWhiteSpace(this.infoLabel.Text))
                    this.infoLabel.Visible = false;
            }

            this.ProgressFinished();
        }

        /// <summary>
        /// Fires the event indicating that the panel has been resized. Inheriting controls should use this in favor of actually listening to the event, but should still call base.onResize to ensure that the event is fired for external listeners.
        /// </summary>
        /// <param name="eventargs">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnResize(System.EventArgs eventargs)
        {
            base.OnResize(eventargs);

            // anchor does not work since this is a flow layout panel
            if (this.progressPanel != null)
                this.progressPanel.Width = this.ClientSize.Width - 6;
        }

        public void ShowProgress(int minimum, int current, int maximum, bool skipSupported)
        {
            if (this.progressPanel == null)
            {
                this.progressPanel = new Panel();
                this.progressPanel.Width = this.ClientSize.Width - 6;
                this.progressPanel.Height = 24;
                this.Controls.Add(this.progressPanel);

                this.progressBar = new ProgressBar();
                this.progressBar.Dock = DockStyle.Fill;
                this.progressPanel.Controls.Add(this.progressBar);

                this.skipButton = new Button();
                this.skipButton.Text = "Skip";
                this.skipButton.Dock = DockStyle.Right;
                this.skipButton.Visible = false;
                this.skipButton.Click += (a, b) =>
                {
                    var h = this.Events[SkipRequestedEvent] as EventHandler;
                    if (h != null)
                        h(this, EventArgs.Empty);
                };
                this.progressPanel.Controls.Add(this.skipButton);
            }

            this.skipButton.Visible = skipSupported;

            this.progressBar.Maximum = maximum;
            this.progressBar.Minimum = minimum;
            this.progressBar.Value = current;
        }

        public void ShowMessage(Transformers.StatusSeverity severity, string message)
        {
            this.SuspendLayout();

            if (severity > this.WorstSeverity)
                this.WorstSeverity = severity;

            Image icon;
            switch (severity)
            {
                default:
                    icon = null;
                    break;

                case GeoTransformer.Transformers.StatusSeverity.Warning:
                    icon = Properties.Resources.Warning;
                    break;

                case GeoTransformer.Transformers.StatusSeverity.Error:
                case GeoTransformer.Transformers.StatusSeverity.FatalError:
                    icon = Properties.Resources.Error;
                    break;
            }

            if (icon != null)
            {
                var img = new PictureBox();
                img.Size = new Size(12, 12);
                img.SizeMode = PictureBoxSizeMode.Zoom;
                img.Margin = new System.Windows.Forms.Padding(1, 3, 1, 1);
                img.Image = icon;

                this.Controls.Add(img);
                if (this.progressPanel != null)
                    this.Controls.SetChildIndex(img, this.Controls.Count - 2);
            }

            Label label;
            if (icon != null || this.infoLabel == null)
            {
                label = new Label();
                label.AutoSize = true;
                label.Margin = new System.Windows.Forms.Padding(1);

                this.Controls.Add(label);
                if (this.progressPanel != null)
                    this.Controls.SetChildIndex(label, this.Controls.Count - 2);
                
                this.SetFlowBreak(label, true);

                if (icon == null)
                    this.infoLabel = label;
            }
            else
            {
                label = this.infoLabel;
                this.Controls.SetChildIndex(this.infoLabel, this.progressPanel == null ? this.Controls.Count : (this.Controls.Count - 2));
            }

            label.Visible = true;
            label.Text = message;

            this.ResumeLayout();
        }
    }
}
