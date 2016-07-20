using Laster.Core.Classes.Collections;
using Laster.Core.Forms;
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
                //args = new string[] { @"C:\Fuentes\LasterConfigs\bancos\bancos_pagos_gastos.tly" };
                args = new string[] { "--edit", @"C:\Fuentes\LasterConfigs\bancos\bancos_pagos_gastos.tly" };
            }
#endif
            bool efects = false;
            DataInputCollection inputs = new DataInputCollection();

            // Leer el contenido del final del archivo para ver si contiene una configuración
            byte[] pack = new byte[8];
            using (FileStream fs = new FileStream(Application.ExecutablePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Seek(fs.Length - pack.Length, SeekOrigin.Begin);
                if (fs.Read(pack, 0, pack.Length) == pack.Length)
                {
                    if (Encoding.ASCII.GetString(pack, pack.Length - 4, 4) == "PACK")
                    {
                        // Sacar el tamaño
                        int l = BitConverter.ToInt32(pack, 0);
                        fs.Seek(fs.Length - pack.Length - l, SeekOrigin.Begin);

                        byte[] data = new byte[l];
                        if (fs.Read(data, 0, l) == l)
                        {
                            // Sacar el contenido
                            data = CompressHelper.Compress(data, 0, l, false);

                            string json = Encoding.UTF8.GetString(data);
                            PacketHeader header = SerializationHelper.DeserializeFromJson<PacketHeader>(json);
                            if (header != null)
                            {
                                header.Encrypt(false);

                                json = Encoding.UTF8.GetString(header.D);
                                if (args != null && args.Length == 1 && args[0] == "--edit")
                                {
                                    if (!efects)
                                    {
                                        Application.SetCompatibleTextRenderingDefault(true);
                                        Application.EnableVisualStyles();
                                        efects = true;
                                    }

                                    string pwd = FInputText.ShowForm("Edit", "Insert edit password", "", true);
                                    if (!string.IsNullOrEmpty(pwd))
                                    {
                                        byte[] hash = Encoding.UTF8.GetBytes(pwd);
                                        hash = HashHelper.HashRaw(HashHelper.EHashType.Sha512, hash, 0, hash.Length);

                                        for (int x = header.H.Length - 1; x >= 0; x--)
                                            if (header.H[x] != hash[x])
                                                return;

                                        args = new string[] { "--edit", json };
                                    }
                                }
                                else
                                {
                                    TLYFile file = TLYFile.Load(json);
                                    if (file != null)
                                    {
                                        file.Compile(inputs);

                                        LasterHelper.SetEnvironmentPath(Application.ExecutablePath);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Ver si quiere editar o ejecutar una configuración
            if (inputs == null || inputs.Count == 0)
                if (args != null && args.Length > 0 && args[0] != "--edit")
                    foreach (string s in args)
                    {
                        TLYFile file = TLYFile.LoadFromFile(s);
                        if (file == null) continue;

                        file.Compile(inputs);
                        LasterHelper.SetEnvironmentPath(s);
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
                    if (!efects)
                    {
                        Application.SetCompatibleTextRenderingDefault(true);
                        Application.EnableVisualStyles();
                        efects = true;
                    }

                    string file = args.Length == 2 && args[0] == "--edit" ? args[1] : null;
                    Application.Run(new FEditTopology(file));
                }
            }
        }
    }
}