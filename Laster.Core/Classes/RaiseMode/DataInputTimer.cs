using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Timers;

namespace Laster.Core.Classes.RaiseMode
{
    public class DataInputTimer : IRaiseMode
    {
        cTimer _Timers = null;
        TimeSpan _Interval = new TimeSpan(0, 0, 10);

        /// <summary>
        /// Intervalo de actualización de la fuente de información
        /// </summary>
        public TimeSpan Interval
        {
            get { return _Interval; }
            set
            {
                _Interval = value;
                if (_Timers != null)
                    _Timers.Interval = IntervalInMilliseconds;
            }
        }
        /// <summary>
        /// Intervalo para el Timer, sin ser posible ser 0
        /// </summary>
        [Browsable(false)]
        public double IntervalInMilliseconds { get { return _Interval.TotalMilliseconds <= 0 ? 1 : _Interval.TotalMilliseconds; } }

        class cTimer : System.Timers.Timer
        {
            /// <summary>
            /// Padrecc
            /// </summary>
            public IDataInput Parent { get; set; }
        }

        public override void Start(IDataInput input)
        {
            if (_Timers != null) return;

            // Creación del timer
            _Timers = new cTimer();
            _Timers.Parent = input;
            _Timers.Interval = IntervalInMilliseconds;
            _Timers.Elapsed += Timer_Elapsed;

            _Timers.Start();

            base.Start(input);
        }
        public override void Stop(IDataInput input)
        {
            if (_Timers != null)
            {
                _Timers.Stop();
                _Timers.Dispose();
                _Timers = null;
            }

            base.Stop(input);
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
        public override string ToString() { return "Timer"; }
    }
}