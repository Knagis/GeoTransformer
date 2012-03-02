/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FetchStatistics
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UISettingsAttribute : Attribute
    {
        public UISettingsAttribute(string headerText)
        {
            this.HeaderText = headerText;
        }

        public string HeaderText { get; set; }

        public bool OrderByDescendingFirst { get; set; }
    }
}
