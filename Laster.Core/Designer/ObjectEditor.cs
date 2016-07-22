using Laster.Core.Forms;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Laster.Core.Designer
{
    public class ObjectEditor : UITypeEditor
    {
        //object editorService;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value == null) return value;

            //if (provider != null) editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            //if (editorService != null)
            ShowValue(value);

            return value;
        }
        public static void ShowValue(object value)
        {
            if (value == null) return;

            if (value is DataSet || value is DataTable)
            {
                Panel p = new Panel();

                ComboBox cm = new ComboBox()
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Dock = DockStyle.Top
                };

                if (value is DataSet) foreach (DataTable dt in ((DataSet)value).Tables) cm.Items.Add(dt);
                else cm.Items.Add(value);

                cm.SelectedIndex = 0;

                DataGridView grid = new DataGridView();
                grid.Dock = DockStyle.Fill;
                grid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
                grid.DataSource = cm.SelectedItem;
                grid.ShowRowErrors = true;
                grid.ShowEditingIcon = true;
                grid.ShowCellErrors = true;

                cm.SelectedIndexChanged += (a, b) => { grid.DataSource = (DataTable)cm.SelectedItem; };

                p.Controls.Add(grid);
                p.Controls.Add(cm);

                FOkCancel.ShowForm("Debug", p);
            }
            else
            {
                FEditObjectProperties.ShowForm("Debug", value);
            }
        }
    }
}