using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Laster.Service
{
    [RunInstaller(true)]
    public class LasterServiceInstaller : ServiceInstaller
    {
        static string DefaultName = "LasterService";
        static string ConfigFile = "";

        public LasterServiceInstaller()
        {
            this.Description = "Servicio de sincronización de carpetas MaisDrive";
            this.DisplayName = DefaultName;
            this.ServiceName = DefaultName;
            this.StartType = ServiceStartMode.Automatic;
            this.DelayedAutoStart = true;
            this.ServicesDependedOn = new string[]
            {
                "Tcpip",
                "Dhcp",
                "LanmanServer"
            };
        }
        public override void Install(IDictionary stateSaver)
        {
            base.Context.Parameters["assemblyPath"] = string.Format("\"{0}\" --service {1}", base.Context.Parameters["assemblyPath"], ConfigFile);
            base.Install(stateSaver);
        }

        public static void Install(string name, params string[] configFiles)
        {
            DefaultName = name;
            string[] commandLineOptions = new string[] { "/ShowCallStack", "/LogFile=install.log" };

            using (AssemblyInstaller installer = new AssemblyInstaller(Application.ExecutablePath, commandLineOptions))
            {
                installer.UseNewContext = true;

                // Configuración de archivos
                ConfigFile = "";
                foreach (string f in configFiles)
                    ConfigFile += " \"" + f + "\"";

                ConfigFile = ConfigFile.Trim();

                IDictionary state = new Hashtable();

                installer.Install(state);
                installer.Commit(state);
            }
        }
        public static void Uninstall(string name)
        {
            DefaultName = name;
            string[] commandLineOptions = new string[] { "/ShowCallStack", "/LogFile=uninstall.log" };

            using (AssemblyInstaller installer = new AssemblyInstaller(Application.ExecutablePath, commandLineOptions))
            {
                installer.UseNewContext = true;

                IDictionary state = new Hashtable();
                installer.Uninstall(state);
            }
        }
        public bool IsInstalled(string name)
        {
            try
            {
                foreach (ServiceController s in ServiceController.GetServices())
                {
                    if (s.ServiceName.ToUpper() == name) return true;
                }
            }
            catch { }
            return false;
        }
    }
}