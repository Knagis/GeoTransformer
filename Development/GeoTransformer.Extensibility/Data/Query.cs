/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GeoTransformer.Data;

namespace GeoTransformer
{
    public static class QueryExtensions
    {
        public static InsertUpdateQuery<TTable> Insert<TTable>(this TTable table)
            where TTable : DatabaseTable, IDatabaseTable
        {
            return new InsertUpdateQuery<TTable>(table, InsertUpdateQueryMode.Insert);
        }

        public static InsertUpdateQuery<TTable> Update<TTable>(this TTable table)
            where TTable : DatabaseTable, IDatabaseTable
        {
            return new InsertUpdateQuery<TTable>(table, InsertUpdateQueryMode.Update);
        }

        public static InsertUpdateQuery<TTable> Replace<TTable>(this TTable table)
            where TTable : DatabaseTable, IDatabaseTable
        {
            return new InsertUpdateQuery<TTable>(table, InsertUpdateQueryMode.Replace);
        }

        public static SelectQuery<TTable> Select<TTable>(this TTable table)
            where TTable : DatabaseTable, IDatabaseTable
        {
            return new SelectQuery<TTable>(table);
        }

        public static DeleteQuery<TTable> Delete<TTable>(this TTable table)
            where TTable : DatabaseTable, IDatabaseTable
        {
            return new DeleteQuery<TTable>(table);
        }
    }
}

namespace GeoTransformer.Data
{
    public enum InsertUpdateQueryMode
    {
        Insert,
        Update,
        Replace
    }

    public enum WhereOperator
    {
        Equal,
        NotEqual,
        Greater,
        Smaller,
        GreaterOrEqual,
        SmallerOrEqual,
        NotIsNull,
        IsNull,
        StringLike
    }

    public class WhereCondition
    {
        public IDatabaseColumn Column { get; set; }
        public WhereOperator Operator { get; set; }
        public object Value { get; set; }
        
        private string _groupingKey;
        public string GroupingKey
        {
            get
            {
                if (this._groupingKey == null)
                {
                    if (this.Operator == WhereOperator.Equal || this.Operator == WhereOperator.StringLike)
                        return this.Column.ToString();
                    return Guid.NewGuid().ToString();
                }
                return this._groupingKey;
            }
            set
            {
                this._groupingKey = value;
            }
        }

        public bool IsValueUsed
        {
            get
            {
                return this.Operator != WhereOperator.NotIsNull && this.Operator != WhereOperator.IsNull;
            }
        }

        public override string ToString()
        {
            switch (this.Operator)
            {
                case WhereOperator.Equal:
                    return this.Column + " = ?";
                case WhereOperator.NotEqual:
                    return this.Column + " <> ?";
                case WhereOperator.Greater:
                    return this.Column + " > ?";
                case WhereOperator.Smaller:
                    return this.Column + " < ?";
                case WhereOperator.GreaterOrEqual:
                    return this.Column + " >= ?";
                case WhereOperator.SmallerOrEqual:
                    return this.Column + " <= ?";
                case WhereOperator.IsNull:
                    return this.Column + " is null";
                case WhereOperator.NotIsNull:
                    return "not " + this.Column + " is null";
                case WhereOperator.StringLike:
                    return this.Column + " like ?";
                default:
                    throw new InvalidOperationException("WhereCondition.Operator set to an invalid value: '" + this.Operator + "'.");
            }
        }
    }

    public class InsertUpdateQuery<TTable>
        where TTable : DatabaseTable, IDatabaseTable
    {
        private TTable _table;
        public TTable Table { get { return this._table; } }

        private InsertUpdateQueryMode _mode;
        public InsertUpdateQuery(TTable table, InsertUpdateQueryMode mode)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (!Enum.IsDefined(typeof(InsertUpdateQueryMode), mode))
                throw new ArgumentOutOfRangeException("mode");

            this._table = table;
            this._mode = mode;
        }

        private Dictionary<IDatabaseColumn, object> _values = new Dictionary<IDatabaseColumn, object>();
        private List<WhereCondition> _where = new List<WhereCondition>();

        public void ValueIfNotNull<TValue>(Func<TTable, DatabaseColumn<TValue>> column, TValue value)
        {
            if (value == null)
                return;

            this.Value(column, value);
        }

