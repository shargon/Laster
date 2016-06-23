using Laster.Core.Data;
using Laster.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Laster.Inputs.DB
{
    public class DBInput : IDataInput
    {
        EServer _Server;
        IDbConnection _Connection;

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

            Scalar = 4,
            NonQuery = 5,
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
        public string SqlQuery { get; set; }
        /// <summary>
        /// Modo de ejecución
        /// </summary>
        public EExecuteMode ExecuteMode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DBInput() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="raiseMode">Modo de lanzamiento</param>
        public DBInput(IDataInputRaiseMode raiseMode) : base(raiseMode) { }

        public override void OnCreate()
        {
            _Connection = CreateConnection();
            base.OnCreate();
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
            using (IDbCommand cmd = c.CreateCommand())
            {
                cmd.CommandText = SqlQuery;

                switch (ExecuteMode)
                {
                    case EExecuteMode.NonQuery: return new DataObject(this, cmd.ExecuteNonQuery());
                    case EExecuteMode.Scalar: return new DataObject(this, cmd.ExecuteScalar());
                    case EExecuteMode.Enumerable:
                    case EExecuteMode.EnumerableWithHeader:
                        {
                            return new DataEnumerable(this, new EnumReader(cmd.ExecuteReader(), ExecuteMode == EExecuteMode.EnumerableWithHeader));
                        }
                    case EExecuteMode.Array:
                    case EExecuteMode.ArrayWithHeader:
                        {
                            List<object[]> rows = new List<object[]>();
                            using (EnumReader reader = new EnumReader(cmd.ExecuteReader(), ExecuteMode == EExecuteMode.ArrayWithHeader))
                                foreach (object[] o in reader) rows.Add(o);

                            return new DataArray(this, rows.ToArray());
                        }
                }
            };

            return new DataEmpty(this);
        }
        public override void Dispose()
        {
            if (_Connection != null)
            {
                _Connection.Close();
                _Connection.Dispose();
                _Connection = null;
            }

            base.Dispose();
        }
        IDbConnection CreateConnection()
        {
            IDbConnection ret;
            switch (_Server)
            {
                case EServer.MySql: ret = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString); break;
                case EServer.SqlServer: ret = _Connection = new SqlConnection(ConnectionString); break;
                case EServer.OleDb: ret = _Connection = new OleDbConnection(ConnectionString); break;
                case EServer.Odbc: ret = _Connection = new OdbcConnection(ConnectionString); break;

                default: return null;
            }

            ret.Open();
            return ret;
        }
    }
}