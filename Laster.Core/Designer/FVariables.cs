using Laster.Core.Classes;
using Laster.Core.Classes.Collections;
using System.Windows.Forms;
using System;
using Laster.Core.Interfaces;

namespace Laster.Core.Designer
{
    public partial class FVariables : FRememberForm
    {
        public static bool ShowForm(DataVariableCollection vars)
        {
            using (FVariables f = new FVariables())
            {
                foreach (Variable va in vars.Values) f.Add(va, false);

                if (f.ShowDialog() == DialogResult.OK)
                {
                    vars.Clear();
                    foreach (ListViewItem it in f.listView1.Items)
                    {
                        vars.Add(it.Text, new Variable(it.Text, it.SubItems[1].Text));
                    }

                    return true;
                }
            }
            return false;
        }
        FVariables() { InitializeComponent(); }

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
                case Keys.Delete:
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;

                        for (int x = listView1.SelectedItems.Count - 1; x >= 0; x--)
                            listView1.Items.Remove(listView1.SelectedItems[0]);

                        break;
                    }
            }
        }
        void button3_Click(object sender, EventArgs e)
        {
            Variable v = FVariable.ShowForm("Var" + (listView1.Items.Count + 1), "");

            if (v != null)
                Add(v, true);
        }
        void Add(Variable v, bool check)
        {
            if (check)
                foreach (ListViewItem it in listView1.Items)
                {
                    if (it.Text == v.Name)
                    {
                        it.SubItems[1].Text = v.Value;
                        return;
                    }
                }

            listView1.Items.Add(new ListViewItem(new string[] { v.Name, v.Value }));
        }
        void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1)
            {
                button3_Click(sender, e);
                return;
            }

            ListViewItem it = listView1.SelectedItems[0];
            Variable v = FVariable.ShowForm(it.Text, it.SubItems[1].Text);

            if (v != null)
            {
                listView1.Items.Remove(it);
                Add(v, true);
            }
        }
        void button4_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }
    }
}