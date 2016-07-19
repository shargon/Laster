using System.ComponentModel;
using System.Windows.Forms;

namespace Laster.Controls
{
    [Browsable(true)]
    public class UCPanelDoubleBufffer : Panel
    {
        public UCPanelDoubleBufffer()
        {
            try
            {
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.ResizeRedraw, true);
                DoubleBuffered = true;
            }
            catch { }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
            e.Graphics.Clear(BackColor);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //e.Graphics.Clear(BackColor);
            base.OnPaint(e);
        }
    }
}