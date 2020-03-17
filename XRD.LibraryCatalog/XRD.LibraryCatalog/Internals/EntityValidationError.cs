namespace XRD.LibCat {
	public class EntityValidationError {
		internal EntityValidationError(string propName, string desc) {
			PropertyName = propName;
			ErrorDescription = desc.Trim();
		}

		/// <summary>
		/// The name of the property that failed validation.
		/// </summary>
		public readonly string PropertyName;
		/// <summary>
		/// The description of the validation failure.
		/// </summary>
		public readonly string ErrorDescription;
	}
}
