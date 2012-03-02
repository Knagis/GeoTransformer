/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Data
{
    /// <summary>
    /// An interface that marks <see cref="DatabaseTable"/> classes that should be stored in the main database file.
    /// Such table is identified by the full type name in the database so it should not be changed in order to preserve data.
    /// </summary>
    public interface IExtensionTable : IDatabaseTable, Extensions.IExtension
    {
    }
}
