using System.Drawing;
using System.Windows.Forms;

namespace Laster.Core.Remembers
{
    public class RememberForm
    {
        public Point Location { get; set; }
        public Size Size { get; set; }
        public FormWindowState State { get; set; }

        public RememberForm() { }
        public RememberForm(Form f)
        {
            State = f.WindowState;
            Location = f.Location;
            Size = f.Size;
        }
        public virtual void Apply(Form f)
        {
            if (Size != Size.Empty && Size.Width > 0 && Size.Height > 0) f.Size = Size;

            f.Location = Location;
            f.WindowState = State;
        }
    }
}