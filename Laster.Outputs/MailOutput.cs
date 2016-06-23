using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System.Text;

namespace Laster.Outputs
{
    public class MailOutput : IDataOutput
    {
        /// <summary>
        /// Formato
        /// </summary>
        public SerializationHelper.EFormat Format { get; set; }
        /// <summary>
        /// Codificación
        /// </summary>
        public SerializationHelper.EEncoding Encoding { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MailOutput()
        {
            Format = SerializationHelper.EFormat.Json;
            Encoding = SerializationHelper.EEncoding.UTF8;
        }

        protected override void OnProcessData(IData data, EEnumerableDataState state)
        {
            // Formato del archivo
            byte[] cad = SerializationHelper.Serialize(data.GetInternalObject(), Encoding, Format);

            base.OnProcessData(data, state);
        }
    }
}