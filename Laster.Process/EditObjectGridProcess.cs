using Laster.Core.Enums;
using Laster.Core.Interfaces;
using Laster.Process.Forms;
using System;
using System.Windows.Forms;

namespace Laster.Process
{
    /// <summary>
    /// Edita un objeto
    /// </summary>
    public class EditObjectGridProcess : IDataProcess
    {
        DataGridView _Grid;
        public string FormTitle { get; set; }

        public override string Title { get { return "Edit Object [Grid]"; } }

        public string EditProperty { get; set; }

        public DataGridView Grid
        {
            get { return _Grid; }
        }
        public EditObjectGridProcess()
        {
            _Grid = new DataGridView();
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            object o = data.GetInternalObject();
            if (o == null) return null;

            object edit = o;
            if (!string.IsNullOrEmpty(EditProperty))
            {
                Type t = edit.GetType();
                edit = t.GetProperty(EditProperty).GetValue(edit);
            }

            _Grid.AutoGenerateColumns = _Grid.ColumnCount == 0;
            _Grid.DataSource = edit;

            if (!FOkCancel.ShowForm(FormTitle, _Grid))
                return null;

            return data;
        }
    }
}