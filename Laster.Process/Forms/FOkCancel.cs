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
                control.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                control.Left = 10;
                control.Top = 10;
                control.Width = f.ClientRectangle.Width - 20;
                control.Height = f.ClientRectangle.Height - 80;

                return f.ShowDialog() == DialogResult.OK;
            }
        }
        FOkCancel()
        {
            InitializeComponent();
        }
    }
}