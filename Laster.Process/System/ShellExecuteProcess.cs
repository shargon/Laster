using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.ComponentModel;
using System.Drawing;
using System.Security;
using Pr = System.Diagnostics;

namespace Laster.Process.System
{
    public class ShellExecuteProcess : IDataProcess
    {
        public enum EFileSource : byte
        {
            FileName = 0,
            Input = 1,
        }

        /// <summary>
        /// Archivo
        /// </summary>
        [Category("Run")]
        public string FileName { get; set; }
        /// <summary>
        /// Argumentos
        /// </summary>
        [Category("Run")]
        public string Arguments { get; set; }

        /// <summary>
        /// Crea ventana
        /// </summary>
        [Category("Style")]
        public bool CreateNoWindow { get; set; }
        /// <summary>
        /// Estilo de ventana
        /// </summary>
        [Category("Style")]
        public Pr.ProcessWindowStyle WindowStyle { get; set; }
        /// <summary>
        /// Dominio
        /// </summary>
        [Category("Credentials")]
        public string Domain { get; set; }
        /// <summary>
        /// Contraseña
        /// </summary>
        [Category("Credentials")]
        public string Password { get; set; }
        /// <summary>
        /// Usuario
        /// </summary>
        [Category("Credentials")]
        public string UserName { get; set; }

        [Category("Source")]
        public EFileSource FileNameSource { get; set; }

        public override string Title { get { return "System - Shell execute"; } }

        public ShellExecuteProcess()
        {
            FileNameSource = EFileSource.FileName;
            DesignBackColor = Color.DeepPink;
        }

        /// <summary>
        /// Saca el contenido de los datos a un archivo
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            string file = FileName;

            if(FileNameSource== EFileSource.Input)
            {
                foreach(object o in data)
                {
                    if (o is string)
                    {
                        file = o.ToString();
                        break;
                    }
                }
            }

            Pr.Process pr = new Pr.Process();
            pr.StartInfo = new Pr.ProcessStartInfo(file, Arguments)
            {
                CreateNoWindow = CreateNoWindow,
                Domain = Domain,
                Password = string.IsNullOrEmpty(Password) ? null : ToSecure(Password),
                UserName = UserName,
                UseShellExecute = true,
                WindowStyle = WindowStyle,
            };

            pr.Start();
            return data;
        }
        SecureString ToSecure(string password)
        {
            SecureString sc = new SecureString();
            foreach (char c in password.ToCharArray()) sc.AppendChar(c);
            return sc;
        }
    }
}