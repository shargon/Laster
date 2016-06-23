using Laster.Core.Classes.RaiseMode;
using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Laster.Core.Classes.Collections
{
    public class DataInputCollection : IDataCollection<IDataInput>, IDisposable
    {
        class cTimer : System.Timers.Timer
        {
            /// <summary>
            /// Padrecc
            /// </summary>
            public IDataInput Parent { get; set; }
        }

        cTimer[] _Timers = null;

        public DataInputCollection() : base() { }
        public DataInputCollection(params IDataInput[] values) : base(values) { }

        /// <summary>
        /// Inicia el procesado
        /// </summary>
        public void Start()
        {
            if (Count == 0) return;

            List<cTimer> timers = new List<cTimer>();

            foreach (IDataInput input in this)
            {
                input.OnCreate();

                if (input.RaiseMode is DataInputInterval)
                {
                    DataInputInterval interval = (DataInputInterval)input.RaiseMode;

                    // Creación del timer
                    cTimer timer = new cTimer();
                    timer.Parent = input;
                    timer.Interval = interval.IntervalInMilliseconds;
                    timer.Elapsed += Timer_Elapsed;

                    timers.Add(timer);
                }
                else
                {
                    if (input.RaiseMode is DataInputTrigger)
                    {
                        DataInputTrigger trigger = (DataInputTrigger)input.RaiseMode;
                        trigger.OnRaiseTrigger += Trigger_OnRaiseTrigger;
                    }
                }
            }

            _Timers = timers.ToArray();
            // Lanzar los timers
            if (_Timers != null)
            {
                foreach (cTimer t in _Timers)
                    t.Start();
            }
        }
        /// <summary>
        /// Para todos el procesado
        /// </summary>
        public void Stop()
        {
            // Vaciar timers
            if (_Timers != null)
            {
                foreach (cTimer t in _Timers)
                {
                    t.Stop();
                    t.Dispose();
                }
                _Timers = null;
            }
            // Eliminar el evento de lanzado
            foreach (IDataInput input in this)
            {
                if (input.RaiseMode is DataInputTrigger)
                {
                    DataInputTrigger trigger = (DataInputTrigger)input.RaiseMode;
                    trigger.OnRaiseTrigger -= Trigger_OnRaiseTrigger;
                }
            }
        }
        void Trigger_OnRaiseTrigger(object sender, EventArgs e)
        {
            DataInputTrigger origin = (DataInputTrigger)sender;

            if (origin.RequireCreateThread)
            {
                // Creamos el hilo
                Thread th = new Thread(new ParameterizedThreadStart(threadStart));
                th.IsBackground = true;
                th.Start(origin.Parent);
            }
            else
            {
                // Lo lanzamos directamente
                origin.Parent.ProcessData();
            }
        }
        static void threadStart(object sender)
        {
            IDataInput origin = ((cTimer)sender).Parent;
            origin.ProcessData();
        }
        /// <summary>
        /// Timer de input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            IDataInput origin = ((cTimer)sender).Parent;
            origin.ProcessData();
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
}