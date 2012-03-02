/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace GeoTransformer.Data
{
    public class Database : IDisposable
    {
        private SQLiteConnection _connection;

        public Database(string databaseFileName)
        {
            if (string.IsNullOrEmpty(databaseFileName))
                throw new ArgumentNullException("databaseFileName");

            this._connection = new SQLiteConnection("Data Source=\"" + databaseFileName.Replace("\"", "\"\"") + "\"");
            this._connection.Open();
        }

        public int ExecuteNonQuery(string sql, params object[] orderedParameters)
        {
            var command = this._connection.CreateCommand();
            command.CommandText = sql;
            if (orderedParameters != null)
                foreach (var val in orderedParameters)
                    command.Parameters.AddWithValue(null, val);

#if DEBUG
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = command.ExecuteNonQuery();
            sw.Stop();
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToShortTimeString() + " SQL (" + sw.ElapsedMilliseconds + "ms): " + sql);
            return result;
#else
            return command.ExecuteNonQuery();
#endif
        }
        public SQLiteDataReader ExecuteReader(string sql, params object[] orderedParameters)
        {
            var command = this._connection.CreateCommand();
            command.CommandText = sql;
            if (orderedParameters != null)
                foreach (var val in orderedParameters)
                    command.Parameters.AddWithValue(null, val);

#if DEBUG
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = command.ExecuteReader();
            sw.Stop();
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToShortTimeString() + " SQL (" + sw.ElapsedMilliseconds + "ms): " + sql);
            return result;
#else
            return command.ExecuteReader();
#endif
        }
        public TResult ExecuteScalar<TResult>(string sql, params object[] orderedParameters)
        {
            var command = this._connection.CreateCommand();
            command.CommandText = sql;
            if (orderedParameters != null)
                foreach (var val in orderedParameters)
                    command.Parameters.AddWithValue(null, val);

#if DEBUG
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = command.ExecuteScalar();
            sw.Stop();
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToShortTimeString() + " SQL (" + sw.ElapsedMilliseconds + "ms): " + sql);
#else
            var result = command.ExecuteScalar();
#endif
            return Convert.ChangeType<TResult>(result, System.Globalization.CultureInfo.InvariantCulture);
        }
        public long LastInsertRowId()
        {
            return this.ExecuteScalar<long>("select last_insert_rowid()");
        }

        public SQLiteTransaction BeginTransaction()
        {
            return this._connection.BeginTransaction();
        }

        #region IDisposable Members

        private bool _disposed;

        ~Database()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
                return;

            this._connection.Close();

            this._disposed = true;
        }

        #endregion
    }
}
