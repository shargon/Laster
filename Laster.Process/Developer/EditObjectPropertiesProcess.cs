using Laster.Core.Enums;
using Laster.Core.Forms;
using Laster.Core.Interfaces;
using System.Drawing;

namespace Laster.Process.Developer
{
    /// <summary>
    /// Edita un objeto
    /// </summary>
    public class EditObjectPropertiesProcess : IDataProcess
    {
        public string FormTitle { get; set; }

        public override string Title { get { return "Developer - Edit Object"; } }

        public EditObjectPropertiesProcess()
        {
            DesignBackColor = Color.Red;
        }

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