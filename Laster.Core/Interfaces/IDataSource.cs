namespace Laster.Core.Interfaces
{
    public class IDataSource : IName
    {
        int _Id;
        /// <summary>
        /// Identificador
        /// </summary>
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
    }
}