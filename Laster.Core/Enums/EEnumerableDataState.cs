namespace Laster.Core.Enums
{
    public enum EEnumerableDataState : byte
    {
        /// <summary>
        /// No se trata de un enumerador
        /// </summary>
        NonEnumerable = 0,
        /// <summary>
        /// Solo hay un registro
        /// </summary>
        OnlyOne = 1,
        /// <summary>
        /// Comienza
        /// </summary>
        Start = 2,
        /// <summary>
        /// Está en medio
        /// </summary>
        Middle = 3,
        /// <summary>
        /// Finaliza
        /// </summary>
        End = 4,
    }
}