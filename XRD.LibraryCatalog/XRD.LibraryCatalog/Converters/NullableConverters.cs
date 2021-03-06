﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace XRD.LibCat.Converters {
	[ValueConversion(typeof(object), typeof(Visibility))]
	public class NullableToVisibilityConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return Visibility.Collapsed;
			if (value is System.Collections.IList coll)
				return (coll.Count < 1) ? Visibility.Collapsed : Visibility.Visible;

			return Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
	}

	[ValueConversion(typeof(object), typeof(bool))]
	public class NullableToEnabledConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return false;
			if (value is System.Collections.IList coll)
				return coll.Count > 0;

			return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
	}
}
