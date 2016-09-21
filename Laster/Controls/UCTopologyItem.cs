using Laster.Core.Interfaces;
using System.Drawing;
using System.Windows.Forms;
using Laster.Core.Extensions;
using System.Drawing.Drawing2D;
using System;
using Laster.Core.Helpers;

namespace Laster.Controls
{
    public partial class UCTopologyItem : UserControl
    {
        bool _Selected = false, _InPlay = false;
        Image _Icon;

        static Brush _UnSelectedWhiteBrush = new SolidBrush(Color.FromArgb(220, Color.White));
        static Brush _InUseWhite = new SolidBrush(Color.FromArgb(200, Color.White));
        //static Brush _UnSelectedWhiteBrush = new HatchBrush(HatchStyle.Percent90, Color.FromArgb(210, Color.White));
        //static Brush _InUseWhite = new HatchBrush(HatchStyle.NarrowHorizontal, Color.FromArgb(200, Color.White));
        static Brush _UnselectedTextBrush = new SolidBrush(Color.Black);
        RectangleEdgeFilter _EdgeFilter;
        float _EdgeBorder;

        static StringFormat _CenterFormat = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };

        bool _IsDataInput;
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

            DoubleBuffered = true;

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
            _IsDataInput = Item == null || Item is IDataInput;

            RefreshDesign();
            if (_IsDataInput)
            {
                Size = new Size((int)(Width * 1.2), (int)(Height * 1.2));
            }

            if (!_IsDataInput && SystemHelper.IsWindows)
            {
                using (Graphics gp = CreateGraphics())
                {
                    if (_IsDataInput)
                        _EdgeFilter = RectangleEdgeFilter.BottomLeft | RectangleEdgeFilter.TopRight;
                    else
                        _EdgeFilter = RectangleEdgeFilter.All;

                    _EdgeBorder = 10F;
                    Region = new Region(gp.GenerateRoundedRectangle(Bounds, _EdgeBorder, _EdgeFilter));
                }
            }
            else
            {
                _EdgeFilter = RectangleEdgeFilter.None;
                _EdgeBorder = 0;
            }
        }
        public void RefreshInPlay(bool inPlay)
        {
            _InPlay = inPlay;
            AreInUse.Clear();
            //Invalidate();
        }
        public void RefreshDesign()
        {
            BackColor = Item.DesignBackColor;
            ForeColor = Item.DesignForeColor;

            if (_UnselectedBorderPen != null) _UnselectedBorderPen.Dispose();
            if (_SelectedTextBrush != null) _SelectedTextBrush.Dispose();

            _UnselectedBorderPen = new Pen(BackColor, 1F);

            _SelectedTextBrush = new SolidBrush(ForeColor);
            //_SelectedTextBrush = new HatchBrush(HatchStyle.NarrowHorizontal, ForeColor);

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
                if (_IsDataInput)
                    e.Graphics.FillRectangle(_UnSelectedWhiteBrush, 34, 0, Width - 34, Height);
                else
                {
                    //if (_EdgeBorder == 0)
                    e.Graphics.FillRectangle(_UnSelectedWhiteBrush, 0, 0, Width, Height);
                    //else
                    //e.Graphics.FillRoundedRectangle(_UnSelectedWhiteBrush, 0, 0, Width, Height, _EdgeBorder, _EdgeFilter);
                }

                if (_EdgeBorder == 0)
                    e.Graphics.DrawRectangle(_UnselectedBorderPen, 0, 0, Width - 1, Height - 1);
                else
                    e.Graphics.DrawRoundedRectangle(_UnselectedBorderPen, 0, 0, Width - 1, Height - 1, _EdgeBorder, _EdgeFilter);
            }

            if (_Icon != null)
                e.Graphics.DrawImage(_Icon, 5, (Height - 24) / 2, 24, 24);

            if (_InPlay && !AreInUse.InUse)
            {
                //e.Graphics.FillRoundedRectangle(_InUseWhite, ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height, 5F, RectangleEdgeFilter.All);
                e.Graphics.FillRectangle(_InUseWhite, ClientRectangle);
            }

            e.Graphics.DrawString(Item.Name, Font, _Selected ? _SelectedTextBrush : _UnselectedTextBrush, (Width + (_IsDataInput ? 25 : 0)) / 2, Height / 2, _CenterFormat);
        }
    }
}