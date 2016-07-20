using Laster.Core.Forms;
using Laster.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Laster.Core.Designer
{
    public class ScriptEditor : UITypeEditor
    {
        object editorService;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }
            if (editorService != null)
            {
                IScriptConfig s = context.Instance is IScriptConfig ? (IScriptConfig)context.Instance : null;

                using (FScriptForm f = new FScriptForm(s == null ? null : s.Options))
                {
                    f.Value = value.ToString();

                    if (f.ShowDialog() == DialogResult.OK)
                        value = f.Value;
                }
            }

            return value;
        }
    }
}