        public void Value<TValue>(Func<TTable, DatabaseColumn<TValue>> column, TValue value)
        {
            this.Value((Func<TTable, IDatabaseColumn>)column, value);
        }
        public void Value(Func<TTable, IDatabaseColumn> column, object value)
        {
            if (column == null)
                throw new ArgumentNullException("column");
            var col = column(this._table);
            if (!this._table.Equals(col.Table))
                throw new ArgumentException("The column is not assigned to the table for this query.", "column");

            this._values[col] = value;
        }

        public void Where(Func<TTable, IDatabaseColumn> column, WhereOperator @operator, object value)
        {
            if (this._mode != InsertUpdateQueryMode.Update)
                throw new InvalidOperationException("Where conditions cannot be applied to Insert statements.");
            if (column == null)
                throw new ArgumentNullException("column");
            var col = column(this._table);
            if (!this._table.Equals(col.Table))
                throw new ArgumentException("The column is not assigned to the table for this query.", "column");

            this._where.Add(new WhereCondition() { Column = col, Operator = @operator, Value = value });
        }

        //public void Where(Func<TTable, IDatabaseColumn> column, object value)
        //{
        //    this.Where(column, WhereOperator.Equal, value);
        //}

        public void Where<TValue>(Func<TTable, DatabaseColumn<TValue>> column, TValue value)
        {
            this.Where(column, WhereOperator.Equal, value);
        }

