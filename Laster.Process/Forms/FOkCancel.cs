using Laster.Core.Interfaces;
using System.Windows.Forms;

namespace Laster.Process.Forms
{
    public partial class FOkCancel : FRememberForm
    {
        /// <summary>
        /// Selecciona un elemento de la lista
        /// </summary>
        /// <param name="title">Título</param>
        /// <param name="array">Lista</param>
        public static bool ShowForm(string title, Control control)
        {
            using (FOkCancel f = new FOkCancel())
            {
                f.Text = title;
                f.Controls.Add(control);

                control.Dock = DockStyle.Fill;
                control.BringToFront();

                return f.ShowDialog() == DialogResult.OK;
            }
        }
        protected FOkCancel()
        {
            InitializeComponent();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F3:
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        DialogResult = DialogResult.OK;
                        return;
                    }
                case Keys.F4:
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        DialogResult = DialogResult.OK;
                        return;
                    }
            }

            base.OnKeyDown(e);
        }
    }
}