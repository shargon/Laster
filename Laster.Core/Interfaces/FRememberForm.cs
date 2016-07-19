using Laster.Core.Helpers;
using Laster.Core.Remembers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Laster.Core.Interfaces
{
    public class FRememberForm : Form
    {
        static Dictionary<string, RememberForm> _Rems;

        bool _CloseOnEscape = true;
        /// <summary>
        /// Close on Escape
        /// </summary>
        public bool CloseOnEscape { get { return _CloseOnEscape; }set { _CloseOnEscape = value; } }

        public FRememberForm()
        {
            KeyPreview = true;
        }

        static FRememberForm()
        {
            string file = Path.ChangeExtension(Application.ExecutablePath, ".rem");

            if (File.Exists(file))
                try
                {
                    string json = File.ReadAllText(file, Encoding.UTF8);
                    if (!string.IsNullOrEmpty(json))
                    {
                        _Rems = SerializationHelper.DeserializeFromJson<Dictionary<string, RememberForm>>(json);
                    }
                }
                catch { }

            if (_Rems == null) _Rems = new Dictionary<string, RememberForm>();
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            RememberForm r;
            if (_Rems != null && _Rems.TryGetValue(GetType().Name, out r) && r != null)
            {
                r.Apply(this);
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            RememberForm r = new RememberForm(this);
            _Rems[GetType().Name] = r;

            string file = Path.ChangeExtension(Application.ExecutablePath, ".rem");
            try
            {
                File.WriteAllText(file, SerializationHelper.SerializeToJson(_Rems), Encoding.UTF8);
            }
            catch { }

            base.OnClosed(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (CloseOnEscape && e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                Close();
                return;
            }

            base.OnKeyDown(e);
        }
    }
}