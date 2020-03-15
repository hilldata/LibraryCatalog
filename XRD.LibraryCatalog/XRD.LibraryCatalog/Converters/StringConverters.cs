using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace XRD.LibCat.Converters {
	[ValueConversion(typeof(string), typeof(Visibility))]
	public class StringToVisibilityConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null || !(value is string))
				return Visibility.Collapsed;

			if (string.IsNullOrWhiteSpace(value as string))
				return Visibility.Collapsed;
			return Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}

	[ValueConversion(typeof(string), typeof(Visibility))]
	public class StringToVisibilityReversedConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null || !(value is string))
				return Visibility.Hidden;

			if (!string.IsNullOrWhiteSpace(value as string))
				return Visibility.Collapsed;
			return Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}

	[ValueConversion(typeof(string), typeof(bool?))]
	public class StringToEnabledConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null || !(value is string))
				return null;

			int minLen = 1;
			if (parameter != null && parameter is int)
				minLen = (int)parameter;

			if (string.IsNullOrWhiteSpace(value as string))
				return false;
			return (value as string).Length >= minLen;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
