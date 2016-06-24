using Laster.Core.Classes.Collections;
using System;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Laster
{
    //  Input  -> Process -> Process -> Output
    //         -> Output
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //args = new string[] { "d:\\test.tly" };

            DataInputCollection inputs = new DataInputCollection();
            if (args != null) foreach (string s in args)
                {
                    if (File.Exists(s))
                    {
                        TLYFile file = TLYFile.Load(s);
                        if (file == null) continue;

                        file.Compile(inputs);
                    }
                }

            if (args.Length >= 1 && args[0].Equals("--service", StringComparison.InvariantCultureIgnoreCase))
            {
                ServiceBase[] ServicesToRun = new ServiceBase[] { new LasterService(inputs) };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                if (inputs == null || inputs.Count > 0)
                {
                    inputs.Start();
                    Application.Run();
                }
                else
                {
                    Application.Run(new FEditTopology());
                }
            }
        }
    }
}
