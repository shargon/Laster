using System.ComponentModel;

namespace Laster.Core.Classes
{
    public class NameClass
    {
        /// <summary>
        /// Nombre
        /// </summary>
        [Category("Design")]
        public string Name { get; set; }

        protected NameClass() { Name = GetType().Name; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
                return GetType().Name;

            string cn = GetType().Name;

            if (Name == cn) return Name;
            return Name + " (" + cn + ")";
        }
    }
}