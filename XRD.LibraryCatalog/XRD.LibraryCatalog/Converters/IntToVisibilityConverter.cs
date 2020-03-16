using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace XRD.LibCat.Converters {
	[ValueConversion(typeof(int), typeof(Visibility))]
	public class IntToVisibilityConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null) {
				if (parameter != null && int.TryParse(parameter.ToString(), out int p)) {
					if (p == 0)
						return Visibility.Visible;
				} else
					return Visibility.Collapsed;
			}
			if(value is int i) {
				if (parameter == null) {
					if (i > 1)
						return Visibility.Visible;
				}else if(int.TryParse(parameter.ToString(), out int p)) {
					if (i == p)
						return Visibility.Visible;
				}
			}
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
