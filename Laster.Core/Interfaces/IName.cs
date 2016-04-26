namespace Laster.Core.Interfaces
{
    public class IName
    {
        /// <summary>
        /// Nombre
        /// </summary>
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}