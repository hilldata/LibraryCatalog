using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XRD.LibCat.Models {
	[Description("Borrowing History")]
	[Table("tblBorrowingHx")]
	public class BorrowingHistory : Abstract.Entity {
		public BorrowingHistory() : base() { }

		public BorrowingHistory(int bookNumber, int patronId) : base(true) {
			BookNumber = bookNumber;
			PatronId = patronId;
		}

		public BorrowingHistory(OwnedBook book, Patron patron) : base(true) {
			Book = book;
			Patron = patron;
		}

		#region DB Columns 
		/// <summary>
		/// The owned book's BookNumber (from the scannable barcode sticker).
		/// </summary>
		public int BookNumber { get; set; }
		public int PatronId { get; set; }

		/// <summary>
		/// The date the book is due back.
		/// </summary>
		[DataType(DataType.Date)]
		[Display(Name = "Due Date", ShortName = "Due", Description = "The date the book is due to be returned.")]
		public DateTime DueDate { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "Checked-In", ShortName = "Returned", Description = "The date/time the book was checked back into the library.")]
		public DateTime? CheckInDate { get; set; }
		#endregion

		#region Navigation
		public virtual OwnedBook Book { get; set; }
		public virtual Patron Patron { get; set; }
		#endregion
		public override List<EntityValidationError> Validate() => null;

		public override string ToString() {
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append($"Checked out:\t{CTime:d}");
			sb.Append(Environment.NewLine);
			if(Patron != null) {
				sb.Append($"\tBy:\t\t{Patron}");
				sb.Append(Environment.NewLine);
			}
			sb.Append($"\tDue:\t\t{DueDate:d}");
			if(CheckInDate.HasValue) {
				sb.Append(Environment.NewLine);
				sb.Append($"\tReturned:\t{CheckInDate:d}");
			}
			return sb.ToString();
		}

		public void CheckIn(DateTime? checkInTime = null) {
			CheckInDate = checkInTime ?? DateTime.Now;
		}

		public class Config : EntityConfig<BorrowingHistory> {
			public override void Configure(EntityTypeBuilder<BorrowingHistory> builder) {
				base.Configure(builder);

				builder.Property(c => c.DueDate)
					.HasColumnType("date");

				builder.HasIndex(c => new {
					c.BookNumber,
					c.PatronId,
					c.DueDate
				}).IsUnique();
			}
		}
	}
}