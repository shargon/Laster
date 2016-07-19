using Laster.Core.Interfaces;
using System.Windows.Forms;

namespace Laster.Process.Forms
{
    public partial class FEditObjectProperties : FRememberForm
    {
        /// <summary>
        /// Selecciona un elemento de la lista
        /// </summary>
        /// <param name="title">Título</param>
        /// <param name="array">Lista</param>
        public static bool ShowForm(string title, object obj)
        {
            using (FEditObjectProperties f = new FEditObjectProperties(obj))
            {
                f.Text = title;
                return f.ShowDialog() == DialogResult.OK;
            }
        }
        FEditObjectProperties(object obj)
        {
            InitializeComponent();

            propertyGrid1.SelectedObject = obj;
        }
    }
}