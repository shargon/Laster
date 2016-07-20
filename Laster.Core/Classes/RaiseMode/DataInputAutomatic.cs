using Laster.Core.Interfaces;
using System.Drawing;

namespace Laster.Core.Classes.RaiseMode
{
    public class DataInputAutomatic : IRaiseMode
    {
        public override void Start(IDataInput input)
        {
            base.Start(input);

            input.ProcessData();
            Stop(input);
        }
        public override Image GetIcon() { return Res.cursor; }
        public override string ToString() { return "Auto"; }
    }
}