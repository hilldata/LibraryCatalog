using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XRD.LibCat.Models {
	[Description("Genre")]
	[Table("tblGenres")]
	public class Genre : Abstract.Entity {
		public Genre() : base() { }
		public Genre(int catId, string genre) : base(true) {
			CatId = catId;
			Value = genre.HasValue() ? genre.Truncate(50) : throw new ArgumentNullException(nameof(genre));
		}

		public Genre(CatalogEntry book, string genre) : base(true) {
			Book = book;
			Value = genre.HasValue() ? genre.Truncate(50) : throw new ArgumentNullException(nameof(genre));
		}
		#region Fields
		public int CatId { get; private set; }

		[StringLength(50), Required]
		[Display(Name = "Genre", ShortName = "Genre", Description = "The genre associated with the cataloged item.")]
		public string Value { get; private set; }
		#endregion

		#region Navigation Properties
		public virtual CatalogEntry Book { get; set; }
		#endregion

		public class Config : EntityConfig<Genre> {
			public override void Configure(EntityTypeBuilder<Genre> builder) {
				base.Configure(builder);

				builder.HasIndex(i => new {
					i.CatId,
					i.Value
				}).IsUnique();
			}
		}
	}
}
