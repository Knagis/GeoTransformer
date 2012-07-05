/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Windows.Forms;

namespace GeoTransformer
{
    /// <summary>
    /// The execution context implementation used by <see cref="TransformerProgress"/>.
    /// </summary>
    internal class ExecutionContext : Transformers.IExecutionContext
    {
        /// <summary>
        /// Holds the value if the <see cref="TransformerMessageDisplay.SkipRequested"/> has been subscribed to.
        /// </summary>
        private bool MessageDisplaySkipEventSubscribed;

        /// <summary>
        /// Holds a value if the user has requested skipping of the current operation.
        /// </summary>
        private bool SkipRequested;

        /// <summary>
        /// Gets or sets the exception that the transformer should have rethrown.
        /// </summary>
        public Transformers.TransformerCancelledException ExpectedException { get; set; }

        /// <summary>
        /// Gets the index of the task that is executed with this context.
        /// </summary>
        /// <value>
        /// The index of the task.
        /// </value>
        public int TaskIndex { get; private set; }

        /// <summary>
        /// Gets the options that instruct how the transformer should proceed.
        /// </summary>
        public Transformers.TransformerOptions Options { get; private set; }

        /// <summary>
        /// Sets the parent window that should be used if the transformer has to show a dialog user interface.
        /// </summary>
        public TransformProgress ParentWindow { get; private set; }

        /// <summary>
        /// Sets the parent window that should be used if the transformer has to show a dialog user interface.
        /// </summary>
        Form Transformers.IExecutionContext.ParentWindow { get { return this.ParentWindow; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionContext"/> class.
        /// </summary>
        /// <param name="taskIndex">Index of the task.</param>
        /// <param name="form">The form that controls the execution and displays status.</param>
        /// <param name="options">The transformer execution options.</param>
        public ExecutionContext(int taskIndex, TransformProgress form, Transformers.TransformerOptions options)
        {
            this.TaskIndex = taskIndex;
            this.ParentWindow = form;
            this.Options = options;
        }

        /// <summary>
        /// Reports the current status of the execution to the user.
        /// </summary>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="message">The message text.</param>
        /// <param name="args">The arguments to pass to <see cref="String.Format(string, object[])"/> function; if specified, the <paramref name="message"/> is considered
        /// to contain the formatting template.</param>
        public void ReportStatus(Transformers.StatusSeverity severity, string message, params object[] args)
        {
            if (args != null && args.Length > 0)
                message = string.Format(message, args);

            if (severity == Transformers.StatusSeverity.FatalError)
            {
                this.ExpectedException = new Transformers.TransformerCancelledException(message, false, true);
                throw this.ExpectedException;
            }
            else if (severity == Transformers.StatusSeverity.Error)
            {
                var result = this.ParentWindow.Invoke(t => MessageBox.Show(t, "The current step finished with an error: " + Environment.NewLine + Environment.NewLine + message + Environment.NewLine + Environment.NewLine + "Do you want to ignore it and continue with the next step?", "Transformer error", MessageBoxButtons.YesNo, MessageBoxIcon.Question));
                this.ExpectedException = new Transformers.TransformerCancelledException(message, false, result != System.Windows.Forms.DialogResult.Yes);
                throw this.ExpectedException;
            }

            this.ParentWindow.ReportStatus(this.TaskIndex, severity, message);
        }

        /// <summary>
        /// Reports the progress of the transformer execution to the user.
        /// </summary>
        /// <param name="initial">The initial value.</param>
        /// <param name="current">The current value. Automatically forced to be between <paramref name="initial"/> and <paramref name="maximum"/>.</param>
        /// <param name="maximum">The maximum value. Must be greater than or equal to <paramref name="initial"/>.</param>
        /// <param name="skipSupported">Specifies if the transformer supports skipping the long running operation.
        /// To support the skipping the code must handle <see cref="TransformerCancelledException"/> exception thrown
        /// by <see cref="ThrowIfCancellationPending"/> method.</param>
        /// <exception cref="ArgumentException">when <paramref name="maximum"/> is smaller than <paramref name="initial"/>.</exception>
        public void ReportProgress(decimal initial, decimal current, decimal maximum, bool skipSupported = false)
        {
            if (skipSupported && !this.MessageDisplaySkipEventSubscribed)
            {
                this.ParentWindow.GetMessageDisplay(this.TaskIndex).SkipRequested += this.HandleSkipRequested;
                this.MessageDisplaySkipEventSubscribed = true;
            }

            if (!skipSupported)
            {
                if (this.MessageDisplaySkipEventSubscribed)
                {
                    this.ParentWindow.GetMessageDisplay(this.TaskIndex).SkipRequested -= this.HandleSkipRequested;
                    this.MessageDisplaySkipEventSubscribed = false;
                }

                this.SkipRequested = false;
            }

            this.ParentWindow.ReportProgress(this.TaskIndex, initial, current, maximum, skipSupported);
        }

        /// <summary>
        /// Handles the <see cref="TransformerMessageDisplay.SkipRequested"/> event.
        /// </summary>
        private void HandleSkipRequested(object sender, EventArgs e)
        {
            this.SkipRequested = true;
        }

        /// <summary>
        /// Throws a <see cref="TransformerCancelledException"/> exception if the engine has requested the transformer to cancel execution.
        /// </summary>
        public void ThrowIfCancellationPending()
        {
            if (this.ParentWindow.WorkerShouldCancel)
            {
                this.ExpectedException = new Transformers.TransformerCancelledException("User cancelled", false, true);
                throw this.ExpectedException;
            }

            if (this.SkipRequested)
            {
                throw new Transformers.TransformerCancelledException("User requested to skip the operation", true, false);
            }
        }

        /// <summary>
        /// Reports that the long running operation has finished and the progress bar can be hidden.
        /// </summary>
        public void ReportProgressFinished()
        {
            if (this.MessageDisplaySkipEventSubscribed)
            {
                this.ParentWindow.GetMessageDisplay(this.TaskIndex).SkipRequested -= this.HandleSkipRequested;
                this.MessageDisplaySkipEventSubscribed = false;
            }

            this.SkipRequested = false;

            var msg = this.ParentWindow.GetMessageDisplay(this.TaskIndex);
            msg.Invoke(m => m.ProgressFinished());
        }
    }
}
