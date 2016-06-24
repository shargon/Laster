using Laster.Core.Classes.Collections;
using System.Windows.Forms;

namespace Laster.Core.Designer
{
    public partial class FVariables : Form
    {
        DataVariableCollection.DesignClassLink _Variables;

        public FVariables(DataVariableCollection.DesignClassLink v)
        {
            InitializeComponent();
            _Variables = v;
        }
        void FVariables_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                Close();
            }
        }
    }
}