        public int Execute()
        {
            if (this._values.Count == 0)
                return 0;

            object[] args = new object[this._values.Count + this._where.Count(o => o.IsValueUsed)];
            int argc = 0;

            var sb = new StringBuilder();
            switch (this._mode)
            {
                case InsertUpdateQueryMode.Insert:
                    sb.Append("insert into ");
                    break;
                case InsertUpdateQueryMode.Update:
                    sb.Append("update ");
                    break;
                case InsertUpdateQueryMode.Replace:
                    sb.Append("replace into ");
                    break;
            }
            sb.Append(this._table);
            if (this._mode == InsertUpdateQueryMode.Update)
                sb.Append(" set ");
            else
                sb.Append(" (");
            bool first = true;
            foreach (var val in this._values)
            {
                if (!first) sb.Append(", ");
                first = false;
                sb.Append(val.Key);
                if (this._mode == InsertUpdateQueryMode.Update)
                    sb.Append(" = ?");
                args[argc] = val.Value;
                argc++;
            }
            if (this._where.Count > 0)
            {
                sb.Append(" where ");
                first = true;
                foreach (var wh in this._where.GroupBy(o => o.GroupingKey))
                {
                    if (!first) sb.Append(" and ");
                    first = false;

                    var condfirst = true;
                    sb.Append("(");
                    foreach (var cond in wh)
                    {
                        if (!condfirst) sb.Append(" or ");
                        condfirst = false;

                        sb.Append(cond);

                        if (cond.IsValueUsed)
                        {
                            args[argc] = cond.Value;
                            argc++;
                        }
                    }
                    sb.Append(")");
                }
            }
            if (this._mode != InsertUpdateQueryMode.Update)
            {
                sb.Append(") values (");
                for (int i = 0; i < argc; i++)
                {
                    if (i != 0)
                        sb.Append(", ");
                    sb.Append("?");
                }
                sb.Append(")");
            }

            return this._table.Schema.Database.ExecuteNonQuery(sb.ToString(), args);
        }
    }

    public class DeleteQuery<TTable>
        where TTable : DatabaseTable, IDatabaseTable
    {
        private TTable _table;
        public TTable Table { get { return this._table; } }
        
        private List<WhereCondition> _where = new List<WhereCondition>();

        public DeleteQuery(TTable table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            this._table = table;
        }

        public void Where(Func<TTable, IDatabaseColumn> column, WhereOperator @operator, object value)
        {
            if (column == null)
                throw new ArgumentNullException("column");
            var col = column(this._table);
            if (!this._table.Equals(col.Table))
                throw new ArgumentException("The column is not assigned to the table for this query.", "column");

            this._where.Add(new WhereCondition() { Column = col, Operator = @operator, Value = value });
        }

        public void Where<TValue>(Func<TTable, DatabaseColumn<TValue>> column, TValue value)
        {
            this.Where(column, WhereOperator.Equal, value);
        }

        public void Where(WhereCondition condition)
        {
            if (condition == null)
                throw new ArgumentNullException("condition");
            if (!this._table.Equals(condition.Column.Table))
                throw new ArgumentException("The column is not assigned to the table for this query.", "condition");

            this._where.Add(condition);
        }

        public int Execute()
        {
            var sb = new StringBuilder();
            object[] args = new object[this._where.Count(o => o.IsValueUsed)];
            int argc = 0;
            bool first;

            sb.Append("delete from ");
            sb.Append(this._table);

            if (this._where.Count > 0)
            {
                sb.Append(" where ");
                first = true;
                foreach (var wh in this._where.GroupBy(o => o.GroupingKey))
                {
                    if (!first) sb.Append(" and ");
                    first = false;

                    var condfirst = true;
                    sb.Append("(");
                    foreach (var cond in wh)
                    {
                        if (!condfirst) sb.Append(" or ");
                        condfirst = false;

                        sb.Append(cond);
                        if (cond.IsValueUsed)
                        {
                            args[argc] = cond.Value;
                            argc++;
                        }
                    }
                    sb.Append(")");
                }
            }

            return this._table.Schema.Database.ExecuteNonQuery(sb.ToString(), args);
        }
    }

    public class SelectQuery<TTable>
        where TTable : DatabaseTable, IDatabaseTable
    {
        private TTable _table;
        public TTable Table { get { return this._table; } }

        public SelectQuery(TTable table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            this._table = table;

            this.Select(o => o.RowId);
        }

        private List<IDatabaseColumn> _columns = new List<IDatabaseColumn>();
        private List<WhereCondition> _where = new List<WhereCondition>();
        private int _maxRows = -1;
        private List<KeyValuePair<string, bool>> _order = new List<KeyValuePair<string, bool>>();
        private bool _distinct;

        public void Distinct()
        {
            this._distinct = true;
        }

        public void SelectAll()
        {
            foreach (var col in this._table.Columns)
            {
                if (this._columns.Contains(col))
                    continue;

                this._columns.Add(col);
            }
        }

        public void Select(params Func<TTable, IDatabaseColumn>[] columns)
        {
            if (columns == null)
                return;

            foreach (var c in columns)
                this.Select(c);
        }

        public void Select(Func<TTable, IDatabaseColumn> column)
        {
            if (column == null)
                throw new ArgumentNullException("column");
            var col = column(this._table);
            if (!this._table.Equals(col.Table))
                throw new ArgumentException("The column is not assigned to the table for this query.", "column");

            if (this._columns.Contains(col))
                return;

            this._columns.Add(col);
        }

        public void Where(Func<TTable, IDatabaseColumn> column, WhereOperator @operator, object value)
        {
            this.Where(column, @operator, value, null);
        }
        public void Where(Func<TTable, IDatabaseColumn> column, WhereOperator @operator, object value, string groupingKey)
        {
            if (column == null)
                throw new ArgumentNullException("column");
            var col = column(this._table);
            if (!this._table.Equals(col.Table))
                throw new ArgumentException("The column is not assigned to the table for this query.", "column");

            this._where.Add(new WhereCondition() { Column = col, Operator = @operator, Value = value, GroupingKey = groupingKey });
        }

        public void Where(Func<TTable, IDatabaseColumn> column, object value)
        {
            this.Where(column, WhereOperator.Equal, value);
        }

        public void Where(WhereCondition condition)
        {
            if (condition == null)
                throw new ArgumentNullException("condition");
            if (!this._table.Equals(condition.Column.Table))
                throw new ArgumentException("The column is not assigned to the table for this query.", "condition");

            this._where.Add(condition);
        }

        public void OrderBy(Func<TTable, IDatabaseColumn> column, bool ascending = true)
        {
            if (column == null)
                throw new ArgumentNullException("column");
            var col = column(this._table);
            if (!this._table.Equals(col.Table))
                throw new ArgumentException("The column is not assigned to the table for this query.", "column");

            this._order.Add(new KeyValuePair<string, bool>(col.ToString(), ascending));
        }
        public void OrderByRandom()
        {
            this._order.Add(new KeyValuePair<string, bool>("RANDOM()", true));
        }

        public void Limit(int maxRows)
        {
            if (maxRows < -1)
                throw new ArgumentOutOfRangeException("maxRows");

            this._maxRows = maxRows;
        }

        public SelectResult<TTable> Execute()
        {
            var sb = new StringBuilder();
            object[] args = new object[this._where.Count(o => o.IsValueUsed)];
            int argc = 0;

            sb.Append("select ");

            if (this._distinct)
                sb.Append("distinct ");

            var first = true;
            foreach (var col in this._columns)
            {
                if (!first) sb.Append(", ");
                first = false;
                sb.Append(col);
            }

            if (first)
            {
                sb.Append("1");
            }

            sb.Append(" from ");
            sb.Append(this._table);

            if (this._where.Count > 0)
            {
                sb.Append(" where ");
                first = true;
                foreach (var wh in this._where.GroupBy(o => o.GroupingKey))
                {
                    if (!first) sb.Append(" and ");
                    first = false;

                    var condfirst = true;
                    sb.Append("(");
                    foreach (var cond in wh)
                    {
                        if (!condfirst) sb.Append(" or ");
                        condfirst = false;

                        sb.Append(cond);
                        if (cond.IsValueUsed)
                        {
                            args[argc] = cond.Value;
                            argc++;
                        }
                    }
                    sb.Append(")");
                }
            }

            if (this._order.Count > 0)
            {
                sb.Append(" order by ");
                first = true;
                foreach (var ob in this._order)
                {
                    if (!first) sb.Append(", ");
                    first = false;
                    sb.Append(ob.Key);
                    if (!ob.Value)
                        sb.Append(" desc");
                }
            }

            if (this._maxRows != -1)
            {
                sb.Append(" limit ");
                sb.Append(this._maxRows.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            var result = this._table.Schema.Database.ExecuteReader(sb.ToString(), args);
            return new SelectResult<TTable>(result, this._columns, this._table);
        }

        public TResult ExecuteScalar<TResult>(Func<TTable, DatabaseColumn<TResult>> column)
        {
            this.Select(column);

            using (var result = this.Execute())
            {
                if (!result.Read())
                    return default(TResult);

                return result.Value<TResult>(column);
            }
        }
    }

    public class SelectResult<TTable> : IDisposable
        where TTable : DatabaseTable, IDatabaseTable
    {
        private class SelectResultEnumerator : IEnumerator<SelectResult<TTable>>
        {
            public SelectResultEnumerator(SelectResult<TTable> result)
            {
                this._result = result;
            }

            private SelectResult<TTable> _result;

            #region IEnumerator<SelectResult<TTable>> Members

            public SelectResult<TTable> Current
            {
                get { return this._result; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                this._result.Dispose();
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return this._result; }
            }

            public bool MoveNext()
            {
                return this._result.Read();
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        private class SelectResultEnumerable : IEnumerable<SelectResult<TTable>>
        {
            private SelectResult<TTable> _result;

            public SelectResultEnumerable(SelectResult<TTable> result)
            {
                this._result = result;
            }

            public IEnumerator<SelectResult<TTable>> GetEnumerator()
            {
                return this._result.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        internal SelectResult(System.Data.SQLite.SQLiteDataReader reader, List<IDatabaseColumn> columns, TTable table)
        {
            this._reader = reader;
            this._columns = columns;
            this._table = table;
        }

        private System.Data.SQLite.SQLiteDataReader _reader;
        private List<IDatabaseColumn> _columns;
        private TTable _table;

        public IEnumerable<SelectResult<TTable>> AsEnumerable()
        {
            return new SelectResultEnumerable(this);
        }

        public IEnumerator<SelectResult<TTable>> GetEnumerator()
        {
            return new SelectResultEnumerator(this);
        }

        public bool Read()
        {
            return this._reader.Read();
        }

        public object Value(Func<TTable, IDatabaseColumn> column)
        {
            var i = this._columns.IndexOf(column(this._table));
            if (i == -1)
                throw new InvalidOperationException("The column was not selected from the database.");
            if (_reader.IsDBNull(i))
                return null;
            return this._reader[i];
        }
        public TValue Value<TValue>(Func<TTable, IDatabaseColumn> column)
        {
            var i = this._columns.IndexOf(column(this._table));
            if (i == -1)
                throw new InvalidOperationException("The column was not selected from the database.");
            return Convert.ChangeType<TValue>(this._reader[i], System.Globalization.CultureInfo.InvariantCulture);
        }
        public TValue Value<TValue>(Func<TTable, DatabaseColumn<TValue>> column)
        {
            var i = this._columns.IndexOf(column(this._table));
            if (i == -1)
                throw new InvalidOperationException("The column was not selected from the database.");
            return Convert.ChangeType<TValue>(this._reader[i], System.Globalization.CultureInfo.InvariantCulture);
        }

        #region IDisposable Members

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SelectResult&lt;TTable&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~SelectResult()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this._reader.Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
