namespace XRD.LibCat.Models {
	/// <summary>
	/// Interface for entities that should only be soft-deleted, never permantly deleted.
	/// </summary>
	public interface ISoftDeleted : IEntity {
		/// <summary>
		/// Has the entity been flagged as deleted/obsolete?
		/// </summary>
		bool IsDeleted { get; set; }
	}
}
