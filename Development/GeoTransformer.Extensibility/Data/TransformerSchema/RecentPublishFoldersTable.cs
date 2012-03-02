/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Data.TransformerSchema
{
    internal class RecentPublishFoldersTable : DatabaseTable
    {
        public RecentPublishFoldersTable()
            : base("RecentPublishFolders", 3)
        {
            this.AddColumn<string>("Path", 3);
            this.AddColumn<DateTime>("LastUsed", 3);

            this.AddIndex(new DatabaseIndex(this, "Path", true, 3, this.Path));
        }

        public DatabaseColumn<string> Path { get { return this.GetColumn<string>("Path"); } }
        public DatabaseColumn<DateTime> LastUsed { get { return this.GetColumn<DateTime>("LastUsed"); } }

        public void SaveRecentFolder(string path)
        {
            var di = new System.IO.DirectoryInfo(path);
            path = di.FullName;
            var rq = this.Replace();
            rq.Value(o => o.Path, path);
            rq.Value(o => o.LastUsed, DateTime.Now.ToUniversalTime());
            rq.Execute();

            var dq = this.Select();
            dq.Select(o => o.RowId);
            dq.OrderBy(o => o.LastUsed, false);
            using (var res = dq.Execute())
            {
                for (int i = 0; i < 5; i++)
                    res.Read();
                while (res.Read())
                {
                    var id = res.Value(o => o.RowId);
                    var dquery = this.Delete();
                    dquery.Where(o => o.RowId, id);
                    dquery.Execute();
                }
            }
        }

        public IEnumerable<string> ReadPaths()
        {
            var dq = this.Select();
            dq.Select(o => o.Path);
            dq.OrderBy(o => o.LastUsed, false);
            foreach (var res in dq.Execute())
            {
                yield return res.Value(o => o.Path);
            }
        }
    }
}
