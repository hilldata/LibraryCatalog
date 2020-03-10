using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace XRD.LibCat {
	/// <summary>
	/// Converter used to retrieve the Display value of the specified enumType from either the <see cref="XRD.EnumDescAttribute"/>, or the <see cref="DescriptionAttribute"/>, or the actual enum value as a string, in that order.
	/// </summary>
	/// <remarks>
	/// H/T: https://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/
	/// </remarks>
	public class EnumDisplayTypeConverter : EnumConverter {
		public EnumDisplayTypeConverter(Type type) : base(type) { }
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				if (value != null) {
					FieldInfo fi = value.GetType().GetField(value.ToString());
					if (fi != null) {
						var descAtt = fi.GetCustomAttribute<DescriptionAttribute>(false);
						if (descAtt != null)
							return descAtt.Description;
					}
					return value.ToString();
				}
				return string.Empty;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}