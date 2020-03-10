using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XRD.LibCat.Models {
	[Description("Owned Copy of Book")]
	[Table("tblOwnedBooks")]
	public class OwnedBook : Abstract.Entity, ISoftDeleted {
		#region Constructors
		public OwnedBook() : base() { }
		public OwnedBook(int bookNumber, int catId) : base(true) {
			BookNumber = bookNumber;
			CatId = catId;
			IsDeleted = false;
		}

		public OwnedBook(int bookNumber, CatalogEntry volume) : base(true) {
			BookNumber = bookNumber;
			Book = volume ?? throw new ArgumentNullException(nameof(volume));
		}
		#endregion

		#region DB Columns
		[Display(Name = "Book ID Number", ShortName = "ID#", Description = "The ID number assigned to the book copy (from the sticker placed in the book).")]
		public int BookNumber { get; set; }

		[Display(Name = "Catalog ID", ShortName = "Cat. ID", Description = "The ID of Catalog record that defines the book copy.")]
		public int CatId { get; set; }

		[Display(Name = "Is Deleted", ShortName = "Is Del?", Description = "Is this book missing or discarded?")]
		public bool IsDeleted { get; set; }
		#endregion

		#region Navigation
		public virtual CatalogEntry Book { get; set; }

		public virtual ICollection<BorrowingHistory> BorrowingHistories { get; set; }
		#endregion

		protected override void InstantiateCollections() {
			base.InstantiateCollections();

			BorrowingHistories = new HashSet<BorrowingHistory>();
		}

		public class Config : EntityConfig<OwnedBook> {
			public override void Configure(EntityTypeBuilder<OwnedBook> builder) {
				base.Configure(builder);

				builder.HasAlternateKey(ob => ob.BookNumber);

				builder.HasMany(ob => ob.BorrowingHistories)
					.WithOne(c => c.Book)
					.HasForeignKey(c => c.BookNumber)
					.HasPrincipalKey(ob => ob.BookNumber)
					.OnDelete(DeleteBehavior.Restrict);
			}
		}
	}
}