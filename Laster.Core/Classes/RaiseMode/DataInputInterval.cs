using Laster.Core.Interfaces;
using System;

namespace Laster.Core.Classes.RaiseMode
{
    public class DataInputInterval: IDataInputRaiseMode
    {
        TimeSpan _Interval = TimeSpan.Zero;

        /// <summary>
        /// Intervalo de actualización de la fuente de información
        /// </summary>
        public TimeSpan Interval { get { return _Interval; } set { _Interval = value; } }
        /// <summary>
        /// Intervalo para el Timer, sin ser posible ser 0
        /// </summary>
        public double IntervalForTimer { get { return _Interval.TotalMilliseconds <= 0 ? 1 : _Interval.TotalMilliseconds; } }
        /// <summary>
        /// Constructor
        /// </summary>
        public DataInputInterval() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interval">Intervalo de actualización de la fuente de información</param>
        public DataInputInterval(TimeSpan interval) { _Interval = interval; }
    }
}