using Laster.Core.Interfaces;
using System;
using System.Threading;
using System.Drawing;

namespace Laster.Core.Classes.RaiseMode
{
    public class DataInputTrigger : IDataInputRaiseMode
    {
        IDataInput _Parent;
        bool _RequireCreateThread;

        /// <summary>
        /// Padre
        /// </summary>
        public IDataInput Parent { get { return _Parent; } }
        /// <summary>
        /// Se lanza cuando se ejecuta el trigger
        /// </summary>
        public event EventHandler OnRaiseTrigger = null;
        /// <summary>
        /// Devuelve si existe algun evento asociado
        /// </summary>
        public bool HasEvent { get { return OnRaiseTrigger != null; } }
        /// <summary>
        /// Necesita crear un hilo para ser lanzado si o no
        /// </summary>
        public bool RequireCreateThread { get { return _RequireCreateThread; } }
        /// <summary>
        /// Lanza el evento
        /// </summary>
        /// <param name="e">Argumentos</param>
        public void RaiseTrigger(EventArgs e)
        {
            OnRaiseTrigger?.Invoke(this, e);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Padre</param>
        /// <param name="requireCreateThread">Necesita crear un hilo para ser lanzado si o no</param>
        public DataInputTrigger(IDataInput parent, bool requireCreateThread)
        {
            _Parent = parent;
            _RequireCreateThread = requireCreateThread;
        }
        public override void Start(IDataInput input)
        {
            OnRaiseTrigger += Trigger_OnRaiseTrigger;
        }
        public override void Stop(IDataInput input)
        {
            OnRaiseTrigger -= Trigger_OnRaiseTrigger;
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
            IDataInput origin = (IDataInput)sender;
            origin.ProcessData();
        }
        public override string ToString()
        {
            return "DataInputTrigger";
        }
    }
}