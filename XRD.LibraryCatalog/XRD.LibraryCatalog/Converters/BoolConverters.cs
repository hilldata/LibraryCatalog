using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace XRD.LibCat.Converters {
	[ValueConversion(typeof(bool?), typeof(Visibility))]
	public class BoolToVisibilityConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null || !(value is bool? || value is bool))
				return Visibility.Hidden;

			if ((bool)value)
				return Visibility.Visible;
			else
				return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null || !(value is Visibility))
				return null;
			return ((Visibility)value) switch
			{
				Visibility.Collapsed => false,
				Visibility.Visible => true,
				_ => null
			};
		}
	}

	[ValueConversion(typeof(bool?), typeof(Visibility))]
	public class BoolToVisibilityReversedConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null || !(value is bool? || value is bool))
				return Visibility.Hidden;
			if ((bool)value)
				return Visibility.Collapsed;
			else
				return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null || !(value is Visibility))
				return null;
			return ((Visibility)value) switch
			{
				Visibility.Collapsed => true,
				Visibility.Visible => false,
				_ => null
			};
		}
	}
}
