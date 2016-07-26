using System.Windows.Forms;

namespace Laster.Core.Remembers
{
    public interface IRemember
    {
        void SaveValues(Form f);
        void GetValues(Form f);
    }
}