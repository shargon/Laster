using Laster.Core.Data;
using Laster.Core.Interfaces;
using System;
using System.Windows.Forms;

namespace Laster.Inputs.File
{
    public class OpenFileInput : IDataInput
    {
        public enum EMultipleType
        {
            Simple,
            MultipleAsArray,
            MultipleAsEnumerator
        }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public OpenFileInput() : base() { }

        public EMultipleType Multiselect { get; set; }
        public string InitialDirectory { get; set; }
        public string Filter { get; set; }
        public int FilterIndex { get; set; }

        public override string Title { get { return "OpenFile"; } }

        protected override IData OnGetData()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = Environment.ExpandEnvironmentVariables(InitialDirectory);
                dialog.Title = Title;
                dialog.Filter = Filter;
                dialog.FilterIndex = FilterIndex;
                dialog.CheckFileExists = true;
                dialog.RestoreDirectory = true;
                dialog.AutoUpgradeEnabled = true;
                dialog.Multiselect = Multiselect != EMultipleType.Simple;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    switch (Multiselect)
                    {
                        case EMultipleType.MultipleAsArray: return new DataArray(this, dialog.FileNames);
                        case EMultipleType.MultipleAsEnumerator: return new DataEnumerable(this, dialog.FileNames);

                        default: return new Core.Data.DataObject(this, dialog.FileName);
                    }
                }
            }

            return new DataEmpty(this);
        }
    }
}