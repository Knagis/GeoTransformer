/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Data;

namespace GeoTransformer.Extensions
{
    internal class ExtensionConfigurationTable : DatabaseTable, IExtensionTable
    {
        public class DataClass
        {
            public DataClass()
            {
            }
            public DataClass(SelectResult<ExtensionConfigurationTable> result)
            {
                this.ClassName = result.Value(o => o.ClassName);
                this.Configuration = result.Value(o => o.Configuration);
            }

            public string ClassName { get; private set; }
            public byte[] Configuration { get; private set; }
        }

        public ExtensionConfigurationTable()
            : base("ExtensionConfiguration", 1)
        {
            this.AddColumn<string>("ClassName", 1);
            this.AddColumn<byte[]>("Configuration", 1);

            this.AddIndex(new DatabaseIndex(this, "ClassName", true, 1, this.GetColumn("ClassName")));
        }

        /// <summary>
        /// Gets the <see cref="Type.FullName"/> of the class for which the configuration is stored.
        /// </summary>
        public DatabaseColumn<string> ClassName { get { return this.GetColumn<string>("ClassName"); } }

        /// <summary>
        /// Gets the configuration for the extension.
        /// </summary>
        public DatabaseColumn<byte[]> Configuration { get { return this.GetColumn<byte[]>("Configuration"); } }

        public IDictionary<string, DataClass> RetrieveAll()
        {
            var q = this.Select();
            q.SelectAll();
            return q.Execute().AsEnumerable().Select(o => new DataClass(o)).ToDictionary(o => o.ClassName, o => o);
        }
    }
}
