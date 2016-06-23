using Laster.Core.Interfaces;

namespace Laster.Inputs
{
    public class EmptyInput : IDataInput
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public EmptyInput() : base() { }
        /// <summary>
        /// Constructor extendido
        /// </summary>
        /// <param name="raiseMode">Modo de lanzamiento</param>
        public EmptyInput(IDataInputRaiseMode raiseMode) : base(raiseMode) { }
    }
}