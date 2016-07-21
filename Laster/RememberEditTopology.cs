using Laster.Core.Remembers;
using System.Windows.Forms;

namespace Laster.Remembers
{
    public class RememberEditTopology : RememberForm
    {
        public int SppliterDistance { get; set; }

         
        public override void GetValues(Form f)
        {
            base.GetValues(f);

            if (SppliterDistance > 100 && f is FEditTopology)
            {
                FEditTopology fe = (FEditTopology)f;
                fe.pGrid.Width = SppliterDistance;
            }
        }
        public override void SaveValues(Form f)
        {
            base.SaveValues(f);

            if (f is FEditTopology)
            {
                FEditTopology fe = (FEditTopology)f;
                SppliterDistance = fe.pGrid.Width;
            }
        }
    }
}