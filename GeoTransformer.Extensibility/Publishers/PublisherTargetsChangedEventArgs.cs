/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Publishers
{
    /// <summary>
    /// Arguments for the <see cref="Extensions.IPublisher.TargetsChanged"/> event.
    /// </summary>
    public class PublisherTargetsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherTargetsChangedEventArgs"/> class.
        /// </summary>
        /// <param name="targets">The list of publisher targets that have to be shown on the form.</param>
        public PublisherTargetsChangedEventArgs(IList<Publishers.PublisherTarget> targets)
        {
            if (targets == null)
                throw new ArgumentNullException("targets");

            this.Targets = new System.Collections.ObjectModel.ReadOnlyCollection<Publishers.PublisherTarget>(targets);
        }

        /// <summary>
        /// Gets the list of publisher targets that have to be shown on the form.
        /// </summary>
        public IList<Publishers.PublisherTarget> Targets { get; private set; }
    }
}
