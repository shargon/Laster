using System.ComponentModel;
using System.ServiceProcess;

namespace Laster.Service
{
    [RunInstaller(true)]
    public class LasterInstallerProcess : ServiceProcessInstaller
    {
        public LasterInstallerProcess()
        {
            this.Account = ServiceAccount.NetworkService;
        }
    }
}