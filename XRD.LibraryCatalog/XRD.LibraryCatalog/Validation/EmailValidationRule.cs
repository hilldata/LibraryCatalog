using System.Globalization;
using System.Windows.Controls;

namespace XRD.LibCat.Validation {
	public class EmailValidationRule : ValidationRule {
		public bool IsRequired { get; set; } = false;
		public EmailValidationRule() { }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo) { 
			if(value == null) {
				if (IsRequired)
					return new ValidationResult(false, "This field is required.");
				else
					return ValidationResult.ValidResult;
			}

			if(value is string s) {
				if(string.IsNullOrWhiteSpace(s)) {
					if (IsRequired)
						return new ValidationResult(false, "This field is required.");
					else
						return ValidationResult.ValidResult;
				}else {
					if (!s.IsValidEmail())
						return new ValidationResult(false, $"[{s}] is not a valid email address.");
				}
			}
			return ValidationResult.ValidResult;
		}
	}
}
