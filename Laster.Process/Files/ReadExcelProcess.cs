using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;

namespace Laster.Process.Files
{
    /// <summary>
    /// Formatea unos datos
    /// </summary>
    public class ReadExcelProcess : IDataProcess
    {
        public enum EExcelSource
        {
            FileName = 0,
            Input = 1,
            FileNameAndInput = 2
        }

        [DefaultValue("")]
        public string SheetName { get; set; }
        [DefaultValue(0)]
        public int StartRecord { get; set; }
        [DefaultValue(int.MaxValue)]
        public int MaxRecords { get; set; }
        [DefaultValue(true)]
        public bool HeaderInFirstRow { get; set; }

        [DefaultValue("")]
        [Category("Source")]
        public string FileName { get; set; }
        [Category("Source")]
        [DefaultValue(EExcelSource.FileName)]
        public EExcelSource ExcelSource { get; set; }

        /// <summary>
        /// Devolver como enumerador
        /// </summary>
        [DefaultValue(false)]
        public bool ReturnAsEnumerable { get; set; }

        public override string Title { get { return "Files - Read Excel"; } }

        public ReadExcelProcess()
        {
            HeaderInFirstRow = true;
            StartRecord = 0;
            MaxRecords = int.MaxValue;
            ExcelSource = EExcelSource.FileName;
            DesignBackColor = Color.Brown;
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (data == null) return DataEmpty();

            List<DataTable> dt = new List<DataTable>();

            if (ExcelSource != EExcelSource.Input && !string.IsNullOrEmpty(FileName))
            {
                DataTable d = ReadExcel(FileName);
                if (d != null) dt.Add(d);
            }
            if (ExcelSource != EExcelSource.FileName)
            {
                foreach (object o in data)
                {
                    DataTable d = ReadExcel(o.ToString());
                    if (d != null) dt.Add(d);
                }
            }

            if (dt.Count == 0) return DataEmpty();
            if (dt.Count == 1) return DataObject(dt[0]);

            if (ReturnAsEnumerable) return DataEnumerable(dt.ToArray());
            return DataArray(dt.ToArray());
        }
        DataTable ReadExcel(string path)
        {
            if (!File.Exists(path)) return null;

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
                        d = new DataTable("Excel");
                        oleExcelReader.Fill(StartRecord, MaxRecords, d);
                    }
                }
                oleExcelConnection.Close();
            }
            return d;
        }
    }
}