using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.Drawing;

namespace Laster.Process.Filters
{
    /// <summary>
    /// No permite que se repitan los datos
    /// </summary>
    public class DontRepeatProcess : IDataProcess
    {
        string Cache = null;

        public StringComparison StringComparison { get; set; }
        public override string Title { get { return "Filters - Dont repeat"; } }


        public DontRepeatProcess()
        {
            StringComparison = StringComparison.InvariantCulture;
            DesignBackColor = Color.Blue;
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            object obj = data.GetInternalObject();
            if (obj == null) return null;

            string ser = SerializationHelper.Serialize(obj, SerializationHelper.EFormat.Json);

            if (Cache == null)
                Cache = ser;
            else
            {
                if (Cache.Equals(ser, StringComparison)) return null;
                Cache = ser;
            }

            return data;
        }

        public override void OnStop()
        {
            Cache = null;
            base.OnStop();
        }
    }
}