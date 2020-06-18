using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
namespace Com.Bing.Report 
{
    public class UnitConverter: TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            return true;
            return base.CanConvertFrom(context, sourceType);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string unitString = (value as string);
                double unitValue = -1;
                if (double.TryParse(unitString.Remove(unitString.Length - 2, 2), out unitValue))
                return new Unit(unitValue, (UnitTypes)Enum.Parse(typeof(UnitTypes), 
		                unitString.Substring(unitString.Length - 2, 2), true));
                else throw new Exception("Property value is not correct.( { ±5.0 × 10-324 to ±1.7 × 10308 } { Cm | Px | In | Mm } )");
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context,CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            return value.ToString();
            else return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
