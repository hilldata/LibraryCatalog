using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XRD.LibCat.Models.Abstract {
	public abstract class Person : ModifiableEntity, ISoftDeleted {
		protected Person() : base() { }
		protected Person(bool isNew) : base(isNew) { }
		protected Person(PersonName personName) : base(true) {
			Prefix = personName.GetValue(PersonalNameProperties.Prefix);
			First = personName.GetValue(PersonalNameProperties.First);
			Middle = personName.GetValue(PersonalNameProperties.Middle);
			Last = personName.GetValue(PersonalNameProperties.Last);
			Suffix = personName.GetValue(PersonalNameProperties.Suffix);
			Nickname = personName.GetValue(PersonalNameProperties.Nickname);
		}
		protected Person(string text) : this(new PersonName(text)) { }
		protected Person(string fName, string lName, string prefix, string mName, string suffix, string nickname) : base(true) {
			First = fName.TrimTo(50);
			Last = lName.TrimTo(50);
			Prefix = prefix.TrimTo(50);
			Middle = mName.TrimTo(50);
			Suffix = suffix.TrimTo(50);
			Nickname = nickname.TrimTo(50);
		}

		#region DB Columns
		private string _prefix;
		[StringLength(50)]
		[Display(Name = "Honorific Prefix", ShortName = "Prefix", Description = "The person's honorific prefix.")]
		public string Prefix {
			get => _prefix;
			set => _prefix = !string.IsNullOrWhiteSpace(value) ? value.TrimTo(50) : null;
		}

		private string _first;
		[StringLength(50)]
		[Required]
		[Display(Name = "First Name", ShortName = "First", Description = "The person's first/given name.")]
		public string First {
			get => _first;
			set => _first = !string.IsNullOrWhiteSpace(value) ? value.TrimTo(50) : null;
		}

		private string _middle;
		[StringLength(50)]
		[Display(Name = "Middle Name/Initial", ShortName = "Middle", Description = "The person's middle/given name(s) or initial(s).")]
		public string Middle {
			get => _middle;
			set => _middle = !string.IsNullOrWhiteSpace(value) ? value.TrimTo(50) : null;
		}

		private string _last;
		[StringLength(50)]
		[Required]
		[Display(Name = "Last Name", ShortName = "Last", Description = "The person's last/family name")]
		public string Last {
			get => _last;
			set => _last = !string.IsNullOrWhiteSpace(value) ? value.TrimTo(50) : null;
		}

		private string _suffix;
		[StringLength(50)]
		[Display(Name = "Honorific Suffix", ShortName = "Suffix", Description = "The person's honorific suffix.")]
		public string Suffix {
			get => _suffix;
			set => _suffix = !string.IsNullOrWhiteSpace(value) ? value.TrimTo(50) : null;
		}

		private string _nickname;
		[StringLength(50)]
		[Display(Name = "Nickname", ShortName = "Alias", Description = "The person's nickname or alias.")]
		public string Nickname {
			get => _nickname;
			set => _nickname = !string.IsNullOrWhiteSpace(value) ? value.TrimTo(50) : null;
		}

		[Display(Name = "Is Deleted?", ShortName = "Obs?", Description = "Has this record been flagged as deleted/obsolete?")]
		public bool IsDeleted { get; set; }
		#endregion

		public override List<EntityValidationError> Validate() {
			List<EntityValidationError> res = new List<EntityValidationError>();
			if (string.IsNullOrWhiteSpace(First))
				res.Add(new EntityValidationError(nameof(First), "First (Given) Name is required."));
			if (string.IsNullOrWhiteSpace(Last))
				res.Add(new EntityValidationError(nameof(Last), "Last (Family) Name is required."));
			return res;
		}

		public PersonName ToPersonName() => new PersonName(First, Last, Prefix, Middle, Suffix, Nickname);
		public override string ToString() => ToPersonName().ToFullName();
	}
}