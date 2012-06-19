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
    #region [ Base classes ]

    public abstract class DatabaseSchema : IDatabaseSchema, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSchema"/> class.
        /// </summary>
        /// <param name="databaseFileName">The path to the database file.</param>
        public DatabaseSchema(string databaseFileName)
        {
            if (string.IsNullOrWhiteSpace(databaseFileName))
                throw new ArgumentNullException("databaseFileName");

            this._database = new Database(databaseFileName);

            this.AddTable(this._tableVersionTable = new TableVersionTable());
        }

        /// <summary>
        /// Returns the object for direct interaction with the database.
        /// </summary>
        /// <returns>The database object for direct interaction.</returns>
        public Database Database()
        {
            return this._database;
        }

        /// <summary>
        /// Gets the value indicating if string columns will be created as "collate nocase".
        /// </summary>
        /// <remarks>
        /// More details on the collate option: http://www.sqlite.org/datatype3.html#collation.
        /// </remarks>
        protected virtual bool CreateStringColumnsCaseInsensitive
        {
            get { return false; }
        }

        #region IDatabaseSchema Members

        private Dictionary<Type, IDatabaseTable> _tables = new Dictionary<Type, IDatabaseTable>();

        protected void AddTable(IDatabaseTable table)
        {
            table.CreateOrder = _tables.Count;
            table.Schema = this;
            this._tables.Add(table.GetType(), table);

            this.EnsureSchema(table);
        }
        protected TDatabaseTable GetTable<TDatabaseTable>()
            where TDatabaseTable : IDatabaseTable
        {

            return (TDatabaseTable)this._tables[typeof(TDatabaseTable)];
        }

        IEnumerable<IDatabaseTable> IDatabaseSchema.Tables
        {
            get { return this._tables.Values.OrderBy(o => o.CreateOrder); }
        }

        private Database _database;
        Database IDatabaseSchema.Database { get { return this._database; } }

        #endregion

        #region [ EnsureSchema ]

        private TableVersionTable _tableVersionTable;

        private long RetrieveCurrentVersion(IDatabaseTable table)
        {
            try
            {
                var tv = this._database.ExecuteScalar<long?>("select " + this._tableVersionTable.Version + " from " + this._tableVersionTable + " where " + this._tableVersionTable.TableName + " = ?", table.Name);
                if (tv.HasValue)
                    return tv.Value;

                if (!(table is IExtensionTable))
                {
                    // fall back to the old style Settings table as some legacy databases could still contain that.
                    return this._database.ExecuteScalar<long>("select CurrentVersion from Settings");
                }
            }
            catch (System.Data.SQLite.SQLiteException)
            {
            }

            return 0;
        }

        private void SaveCurrentVersion(IDatabaseTable table, long version)
        {
            this._database.ExecuteNonQuery("replace into " + this._tableVersionTable + " (" + this._tableVersionTable.Version + ", " + this._tableVersionTable.TableName + ") values (?,?)", version, table.Name);
        }

        private void EnsureSchema(IDatabaseTable table)
        {
            try
            {
                var currentVersion = this.RetrieveCurrentVersion(table);
                var newVersion = this.EnsureSchema(currentVersion, table);
                if (currentVersion < newVersion)
                    this.SaveCurrentVersion(table, newVersion);
            }
            catch (Exception ex)
            {
                if (table is IExtensionTable)
                {
                    System.Windows.Forms.MessageBox.Show("Extension table '" + table.Name + "' could not be created due to an error." + Environment.NewLine + Environment.NewLine + ex.Message, "Extension error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    throw;
                }
            }
        }

        private long EnsureSchema(long currentVersion, IDatabaseTable table)
        {
            long newVersion = currentVersion;
            if (currentVersion < table.Version)
            {
                var sb = new StringBuilder();
                sb.Append("create table ");
                sb.Append(table.Name);
                sb.Append(" (");
                bool first = true;
                foreach (var column in table.Columns)
                {
                    if (newVersion < column.Version)
                        newVersion = column.Version;

                    if (string.Equals(column.Name, "\"oid\"", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(column.Name, "oid", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(column.Name, "\"_RowId_\"", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(column.Name, "_RowId_", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(column.Name, "RowId", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(column.Name, "\"RowId\"", StringComparison.OrdinalIgnoreCase)
                        )
                        continue;

                    if (!first) sb.Append(", ");
                    first = false;

                    sb.Append(column.Name);
                    sb.Append(" ");
                    sb.Append(column.DataTypeName);
                    if (column.DataType == System.Data.SQLite.TypeAffinity.Text && this.CreateStringColumnsCaseInsensitive)
                        sb.Append(" collate nocase");
                }
                sb.Append(")");
                this._database.ExecuteNonQuery(sb.ToString());

                foreach (var index in table.Indexes)
                {
                    var ver = this.EnsureSchema(currentVersion, index);
                    if (newVersion < ver)
                        newVersion = ver;
                }
            }
            else
            {
                foreach (var column in table.Columns)
                {
                    if (currentVersion < column.Version)
                    {
                        var ver = this.EnsureSchema(currentVersion, column);
                        if (newVersion < ver)
                            newVersion = ver;
                    }
                }
                foreach (var index in table.Indexes)
                {
                    if (currentVersion < index.Version)
                    {
                        var ver = this.EnsureSchema(currentVersion, index);
                        if (newVersion < ver)
                            newVersion = ver;
                    }
                }
            }

            return newVersion;
        }
        private long EnsureSchema(long currentVersion, IDatabaseColumn column)
        {
            if (column.DataType == System.Data.SQLite.TypeAffinity.Text && this.CreateStringColumnsCaseInsensitive)
                this._database.ExecuteNonQuery("alter table " + column.Table.Name + " add column " + column.Name + " " + column.DataTypeName + " collate nocase");
            else
                this._database.ExecuteNonQuery("alter table " + column.Table.Name + " add column " + column.Name + " " + column.DataTypeName);
            return column.Version;
        }
        private long EnsureSchema(long currentVersion, IDatabaseIndex index)
        {
            var sb = new StringBuilder();
            sb.Append("drop index if exists ");
            sb.Append(index.Name);

            sb.Append("; create ");
            if (index.Unique) sb.Append("unique ");
            sb.Append("index ");
            sb.Append(index.Name);
            sb.Append(" on ");
            sb.Append(index.Table);
            sb.Append("(");
            bool first = true;
            foreach (var col in index.Columns)
            {
                if (!first) sb.Append(", ");
                first = false;
                sb.Append(col);
                if (col.DataType == System.Data.SQLite.TypeAffinity.Text && this.CreateStringColumnsCaseInsensitive)
                    sb.Append(" collate nocase");
            }
            sb.Append(");");
            this._database.ExecuteNonQuery(sb.ToString());
            return index.Version;
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="DatabaseSchema"/> is reclaimed by garbage collection.
        /// </summary>
        ~DatabaseSchema()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this._database != null)
                this._database.Dispose();
        }
    }

    public abstract class DatabaseTable : IDatabaseTable
    {
        public DatabaseTable(string name, long version)
        {
            if (version < 1)
                throw new ArgumentOutOfRangeException("version");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (this is IExtensionTable)
                name = this.GetType().FullName + "." + name;

            this._name = "\"" + name + "\"";
            this._version = version;

            this.AddColumn<long>("oid", 1);
        }

        public DatabaseColumn<long> RowId { get { return this.GetColumn<long>("oid"); } }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this._name;
        }

        /// <summary>
        /// Returns the database instance that can be used for direct database access.
        /// </summary>
        /// <exception cref="InvalidOperationException">when the table is not yet added to a schema</exception>
        public Database Database()
        {
            return ((IDatabaseTable)this).Schema.Database;
        }

        #region IDatabaseTable Members

        private int _createOrder;

        int IDatabaseTable.CreateOrder
        {
            get { return this._createOrder; }
            set { this._createOrder = value; }
        }

        private long _version;

        long IDatabaseTable.Version
        {
            get { return this._version; }
        }

        private string _name;

        string IDatabaseTable.Name
        {
            get { return _name; }
        }

        private Dictionary<string, IDatabaseColumn> _columns = new Dictionary<string, IDatabaseColumn>();
        protected void AddColumn(DatabaseColumn column)
        {
            this._columns.Add(((IDatabaseColumn)column).Name, column);
        }
        protected void AddColumn<TValue>(string name, long version)
        {
            this.AddColumn(new DatabaseColumn<TValue>(this, name, version));
        }
        public DatabaseColumn GetColumn(string name)
        {
            return (DatabaseColumn)this._columns["\"" + name + "\""];
        }
        public DatabaseColumn<TValue> GetColumn<TValue>(string name)
        {
            return (DatabaseColumn<TValue>)this._columns["\"" + name + "\""];
        }

        IEnumerable<IDatabaseColumn> IDatabaseTable.Columns
        {
            get { return this._columns.Values; }
        }

        private Dictionary<string, IDatabaseIndex> _indexes = new Dictionary<string, IDatabaseIndex>();
        protected void AddIndex(DatabaseIndex Index)
        {
            this._indexes.Add(((IDatabaseIndex)Index).Name, Index);
        }
        protected DatabaseIndex GetIndex(string name)
        {
            return (DatabaseIndex)this._indexes["\"" + name + "\""];
        }
        protected TDatabaseIndex GetIndex<TDatabaseIndex>(string name)
            where TDatabaseIndex : DatabaseIndex
        {
            return (TDatabaseIndex)this._indexes["\"" + name + "\""];
        }

        IEnumerable<IDatabaseIndex> IDatabaseTable.Indexes
        {
            get { return this._indexes.Values; }
        }

        private IDatabaseSchema _schema;
        IDatabaseSchema IDatabaseTable.Schema 
        { 
            get 
            {
                if (this._schema == null)
                    throw new InvalidOperationException("The table is not yet added to a DatabaseSchema object.");
                return this._schema; 
            }
            set
            {
                this._schema = value;
            }
        }

        #endregion
    }

    public abstract class DatabaseColumn : IDatabaseColumn
    {
        protected DatabaseColumn(DatabaseTable table, string name, System.Data.SQLite.TypeAffinity dataType, long version)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (version < 1)
                throw new ArgumentOutOfRangeException("version");
            if (!Enum.IsDefined(typeof(System.Data.SQLite.TypeAffinity), dataType))
                throw new ArgumentOutOfRangeException("dataType");
            if (table == null)
                throw new ArgumentNullException("table");

            this._version = version;
            this._name = "\"" + name + "\"";
            this._dataType = dataType;
            this._table = table;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this._name;
        }

        #region IDatabaseColumn Members

        private long _version;
        long IDatabaseColumn.Version
        {
            get { return this._version; }
        }

        private string _name;
        string IDatabaseColumn.Name
        {
            get { return this._name; }
        }

        private System.Data.SQLite.TypeAffinity _dataType;
        public System.Data.SQLite.TypeAffinity DataType
        {
            get { return this._dataType; }
        }

        string IDatabaseColumn.DataTypeName
        {
            get
            {
                switch (this._dataType)
                {
                    case System.Data.SQLite.TypeAffinity.Blob:
                        return "blob";
                    case System.Data.SQLite.TypeAffinity.DateTime:
                        return "datetime";
                    case System.Data.SQLite.TypeAffinity.Double:
                        return "double";
                    case System.Data.SQLite.TypeAffinity.Int64:
                        return "integer";
                    case System.Data.SQLite.TypeAffinity.Text:
                        return "nvarchar";
                    case System.Data.SQLite.TypeAffinity.None:
                    case System.Data.SQLite.TypeAffinity.Null:
                    case System.Data.SQLite.TypeAffinity.Uninitialized:
                    default:
                        throw new InvalidOperationException("The DataType property is set to an invalid value.");
                }
            }
        }

        private IDatabaseTable _table;
        IDatabaseTable IDatabaseColumn.Table { get { return this._table; } }
        protected IDatabaseTable Table { get { return this._table; } }

        #endregion
    }

    public class DatabaseColumn<TValue> : DatabaseColumn
    {
        private static System.Data.SQLite.TypeAffinity _defaultDataType = System.Data.SQLite.TypeAffinity.Uninitialized;
        private static System.Data.SQLite.TypeAffinity DefaultDataType
        {
            get
            {
                if (_defaultDataType != System.Data.SQLite.TypeAffinity.Uninitialized)
                    return _defaultDataType;

                var t = typeof(TValue);
                t = Nullable.GetUnderlyingType(t) ?? t;

                if (t == typeof(long) || t == typeof(int) || t == typeof(bool))
                    _defaultDataType = System.Data.SQLite.TypeAffinity.Int64;
                else if (t == typeof(double))
                    _defaultDataType = System.Data.SQLite.TypeAffinity.Double;
                else if (t == typeof(string))
                    _defaultDataType = System.Data.SQLite.TypeAffinity.Text;
                else if (t == typeof(DateTime))
                    _defaultDataType = System.Data.SQLite.TypeAffinity.DateTime;
                else if (t == typeof(byte[]))
                    _defaultDataType = System.Data.SQLite.TypeAffinity.Blob;
                else
                    throw new InvalidOperationException("Unable to use type " + t.FullName + " with DatabaseColumn<> type.");

                return _defaultDataType;
            }
        }

        public DatabaseColumn(DatabaseTable table, string name, long version)
            : base(table, name, DefaultDataType, version)
        {
        }
    }

    public class DatabaseIndex : IDatabaseIndex
    {
        public DatabaseIndex(DatabaseTable table, string name, bool unique, long version, params IDatabaseColumn[] columns)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (version < 1)
                throw new ArgumentOutOfRangeException("version");
            if (table == null)
                throw new ArgumentNullException("table");
            if (columns == null || columns.Length == 0)
                throw new ArgumentNullException("columns");

            this.Version = version;
            this.Name = "\"" + table.ToString().Trim('"') + name + "\"";
            this.Unique = unique;
            this.Columns = columns;

            this.Table = table;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }

        #region IDatabaseIndex Members

        public long Version { get; private set; }

        public string Name { get; private set; }

        public bool Unique { get; private set; }

        public IEnumerable<IDatabaseColumn> Columns { get; private set; }

        public IDatabaseTable Table { get; private set; }

        #endregion
    }

    #endregion

    #region [ Interfaces ]

    public interface IDatabaseSchema
    {
        Database Database { get; }
        IEnumerable<IDatabaseTable> Tables { get; }
    }

    public interface IDatabaseTable
    {
        int CreateOrder { get; set; }
        long Version { get; }
        string Name { get; }
        IEnumerable<IDatabaseColumn> Columns { get; }
        IEnumerable<IDatabaseIndex> Indexes { get; }
        IDatabaseSchema Schema { get; set; }
    }

    public interface IDatabaseColumn
    {
        long Version { get; }
        string Name { get; }
        System.Data.SQLite.TypeAffinity DataType { get; }
        string DataTypeName { get; }

        IDatabaseTable Table { get; }
    }

    public interface IDatabaseIndex
    {
        long Version { get; }
        string Name { get; }
        bool Unique { get; }
        IEnumerable<IDatabaseColumn> Columns { get; }

        IDatabaseTable Table { get; }
    }

    #endregion
}
