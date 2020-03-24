using System.Globalization;
using System.Windows.Controls;

namespace XRD.LibCat.Validation {
	public class RequiredStringValidationRule : ValidationRule {
		public int MinLength { get; set; } = 1;

		public RequiredStringValidationRule() { }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
			if (value == null)
				return new ValidationResult(false, "This field is required.");
			if (value is string s) {
				if (string.IsNullOrWhiteSpace(s))
					return new ValidationResult(false, "This field is required.");
				else if (s.Trim().Length < MinLength)
					return new ValidationResult(false, $"This field must contain at least {MinLength} characters.");
			}

			return ValidationResult.ValidResult;
		}
	}
}