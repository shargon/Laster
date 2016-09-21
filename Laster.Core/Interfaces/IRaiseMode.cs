using System.ComponentModel;
using System.Drawing;

namespace Laster.Core.Interfaces
{
    public class IRaiseMode
    {
        /// <summary>
        /// Devuelve si está iniciado o no
        /// </summary>
        [Browsable(false)]
        public bool IsStarted { get; private set; }
        /// <summary>
        /// Constructor privado
        /// </summary>
        internal protected IRaiseMode() { }

        IDataInput _Parent;

        public virtual void Start(IDataInput input) { IsStarted = true; _Parent = input; }
        public virtual void Stop(IDataInput input) { IsStarted = false; }
        public virtual Image GetIcon() { return null; }
    }
}