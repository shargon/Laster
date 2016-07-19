using Laster.Core.Enums;
using Laster.Core.Interfaces;
using Laster.Process.Forms;

namespace Laster.Process
{
    /// <summary>
    /// Edita un objeto
    /// </summary>
    public class EditObjectPropertiesProcess : IDataProcess
    {
        public string FormTitle { get; set; }

        public override string Title { get { return "Edit Object [Property Grid]"; } }

        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            object o = data.GetInternalObject();
            if (o == null) return null;

            if (!FEditObjectProperties.ShowForm(FormTitle, o))
                return null;

            return data;
        }
    }
}