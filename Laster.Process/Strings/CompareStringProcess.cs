using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

namespace Laster.Process.Strings
{
    public class CompareStringProcess : IDataProcess
    {
        public enum EExpected : byte
        {
            Contains = 0,
            Equal = 1,
            Distinct = 2,
            More = 3,
            Less = 4,
            MoreOrEqual = 5,
            LessOrEqual = 6
        }

        public enum ECount : byte
        {
            Any = 0,
            All = 1,
        } 

        /// <summary>
        /// Nombre del evento
        /// </summary>
        [DefaultValue("")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Text { get; set; }
        /// <summary>
        /// Esperado
        /// </summary>
        [DefaultValue(EExpected.Contains)]
        public EExpected Expected { get; set; }
        /// <summary>
        /// Cantidad de elementos
        /// </summary>
        [DefaultValue(ECount.Any)]
        public ECount Count { get; set; }
        /// <summary>
        /// Realizar un trim antes de la comprobación
        /// </summary>
        [DefaultValue(TrimStringProcess.ETrim.None)]
        public TrimStringProcess.ETrim TrimBefore { get; set; }

        public override string Title { get { return "Strings - Compare"; } }

        public CompareStringProcess()
        {
            DesignBackColor = Color.Blue;
            TrimBefore = TrimStringProcess.ETrim.None;
        }

        /// <summary>
        /// Saca el contenido de los datos a un archivo
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            switch (Count)
            {
                case ECount.All:
                    {
                        foreach (object o in data)
                            if (!Compare(TrimBefore, o.ToString(), Text, Expected))
                                return DataBreak();

                        return data;
                    }
                case ECount.Any:
                    {
                        foreach (object o in data)
                            if (Compare(TrimBefore, o.ToString(), Text, Expected))
                                return data;
                        break;
                    }
            }
            return DataBreak();
        }

        bool Compare(TrimStringProcess.ETrim trim, string value, string text, EExpected expected)
        {
            switch (trim)
            {
                case TrimStringProcess.ETrim.All: value = value.Trim(); break;
                case TrimStringProcess.ETrim.Start: value = value.TrimStart(); break;
                case TrimStringProcess.ETrim.End: value = value.TrimEnd(); break;
            }

            if (expected == EExpected.Contains)
                return value.Contains(text);

            int ix = value.CompareTo(text);

            switch (expected)
            {
                case EExpected.Equal: return ix == 0;
                case EExpected.Distinct: return ix != 0;
                case EExpected.Less: return ix < 0;
                case EExpected.More: return ix > 0;
                case EExpected.LessOrEqual: return ix <= 0;
                case EExpected.MoreOrEqual: return ix >= 0;
            }

            return false;
        }
    }
}