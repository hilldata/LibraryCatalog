using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XRD.LibCat.Models {
	[Description("Identifier")]
	[Table("tblIdentifiers")]
	public class Identifier : Abstract.Entity {
		public Identifier() : base() { }
		public Identifier(int catId, string identifier) : base(true) {
			CatId = catId;
			Value = identifier.HasValue() ? FixValue(identifier) : throw new ArgumentNullException(nameof(identifier));
		}

		public Identifier(CatalogEntry book, string identifier) : base(true) {
			Book = book;
			Value = identifier.HasValue() ? FixValue(identifier) : throw new ArgumentNullException(nameof(identifier));
		}
		#region Fields
		public int CatId { get; private set; }

		[StringLength(50), Required]
		[Display(Name = "Identifier", ShortName = "ISBN", Description = "The identifier (e.g. ISBN) associated with the cataloged item.")]
		public string Value { get; private set; }
		#endregion
		public override string ToString() => Value;

		#region Navigation Properties
		public virtual CatalogEntry Book { get; set; }
		#endregion

		public override List<EntityValidationError> Validate() {
			List<EntityValidationError> res = new List<EntityValidationError>();
			if (string.IsNullOrWhiteSpace(Value))
				res.Add(new EntityValidationError(nameof(Value), "Identifier Value is required."));
			if (Book == null && CatId == 0)
				res.Add(new EntityValidationError(nameof(Book), "No Catalog Entry has been assigned."));
			return res;
		}

		public class Config : EntityConfig<Identifier> {
			public override void Configure(EntityTypeBuilder<Identifier> builder) {
				base.Configure(builder);

				builder.HasIndex(i => new {
					i.CatId,
					i.Value
				}).IsUnique();
			}
		}

		internal static string FixValue(string s) {
			if (string.IsNullOrWhiteSpace(s))
				return null;
			return s.Truncate(50).ToUpper();
		}
	}
}
