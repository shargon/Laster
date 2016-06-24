using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Timers;

namespace Laster.Core.Classes.RaiseMode
{
    public class DataInputTimer : IDataInputRaiseMode
    {
        TimeSpan _Interval = TimeSpan.Zero;

        /// <summary>
        /// Intervalo de actualización de la fuente de información
        /// </summary>
        public TimeSpan Interval { get { return _Interval; } set { _Interval = value; } }
        /// <summary>
        /// Intervalo para el Timer, sin ser posible ser 0
        /// </summary>
        [Browsable(false)]
        public double IntervalInMilliseconds { get { return _Interval.TotalMilliseconds <= 0 ? 1 : _Interval.TotalMilliseconds; } }
     
        /// <summary>
        /// Constructor
        /// </summary>
        public DataInputTimer() { }

        class cTimer : System.Timers.Timer
        {
            /// <summary>
            /// Padrecc
            /// </summary>
            public IDataInput Parent { get; set; }
        }

        List<cTimer> _Timers = new List<cTimer>();
        public override void Start(IDataInput input)
        {
            // Creación del timer
            cTimer timer = new cTimer();
            timer.Parent = input;
            timer.Interval = IntervalInMilliseconds;
            timer.Elapsed += Timer_Elapsed;

            _Timers.Add(timer);

            timer.Start();
        }
        public override void Stop(IDataInput input)
        {
            if (_Timers != null)
            {
                foreach (cTimer t in _Timers)
                {
                    t.Stop();
                    t.Dispose();
                }
                _Timers.Clear();
            }
        }
        /// <summary>
        /// Timer de input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IDataInput origin = ((cTimer)sender).Parent;
            origin.ProcessData();
        }
        public override Image GetIcon() { return Res.timer; }
        public override string ToString()
        {
            return "Timer";
        }
    }
}