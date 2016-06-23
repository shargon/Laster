using Laster.Controls;
using Laster.Core.Classes.Collections;
using Laster.Core.Interfaces;
using System.Drawing;
using System.Windows.Forms;

namespace Laster
{
    public class ConnectedLine
    {
        public ITopologyItem ToItem { get { return To == null ? null : To.Item; } }
        public ITopologyItem FromItem { get { return From == null ? null : From.Item; } }

        public UCTopologyItem From { get; set; }
        public UCTopologyItem To { get; set; }

        public enum EPosition { Left, Right, Top, Bottom }

        public bool IsAllowed(UCTopologyItem to)
        {
            if (to == null) return false;

            ITopologyItem tfrom = FromItem;
            ITopologyItem tto = to.Item;

            if (tfrom is IDataInput || tfrom is IDataProcess)
                return tto is IDataOutput || tto is IDataProcess;

            return false;
        }

        public static Point GetPositionFrom(Rectangle r, EPosition position)
        {
            switch (position)
            {
                case EPosition.Left: return new Point(r.Left, r.Top + (r.Height / 2));
                case EPosition.Right: return new Point(r.Right, r.Top + (r.Height / 2));
                case EPosition.Top: return new Point(r.Left + (r.Width / 2), r.Top);
                case EPosition.Bottom: return new Point(r.Left + (r.Width / 2), r.Bottom);
            }
            return Point.Empty;
        }

        public void GetPointFromDraw(Control parent, out Point from, out Point to)
        {
            from = Point.Empty;
            to = Point.Empty;

            Rectangle rTo;
            if (To != null) rTo = To.Bounds;
            else rTo = new Rectangle(parent.PointToClient(Cursor.Position), new Size(2, 2));

            Rectangle rFrom = From.Bounds;

            if (rFrom.Left > rTo.Right)
            {
                // Se sale por la derecha
                from = GetPositionFrom(rFrom, EPosition.Left);
                to = GetPositionFrom(rTo, EPosition.Right);
            }
            else
            {
                if (rFrom.Right < rTo.Left)
                {
                    // Se sale por la izquierda
                    from = GetPositionFrom(rFrom, EPosition.Right);
                    to = GetPositionFrom(rTo, EPosition.Left);
                }
                else
                {
                    if (rFrom.Top < rTo.Bottom)
                    {
                        // Está arriba
                        from = GetPositionFrom(rFrom, EPosition.Bottom);
                        to = GetPositionFrom(rTo, EPosition.Top);
                    }
                    else
                    {
                        // Está abajo
                        from = GetPositionFrom(rFrom, EPosition.Top);
                        to = GetPositionFrom(rTo, EPosition.Bottom);
                    }
                }
            }
        }
        public bool Apply(UCTopologyItem top)
        {
            if (!IsAllowed(top)) return false;

            ITopologyItem to = top.Item;

            DataProcessCollection proc;
            DataOutputCollection dout;

            if (From.Item is ITopologyRelationableItem)
            {
                ITopologyRelationableItem d = (ITopologyRelationableItem)From.Item;
                proc = d.Process;
                dout = d.Out;
            }
            else return false;

            if (to is IDataProcess)
            {
                if (!proc.Contains((IDataProcess)to))
                {
                    proc.Add((IDataProcess)to);
                    To = top;
                    return true;
                }
            }
            if (to is IDataOutput)
            {
                if (!dout.Contains((IDataOutput)to))
                {
                    dout.Add((IDataOutput)to);
                    To = top;
                    return true;
                }
            }

            return false;
        }
    }
}