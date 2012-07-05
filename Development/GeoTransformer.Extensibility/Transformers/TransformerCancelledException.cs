/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;

namespace GeoTransformer.Transformers
{
    /// <summary>
    /// An exception that is thrown when the transformer execution has been interrupted. The code that catches this exception has to check
    /// the <see cref="CanContinue"/> property - if it is <c>false</c> the exception must be rethrown.
    /// </summary>
    [Serializable]
    public class TransformerCancelledException : Exception
    {
        /// <summary>
        /// Gets the value that indicates if the current transformer is allowed to continue with the execution.
        /// If the transformer caught this exception and the value is <c>False</c> it must be rethrown.
        /// </summary>
        public bool CanContinue { get; private set; }

        /// <summary>
        /// Gets the value that indicates if the whole transformation process is about to be stopped.
        /// </summary>
        public bool TransformationStopping { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformerCancelledException"/> class.
        /// </summary>
        public TransformerCancelledException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformerCancelledException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="canContinue">if set to <c>true</c> indicates that the current transformer is allowed to continue.</param>
        /// <param name="transformationStopping">if set to <c>true</c> indicates that the whole transformation process is stopping.</param>
        internal TransformerCancelledException(string message, bool canContinue, bool transformationStopping)
            : base(message)
        {
            this.CanContinue = canContinue;
            this.TransformationStopping = transformationStopping;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformerCancelledException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public TransformerCancelledException(string message) : base(message) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformerCancelledException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public TransformerCancelledException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformerCancelledException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected TransformerCancelledException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) 
        {
            this.CanContinue = info.GetBoolean("CanContinue");
            this.TransformationStopping = info.GetBoolean("TransformationStopping");
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception>
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("CanContinue", this.CanContinue);
            info.AddValue("TransformationStopping", this.TransformationStopping);
        }

    }
}
