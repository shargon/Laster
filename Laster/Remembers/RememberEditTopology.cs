using System.Windows.Forms;

namespace Laster.Remembers
{
    public class RememberEditTopology : RememberForm
    {
        public int SppliterDistance { get; set; }

        public RememberEditTopology() : base() { }
        public RememberEditTopology(FEditTopology f) : base(f)
        {
            SppliterDistance = f.pGrid.Width;
        }

        public override void Apply(Form f)
        {
            base.Apply(f);

            if (SppliterDistance > 100 && f is FEditTopology)
            {
                FEditTopology fe = (FEditTopology)f;
                fe.pGrid.Width = SppliterDistance;
            }
        }
    }
}