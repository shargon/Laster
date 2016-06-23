namespace Laster.Core.Classes
{
    public class NameClass
    {
        /// <summary>
        /// Nombre
        /// </summary>
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public NameClass()
        {
            Name = GetType().Name;
        }
    }
}