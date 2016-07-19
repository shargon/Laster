using Laster.Core.Classes.Collections;
using Laster.Core.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace Laster
{
    //  Input  -> Process -> Process
    //         -> Process
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

            // Leer el contenido del final del archivo para ver si contiene una configuración
            byte[] pack = new byte[8];
            using (FileStream fs = new FileStream(Application.ExecutablePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Seek(fs.Length - 8, SeekOrigin.Begin);
                if (fs.Read(pack, 0, 8) == 8)
                {
                    if (Encoding.ASCII.GetString(pack, 4, 4) == "PACK")
                    {
                        // Sacar el tamaño
                        int l = BitConverter.ToInt32(pack, 0);
                        fs.Seek(fs.Length - 8 - l, SeekOrigin.Begin);

                        byte[] data = new byte[l];
                        if (fs.Read(data, 0, l) == l)
                        {
                            // Sacar el contenido
                            data = CompressHelper.Compress(data, 0, l, false);
                            string json = Encoding.UTF8.GetString(data);
                            TLYFile file = TLYFile.Load(json);
                            if (file != null)
                            {
                                file.Compile(inputs);

                                Environment.SetEnvironmentVariable("LasterConfigFile", Application.ExecutablePath, EnvironmentVariableTarget.Process);
                                Environment.SetEnvironmentVariable("LasterConfigPath", Path.GetDirectoryName(Application.ExecutablePath), EnvironmentVariableTarget.Process);
                            }
                        }
                    }
                }
            }

            // Ver si quiere editar o ejecutar una configuración
            if (inputs == null || inputs.Count > 0)
                if (args != null && args.Length > 0 && args[0] != "--edit")
                    foreach (string s in args)
                    {
                        TLYFile file = TLYFile.LoadFromFile(s);
                        if (file == null) continue;

                        file.Compile(inputs);

                        Environment.SetEnvironmentVariable("LasterConfigFile", s, EnvironmentVariableTarget.Process);
                        Environment.SetEnvironmentVariable("LasterConfigPath", Path.GetDirectoryName(s), EnvironmentVariableTarget.Process);
                    }

            // Ver si se ejecuta como servicio
            if (args.Length >= 1 && args[0].Equals("--service", StringComparison.InvariantCultureIgnoreCase))
            {
                ServiceBase[] ServicesToRun = new ServiceBase[] { new LasterService(inputs) };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                // Ver si se ejecuta o se muestra la edición
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
