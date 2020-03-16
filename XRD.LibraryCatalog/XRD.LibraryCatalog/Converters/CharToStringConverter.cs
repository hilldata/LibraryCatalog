using System;
using System.Globalization;
using System.Windows.Data;

namespace XRD.LibCat.Converters {
	[ValueConversion(typeof(char), typeof(string))]
	public class CharToStringConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return string.Empty;
			if (value is char c)
				return c.ToString();
			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
