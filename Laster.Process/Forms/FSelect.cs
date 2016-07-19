using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Laster.Process.Forms
{
    public partial class FSelect<T> : FRememberForm
    {
        /// <summary>
        /// Selecciona un elemento de la lista
        /// </summary>
        /// <param name="title">Título</param>
        /// <param name="array">Lista</param>
        public static T SelectOne(string title, params T[] array)
        {
            using (FSelect<T> f = new Forms.FSelect<T>(array))
            {
                f.Text = title;
                f.listBox1.SelectionMode = SelectionMode.One;

                if (f.ShowDialog() == DialogResult.OK)
                    return (T)f.listBox1.SelectedItem;
            }

            return default(T);
        }

        /// <summary>
        /// Selecciona un elemento de la lista
        /// </summary>
        /// <param name="title">Título</param>
        /// <param name="array">Lista</param>
        public static T[] SelectMultiple(string title, params T[] array)
        {
            using (FSelect<T> f = new Forms.FSelect<T>(array))
            {
                f.Text = title;
                f.listBox1.SelectionMode = SelectionMode.MultiExtended;

                if (f.ShowDialog() == DialogResult.OK)
                {
                    List<T> l = new List<T>();
                    foreach (T o in f.listBox1.SelectedItems) l.Add(o);
                    return l.ToArray();
                }
            }

            return default(T[]);
        }
        FSelect(params T[] array)
        {
            InitializeComponent();

            foreach (T o in array)
                listBox1.Items.Add(o);
        }
        void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem != null && listBox1.SelectionMode == SelectionMode.One)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}