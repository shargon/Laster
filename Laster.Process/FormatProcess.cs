using Laster.Core.Data;
using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;

namespace Laster.Process
{
    /// <summary>
    /// Formatea unos datos
    /// </summary>
    public class FormatProcess : IDataProcess
    {
        /// <summary>
        /// Formato
        /// </summary>
        public SerializationHelper.EFormat Format { get; set; }

        public override string Title { get { return "Format"; } }

        public FormatProcess()
        {
            Format = SerializationHelper.EFormat.Json;
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            object obj = data.GetInternalObject();

            if (obj == null) return new DataEmpty(this);
            return new DataObject(this, SerializationHelper.Serialize(obj, Format));
        }
    }
}