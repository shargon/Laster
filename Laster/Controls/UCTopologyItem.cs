using Laster.Core.Interfaces;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace Laster.Controls
{
    public partial class UCTopologyItem : UserControl
    {
        bool _Selected = false;
        Image _Icon;

        public ITopologyItem Item { get; private set; }
        public string Title { get { return Item.Name; } }

        /// <summary>
        /// Controla la selección de elementos
        /// </summary>
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                if (_Selected == value) return;

                _Selected = value;
                Invalidate();
                if (value)
                {
                    BringToFront();

                    foreach (Control c in Parent.Controls)
                    {
                        if (c == this) continue;
                        if (c is UCTopologyItem)
                        {
                            UCTopologyItem ci = (UCTopologyItem)c;
                            if (ci.Selected)
                            {
                                ci.Selected = false;
                                break;
                            }
                        }
                    }
                }
            }
        }
        public UCTopologyItem()
        {
            InitializeComponent();
        }
        public UCTopologyItem(ITopologyItem v) : this()
        {
            Item = v;

            if (v is IDataInput)
            {
                BackColor = Color.Red;
                ForeColor = Color.White;
                RefreshIcon();
            }
            else
            {
                if (v is IDataProcess)
                {
                    BackColor = Color.Blue;
                    ForeColor = Color.White;
                }
                else
                {
                    if (v is IDataOutput)
                    {
                        BackColor = Color.Orange;
                        ForeColor = Color.White;
                    }
                }
            }
        }
        void RefreshIcon()
        {
            if (_Icon != null)
            {
                //_Icon.Dispose();
                _Icon = null;
            }
            if (Item is IDataInput)
            {
                IDataInput r = (IDataInput)Item;
                if (r.RaiseMode != null) _Icon = r.RaiseMode.GetIcon();
            }
            Invalidate();
        }
        public override string ToString()
        {
            return Item != null ? Item.Name : base.ToString();
        }
        void UCTopologyItem_Paint(object sender, PaintEventArgs e)
        {
            if (!_Selected)
            {
                using (Brush br = new SolidBrush(Color.FromArgb(210, Color.White)))
                    e.Graphics.FillRectangle(br, 0, 0, Width, Height);

                using (Pen pen = new Pen(BackColor, 1F))
                    e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }

            if (_Icon!= null)
            {
                e.Graphics.DrawImage(_Icon, 5, 7, 24, 24);
            }

            using (Brush br = new SolidBrush(_Selected ? ForeColor : Color.Black))
            using (StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
            })
                e.Graphics.DrawString(this.ToString(), Font, br, Width / 2, Height / 2, format);
        }
    }
}