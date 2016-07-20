namespace Laster.Core.Classes
{
    public class Variable
    {
        /// <summary>
        /// Valor
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        /// Valor
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Constructor privado
        /// </summary>
        public Variable() { }
        /// <summary>
        /// Constructor privado
        /// </summary>
        public Variable(string name, string property, string value)
        {
            Name = name;
            Property = property;
            Value = value;
        }

        public Variable Clone()
        {
            return new Variable(Name, Property, Value);
        }
    }
}