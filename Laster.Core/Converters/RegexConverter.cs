using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Laster.Core.Converters
{
    public class RegexConverter : TypeConverter
    {
        // Overrides the CanConvertFrom method of TypeConverter.
        // The ITypeDescriptorContext interface provides the context for the
        // conversion. Typically, this interface is used at design time to 
        // provide information about the design-time container.
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }
        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                if (string.IsNullOrEmpty(value.ToString()))
                    return null;

                return new Regex(value.ToString());
            }
            return base.ConvertFrom(context, culture, value);
        }
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null) return "";
                return ((Regex)value).ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}