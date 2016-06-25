using System.Collections;
using System.Windows.Forms;

namespace Laster
{
    public class ToolStripItemComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            ToolStripItem oItem1 = (ToolStripItem)x;
            ToolStripItem oItem2 = (ToolStripItem)y;
            return string.Compare(oItem1.Text, oItem2.Text, true);
        }

        public static void SortToolStripItemCollection(ToolStripItemCollection coll)
        {
            System.Collections.ArrayList oAList = new System.Collections.ArrayList(coll);
            oAList.Sort(new ToolStripItemComparer());
            coll.Clear();

            foreach (ToolStripItem oItem in oAList)
            {
                coll.Add(oItem);
            }
        }
    }
}