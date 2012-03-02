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
    public class Extension : GeoTransformer.Extensions.ITopLevelTabPage
    {
        public System.Drawing.Image TabPageImage
        {
            get { return null; }
        }

        public string TabPageTitle
        {
            get { return "LV statistics"; }
        }

        public System.Windows.Forms.Control Initialize()
        {
            return new Window();
        }
    }
}
