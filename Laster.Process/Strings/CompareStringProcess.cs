using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;

namespace Laster.Process.Strings
{
    public class CompareStringProcess : IDataProcess
    {
        public enum EExpected : byte
        {
            Contains = 0,
            NotContains = 1,

            ContainsAnyWord = 2,
            NotContainsAnyWord = 3,

            Equal = 4,
            Distinct = 5,
            More = 6,
            Less = 7,
            MoreOrEqual = 8,
            LessOrEqual = 9
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
        [Category("String modification")]
        [DefaultValue(AlterStringProcess.ETrim.None)]
        public AlterStringProcess.ETrim TrimBefore { get; set; }
        /// <summary>
        /// Realizar un trim antes de la comprobación
        /// </summary>
        [Category("String modification")]
        [DefaultValue(AlterStringProcess.ECase.None)]
        public AlterStringProcess.ECase CaseBefore { get; set; }

        public override string Title { get { return "Strings - Compare"; } }

        public CompareStringProcess()
        {
            DesignBackColor = Color.Blue;
            TrimBefore = AlterStringProcess.ETrim.None;
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
                            if (!Compare(AlterStringProcess.Alter(o, TrimBefore, CaseBefore), Text, Expected))
                                return DataBreak();

                        return data;
                    }
                case ECount.Any:
                    {
                        List<object> ls = new List<object>();

                        foreach (object o in data)
                            if (Compare(AlterStringProcess.Alter(o, TrimBefore, CaseBefore), Text, Expected))
                                ls.Add(o);

                        return Reduce(EReduceZeroEntries.Break, ls);
                    }
            }
            return DataBreak();
        }

        bool Compare(string value, string text, EExpected expected)
        {
            switch (expected)
            {
                case EExpected.Contains: return value.Contains(text);
                case EExpected.NotContains: return !value.Contains(text);
                case EExpected.ContainsAnyWord: { return text.Split(new char[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries).Any(o => value.Contains(o)); }
                case EExpected.NotContainsAnyWord: { return !text.Split(new char[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries).Any(o => value.Contains(o)); }

                case EExpected.Equal: return value.CompareTo(text) == 0;
                case EExpected.Distinct: return value.CompareTo(text) != 0;
                case EExpected.Less: return value.CompareTo(text) < 0;
                case EExpected.More: return value.CompareTo(text) > 0;
                case EExpected.LessOrEqual: return value.CompareTo(text) <= 0;
                case EExpected.MoreOrEqual: return value.CompareTo(text) >= 0;
            }

            return false;
        }
    }
}