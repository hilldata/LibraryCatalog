using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace XRD.LibCat.Converters {

	[ValueConversion(typeof(bool?), typeof(Brush))]
	public class IsDeletedToForegroundConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			Brush def;
			if (parameter != null && parameter is Brush)
				def = (Brush)parameter;
			else
				def = SystemColors.WindowTextBrush;

			if (value == null || !(value is bool? || value is bool))
				return def;

			if ((bool)value)
				return Brushes.Red;
			else
				return def;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
	}

	[ValueConversion(typeof(bool?), typeof(Brush))]
	public class IsDeletedToButtonForegroundConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null || !(value is bool))
				return SystemColors.WindowTextBrush;

			if ((bool)value) {
				if (parameter == null || !(parameter is Brush))
					return SystemColors.WindowTextBrush;
				else
					return (Brush)parameter;
			} else
				return Brushes.Red;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}

	[ValueConversion(typeof(bool?), typeof(string))]
	public class IsDeletedToolTipConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null || !(value is bool)) {
				return Binding.DoNothing;
			}
			string t = "record";
			if (parameter is Type type)
				t = Models.LibraryContext.GetDisplayName(type);

			if ((bool)value) {
				return string.Format(RESTORE, t); //$"Restore this record.{Environment.NewLine}(Clears the \"Is Obsolete\" flag so it will again be displayed in lists and search results.)";
			} else {
				return string.Format(DELETE, t);// $"Delete this record.{Environment.NewLine}(This is not a permanent deletion, but simply sets the \"Is Obsolete\" flag so the record will be hidden.)";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;

		public const string RESTORE = "Restore this {0}.\r\n(Clears the \"Is Obsolete\" flag so it will again be displayed in lists and search results.)";
		public const string DELETE = "Delete this {0}.\r\n(This is not a permanent deletion, but simply sets the \"Is Obsolete\" flag so the record will be hidden.)";
	}
}
