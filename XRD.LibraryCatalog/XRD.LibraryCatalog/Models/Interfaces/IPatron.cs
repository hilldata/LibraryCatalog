namespace XRD.LibCat.Models {
	/// <summary>
	/// Interface for patrons.
	/// </summary>
	public interface IPatron : IHasRestrictions {
		/// <summary>
		/// The Patron's current age
		/// </summary>
		int? Age { get; }
		/// <summary>
		/// The Patron's current grade level
		/// </summary>
		GradeLevels Grade { get; set; }
	}
}