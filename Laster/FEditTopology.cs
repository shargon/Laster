using Laster.Controls;
using Laster.Core.Classes;
using Laster.Core.Classes.Collections;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using Laster.Inputs;
using Laster.Outputs;
using Laster.Process;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace Laster
{
    public partial class FEditTopology : Form
    {
        Point MouseDownLocation;
        DataVariableCollection _Vars = new DataVariableCollection();
        BindingList<UCTopologyItem> _List = new BindingList<UCTopologyItem>();
        List<ConnectedLine> _Lines = new List<ConnectedLine>();

        ConnectedLine _Current = new ConnectedLine();

        public FEditTopology()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            BindingSource bs = new BindingSource();
            bs.DataSource = _List;
            cmItems.DataSource = _List;
            cmItems.DisplayMember = "Title";
            cmItems.ValueMember = "Title";

            LoadActions(Assembly.GetAssembly(typeof(EmptyInput)));
            LoadActions(Assembly.GetAssembly(typeof(FileOutput)));
            LoadActions(Assembly.GetAssembly(typeof(ScriptProcess)));
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
                if (!ReflectionHelper.HavePublicConstructor(t)) continue;
                
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
            CreateItem(n, Point.Empty);
        }
        void Select(UCTopologyItem top)
        {
            if (top == null)
            {
                if (cmItems.SelectedItem != null && cmItems.SelectedItem is UCTopologyItem)
                {
                    ((UCTopologyItem)cmItems.SelectedItem).Selected = false;
                }

                propertyGrid1.SelectedObject = _Vars.Designer;
                cmItems.SelectedItem = null;
                _Current = new ConnectedLine();
                pItems.Invalidate();
            }
            else
            {
                cmItems.SelectedItem = top;
                propertyGrid1.SelectedObject = top.Item;
                top.Selected = true;
            }

            propertyGrid1.Update();
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
            Select(top);

            switch (e.Button)
            {
                default:
                    {
                        if (_Current.From != null)
                        {
                            goto case MouseButtons.Right;
                        }
                        break;
                    }
                case MouseButtons.Right:
                    {
                        if (_Current.From == null)
                        {
                            if (!(top.Item is IDataOutput))
                            {
                                _Current.From = top;
                                pItems.Invalidate();
                            }
                        }
                        else
                        {
                            if (_Current.IsAllowed(top))
                            {
                                if (_Current.Apply(top))
                                {
                                    _Lines.Add(_Current);
                                    _Current = new ConnectedLine();
                                    pItems.Invalidate();
                                }
                                else
                                {
                                    _Current.To = null;
                                    _Current.From = null;
                                    pItems.Invalidate();
                                }
                            }
                            else
                            {
                                _Current.To = null;
                                _Current.From = null;
                                pItems.Invalidate();
                            }
                        }
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

                if (_Lines.Count > 0) pItems.Invalidate();
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
            Select(top);
        }
        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        void pItems_Paint(object sender, PaintEventArgs e)
        {
            Point from;
            Point to;

            if (_Current.From != null)
            {
                _Current.GetPointFromDraw(pItems, out from, out to);

                using (Pen pen = new Pen(_Current.From.BackColor, 10F))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.ArrowAnchor;

                    e.Graphics.DrawLine(pen, from, to);
                }
            }

            foreach (ConnectedLine c in _Lines)
            {
                c.GetPointFromDraw(pItems, out from, out to);

                using (Pen pen = new Pen(c.From.BackColor, 10F))
                {
                    pen.StartCap = LineCap.RoundAnchor;
                    pen.EndCap = LineCap.ArrowAnchor;

                    e.Graphics.DrawLine(pen, from, to);
                }
            }
        }
        void pItems_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Current != null && _Current.From != null)
                pItems.Invalidate();
        }
        void FEditTopology_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        if (_Current.From != null)
                        {
                            _Current.From = null;
                            _Current.To = null;
                            pItems.Invalidate();

                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        break;
                    }
                case Keys.Delete:
                    {
                        if (_Current.From != null)
                            goto case Keys.Escape;

                        UCTopologyItem uc = (UCTopologyItem)cmItems.SelectedItem;
                        if (uc != null)
                        {
                            bool entra = false;

                            for (int x = _Lines.Count - 1; x >= 0; x--)
                            {
                                ConnectedLine c = _Lines[x];
                                if (c.From == uc || c.To == uc)
                                {
                                    _Lines.Remove(c);

                                    if (c.FromItem is ITopologyRelationableItem)
                                    {
                                        ITopologyRelationableItem cx = (ITopologyRelationableItem)c.FromItem;
                                        cx.Process.Clear();
                                        cx.Out.Clear();
                                    }
                                    if (c.ToItem is ITopologyRelationableItem)
                                    {
                                        ITopologyRelationableItem cx = (ITopologyRelationableItem)c.ToItem;
                                        cx.Process.Clear();
                                        cx.Out.Clear();
                                    }
                                    entra = true;
                                }
                            }

                            if (!entra)
                            {
                                _List.Remove(uc);
                                uc.Parent.Controls.Remove(uc);
                                uc.Dispose();

                                foreach (UCTopologyItem c in pItems.Controls)
                                {
                                    Select(c);
                                    break;
                                }
                            }

                            pItems.Invalidate();
                            propertyGrid1.Update();

                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        break;
                    }
            }
        }
        void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sv = new SaveFileDialog()
            {
                Title = "Topology Files",
                Filter = "Topology Files|*.tly"
            })
            {
                if (sv.ShowDialog() != DialogResult.OK) return;

                TLYFile t = new TLYFile();
                t.Variables = _Vars;

                int id = 0;
                foreach (UCTopologyItem u in pItems.Controls)
                {
                    u.Item.Id = id;
                    id++;

                    t.Items.Add(u.Item.Id, new TLYFile.TopologyItem() { Item = u.Item, Position = u.Location });
                }

                foreach (ConnectedLine line in _Lines)
                    t.Relations.Add(new TLYFile.Relation() { From = line.FromItem.Id, To = line.ToItem.Id });

                t.Save(sv.FileName);
            }
        }
        void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog sv = new OpenFileDialog()
            {
                Title = "Topology Files",
                Filter = "Topology Files|*.tly"
            })
            {
                if (sv.ShowDialog() != DialogResult.OK) return;

                TLYFile t = TLYFile.Load(sv.FileName);
                if (t != null)
                {
                    _Lines.Clear();
                    _Current = new ConnectedLine();
                    _List.Clear();
                    _Vars.Clear();
                    Select(null);
                    pItems.Controls.Clear();

                    if (t.Variables != null)
                    {
                        foreach (Variable v in t.Variables.Values)
                            _Vars.Add(v);
                    }

                    if (t.Items.Values != null)
                    {
                        foreach (TLYFile.TopologyItem i in t.Items.Values)
                        {
                            CreateItem(i.Item, i.Position);
                        }

                        if (t.Relations != null)
                        {
                            foreach (TLYFile.Relation rel in t.Relations)
                            {
                                TLYFile.TopologyItem from, to;
                                if (t.Items.TryGetValue(rel.From, out from) && t.Items.TryGetValue(rel.To, out to) && from != null && to != null)
                                {
                                    UCTopologyItem searchFrom = SearchControl(from.Item);
                                    UCTopologyItem searchTo = SearchControl(to.Item);

                                    if (searchFrom != null && searchTo != null)
                                    {
                                        _Lines.Add(new ConnectedLine() { From = searchFrom, To = searchTo });

                                        if (from.Item is ITopologyRelationableItem)
                                        {
                                            ITopologyRelationableItem rfrom = (ITopologyRelationableItem)from.Item;

                                            if (to.Item is IDataProcess) rfrom.Process.Add((IDataProcess)to.Item);
                                            else if (to.Item is IDataOutput) rfrom.Out.Add((IDataOutput)to.Item);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    pItems.Invalidate();
                }
            }
        }
        UCTopologyItem SearchControl(ITopologyItem item)
        {
            if (item == null) return null;
            foreach (UCTopologyItem c in pItems.Controls)
            {
                if (c.Item == item) return c;
            }
            return null;
        }
        void CreateItem(ITopologyItem n, Point location)
        {
            if (n == null) return;

            UCTopologyItem top = new UCTopologyItem(n);
            top.Location = location;

            top.MouseDown += pictureBox1_MouseDown;
            top.MouseMove += pictureBox1_MouseMove;
            pItems.Controls.Add(top);

            _List.Add(top);
            Select(top);
        }
        void pItems_MouseDown(object sender, MouseEventArgs e)
        {
            Select(null);
        }
    }
}
