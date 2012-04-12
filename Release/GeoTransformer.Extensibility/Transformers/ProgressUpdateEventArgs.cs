/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Transformers
{
    /// <summary>
    /// Contains the event arguments for <see cref="Extensions.ITransformer.ProgressUpdate"/> event.
    /// </summary>
    public class ProgressUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressUpdateEventArgs"/> class.
        /// </summary>
        /// <param name="initial">The initial value.</param>
        /// <param name="current">The current value. Automatically forced to be between <paramref name="initial"/> and <paramref name="maximum"/>.</param>
        /// <param name="maximum">The maximum value. Must be greater than or equal to <paramref name="initial"/>.</param>
        /// <exception cref="ArgumentException">when <paramref name="maximum"/> is smaller than <paramref name="initial"/>.</exception>
        public ProgressUpdateEventArgs(decimal initial, decimal current, decimal maximum)
        {
            if (maximum < initial)
                throw new ArgumentException("The maximum value cannot be smaller than initial value.", "maximum");

            if (current < initial)
                current = initial;

            if (current > maximum)
                current = maximum;

            this.InitialValue = initial;
            this.CurrentValue = current;
            this.MaximumValue = maximum;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressUpdateEventArgs"/> class.
        /// </summary>
        /// <param name="current">The current value. Automatically forced to be between <c>zero</c> and <paramref name="maximum"/>.</param>
        /// <param name="maximum">The maximum value. Must be greater than or equal to <c>zero</c>.</param>
        /// <exception cref="ArgumentException">when <paramref name="maximum"/> is smaller than <c>zero</c>.</exception>
        public ProgressUpdateEventArgs(decimal current, decimal maximum)
        {
            if (maximum < 0m)
                throw new ArgumentException("The maximum value cannot be smaller than initial value.", "maximum");

            if (current < 0m)
                current = 0m;

            if (current > maximum)
                current = maximum;

            this.CurrentValue = current;
            this.MaximumValue = maximum;
        }

        /// <summary>
        /// Gets the initial value of the process (typically this would be zero).
        /// </summary>
        public decimal InitialValue { get; private set; }

        /// <summary>
        /// Gets the current value of the process (progress between <see cref="InitialValue"/> and <see cref="MaximumValue"/>.
        /// </summary>
        public decimal CurrentValue { get; private set; }

        /// <summary>
        /// Gets the maximum value of the process (value when the process is completed).
        /// </summary>
        public decimal MaximumValue { get; private set; }
    }
}
