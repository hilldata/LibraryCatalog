using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XRD.LibCat.Models {
	[Description("Staff Member")]
	[Table("tblStaffMembers")]
	public class StaffMember : Abstract.Person {
		#region Constructors
		public StaffMember() : base() { }

		public StaffMember(bool isNew) : base(isNew) { }

		public StaffMember(string text) : base(text) { }

		public StaffMember(PersonName personName) : base(personName) { }

		public StaffMember(string fName, string lName, string prefix, string mName, string suffix, string nickname) : base(fName, lName, prefix, mName, suffix, nickname) { }
		#endregion

		#region Db Fields
		private string _room;
		[StringLength(50)]
		[Display(Name = "Room Name", ShortName = "Room", Description = "The name of the room the staff member is normally associated with (classroom for teachers).")]
		public string Room {
			get => _room;
			set => _room = !string.IsNullOrWhiteSpace(value) ? value.TrimTo(50) : null;
		}

		private string _email;
		[EmailAddress, StringLength(150)]
		[Display(Name = "Email Address", ShortName = "Email", Description = "The staff member's email address for notification of past-due books/check-out reports.")]
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

		[Display(Name = "Grades Taught", ShortName = "Grades", Description = "The grade levels taught by this staff member.")]
		public GradeLevels GradesTaught { get; set; }

		[Display(Name = "Subjects Taught", ShortName = "Subjects", Description = "List of subjects taught by this staff member.")]
		public System.Collections.ObjectModel.ObservableCollection<string> Subjects { get; set; }
		#endregion

		#region Navigation
		public virtual ICollection<Patron> Students { get; set; }
		#endregion

		protected override void InstantiateCollections() {
			base.InstantiateCollections();
			Students = new HashSet<Patron>();
		}
		public override List<EntityValidationError> Validate() => base.Validate();

		public class Config : EntityConfig<StaffMember> {
			public override void Configure(EntityTypeBuilder<StaffMember> builder) {
				base.Configure(builder);

				builder.Property(s => s.Subjects).HasConversion(new ObservableStringCollectionConverter());
				builder.Property(s => s.Subjects).Metadata.SetValueComparer(new ObservableStringCollectionValueComparer());

				builder.HasMany(s => s.Students)
					.WithOne(s => s.Teacher)
					.HasForeignKey(s => s.TeacherId)
					.HasPrincipalKey(s => s.Id)
					.OnDelete(DeleteBehavior.Restrict);
			}
		}

		public override string ToString() {
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (!string.IsNullOrWhiteSpace(Prefix))
				sb.Append(Prefix);
			if (!string.IsNullOrWhiteSpace(Last)) {
				if (sb.Length > 0)
					sb.Append(" ");
				sb.Append(Last);
			}
			if (!string.IsNullOrWhiteSpace(First)) {
				if (sb.Length > 0) {
					if (!string.IsNullOrWhiteSpace(Last))
						sb.Append(", ");
					else
						sb.Append(" ");
				}
				sb.Append(First);
			}
			if (sb.Length > 0)
				return sb.ToString();
			else {
				return base.ToString();
			}
		}
	}
}