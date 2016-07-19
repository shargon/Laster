using Laster.Core.Classes;
using Laster.Core.Interfaces;
using System;
using System.Windows.Forms;

namespace Laster.Core.Designer
{
    public partial class FVariable : FRememberForm
    {
        public static Variable ShowForm(string name, string value)
        {
            using (FVariable f = new FVariable())
            {
                f.tName.Text = name;
                f.tValue.Text = value;

                if (f.ShowDialog() == DialogResult.OK && f.tName.Text.Trim() != "")
                    return new Variable(f.tName.Text, f.tValue.Text);
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
                        if (tName.Focused)
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