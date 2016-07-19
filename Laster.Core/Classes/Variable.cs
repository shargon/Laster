namespace Laster.Core.Classes
{
    public class Variable : NameClass
    {
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
        public Variable(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}