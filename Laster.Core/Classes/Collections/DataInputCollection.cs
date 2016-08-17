using Laster.Core.Interfaces;
using System;
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

            bool somethingStarted = false;

            Parallel.ForEach<IDataInput>(this, new ParallelOptions() { }, input =>
            {
                lock (input)
                {
                    if (input.RaiseMode == null) return;

                    try { input.OnStart(); }
                    catch (Exception e)
                    {
                        input.OnError(e);
                        return;
                    }
                    input.RaiseMode.Start(input);

                    if (input.RaiseMode.IsStarted)
                    {
                        somethingStarted = true;
                    }
                }
            });

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
            Parallel.ForEach<IDataInput>(this, new ParallelOptions() { }, input =>
            {
                lock (input)
                {
                    if (input.RaiseMode != null)
                        try { input.RaiseMode.Stop(input); }
                        catch (Exception e)
                        {
                            input.OnError(e);
                        }

                    try { input.OnStop(); }
                    catch (Exception e)
                    {
                        input.OnError(e);
                    }
                }
            });

            if (OnStop != null) OnStop(this, EventArgs.Empty);
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public void Dispose() { Stop(); }
    }
}