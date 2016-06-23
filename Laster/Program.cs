using Laster.Core.Classes.Collections;
using Laster.Inputs;
using Laster.Outputs;
using Laster.Process;
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
            args = new string[] { "D:\\test.tly" };

            // Cargar los ensamblados por defecto de primeras
            // Todo que el TLYFile tenga una cabecera de los tipos utilizados para cargarlos previamente

            Type t = typeof(EmptyInput);
            t = typeof(FileOutput);
            t = typeof(EmptyProcess);

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
