using System.ComponentModel;
using System.Drawing.Design;

namespace Laster.Core.Designer
{
    public class DataInputRaiseEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            //IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            //Foo foo = value as Foo;
            //if (svc != null && foo != null)
            //{
            //    using (FooForm form = new FooForm())
            //    {
            //        form.Value = foo.Bar;
            //        if (svc.ShowDialog(form) == DialogResult.OK)
            //        {
            //            foo.Bar = form.Value; // update object
            //        }
            //    }
            //}
            return value;
        }
    }
}
