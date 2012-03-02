/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Extensions;

namespace GeoTransformer.Transformers.CreateViewerCache
{
    internal class CreateViewerCache : TransformerBase, ISpecial
    {
        public override string Title
        {
            get { return "Set in-memory copy for viewers"; }
        }

        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.CreateViewerCache; }
        }

        public override void Process(IList<System.Xml.Linq.XDocument> xmlFiles, TransformerOptions options)
        {
            this.Data = xmlFiles;
        }

        public IList<System.Xml.Linq.XDocument> Data { get; private set; }
    }
}
