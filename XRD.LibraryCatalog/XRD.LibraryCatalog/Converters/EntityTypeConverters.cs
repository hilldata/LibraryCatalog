using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace XRD.LibCat.Converters {
	[ValueConversion(typeof(Type), typeof(Visibility))]
	public class EntityTypeToVisibilityConverter : BaseConv, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return Binding.DoNothing;

			if (parameter == null)
				return Binding.DoNothing;

			if(value is Type eType) {
				if(parameter is Type pType) {
					if (pType.IsAssignableFrom(eType))
						return Visibility.Visible;
					else
						return Visibility.Collapsed;
				}
			}
			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}

	[ValueConversion(typeof(Type), typeof(string))]
	public class EntityTypeToToolTipConverter : BaseConv, IValueConverter {
		/// <summary>
		/// Returns a formatted string for ToolTip that contains the entity display name.
		/// </summary>
		/// <param name="value">The type of Entity that is bound</param>
		/// <param name="targetType"></param>
		/// <param name="parameter">The ConverterParameter is the text of the ToolTip, including \"{0}\" as the placeholder for the Entity type display name.</param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return Binding.DoNothing;
			if (parameter == null)
				return Binding.DoNothing;

			if(value is Type eType && parameter is string toolTip) {
				return string.Format(toolTip, Models.LibraryContext.GetDisplayName(eType));
			}
			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
