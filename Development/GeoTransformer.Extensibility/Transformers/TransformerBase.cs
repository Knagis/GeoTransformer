/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoTransformer.Transformers
{
    /// <summary>
    /// A base class for transformers providing some basic implementations for <see cref="Extensions.ITransformer"/> interface.
    /// </summary>
    public abstract class TransformerBase : Extensions.ITransformer
    {
        /// <summary>
        /// Stores a value indication whether <see cref="CancelExecution"/> method has been called.
        /// </summary>
        private bool _cancelExecutionCalled;

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Occurs when the transformer has a new status to report to the user.
        /// </summary>
        public event EventHandler<StatusMessageEventArgs> StatusUpdate;

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public abstract ExecutionOrder ExecutionOrder { get; }

        /// <summary>
        /// Reports the status to the user.
        /// </summary>
        /// <param name="message">The message (using format placeholders).</param>
        /// <param name="args">The arguments that are used to format the message.</param>
        protected virtual void ReportStatus(string message, params object[] args)
        {
            if (args == null)
                this.ReportStatus(message);
            else
                this.ReportStatus(string.Format(System.Globalization.CultureInfo.CurrentCulture, message, args));
        }

        /// <summary>
        /// Reports the status to the user.
        /// </summary>
        /// <param name="message">The status message.</param>
        protected virtual void ReportStatus(string message)
        {
            if (this.StatusUpdate != null)
                this.StatusUpdate(this, new StatusMessageEventArgs(message));
        }

        /// <summary>
        /// Reports the status to the user. If the <paramref name="isError"/> is <c>true</c> then the transformer execution is stopped.
        /// </summary>
        /// <param name="isError">if set to <c>true</c> signals that an error occured.</param>
        /// <param name="message">The status message.</param>
        protected virtual void ReportStatus(bool isError, string message)
        {
            if (this.StatusUpdate != null)
                this.StatusUpdate(this, new StatusMessageEventArgs(isError, message));
        }

        /// <summary>
        /// Processes the specified XML files. The default implementation calls <see cref="Process(XDocument)"/> for each file.
        /// </summary>
        /// <param name="xmlFiles">The XML documents currently in the process. The list can be changed if needed</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public virtual void Process(IList<XDocument> xmlFiles, Transformers.TransformerOptions options)
        {
            var total = xmlFiles.Count;
            int i = 0;
            foreach (var xml in xmlFiles)
            {
                i++;
                this.ReportStatus("Processing file " + i + " of " + total);
                this.Process(xml);

                if (i != total) this.TerminateExecutionIfNeeded();
            }

            this.ReportStatus("Processed " + total + " file" + (total % 10 ==1 && total !=11 ? "":"s"));
        }

        /// <summary>
        /// Checks if <see cref="CancelPending"/> is not set to <c>true</c> and if it is then stops the execution of the transformer.
        /// </summary>
        protected void TerminateExecutionIfNeeded()
        {
            if (this._cancelExecutionCalled)
                this.ReportStatus(true, "User cancelled");
        }

        /// <summary>
        /// Processes the specified input document.
        /// </summary>
        /// <param name="input">The XML document that needs processing.</param>
        public virtual void Process(XDocument xml)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Notifies the transformer that the <see cref="Execute"/> method should be now terminated. This is called from a separate thread by the controlling form.
        /// Note that the implementing method has to support multiple calls to <see cref="Process"/> method on the same instance.
        /// </summary>
        void Extensions.ITransformer.CancelExecution()
        {
            this._cancelExecutionCalled = true;
        }

        /// <summary>
        /// Processes the specified XML files. Note that this method can be called multiple times
        /// </summary>
        /// <param name="xmlFiles">The XML files.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        void Extensions.ITransformer.Process(IList<XDocument> xmlFiles, Transformers.TransformerOptions options)
        {
            this._cancelExecutionCalled = false;
            this.Process(xmlFiles, options);
        }
    }
}
