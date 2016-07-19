using Laster.Core.Classes.Collections;
using System;
using System.Diagnostics;
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
#if DEBUG
            if (Debugger.IsAttached)
            {
                //args = new string[] { @"C:\Users\Fernando\Desktop\bancos_pagos_gastos.tly" };
                args = new string[] { "--edit", @"C:\Users\Fernando\Desktop\bancos\Source\bancos_pagos_gastos.tly" };
            }
#endif
            DataInputCollection inputs = new DataInputCollection();
            if (args != null && args.Length > 0 && args[0] != "--edit")
            {
                foreach (string s in args)
                {
                    if (File.Exists(s))
                    {
                        TLYFile file = TLYFile.Load(s);
                        if (file == null) continue;

                        file.Compile(inputs);

                        Environment.SetEnvironmentVariable("LasterConfigFile", s, EnvironmentVariableTarget.Process);
                        Environment.SetEnvironmentVariable("LasterConfigPath", Path.GetDirectoryName(s), EnvironmentVariableTarget.Process);
                    }
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
                    if (inputs.Start())
                        Application.Run();
                }
                else
                {
                    string file = args.Length == 2 && args[0] == "--edit" ? args[1] : null;
                    Application.Run(new FEditTopology(file));
                }
            }
        }
    }
}
