using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Laster.Process.Filters
{
    /// <summary>
    /// No permite que se repitan los datos
    /// </summary>
    public class DontRepeatProcess : IDataProcess
    {
        Dictionary<string, DateTime> Cache = new Dictionary<string, DateTime>();

        /// <summary>
        /// Tiempo de expiración de la caché
        /// </summary>
        public TimeSpan ExpireIn { get; set; }
        /// <summary>
        /// Permitir mas de un elemento en la caché
        /// </summary>
        [DefaultValue(true)]
        public bool AllowMultipleCache { get; set; }
        /// <summary>
        /// Pasar a minúsculas el contenido del serializado
        /// </summary>
        [DefaultValue(false)]
        public bool IgnoreCase { get; set; }

        public override string Title { get { return "Filters - Dont repeat"; } }

        public DontRepeatProcess()
        {
            IgnoreCase = false;
            DesignBackColor = Color.Green;
            AllowMultipleCache = true;
            ExpireIn = TimeSpan.FromMinutes(1);
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (data == null) return DataBreak();
            object obj = data.GetInternalObject();
            if (obj == null) return DataBreak();

            string ser = SerializationHelper.Serialize(obj, SerializationHelper.EFormat.Json).Trim();
            if (IgnoreCase) ser = ser.ToLowerInvariant();

            DateTime date;
            DateTime utc = DateTime.UtcNow;
            if (Cache.TryGetValue(ser, out date))
            {
                if (date > utc)
                    return DataBreak();
            }

            if (!AllowMultipleCache) Cache.Clear();
            Cache[ser] = utc.Add(ExpireIn);

            return data;
        }
        public override void OnStart()
        {
            Cache.Clear();
            base.OnStart();
        }
        public override void OnStop()
        {
            Cache.Clear();
            base.OnStop();
        }
    }
}