using Laster.Core.Interfaces;
using System;
using System.Windows.Forms;

namespace Laster.Core.Forms
{
    public partial class FCreatePassword : FRememberForm
    {
        public static string ShowForm()
        {
            using (FCreatePassword f = new FCreatePassword())
            {
                if (f.ShowDialog() == DialogResult.OK && f.tPwd1.Text.Trim() != "")
                    return f.tPwd1.Text;
            }
            return null;
        }
        FCreatePassword() { InitializeComponent(); }
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
                        if (tPwd1.Focused)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                            tPwd2.Focus();
                            break;
                        }
                        if (tPwd2.Focused)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                            DialogResult = DialogResult.OK;
                        }
                        break;
                    }
            }
        }
        void textBox1_Enter(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.SelectAll();
        }
    }
}