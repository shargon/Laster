using Laster.Core.Helpers;
using Laster.Core.Remembers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Laster.Core.Interfaces
{
    public class FRememberForm : FRememberForm<RememberForm>
    {
        internal static Dictionary<string, string> Remembers;

        static FRememberForm()
        {
            if (Remembers != null) return;

            string file = Path.ChangeExtension(Application.ExecutablePath, ".rem");

            if (File.Exists(file))
                try
                {
                    string json = File.ReadAllText(file, Encoding.UTF8);
                    if (!string.IsNullOrEmpty(json))
                    {
                        Remembers = SerializationHelper.DeserializeFromJson<Dictionary<string, string>>(json);
                    }
                }
                catch { }

            if (Remembers == null) Remembers = new Dictionary<string, string>();
        }
        public FRememberForm() : base() { }

        protected override void OnGetValues(RememberForm sender) { sender.GetValues(this); }
        protected override void OnSaveValues(RememberForm sender) { sender.SaveValues(this); }
    }
}