using Laster.Core.Remembers;

namespace Laster.Core.Interfaces
{
    public class FRememberForm : FRememberForm<RememberForm>
    {
        public FRememberForm() : base() { }

        protected override void OnGetValues(RememberForm sender) { sender.GetValues(this); }
        protected override void OnSaveValues(RememberForm sender) { sender.SaveValues(this); }
    }
}