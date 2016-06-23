using System.Drawing;

namespace Laster.Core.Interfaces
{
    public class IDataInputRaiseMode
    {
        /// <summary>
        /// Constructor privado
        /// </summary>
        internal protected IDataInputRaiseMode() { }

        public virtual void Start(IDataInput input) { }
        public virtual void Stop(IDataInput input) { }
        public virtual Image GetIcon() { return null; }
    }
}