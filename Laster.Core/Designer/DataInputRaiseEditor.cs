using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Laster.Core.Designer
{
    public class DataInputRaiseEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        IWindowsFormsEditorService _editorService;
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            _editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            // use a list box
            ListBox lb = new ListBox();
            lb.SelectionMode = SelectionMode.One;
            lb.SelectedValueChanged += OnListBoxSelectedValueChanged;

            // use the IBenchmark.Name property for list box display
            lb.DisplayMember = "Name";

            foreach (Type tp in ReflectionHelper.GetTypesAssignableFrom(typeof(IDataInputRaiseMode), AppDomain.CurrentDomain.GetAssemblies()))
            {
                if (!ReflectionHelper.HavePublicConstructor(tp)) continue;

                int index = lb.Items.Add(Activator.CreateInstance(tp));
                if (tp == value.GetType())
                {
                    lb.SelectedIndex = index;
                }
            }

            // show this model stuff
            _editorService.DropDownControl(lb);
            if (lb.SelectedItem == null) // no selection, return the passed-in value as is
                return value;

            return lb.SelectedItem;
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
            //return value;
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            // close the drop down as soon as something is clicked
            _editorService.CloseDropDown();
        }
    }
}
