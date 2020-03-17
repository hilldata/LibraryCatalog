using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XRD.LibCat.Models {
	[Description("Author/Contributor")]
	[Table("tblAuthors")]
	public class Author : Abstract.Entity {
		public Author() : base() { }

		public Author(int catId, string authorName, int ordIndex = 0, string role = null) : base(true) {
			CatId = catId;
			FullName = authorName;
			OrdIndex = ordIndex;
			Role = role;
		}

		public Author(CatalogEntry book, string authorName, int ordIndex = 0, string role = null) : base(true) {
			Book = book;
			FullName = authorName;
			OrdIndex = ordIndex;
			Role = role;
		}

		#region DB Columns
		/// <summary>
		/// FK to the Book
		/// </summary>
		public int CatId { get; private set; }

		private string _name;
		[StringLength(400)]
		[Display(Name = "Full Name", ShortName = "Name", Description = "The author's name.")]
		public string FullName {
			get => _name;
			set {
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException(nameof(FullName), "The Author's name is required.");
				PersonName name = new PersonName(value);
				_name = name.ToFullName(includeMiddle: true).TrimTo(1000);
			}
		}

		public int OrdIndex { get; set; }

		private string _role;
		[StringLength(150)]
		[Display(Name = "Role(s)", ShortName = "Role(s)", Description = "The author's role(s) in creating the volume.")]
		public string Role {
			get => _role;
			set => _role = !string.IsNullOrWhiteSpace(value) ? value.TrimTo(150) : null;
		}

		[NotMapped, System.Text.Json.Serialization.JsonIgnore]
		public PersonName PersonName {
			get => new PersonName(_name);
			set => _name = value == null ? throw new ArgumentNullException(nameof(PersonName)) : value.ToFullName(includeMiddle: true);
		}
		#endregion

		public override string ToString() => $"{OrdIndex}. {FullName}{(!string.IsNullOrWhiteSpace(Role) ? " (" + Role + ")" : string.Empty)}";
		public override List<EntityValidationError> Validate() {
			List<EntityValidationError> res = new List<EntityValidationError>();
			if (string.IsNullOrWhiteSpace(FullName))
				res.Add(new EntityValidationError(nameof(FullName), "Full Name is required"));
			if (Book == null && CatId == 0)
				res.Add(new EntityValidationError(nameof(Book), "No Catalog Entry has been assigned."));
			return res;
		}

		#region Navigation
		public virtual CatalogEntry Book { get; set; }
		#endregion

		public class Config : EntityConfig<Author> {
			public override void Configure(EntityTypeBuilder<Author> builder) {
				base.Configure(builder);

				builder.HasIndex(ba => new {
					ba.CatId,
					ba.FullName
				}).IsUnique();

				builder.HasIndex(ba => new {
					ba.CatId,
					ba.OrdIndex
				});
			}
		}
	}
}