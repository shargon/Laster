using Laster.Core.Classes.Collections;
using System.ServiceProcess;

namespace Laster
{
    public partial class LasterService : ServiceBase
    {
        DataInputCollection inputs;

        public LasterService() { InitializeComponent(); }
        public LasterService(DataInputCollection inputs) { this.inputs = inputs; }
        protected override void OnStart(string[] args) { inputs.Start(); }
        protected override void OnStop() { inputs.Stop(); }
    }
}