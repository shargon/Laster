using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using Laster.Process.Helpers;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laster.Process
{
    public class DataTableToExcelProcess : IDataProcess
    {
        public enum EFileSource : byte
        {
            FileName = 0,
            Input = 1,
            Dialog = 2,
            TempFile = 3
        }

        public enum EReturnMode : byte
        {
            Origin = 0,
            FileName = 1,
        }

        [DefaultValue("")]
        [Category("Source")]
        public string FileName { get; set; }

        [Category("Source")]
        public EFileSource FileSource { get; set; }
        [Category("Output")]
        public EReturnMode Return { get; set; }

        public override string Title { get { return "DataTable to Excel"; } }

        public DataTableToExcelProcess()
        {
            FileSource = EFileSource.FileName;
            Return = EReturnMode.Origin;
        }

        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (data == null) return data;

            foreach (object obj in data)
            {
                if (obj == null) continue;

                string file = null;
                if (obj is DataTable)
                {
                    DataTable dt = (DataTable)obj;
                    file = GetFile(data);

                    if (!string.IsNullOrEmpty(file))
                        ExcelHelper.CreateXlsFromDataTable(file, dt);
                }
                else
                {
                    if (obj is DataSet)
                    {
                        DataSet dt = (DataSet)obj;
                        file = GetFile(data);

                        if (!string.IsNullOrEmpty(file))
                            ExcelHelper.CreateXlsFromDataTable(file, dt.Tables.Cast<DataTable>().ToArray());
                    }
                }

                if (!string.IsNullOrEmpty(file))
                {
                    if (Return == EReturnMode.FileName)
                        return DataObject(file);
                }
            }

            if (Return == EReturnMode.FileName)
                return null;

            return data;
        }

        string GetFile(IData data)
        {
            switch (FileSource)
            {
                case EFileSource.FileName: { return FileName; }
                case EFileSource.Dialog:
                    {
                        if (!Environment.UserInteractive) return null;

                        Task<string> t = TaskHelper.StartSTATask<string>(() => { return GetFileByDialog(); });
                        t.Wait();
                        return t.Result;
                    }
                case EFileSource.TempFile:
                    {
                        string file = Path.GetTempFileName();
                        string fileXls = Path.ChangeExtension(file, ".xls");
                        File.Move(file, fileXls);
                        return fileXls;
                    }
                case EFileSource.Input:
                    {
                        foreach (object obj in data)
                            if (obj is string) return obj.ToString();

                        break;
                    }
            }

            return FileName;
        }
        string GetFileByDialog()
        {
            using (SaveFileDialog sv = new SaveFileDialog()
            {
                Filter = "Excel file|*.xls;*.xlsx",
                DefaultExt = ".xls"
            })
            {
                if (sv.ShowDialog() != DialogResult.OK) return null;
                return sv.FileName;
            }
        }
    }
}