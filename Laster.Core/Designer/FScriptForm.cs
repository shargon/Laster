using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Laster.Core.Designer
{
    public partial class FScriptForm : FRememberForm
    {
        ScriptHelper.ScriptOptions _Options;
        public string Value { get { return tEdit.Text; } set { tEdit.Text = value; } }
        public FScriptForm(ScriptHelper.ScriptOptions opt) : base()
        {
            InitializeComponent();

            _Options = opt;
            tEdit.SetHighlighting("C#");
            tEdit.ActiveTextAreaControl.TextEditorProperties.IndentationSize = 0;
            tEdit.ActiveTextAreaControl.TextArea.KeyUp += TextArea_KeyUp;
        }
        void toolStripButton3_Click(object sender, EventArgs e) { Close(); }
        void toolStripButton1_Click(object sender, EventArgs e)
        {
            toolStripButton2_Click(sender, e);
            if (tError.Tag == null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        void toolStripButton2_Click(object sender, EventArgs e)
        {
            tError.Tag = null;
            tError.Text = "";
            tError.Visible = true;

            try
            {
                ScriptHelper helper = ScriptHelper.CreateFromString(tEdit.Text, _Options);
                if (helper != null)
                {
                    IScriptProcess sc = helper.CreateNewInstance<IScriptProcess>();
                    if (sc == null) throw (new Exception("Error"));
                }
                else throw (new Exception("Error"));

                tError.ForeColor = Color.Green;
                tError.Text = "OK";
            }
            catch (Exception ex)
            {
                tError.Tag = true;
                tError.ForeColor = Color.Red;
                tError.Text = ex.Message;
            }
        }
        void tError_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            tError.Visible = false;
        }
        void TextArea_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                Close();
            }
        }
        void FScriptForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F5:
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        toolStripButton2_Click(null, null);
                        break;
                    }
                case Keys.F3:
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        toolStripButton1_Click(null, null);
                        break;
                    }
            }
        }
    }
}