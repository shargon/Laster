using Laster.Core.Classes.Collections;
using Laster.Core.Forms;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using Laster.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Threading;
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
            Application.ThreadException += Application_ThreadException;
            // Error al cargar la libería
            ReflectionHelper.RedirectAssembly("Newtonsoft.Json", new Version(9, 0), "30ad4fe6b2a6aeed");

#if DEBUG
            if (Debugger.IsAttached)
            {
                //args = new string[] { "--edit", @"C:\Users\Fernando\Desktop\ejemplo.tly" };
                //args = new string[] { "--install",/* "--name=Laster",*/ @"C:\Users\Fernando\Desktop\ejemplo.tly" };
                //args = new string[] { "--service", @"C:\Users\Fernando\Desktop\ejemplo.tly" };
            }
#endif
            bool isService = false, isEdit = false;
            List<string> cfgFiles = new List<string>();

            if (args != null && args.Length > 0)
            {
                string name = "LasterService";
                int install = 0;
                foreach (string arg in args)
                {
                    if (string.IsNullOrEmpty(arg)) continue;

                    if (!arg.StartsWith("--") && File.Exists(arg))
                        cfgFiles.Add(arg);
                    else
                    {
                        string iz, dr;
                        StringHelper.Split(arg, '=', out iz, out dr);

                        switch (iz)
                        {
                            case "--name": { name = dr.Trim(); break; }

                            case "--service": { isService = true; break; }
                            case "--edit": { isEdit = true; break; }

                            case "--install": { install = 1; break; }
                            case "--uninstall": { install = -1; break; }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }

                if (install != 0)
                {
                    // Requiere acción de instalación
                    if (install == 1) LasterServiceInstaller.Install(name, cfgFiles.ToArray());
                    else LasterServiceInstaller.Uninstall(name);
                    return;
                }
            }

            bool efects = false;
            Application.OleRequired();
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
                                if (isEdit)
                                {
                                    // Edición
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

                                        cfgFiles.Add(json);
                                    }
                                }
                                else
                                {
                                    // Ejecución

                                    TLYFile file = TLYFile.Load(json);
                                    if (file != null)
                                        file.Compile(inputs, Application.ExecutablePath);
                                }
                            }
                        }
                    }
                }
            }

            // Ver si quiere editar o ejecutar una configuración
            if (!isEdit && (inputs == null || inputs.Count == 0))
                foreach (string s in cfgFiles)
                {
                    TLYFile file = TLYFile.LoadFromFile(s);
                    if (file != null) file.Compile(inputs, s);
                }

            // Ver si se ejecuta como servicio
            if (isService)
            {
                ITopologyItem.OnException += ITopologyItem_OnException;
                ServiceBase[] ServicesToRun = new ServiceBase[] { new LasterService(inputs) };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                // Ver si se ejecuta o se muestra la edición
                if (inputs != null && inputs.Count > 0)
                {
                    ITopologyItem.OnException += ITopologyItem_OnException;
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

                    // Editar en caso de que haya alguna configuración esa, sino una nueva
                    Application.Run(new FEditTopology(cfgFiles.Count == 0 ? null : cfgFiles[0]));
                }
            }
        }
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (e == null) return;
            ITopologyItem_OnException(null, e.Exception);
        }
        static void ITopologyItem_OnException(ITopologyItem sender, Exception e)
        {
            if (e == null) return;

            if (Environment.UserInteractive)
            {
                MessageBox.Show(e.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                File.WriteAllLines(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "error.log"), new string[] { e.ToString() });
            }
        }
    }
}