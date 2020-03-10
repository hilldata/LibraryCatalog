using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XRD.LibCat.Models.Abstract {
	/// <summary>
	/// Base class for all entity models.
	/// </summary>
	/// <remarks>
	/// Only inherit directly from this class if the entity will never be edited.
	/// Modifiable entities should inherit from the <see cref="ModifiableEntity"/> abstract class
	/// </remarks>
	public abstract class Entity : IEntity {
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="isNew">Is a new entity being created?</param>
		protected Entity(bool isNew = false) {
			InstantiateCollections();
			if (isNew) {
				Uid = GuidSequential.New();
				if (typeof(ISoftDeleted).IsAssignableFrom(GetType()))
					(this as ISoftDeleted).IsDeleted = false;
			}
		}

		#region Db Fields
		/// <summary>
		/// The Primary key.
		/// </summary>
		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Display(Name = "Record ID", ShortName = "ID", Description = "The unique identifier for the record (Primary Key).")]
		public int Id { get; private set; }

		[Required]
		[Display(Name = "Record Guid", ShortName = "UID", Description = "The unique (across all domains) identifier for hte record. (Alternate Key; used for synchronizing.)")]
		public Guid Uid { get; private set; }
		#endregion

		#region Read-Only Fields
		/// <summary>
		/// The datetime the record was created.
		/// </summary>
		[Display(Name = "Created", ShortName = "CTime", Description = "When the record was created.")]
		public DateTime? CTime => Uid.GetCreateTime()?.ToLocalTime();
		#endregion

		protected virtual void InstantiateCollections() { }

		public abstract class EntityConfig<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Entity {
			public virtual void Configure(EntityTypeBuilder<TEntity> builder) {
				builder.HasKey(e => e.Id);
				builder.HasAlternateKey(e => e.Uid);

				if (typeof(ModifiableEntity).IsAssignableFrom(typeof(TEntity)))
					builder.HasIndex(e => (e as ModifiableEntity).Ts);
				if (typeof(ISoftDeleted).IsAssignableFrom(typeof(TEntity))) {
					builder.HasIndex(e => (e as ISoftDeleted).IsDeleted);
				}
			}
		}
	}
}
