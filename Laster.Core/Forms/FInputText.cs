using Laster.Core.Interfaces;
using System.Windows.Forms;

namespace Laster.Core.Forms
{
    public partial class FInputText : FRememberForm
    {
        public static string ShowForm(string title, string label, string def = "", bool isPassword = false)
        {
            using (FInputText f = new FInputText())
            {
                f.Text = title;
                f.label1.Text = label;
                f.tValue.Text = def;
                f.tValue.SelectAll();

                if (isPassword)
                    f.tValue.UseSystemPasswordChar = true;

                if (f.ShowDialog() == DialogResult.OK)
                    return f.tValue.Text;
            }
            return null;
        }
        FInputText()
        {
            InitializeComponent();
        }
        void FVariables_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        Close();
                        break;
                    }
                case Keys.Enter:
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        DialogResult = DialogResult.OK;
                        break;
                    }
            }
        }
    }
}