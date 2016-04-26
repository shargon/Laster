using Laster.Core.Interfaces;

namespace Laster.Inputs.Http
{
    public class RSSInput : HttpRestInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RSSInput() : base() { }


        protected override IData OnGetData()
        {
            IData ret = base.OnGetData();

            // Ya tenemos la página web, vamos a tratarla

            return ret;
        }
    }
}