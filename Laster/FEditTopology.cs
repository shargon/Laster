using Laster.Controls;
using Laster.Core.Classes;
using Laster.Core.Interfaces;
using Laster.Inputs;
using Laster.Outputs;
using Laster.Process;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Laster
{
    public partial class FEditTopology : Form
    {
        Point MouseDownLocation;
        BindingList<UCTopologyItem> _List = new BindingList<UCTopologyItem>();

        public FEditTopology()
        {
            InitializeComponent();

            BindingSource bs = new BindingSource();
            bs.DataSource = _List;
            cmItems.DataSource = _List;
            cmItems.DisplayMember = "Title";
            cmItems.ValueMember = "Title";

            //LoadActions(Assembly.GetAssembly(typeof(FEditTopology)));
            LoadActions(Assembly.GetAssembly(typeof(EmptyInput)));
            LoadActions(Assembly.GetAssembly(typeof(FileOutput)));
            LoadActions(Assembly.GetAssembly(typeof(EmptyProcess)));
        }

        public void LoadActions(Assembly asm)
        {
            Type tin = typeof(IDataInput);
            Type tou = typeof(IDataOutput);
            Type tpr = typeof(IDataProcess);

            foreach (Type t in asm.GetTypes())
            {
                if (t == tin || t == tou || t == tpr) continue;
                if (!t.IsPublic) continue;

                bool hay = false;
                foreach (ConstructorInfo o in t.GetConstructors())
                {
                    if (!o.IsPublic) continue;

                    hay = true;
                    break;
                }

                if (!hay) continue;

                if (tin.IsAssignableFrom(t)) { CreateDataInput(t); }
                else if (tpr.IsAssignableFrom(t)) { CreateDataProcess(t); }
                else if (tou.IsAssignableFrom(t)) { CreateDataOutput(t); }

            }
        }
        void AddItem(ITopologyItem n, ToolStripMenuItem parent)
        {
            ToolStripMenuItem m = new ToolStripMenuItem();
            m.Text = n.Name;

            m.Tag = n.GetType();
            parent.DropDownItems.Add(m);
            m.Click += M_Click;
        }
        void M_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            ITopologyItem n = (ITopologyItem)Activator.CreateInstance((Type)t.Tag);

            UCTopologyItem top = new UCTopologyItem((ITopologyItem)n);

            top.MouseDown += pictureBox1_MouseDown;
            top.MouseMove += pictureBox1_MouseMove;
            pItems.Controls.Add(top);

            _List.Add(top);

            cmItems.SelectedItem = top;
            propertyGrid1.SelectedObject = top.Item;
            top.Selected = true;
        }
        void CreateDataProcess(Type t)
        {
            using (IDataProcess d = (IDataProcess)Activator.CreateInstance(t))
            {
                AddItem(d, processToolStripMenuItem);
            }
        }
        void CreateDataOutput(Type t)
        {
            using (IDataOutput d = (IDataOutput)Activator.CreateInstance(t))
            {
                AddItem(d, outputsToolStripMenuItem);
            }
        }
        void CreateDataInput(Type t)
        {
            using (IDataInput d = (IDataInput)Activator.CreateInstance(t))
            {
                AddItem(d, inputToolStripMenuItem);
            }
        }

        void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            UCTopologyItem top = (UCTopologyItem)sender;

            MouseDownLocation = e.Location;
            cmItems.SelectedItem = top;
            propertyGrid1.SelectedObject = top.Item;
            top.Selected = true;

            switch (e.Button)
            {
                case MouseButtons.Right:
                    {

                        break;
                    }
            }
        }
        void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Control c = (Control)sender;
                c.Left = e.X + c.Left - MouseDownLocation.X;
                c.Top = e.Y + c.Top - MouseDownLocation.Y;
            }
        }
        void cmItems_Format(object sender, ListControlConvertEventArgs e)
        {
            UCTopologyItem top = (UCTopologyItem)e.ListItem;
            e.Value = top.ToString();
        }
        void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGrid1.SelectedObject is ITopologyItem)
            {
                ITopologyItem t = (ITopologyItem)propertyGrid1.SelectedObject;

                pItems.Invalidate(true);

                for (int x = 0; x < _List.Count; x++)
                    _List.ResetItem(x);
            }
        }
        void cmItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            UCTopologyItem top = (UCTopologyItem)cmItems.SelectedItem;
            propertyGrid1.SelectedObject = top.Item;
            top.Selected = true;
        }
        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
