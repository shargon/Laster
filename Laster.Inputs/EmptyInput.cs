using Laster.Core.Interfaces;
using System.Drawing;

namespace Laster.Inputs
{
    public class EmptyInput : IDataInput
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public EmptyInput() : base()
        {
            DesignBackColor = Color.Black;
            DesignForeColor = Color.White;
        }

        public override string Title { get { return "Empty"; } }
    }
}