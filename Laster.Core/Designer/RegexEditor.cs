using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Laster.Core.Designer
{
    public class RegexEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        IWindowsFormsEditorService _editorService;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            _editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            // use a list box
            TextBox lb = new TextBox();
            lb.Multiline = true;
            lb.Width = 300;
            lb.Height = 100;
            lb.Text = value == null ? "" : value.ToString();

            // show this model stuff
            _editorService.DropDownControl(lb);

            if (!string.IsNullOrEmpty(lb.Text))
                return new Regex(lb.Text);

            return null;
        }

        void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            // close the drop down as soon as something is clicked
            _editorService.CloseDropDown();
        }
    }
}