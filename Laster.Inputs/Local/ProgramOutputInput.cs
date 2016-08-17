using Laster.Core.Interfaces;
using System.ComponentModel;
using System.Drawing;
using System.Security;
using Pr = System.Diagnostics;

namespace Laster.Inputs.Local
{
    /// <summary>
    /// Captura eventos del sistema
    /// </summary>
    public class ProgramOutputInput : IDataInput
    {
        /// <summary>
        /// Archivo
        /// </summary>
        [DefaultValue("")]
        [Category("Run")]
        public string File { get; set; }
        /// <summary>
        /// Argumentos
        /// </summary>
        [DefaultValue("")]
        [Category("Run")]
        public string Arguments { get; set; }

        /// <summary>
        /// Dominio
        /// </summary>
        [DefaultValue("")]
        [Category("Credentials")]
        public string Domain { get; set; }
        /// <summary>
        /// Contraseña
        /// </summary>
        [DefaultValue("")]
        [Category("Credentials")]
        public string Password { get; set; }
        /// <summary>
        /// Usuario
        /// </summary>
        [DefaultValue("")]
        [Category("Credentials")]
        public string UserName { get; set; }

        public override string Title { get { return "Local - Program Output"; } }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public ProgramOutputInput() : base()
        {
            DesignBackColor = Color.Brown;
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