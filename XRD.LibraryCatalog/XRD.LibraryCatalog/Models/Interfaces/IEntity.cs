using System;

namespace XRD.LibCat.Models {
	/// <summary>
	/// Base Entity interface
	/// </summary>
	public interface IEntity {
		/// <summary>
		/// The Primary Key.
		/// </summary>
		int Id { get; }
		/// <summary>
		/// The globally-unique Alternate Key
		/// </summary>
		Guid Uid { get; }
	}
}
