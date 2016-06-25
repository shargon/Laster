using Laster.Core.Interfaces;

namespace Laster.Inputs
{
    public class EmptyInput : IDataInput
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public EmptyInput() : base() { }

        public override string Title { get { return "Empty"; } }
    }
}