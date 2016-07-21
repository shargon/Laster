using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System.Drawing;

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
            DesignBackColor = Color.DarkViolet;
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            object obj = data.GetInternalObject();

            if (obj == null) return DataEmpty();
            return DataObject(SerializationHelper.Serialize(obj, Format));
        }
    }
}