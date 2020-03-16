using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Windows.Data;

namespace XRD.LibCat.Converters {
	[ValueConversion(typeof(bool), typeof(double))]
	public class IsEnabledToOpacityConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return 1;
			if(value is bool isEnabled) {
				if (isEnabled)
					return 1;
			}
			return 0.5;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return false;
			if (value is double d) {
				if (d > 0.5)
					return true;
			}
			return false;
		}
	}
}
