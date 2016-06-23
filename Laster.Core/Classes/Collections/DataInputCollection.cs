using Laster.Core.Interfaces;
using System;

namespace Laster.Core.Classes.Collections
{
    public class DataInputCollection : IDataCollection<IDataInput>, IDisposable
    {
        public DataInputCollection() : base() { }
        public DataInputCollection(params IDataInput[] values) : base(values) { }

        /// <summary>
        /// Inicia el procesado
        /// </summary>
        public void Start()
        {
            if (Count == 0) return;

            foreach (IDataInput input in this)
            {
                if (input.RaiseMode == null) continue;

                input.OnCreate();
                input.RaiseMode.Start(input);
            }
        }
        /// <summary>
        /// Para todos el procesado
        /// </summary>
        public void Stop()
        {
            foreach (IDataInput input in this)
            {
                if (input.RaiseMode == null) continue;
                input.RaiseMode.Stop(input);
            }
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