using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

            bool error = false;
            bool somethingStarted = false;

            Parallel.ForEach(this, new ParallelOptions() { }, input =>
            {
                if (input.RaiseMode == null) return;

                try
                {
                    input.Start();
                }
                catch (Exception e)
                {
                    error = true;
                    input.OnError(e);
                    return;
                }

                try
                {
                    input.RaiseMode.Start(input);
                    if (input.RaiseMode.IsStarted)
                        somethingStarted = true;
                }
                catch (Exception e)
                {
                    error = true;
                    input.OnError(e);
                    return;
                }
            });

            if (OnStart != null) OnStart(this, EventArgs.Empty);

            if (!somethingStarted || error)
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
            Parallel.ForEach<IDataInput>(this, new ParallelOptions() { }, input =>
            {
                if (input.RaiseMode != null)
                    try
                    {
                        input.RaiseMode.Stop(input);
                    }
                    catch (Exception e) { input.OnError(e); }

                try
                {
                    input.Stop();
                }
                catch (Exception e) { input.OnError(e); }
            });

            if (OnStop != null) OnStop(this, EventArgs.Empty);
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public void Dispose() { Stop(); }
    }
}