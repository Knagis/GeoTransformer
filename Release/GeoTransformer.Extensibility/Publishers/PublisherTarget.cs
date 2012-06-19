/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;

namespace GeoTransformer.Publishers
{
    /// <summary>
    /// A class that describes a menu item for a specific publishing action.
    /// </summary>
    /// <remarks>
    /// The class can be derived from if the extension needs to store more information with it.
    /// </remarks>
    public class PublisherTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherTarget"/> class.
        /// </summary>
        /// <param name="publisher">The publisher extension that will handle the action if it is chosen by the user.</param>
        /// <param name="text">The text that is displayed on the menu button.</param>
        /// <param name="icon">The icon that is displayed on the menu button.</param>
        /// <param name="key">The key that is used to uniquely identify this action. The key is used to store frequently accessed actions.</param>
        public PublisherTarget(Extensions.IPublisher publisher, string text, System.Drawing.Image icon = null, Guid? key = null, object tag = null)
        {
            if (publisher == null)
                throw new ArgumentNullException("publisher");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            this.Publisher = publisher;
            this.Icon = icon;
            this.Text = text;
            this.Children = new List<PublisherTarget>();
            this.Tag = tag;

            if (key == null)
            {
                var p1 = BitConverter.GetBytes(this.Publisher.GetType().FullName.GetHashCode());
                var p2 = BitConverter.GetBytes(this.Text.GetHashCode());
                byte[] mas = new byte[16];
                Array.Copy(p1, mas, p1.Length);
                Array.Copy(p2, 0, mas, p1.Length, p2.Length);
                this.Key = new Guid(mas);
            }
            else
            {
                this.Key = key.Value;
            }
        }

        /// <summary>
        /// Gets the publisher extension that will handle the action if it is chosen by the user.
        /// </summary>
        public Extensions.IPublisher Publisher
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the icon that is displayed on the menu button.
        /// </summary>
        public System.Drawing.Image Icon
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the text that is displayed on the menu button.
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the key that is used to uniquely identify this action. The key is used to store frequently accessed actions.
        /// </summary>
        public Guid Key
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the child menu items. If the collection is not empty, a drop down menu is created and the parent itself does not invoke the publisher.
        /// </summary>
        public IList<PublisherTarget> Children
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a related object that is used by the publisher to identify each target. Should not be modified by anything else.
        /// </summary>
        public object Tag
        {
            get;
            set;
        }
    }
}
