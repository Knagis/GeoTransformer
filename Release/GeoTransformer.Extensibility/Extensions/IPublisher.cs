/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;

namespace GeoTransformer.Extensions
{
    /// <summary>
    /// An extension that provides options for the user to publish (export) data to devices or files.
    /// </summary>
    public interface IPublisher : IExtension
    {
        /// <summary>
        /// Gets the needed transformers marked with <see cref="ISpecial"/> interface that will publish the data
        /// for the given <paramref name="target"/>. The transformers are added to the list of standard transformers.
        /// </summary>
        /// <param name="target">The target that the user has chosen.</param>
        /// <param name="cancel">If set to <c>true</c> the publishing is cancelled without running transformers.</param>
        /// <returns>List of transformers.</returns>
        IEnumerable<ITransformer> GetSpecialTransformers(Publishers.PublisherTarget target, out bool cancel);

        /// <summary>
        /// Occurs when the publisher targets have been changed (for example, due to removable device being removed).
        /// The previous target list for the publisher is overwritten with the new one.
        /// </summary>
        event EventHandler<Publishers.PublisherTargetsChangedEventArgs> TargetsChanged;

        /// <summary>
        /// Called by the application to notify the extension that some of the removable devices have been changed. 
        /// If the extension targets removable drives this method should check if the previously given targets are 
        /// still actual and raise <see cref="TargetsChanged"/> event if they have changed.
        /// </summary>
        void NotifyDevicesChanged();

        /// <summary>
        /// Initializes the publisher and returns the initially available publisher targets.
        /// </summary>
        /// <returns>The publisher targets that are available at this moment.</returns>
        IEnumerable<Publishers.PublisherTarget> Initialize();
    }
}
