using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XRD.LibCat.Models {
	[Description("Library Patron")]
	[Table("tblPatrons")]
	public class Patron : Abstract.Person, ISoftDeleted, IPatron {
		#region Constructors
		public Patron() : base() { }

		public Patron(string text) : base(text) { }

		public Patron(PersonName personName) : base(personName) { }

		public Patron(string fName, string lName, string prefix, string mName, string suffix, string nickname) : base(fName, lName, prefix, mName, suffix, nickname) { }
		#endregion

		#region DB Columns
		[Display(Name = "Current Age", ShortName = "Curr. Age", Description = "The patron's current age.")]
		public int? Age { get; set; }

		[Display(Name = "Current Grade Level", ShortName = "Curr. Grade", Description = "The patron's current grade level.")]
		public GradeLevels Grade { get; set; } = GradeLevels.NotSet;

		[Display(Name = "Minimum Allowed Age", ShortName = "Min. Age", Description = "The minimum age-rated book the patron is allowed to check out.")]
		public int? MinAge { get; set; } = null;

		[Display(Name = "Maximum Allowed Age", ShortName = "Max. Age", Description = "The maximum age-rated book the patron is allowed to check out.")]
		public int? MaxAge { get; set; } = null;

		[Display(Name = "Minimum Allowed Grade", ShortName = "Min. Grade", Description = "The minimum grade level-rated book the patron is allowed to check out.")]
		public GradeLevels MinGrade { get; set; } = GradeLevels.NotSet;

		[Display(Name = "Maximum Allowed Grade", ShortName = "Max. Grade", Description = "The maximum grade level-rated book the patron is allowed to check out.")]
		public GradeLevels MaxGrade { get; set; } = GradeLevels.NotSet;

		[Column("Email")]
		private string _email;
		[EmailAddress, StringLength(150)]
		[Display(Name = "Email Address", ShortName = "Email", Description = "The patron's (or patron's guardian's) email address for notification of past-due books.")]
		public string Email {
			get => _email;
			set {
				if (string.IsNullOrWhiteSpace(value)) {
					_email = null;
					return;
				}
				string temp = value.TrimTo(150);
				if (!temp.IsValidEmail())
					throw new ArgumentOutOfRangeException(nameof(Email), $"The email provided [{temp}] is not a valid email address.");
				_email = temp;
			}
		}

		/// <summary>
		/// The Foreign Key to the patron's primary teacher.
		/// </summary>
		public int? TeacherId { get; set; }
		#endregion

		#region Navigation
		public virtual ICollection<BorrowingHistory> BorrowingHistories { get; set; }

		public virtual StaffMember Teacher { get; set; }
		#endregion

		protected override void InstantiateCollections() {
			base.InstantiateCollections();
			BorrowingHistories = new HashSet<BorrowingHistory>();
		}

		public class Config : EntityConfig<Patron> {
			public override void Configure(EntityTypeBuilder<Patron> builder) {
				base.Configure(builder);

				builder.HasMany(p => p.BorrowingHistories)
					.WithOne(c => c.Patron)
					.HasForeignKey(c => c.PatronId)
					.HasPrincipalKey(p => p.Id)
					.OnDelete(DeleteBehavior.Restrict);
			}
		}
	}
}