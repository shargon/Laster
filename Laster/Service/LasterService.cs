using Laster.Core.Classes.Collections;
using System.ServiceProcess;

namespace Laster.Service
{
    public partial class LasterService : ServiceBase
    {
        LasterService _Current;
        DataInputCollection inputs;

        public LasterService(string name = "LasterService")
        {
            this.AutoLog = true;
            this.ServiceName = name;
            this.CanShutdown = true;
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            //this.EventLog.Log = MaisDriveService.Name;
            this.EventLog.Source = name;

            _Current = this;
        }
        public LasterService(DataInputCollection inputs)
        {
            this.inputs = inputs;
        }
        protected override void OnStart(string[] args)
        {
            if (!inputs.Start())
                Stop();
        }
        protected override void OnStop()
        {
            inputs.Stop();
        }
    }
}