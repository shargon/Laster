using System;
using System.ComponentModel;
using System.Threading;

namespace Laster.Core.Interfaces
{
    public class IDataInputTriggerRaiseMode : IDataInputRaiseMode
    {
        /// <summary>
        /// Padre
        /// </summary>
        [Browsable(false)]
        internal IDataInput Parent { get; set; }
        /// <summary>
        /// Se lanza cuando se ejecuta el trigger
        /// </summary>
        public event EventHandler OnRaiseTrigger = null;
        /// <summary>
        /// Devuelve si existe algun evento asociado
        /// </summary>
        [Browsable(false)]
        public bool HasEvent { get { return OnRaiseTrigger != null; } }
        /// <summary>
        /// Necesita crear un hilo para ser lanzado si o no
        /// </summary>
        [Browsable(false)]
        protected virtual bool RequireCreateThread { get { return false; } }
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
        protected IDataInputTriggerRaiseMode() { }
        public override void Start(IDataInput input)
        {
            Parent = input;
            OnRaiseTrigger += RaiseTrigger;
        }
        public override void Stop(IDataInput input)
        {
            Parent = input;
            OnRaiseTrigger -= RaiseTrigger;
        }
        protected void RaiseTrigger(object sender, EventArgs e)
        {
            if (RequireCreateThread)
            {
                // Creamos el hilo
                Thread th = new Thread(new ParameterizedThreadStart(threadStart));
                th.IsBackground = true;
                th.Start(Parent);
            }
            else
            {
                // Lo lanzamos directamente
                Parent.ProcessData();
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