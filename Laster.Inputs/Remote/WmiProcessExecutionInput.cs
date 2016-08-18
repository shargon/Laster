using Laster.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Management;

namespace Laster.Inputs.Remote
{
    /// <summary>
    /// Ejecuta un proceso en remoto
    /// </summary>
    public class WmiProcessExecutionInput : IDataInput
    {
        public enum EView : ushort
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            SW_HIDE = 0,
            /// <summary>
            /// Activates and displays a window.If the window is minimized or maximized, the system restores it to the original size and position.An application specifies this flag when displaying the window for the first time.
            /// </summary>
            SW_NORMAL = 1,
            /// <summary>
            /// Activates the window, and displays it as a minimized window.
            /// </summary>
            SW_SHOWMINIMIZED = 2,
            /// <summary>
            /// Activates the window, and displays it as a maximized window.
            /// </summary>
            SW_SHOWMAXIMIZED = 3,
            /// <summary>
            /// Displays a window in its most recent size and position.This value is similar to SW_NORMAL, except that the window is not activated.
            /// </summary>
            SW_SHOWNOACTIVATE = 4,
            /// <summary>
            /// Activates the window, and displays it at the current size and position.
            /// </summary>
            SW_SHOW = 5,
            /// <summary>
            /// Minimizes the specified window, and activates the next top-level window in the Z order.
            /// </summary>
            SW_MINIMIZE = 6,
            /// <summary>
            /// Displays the window as a minimized window.This value is similar to SW_SHOWMINIMZED, except that the window is not activated.
            /// </summary>
            SW_SHOWMINNOACTIVE = 7,
            /// <summary>
            /// Displays the window at the current size and position.This value is similar to SW_SHOW, except that the window is not activated.
            /// </summary>
            SW_SHOWNA = 8,
            /// <summary>
            /// Activates and displays the window.If the window is minimized or maximized, the system restores it to the original size and position.An application specifies this flag when restoring a minimized window.
            /// </summary>
            SW_RESTORE = 9,
            /// <summary>
            /// Sets the show state based on the SW_* value that is specified in the STARTUPINFO structure passed to the CreateProcess function by the program that starts the application.
            /// </summary>
            SW_SHOWDEFAULT = 10,
            /// <summary>
            /// Minimizes a window, even when the thread that owns the window stops responding. Only use this flag when minimizing windows from a different thread
            /// </summary>
            SW_FORCEMINIMIZE = 11,
        }

        public override string Title { get { return "Remote - WMI Process execution"; } }

        [DefaultValue(EView.SW_HIDE)]
        public EView ShowWindowMode { get; set; }
        [DefaultValue(".")]
        public string Host { get; set; }

        [DefaultValue("")]
        public string CommandLine { get; set; }
        [DefaultValue("")]
        public string CurrentDirectory { get; set; }
        public TimeSpan Timeout { get; set; }

        [Category("Authentication")]
        [DefaultValue("")]
        public string Password { get; set; }
        [Category("Authentication")]
        [DefaultValue("")]
        public string Username { get; set; }
        /// <summary>
        /// ntlmdomain:NAMEOFDOMAIN
        /// </summary>
        [Category("Authentication")]
        [DefaultValue("")]
        public string Authority { get; set; }
        [Category("Authentication")]
        [DefaultValue(true)]
        public bool EnablePrivileges { get; set; }
        [Category("Authentication")]
        [DefaultValue(ImpersonationLevel.Default)]
        public ImpersonationLevel Impersonation { get; set; }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public WmiProcessExecutionInput() : base()
        {
            DesignBackColor = Color.DeepPink;
            Impersonation = ImpersonationLevel.Default;
            EnablePrivileges = true;
            Host = ".";
            ShowWindowMode = EView.SW_HIDE;
            Timeout = TimeSpan.FromSeconds(30);
        }

        protected override IData OnGetData()
        {
            ManagementScope scope = new ManagementScope("\\\\" + Host + "\\root\\cimv2")
            {
                Options = new ConnectionOptions()
                {
                    EnablePrivileges = EnablePrivileges,
                    Impersonation = Impersonation,
                }
            };

            if (!string.IsNullOrEmpty(Username)) scope.Options.Username = Username;
            if (!string.IsNullOrEmpty(Password)) scope.Options.Password = Password;
            if (!string.IsNullOrEmpty(Authority)) scope.Options.Authority = Authority;

            using (ManagementClass Win32_ProcessStartup = new ManagementClass(scope, new ManagementPath("Win32_ProcessStartup"), new ObjectGetOptions()))
            using (ManagementClass Win32_Process = new ManagementClass(scope, new ManagementPath("Win32_Process"), new ObjectGetOptions()))
            {
                using (ManagementBaseObject startInfo = Win32_ProcessStartup.CreateInstance())
                {
                    startInfo["ShowWindow"] = (ushort)ShowWindowMode;

                    using (ManagementBaseObject inParams = Win32_Process.GetMethodParameters("Create"))
                    {
                        inParams["CommandLine"] = CommandLine;
                        if (!string.IsNullOrEmpty(CurrentDirectory))
                            inParams["CurrentDirectory"] = CurrentDirectory;
                        inParams["ProcessStartupInformation"] = startInfo;

                        using (ManagementBaseObject outParams = Win32_Process.InvokeMethod("Create", inParams, new InvokeMethodOptions() { Timeout = Timeout }))
                        {
                            uint rtn = Convert.ToUInt32(outParams["returnValue"]);
                            uint processID = Convert.ToUInt32(outParams["processId"]);
                        }
                    }
                }

            }
            return DataEmpty();
        }
    }
}