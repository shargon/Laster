using Laster.Core.Classes;
using Laster.Core.Interfaces;
using System;
using System.Windows.Forms;

namespace Laster.Core.Forms
{
    public partial class FVariable : FRememberForm
    {
        public static Variable ShowForm(string obj, string property, string value)
        {
            using (FVariable f = new FVariable())
            {
                f.tObject.Text = obj;
                f.tProperty.Text = property;
                f.tValue.Text = value;

                if (f.ShowDialog() == DialogResult.OK && f.tProperty.Text.Trim() != "")
                    return new Variable(f.tObject.Text, f.tProperty.Text, f.tValue.Text);
            }
            return null;
        }
        public FVariable()
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
                        if (tObject.Focused)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                            tProperty.Focus();
                            break;
                        }
                        if (tProperty.Focused)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                            tValue.Focus();
                            break;
                        }
                        if (tValue.Focused)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                            DialogResult = DialogResult.OK;
                            break;
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