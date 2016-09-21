using Laster.Core.Interfaces;
using System.ComponentModel;
using System.Drawing;

namespace Laster.Core.Classes.RaiseMode
{
    public class DataInputAutomatic : ITriggerRaiseMode
    {
        /// <summary>
        /// True for when start, raise action, and stop
        /// </summary>
        [DefaultValue(true)]
        public bool RunOnStart { get; set; }
        /// <summary>
        /// Stop on start
        /// </summary>
        [DefaultValue(true)]
        public bool StopOnStart { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DataInputAutomatic()
        {
            RunOnStart = true;
            StopOnStart = true;
        }

        public override void Start(IDataInput input)
        {
            base.Start(input);

            if (RunOnStart) input.ProcessData();
            if (StopOnStart) Stop(input);
        }
        public override Image GetIcon() { return Res.cursor; }
        public override string ToString() { return "Auto"; }
    }
}