using Laster.Core.Interfaces;
using System;

namespace Laster.Core.Classes.Collections
{
    public class DataInputCollection : IDataCollection<IDataInput>, IDisposable
    {
        public event EventHandler OnStart;
        public event EventHandler OnStop;

        public DataInputCollection() : base() { }
        public DataInputCollection(params IDataInput[] values) : base(values) { }

        /// <summary>
        /// Inicia el procesado
        /// </summary>
        public bool Start()
        {
            if (Count == 0) return false;

            bool somethingStarted = false;
            foreach (IDataInput input in this)
            {
                if (input.RaiseMode == null) continue;

                input.OnStart();
                input.RaiseMode.Start(input);

                if (input.RaiseMode.IsStarted)
                {
                    somethingStarted = true;
                }
            }

            if (OnStart != null) OnStart(this, EventArgs.Empty);

            if (!somethingStarted)
            {
                Stop();
                return false;
            }
            return true;
        }
        /// <summary>
        /// Para todos el procesado
        /// </summary>
        public void Stop()
        {
            foreach (IDataInput input in this)
            {
                if (input.RaiseMode == null) continue;

                input.OnStop();
                input.RaiseMode.Stop(input);
            }

            if (OnStop != null) OnStop(this, EventArgs.Empty);
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