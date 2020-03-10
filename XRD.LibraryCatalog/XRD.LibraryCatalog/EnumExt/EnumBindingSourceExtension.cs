using System;
using System.Windows.Markup;

namespace XRD.LibCat {
	/// <summary>
	/// Extension for inline binding to Enums as an ItemsSource
	/// </summary>
	/// <remarks>
	/// H/T: https://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/ 
	/// </remarks>
	public class EnumBindingSourceExtension : MarkupExtension {
		private Type _enumType;
		public Type EnumType {
			get => _enumType;
			set {
				if (value != _enumType) {
					if (value != null) {
						Type enumType = Nullable.GetUnderlyingType(value) ?? value;
						if (!enumType.IsEnum) {
							throw new ArgumentException("Type must be an Enum.");
						}
						_enumType = value;
					}
				}
			}
		}

		public EnumBindingSourceExtension() { }
		public EnumBindingSourceExtension(Type enumType) {
			EnumType = enumType;
		}

		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (_enumType == null)
				throw new InvalidOperationException("The EnumType must be specified.");

			Type actualEnumType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
			Array enumValues = Enum.GetValues(actualEnumType);
			if (actualEnumType == _enumType)
				return enumValues;

			Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
			enumValues.CopyTo(tempArray, 1);
			return tempArray;
		}
	}
}