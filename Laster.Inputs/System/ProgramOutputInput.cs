using Laster.Core.Interfaces;
using System.ComponentModel;
using System.Drawing;
using System.Security;
using Pr = System.Diagnostics;

namespace Laster.Inputs.System
{
    /// <summary>
    /// Captura eventos del sistema
    /// </summary>
    public class ProgramOutputInput : IDataInput
    {
        /// <summary>
        /// Archivo
        /// </summary>
        [Category("Run")]
        public string File { get; set; }
        /// <summary>
        /// Argumentos
        /// </summary>
        [Category("Run")]
        public string Arguments { get; set; }

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

        public override string Title { get { return "System - Program Output"; } }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public ProgramOutputInput() : base()
        {
            DesignBackColor = Color.DeepPink;
        }

        protected override IData OnGetData()
        {
            Pr.Process pr = new Pr.Process();
            pr.StartInfo = new Pr.ProcessStartInfo(File, Arguments)
            {
                CreateNoWindow = true,
                Domain = Domain,
                Password = string.IsNullOrEmpty(Password) ? null : ToSecure(Password),
                UserName = UserName,
                UseShellExecute = false,
                WindowStyle = Pr.ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            pr.Start();
            pr.WaitForExit();

            string o = pr.StandardOutput.ReadToEnd();
            string e = pr.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(e))
                o += e;

            return DataObject(o);
        }

        SecureString ToSecure(string password)
        {
            SecureString sc = new SecureString();
            foreach (char c in password.ToCharArray()) sc.AppendChar(c);
            return sc;
        }
    }
}