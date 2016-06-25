using Laster.Core.Interfaces;
using System.Drawing;
using System.Windows.Forms;

namespace Laster.Controls
{
    public partial class UCTopologyItem : UserControl
    {
        bool _Selected = false, _InPlay = false;
        Image _Icon;

        static Brush _UnSelectedWhiteBrush = new SolidBrush(Color.FromArgb(210, Color.White));
        static Brush _InUseWhite = new SolidBrush(Color.FromArgb(160, Color.White));
        static Brush _UnselectedTextBrush = new SolidBrush(Color.Black);

        static StringFormat _CenterFormat = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };

        Pen _UnselectedBorderPen;
        Brush _SelectedTextBrush;

        public ITopologyItem Item { get; private set; }
        public string Title { get { return Item.ToString(); } }
        public AreInUse AreInUse { get; set; }

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

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);

            Disposed += UCTopologyItem_Disposed;
        }
        void UCTopologyItem_Disposed(object sender, System.EventArgs e)
        {
            if (_UnselectedBorderPen != null)
            {
                _UnselectedBorderPen.Dispose();
                _UnselectedBorderPen = null;
            }
            if (_SelectedTextBrush != null)
            {
                _SelectedTextBrush.Dispose();
                _SelectedTextBrush = null;
            }
            Item.Dispose();
            Item = null;
        }
        public UCTopologyItem(ITopologyItem v) : this()
        {
            Item = v;
            AreInUse = new AreInUse();

            if (v is IDataInput)
            {
                BackColor = Color.Green;
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

            _UnselectedBorderPen = new Pen(BackColor, 1F);
            _SelectedTextBrush = new SolidBrush(ForeColor);
        }
        public void RefreshInPlay(bool inPlay)
        {
            _InPlay = inPlay;
            //Invalidate();
        }
        public void RefreshIcon()
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
            // Disposed
            if (_UnselectedBorderPen == null) return;

            if (!_Selected)
            {
                e.Graphics.FillRectangle(_UnSelectedWhiteBrush, 0, 0, Width, Height);
                e.Graphics.DrawRectangle(_UnselectedBorderPen, 0, 0, Width - 1, Height - 1);
            }

            if (_Icon != null)
                e.Graphics.DrawImage(_Icon, 5, 7, 24, 24);

            if (_InPlay && !AreInUse.InUse)
                e.Graphics.FillRectangle(_InUseWhite, ClientRectangle);

            e.Graphics.DrawString(Item.Name, Font, _Selected ? _SelectedTextBrush : _UnselectedTextBrush, Width / 2, Height / 2, _CenterFormat);
        }
    }
}