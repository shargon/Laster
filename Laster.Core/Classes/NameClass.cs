using System.ComponentModel;

namespace Laster.Core.Classes
{
    public class NameClass
    {
        /// <summary>
        /// Nombre
        /// </summary>
        [Category("General")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
        protected NameClass()
        {
            Name = GetType().Name;
        }
    }
}