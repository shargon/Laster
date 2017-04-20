using System.Dynamic;
using System.Windows.Forms;

namespace Laster.Core.Forms
{
    public partial class FEditObjectProperties : FOkCancel
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
        FEditObjectProperties() : base()
        {
            InitializeComponent();
        }
        FEditObjectProperties(object obj) : this()
        {
            if (obj != null && obj is string)
            {
                dynamic d = new ExpandoObject();
                d.String = (string)obj;
                obj = d;
            }
            propertyGrid1.SelectedObject = obj;
        }
    }
}