using Laster.Core.Helpers;
using Laster.Core.Remembers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Laster.Core.Interfaces
{
    public class FRememberForm<T> : Form where T : IRemember
    {
        static Dictionary<string, string> _Rems;

        bool _CloseOnEscape = true;
        /// <summary>
        /// Close on Escape
        /// </summary>
        public bool CloseOnEscape { get { return _CloseOnEscape; } set { _CloseOnEscape = value; } }

        public FRememberForm() { KeyPreview = true; }
        static FRememberForm()
        {
            if (_Rems != null) return;

            string file = Path.ChangeExtension(Application.ExecutablePath, ".rem");

            if (File.Exists(file))
                try
                {
                    string json = File.ReadAllText(file, Encoding.UTF8);
                    if (!string.IsNullOrEmpty(json))
                    {
                        _Rems = SerializationHelper.DeserializeFromJson<Dictionary<string, string>>(json);
                    }
                }
                catch { }

            if (_Rems == null) _Rems = new Dictionary<string, string>();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            string json;
            if (_Rems != null && _Rems.TryGetValue(GetType().Name, out json) && json != null)
            {
                T r = SerializationHelper.DeserializeFromJson<T>(json, false);
                if (r != null) OnGetValues(r);
            }
        }
        protected virtual void OnGetValues(T sender) { }
        protected virtual void OnSaveValues(T sender) { }
        protected override void OnClosed(EventArgs e)
        {
            T r = Activator.CreateInstance<T>();
            OnSaveValues(r);

            try
            {
                _Rems[GetType().Name] = SerializationHelper.SerializeToJson(r, false, false);

                string file = Path.ChangeExtension(Application.ExecutablePath, ".rem");
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