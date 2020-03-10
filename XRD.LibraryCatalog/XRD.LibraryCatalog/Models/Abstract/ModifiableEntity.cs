using System;
using System.ComponentModel.DataAnnotations;

namespace XRD.LibCat.Models.Abstract {
	/// <summary>
	/// Base class for entities that are modifiable
	/// </summary>
	public abstract class ModifiableEntity : Entity {
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="isNew">Is a new entity being created?</param>
		protected ModifiableEntity(bool isNew = false) : base(isNew) {
			if (isNew) {
				Ec = 0;
				Ts = DateTime.MinValue;
			}
		}

		#region Db Fields
		/// <summary>
		/// The TimeStamp of the last save of the entity to the underlying store.
		/// </summary>
		[Display(Name = "Last Saved", ShortName = "Modified", Description = "When was the record last saved?")]
		public DateTime Ts { get; internal set; }

		/// <summary>
		/// The total number of times the entity has been modified and saved.
		/// </summary>
		[Display(Name = "Total Edits", ShortName = "#Edits", Description = "The total number of times the record has been modified.")]
		public int Ec { get; internal set; }
		#endregion

		public DateTime? LastSave => Ts == DateTime.MinValue ? (DateTime?)null : Ts.ToLocalTime();
	}
}