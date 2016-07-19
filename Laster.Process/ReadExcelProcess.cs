using Laster.Core.Data;
using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Laster.Process
{
    /// <summary>
    /// Formatea unos datos
    /// </summary>
    public class ReadExcelProcess : IDataProcess
    {
        public string SheetName { get; set; }
        public int StartRecord { get; set; }
        public int MaxRecords { get; set; }
        public bool HeaderInFirstRow { get; set; }

        /// <summary>
        /// Devolver como enumerador
        /// </summary>
        public bool ReturnAsEnumerable { get; set; }
        public override string Title { get { return "ReadExcel"; } }

        public ReadExcelProcess()
        {
            HeaderInFirstRow = true;
            StartRecord = 0;
            MaxRecords = int.MaxValue;
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (data == null) return new DataEmpty(this);

            List<DataTable> dt = new List<DataTable>();
            foreach (object o in data)
            {
                string path = o.ToString();
                if (!File.Exists(path)) continue;

                DataTable d = null;
                string sSheetName = SheetName;

                using (OleDbConnection oleExcelConnection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path +
                    ";Extended Properties=\"Excel 12.0;HDR=" + (HeaderInFirstRow ? "Yes" : "No") + ";IMEX=1\""))
                {
                    oleExcelConnection.Open();

                    if (string.IsNullOrEmpty(sSheetName))
                    {
                        using (DataTable dtTablesList = oleExcelConnection.GetSchema("Tables"))
                        {
                            if (dtTablesList.Rows.Count > 0)
                                sSheetName = dtTablesList.Rows[0]["TABLE_NAME"].ToString();
                        }
                    }

                    if (!string.IsNullOrEmpty(sSheetName))
                    {
                        using (OleDbDataAdapter oleExcelReader = new OleDbDataAdapter("Select * From [" + sSheetName + "]", oleExcelConnection))
                        {
                            d = new DataTable();
                            oleExcelReader.Fill(StartRecord, MaxRecords, d);
                        }
                    }
                    oleExcelConnection.Close();
                }

                if (d != null) dt.Add(d);
            }

            if (dt.Count == 1) return new DataObject(this, dt[0]);

            if (ReturnAsEnumerable) return new DataEnumerable(this, dt.ToArray());
            return new DataArray(this, dt.ToArray());
        }
    }
}