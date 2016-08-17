using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System.ComponentModel;
using System.Drawing;

namespace Laster.Process.Strings
{
    /// <summary>
    /// Formatea unos datos
    /// </summary>
    public class SerializeProcess : IDataProcess
    {
        /// <summary>
        /// Formato
        /// </summary>
        [DefaultValue(SerializationHelper.EFormat.Json)]
        public SerializationHelper.EFormat Format { get; set; }

        public override string Title { get { return "Strings - Serialize"; } }

        public SerializeProcess()
        {
            Format = SerializationHelper.EFormat.Json;
            DesignBackColor = Color.Blue;
            //DesignBackColor = Color.DarkViolet;
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            object obj = data.GetInternalObject();

            if (obj == null) return DataEmpty();
            return DataObject(SerializationHelper.Serialize(obj, Format));
        }
    }
}