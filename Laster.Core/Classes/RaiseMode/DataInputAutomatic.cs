using Laster.Core.Interfaces;
using System.Drawing;

namespace Laster.Core.Classes.RaiseMode
{
    public class DataInputAutomatic : IRaiseMode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DataInputAutomatic() { }

        public override void Start(IDataInput input)
        {
            base.Start(input);
            input.ProcessData();
            Stop(input);
        }
        public override void Stop(IDataInput input)
        {
            base.Stop(input);
        }
        public override Image GetIcon() { return Res.cursor; }
        public override string ToString()
        {
            return "Auto";
        }
    }
}