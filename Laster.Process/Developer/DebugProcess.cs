using Laster.Core.Designer;
using Laster.Core.Enums;
using Laster.Core.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Laster.Process.Developer
{
    /// <summary>
    /// Debug
    /// </summary>
    public class DebugProcess : IDataProcess
    {
        /// <summary>
        /// Formato
        /// </summary>
        [JsonIgnore]
        [ReadOnly(true)]
        [EditorAttribute(typeof(ObjectEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object Value { get; private set; }

        public bool BreakPoint { get; set; }

        public override string Title { get { return "Developer - Debug"; } }

        public DebugProcess()
        {
            Value = null;
            DesignBackColor = Color.Red;
            BreakPoint = true;
        }

        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (data == null) Value = null;
            else Value = data.GetInternalObject();

            if (BreakPoint)
                ObjectEditor.ShowValue(Value);

            return data;
        }
    }
}