namespace XRD.LibCat.Models {
	/// <summary>
	/// Interface for entities that implement age/grade restrictions.
	/// </summary>
	public interface IHasRestrictions : ISoftDeleted {
		/// <summary>
		/// The minimum allowed age level
		/// </summary>
		int? MinAge { get; set; }
		/// <summary>
		/// The maximum allowed age level
		/// </summary>
		int? MaxAge { get; set; }
		/// <summary>
		/// The minimum allowed grade level
		/// </summary>
		GradeLevels MinGrade { get; set; }
		/// <summary>
		/// The maximum allowed grade level
		/// </summary>
		GradeLevels MaxGrade { get; set; }
	}
}