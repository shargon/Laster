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
        [Category("Cache")]
        public TimeSpan ExpireIn { get; set; }
        /// <summary>
        /// Permitir mas de un elemento en la caché
        /// </summary>
        [Category("Cache")]
        [DefaultValue(true)]
        public bool AllowMultipleCache { get; set; }
        /// <summary>
        /// Formato
        /// </summary>
        [Category("Check Method")]
        [DefaultValue(SerializationHelper.EFormat.Json)]
        public SerializationHelper.EFormat Format { get; set; }
        /// <summary>
        /// Pasar a minúsculas el contenido del serializado
        /// </summary>
        [Category("Check Method")]
        [DefaultValue(false)]
        public bool IgnoreCase { get; set; }

        public override string Title { get { return "Filters - Dont repeat"; } }

        public DontRepeatProcess()
        {
            IgnoreCase = false;
            DesignBackColor = Color.Gold;
            AllowMultipleCache = true;
            Format = SerializationHelper.EFormat.Json;
            ExpireIn = TimeSpan.FromMinutes(1);
        }

        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (data == null) return DataBreak();

            DateTime date, utc = DateTime.UtcNow;

            List<object> lo = new List<object>();
            foreach (object o in data)
            {
                string ser = SerializationHelper.Serialize(o, Format).Trim();
                if (IgnoreCase) ser = ser.ToLowerInvariant();

                lock (Cache)
                {
                    if (Cache.TryGetValue(ser, out date) && date > utc) continue;

                    if (!AllowMultipleCache) Cache.Clear();
                    Cache[ser] = utc.Add(ExpireIn);
                }

                lo.Add(o);
            }

            return Reduce(EReduceZeroEntries.Break, lo);
        }
        protected override void OnStart() { Cache.Clear(); }
        protected override void OnStop() { Cache.Clear(); }
    }
}