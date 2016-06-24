using Laster.Core.Classes.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Laster.Core.Designer
{
    public class DataVariableCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            DataVariableCollection.DesignClassLink v = (DataVariableCollection.DesignClassLink)value;

            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            if (svc != null && v != null)
            {
                using (FVariables form = new FVariables(v))
                {
                    if (svc.ShowDialog(form) == DialogResult.OK)
                    {
                        v.ApplyToParent();
                    }
                }
            }
            return value;
        }
    }
}
