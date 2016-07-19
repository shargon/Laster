using Laster.Core.Data;
using Laster.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Laster.Inputs.DB
{
    public class DBInput : IDataInput
    {
        EServer _Server;
        DbConnection _Connection;
        DbProviderFactory _DbFactory;

        public enum EServer : byte
        {
            MySql = 0,
            SqlServer = 1,
            OleDb = 2,
            Odbc = 3
        }

        public enum EExecuteMode : byte
        {
            Enumerable = 0,
            EnumerableWithHeader = 1,

            Array = 2,
            ArrayWithHeader = 3,

            DataSet = 4,
            DataTable = 5,

            Scalar = 6,
            NonQuery = 7,
        }

        class EnumReader : IEnumerable<object>, IDisposable
        {
            bool _WithHeader;
            IDataReader reader;

            public EnumReader(IDataReader reader, bool withHeader)
            {
                this.reader = reader;
                _WithHeader = withHeader;
            }

            public IEnumerator<object> GetEnumerator()
            {
                if (reader.Read())
                {
                    int fieldCount = reader.FieldCount;

                    if (_WithHeader)
                    {
                        object[] values = new object[fieldCount];
                        for (int x = values.Length - 1; x >= 0; x--) values[x] = reader.GetName(x);
                        yield return values;
                    }

                    do
                    {
                        object[] values = new object[fieldCount];
                        reader.GetValues(values);
                        yield return values;
                    }
                    while (reader.Read());
                }

                Dispose();
            }
            IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

            public void Dispose()
            {
                if (reader == null) return;

                reader.Close();
                reader.Dispose();
                reader = null;
            }
        }

        /// <summary>
        /// Tipo de servidor
        /// </summary>
        public EServer Server { get { return _Server; } set { _Server = value; } }
        /// <summary>
        /// Cadena de conexión
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Query
        /// </summary>
        public string[] SqlQuery { get; set; }
        /// <summary>
        /// Modo de ejecución
        /// </summary>
        public EExecuteMode ExecuteMode { get; set; }

        public override string Title { get { return "DB"; } }

        public override void OnStart()
        {
            Free();
            _Connection = CreateConnection();

            base.OnStart();
        }
        protected override IData OnGetData()
        {
            if (_Connection == null || _Connection.State != ConnectionState.Open)
            {
                _Connection.Close();
                _Connection.Dispose();
                _Connection = CreateConnection();
            }

            return GetData(_Connection);
        }
        IData GetData(IDbConnection c)
        {
            switch (ExecuteMode)
            {
                case EExecuteMode.NonQuery:
                    {
                        int ix = 0;
                        foreach (string sql in SqlQuery)
                            using (IDbCommand cmd = c.CreateCommand())
                            {
                                cmd.CommandText = sql;
                                ix += cmd.ExecuteNonQuery();
                            }

                        return new DataObject(this, ix);
                    }
                case EExecuteMode.Scalar:
                    {
                        List<object> ls = new List<object>();
                        foreach (string sql in SqlQuery)
                            using (IDbCommand cmd = c.CreateCommand())
                            {
                                cmd.CommandText = sql;
                                ls.Add(cmd.ExecuteScalar());
                            }

                        if (ls.Count == 0) return new DataObject(this, ls[0]);
                        return new DataArray(this, ls.ToArray());
                    }
                case EExecuteMode.Enumerable:
                case EExecuteMode.EnumerableWithHeader:
                    {
                        foreach (string sql in SqlQuery)
                            using (IDbCommand cmd = c.CreateCommand())
                                return new DataEnumerable(this, new EnumReader(cmd.ExecuteReader(), ExecuteMode == EExecuteMode.EnumerableWithHeader));
                        break;
                    }
                case EExecuteMode.Array:
                case EExecuteMode.ArrayWithHeader:
                    {
                        List<object[]> rows = new List<object[]>();
                        foreach (string sql in SqlQuery)
                            using (IDbCommand cmd = c.CreateCommand())
                            {
                                cmd.CommandText = sql;
                                using (EnumReader reader = new EnumReader(cmd.ExecuteReader(), ExecuteMode == EExecuteMode.ArrayWithHeader))
                                    foreach (object[] o in reader) rows.Add(o);
                            }
                        return new DataArray(this, rows.ToArray());
                    }
                case EExecuteMode.DataSet:
                case EExecuteMode.DataTable:
                    {
                        DataSet ds = new DataSet();

                        int x = 0;
                        foreach (string sql in SqlQuery)
                            using (IDbCommand cmd = c.CreateCommand())
                            {
                                cmd.CommandText = sql;

                                DataTable dt = new DataTable()
                                {
                                    TableName = "Table_" + x.ToString(),
                                };
                                using (DbDataAdapter dbAdapter = _DbFactory.CreateDataAdapter())
                                {
                                    dbAdapter.SelectCommand = (DbCommand)cmd;
                                    dbAdapter.Fill(dt);
                                }

                                if (ExecuteMode == EExecuteMode.DataTable)
                                {
                                    ds.Dispose();
                                    return new DataObject(this, dt);
                                }
                                else
                                {
                                    ds.Tables.Add(dt);
                                }
                                x++;
                            }

                        return new DataObject(this, ds);
                    }
            }

            return new DataEmpty(this);
        }
        /// <summary>
        /// Libración de recursos
        /// </summary>
        public override void Dispose()
        {
            Free();
            base.Dispose();
        }
        void Free()
        {
            if (_Connection != null)
            {
                _Connection.Close();
                _Connection.Dispose();
                _Connection = null;
            }
        }

        DbConnection CreateConnection()
        {
            DbConnection ret;
            switch (_Server)
            {
                case EServer.MySql: ret = new MySql.Data.MySqlClient.MySqlConnection(ReplaceComodin(ConnectionString)); break;
                case EServer.SqlServer: ret = new SqlConnection(ReplaceComodin(ConnectionString)); break;
                case EServer.OleDb: ret = new OleDbConnection(ReplaceComodin(ConnectionString)); break;
                case EServer.Odbc: ret = new OdbcConnection(ReplaceComodin(ConnectionString)); break;

                default: return null;
            }

            _DbFactory = DbProviderFactories.GetFactory(ret);
            ret.Open();
            return ret;
        }
        string ReplaceComodin(string connectionString)
        {
            return connectionString.Replace("{Year}", DateTime.Now.Year.ToString());
        }
    }
}