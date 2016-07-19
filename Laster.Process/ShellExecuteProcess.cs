using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.ComponentModel;
using System.Security;
using Pr = System.Diagnostics;

namespace Laster.Process
{
    public class ShellExecuteProcess : IDataProcess
    {
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

        public override string Title { get { return "Shell execute"; } }

        /// <summary>
        /// Saca el contenido de los datos a un archivo
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            Pr.Process pr = new Pr.Process();
            pr.StartInfo = new Pr.ProcessStartInfo(FileName, Arguments)
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