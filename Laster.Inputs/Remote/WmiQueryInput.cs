using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Dynamic;
using System.Management;

namespace Laster.Inputs.Local
{
    /// <summary>
    /// Captura eventos del sistema
    /// </summary>
    public class WmiQueryInput : IDataInput
    {
        public override string Title { get { return "Remote - WMI Query"; } }

        /// <summary>
        /// SELECT Name, ProcessID FROM Win32_Process WHERE Name LIKE 'chrome.exe'
        /// </summary>
        [Category("Filter")]
        [DefaultValue("")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Query { get; set; }

        [Category("Authentication")]
        [DefaultValue(@"\\.\root\CIMV2")]
        public string Scope { get; set; }
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
        public WmiQueryInput() : base()
        {
            DesignBackColor = Color.DeepPink;
            Impersonation = ImpersonationLevel.Default;
            EnablePrivileges = true;
            Scope = @"\\.\root\CIMV2";
        }

        protected override IData OnGetData()
        {
            List<dynamic> ls = new List<dynamic>();

            ManagementScope scope = new ManagementScope(Scope)
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

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(Scope, Query))
            using (ManagementObjectCollection retObjectCollection = searcher.Get())
                foreach (ManagementObject retObject in retObjectCollection)
                    using (retObject)
                    {
                        dynamic obj = new ExpandoObject();
                        foreach (PropertyData p in retObject.Properties)
                            ((IDictionary<string, object>)obj)[p.Name] = retObject[p.Name];

                        ls.Add(obj);
                    }

            return Reduce(EZeroEntries.Break, ls);
        }
    }
}