using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace XRD.LibCat {
	/// <summary>
	/// Pre-defined lists of Personal Honorific Titles
	/// </summary>
	public static class Honorifics {
		/// <summary>
		/// Enumeration of the defined Title class lists. (This is a Flag enumeration and can be combined using "|".)
		/// </summary>
		[Flags]
		public enum TitleClasses {
			/// <summary>
			/// Common personal titles
			/// </summary>
			[Description("Common personal titles")]
			Common = 0b_0000_0000,
			/// <summary>
			/// Formal honorific titles in the UK
			/// </summary>
			[Description("Formal honorific titles in the UK.")]
			Formal_UK = 0b_0000_0001,
			/// <summary>
			/// Formal honorific titles in the US
			/// </summary>
			[Description("Formal honorific titles in the US.")]
			Formal_US = 0b_0000_0010,
			/// <summary>
			/// Academic honorific titles
			/// </summary>
			[Description("Academic honorific titles.")]
			Academic = 0b_0000_0100,
			/// <summary>
			/// Professional honorific titles (US & Europe)
			/// </summary>
			[Description("Professional honorific titles (US & Europe).")]
			Professional = 0b_0000_1000,
			/// <summary>
			/// Roman Catholic & Orthodox Christian honorific titles
			/// </summary>
			[Description("Roman Catholic & Orthodox Christian honoric titles.")]
			CatholicOrthodox = 0b_0001_0000,
			/// <summary>
			/// Protestant Christian honorific titles.
			/// </summary>
			[Description("Protestant Christian honorific titles.")]
			Protestant = 0b_0010_0000,
			/// <summary>
			/// Judiac honorific titles.
			/// </summary>
			[Description("Judiac honorific titles.")]
			Judaic = 0b_0100_0000,
			/// <summary>
			/// Islamin honorific titles.
			/// </summary>
			[Description("Islamic honorific titles.")]
			Islamic = 0b_1000_0000
		}

		/// <summary>
		/// Get a list of personal honorific titles from the specified TitleClasses. (Common are always included.)
		/// </summary>
		/// <param name="titleClasses">The various classes of titles to be included.</param>
		/// <returns>A list of unique titles, in alphabetical order.</returns>
		public static List<string> GetHonorificList(TitleClasses titleClasses) {
			List<string> res = new List<string>(Common);
			if (titleClasses.HasFlag(TitleClasses.Academic))
				res.AddRangeIfNotNull(Academic);
			if (titleClasses.HasFlag(TitleClasses.CatholicOrthodox))
				res.AddRangeIfNotNull(Catholic);
			if (titleClasses.HasFlag(TitleClasses.Formal_UK))
				res.AddRangeIfNotNull(UKFormal);
			if (titleClasses.HasFlag(TitleClasses.Formal_US))
				res.AddRangeIfNotNull(USFormal);
			if (titleClasses.HasFlag(TitleClasses.Islamic))
				res.AddRangeIfNotNull(Islamic);
			if (titleClasses.HasFlag(TitleClasses.Judaic))
				res.AddRangeIfNotNull(Judaic);
			if (titleClasses.HasFlag(TitleClasses.Professional))
				res.AddRangeIfNotNull(Professional);
			if (titleClasses.HasFlag(TitleClasses.Protestant))
				res.AddRangeIfNotNull(Protestant);
			return res.Distinct().OrderBy(a => a).ToList();
		}

		/// <summary>
		/// Get a list of all common honorific prefixes for name splitting.
		/// </summary>
		/// <returns></returns>
		internal static List<string> GetAll() {
			var res = GetHonorificList(
			TitleClasses.Academic |
			TitleClasses.CatholicOrthodox |
			TitleClasses.Formal_UK |
			TitleClasses.Formal_US |
			TitleClasses.Islamic |
			TitleClasses.Judaic |
			TitleClasses.Professional |
			TitleClasses.Protestant
			);
			for (int i = 0; i < res.Count; i++) {
				if (res[i].Contains("."))
					res.Add(res[i].Replace(".", string.Empty));
			}
			return res;
		}

		internal static string[] ForParsingName = new string[] {
			"Master",
			"Mr.",
			"Mr",
			"Mister",
			"Miss",
			"Mrs.",
			"Mrs",
			"Missus",
			"Ms.",
			"Ms",
			"Mx.",
			"Mx",
			"Sir",
			"Madam",
			"Dame",
			"Lord",
			"Lady",
			"Her Excellency",
			"His Excellency",
			"Her Honour",
			"His Honour",
			"The Hon.",
			"The Hon",
			"Hon.",
			"Hon",
			"The",
			"The Honourable",
			"The Right Honourable",
			"The Most Honourable",
			"M.P.",
			"MP",
			"Rep.",
			"Rep",
			"Representative",
			"Sen.",
			"Sen",
			"Senator",
			"Dr.",
			"Dr",
			"Doctor",
			"Prof.",
			"Prof",
			"Professor",
			"Chancellor",
			"Vice-Chancellor",
			"Vice Chancellor",
			"Principal",
			"President",
			"Warden",
			"Dean",
			"Regent",
			"Rector",
			"Provost",
			"Director",
			"Chief Executive",
			"Cl",
			"Counsel",
			"SCl",
			"Senior Counsel",
			"Eur Ing",
			"European Engineer",
			"HH",
			"His Holiness",
			"HAH",
			"His All Holiness",
			"His Beatitude",
			"HE",
			"His Excellency",
			"HMEH",
			"His Most Eminent Highness",
			"His Eminence",
			"The Most Rev.",
			"The Most Rev",
			"The Most Reverend",
			"His Grace",
			"The Rt. Rev.",
			"The Rt Rev",
			"The Right Reverend",
			"The Rev.",
			"The Rev",
			"The Reverend",
			"Fr.",
			"Fr",
			"Father",
			"Br.",
			"Br",
			"Brother",
			"Sr.",
			"Sr",
			"Sister",
			"Pr.",
			"Pr",
			"Pastor",
			"Rev.",
			"Rev",
			"Reverend",
			"Elder",
			"Rabbi",
			"Imām",
			"Shaykh",
			"Muftī",
			"Hāfiz",
			"Hāfizah",
			"Qārī",
			"Mawlānā",
			"Hājī",
			"Sayyid",
			"Sayyidah",
			"Sharif"
		};

		/// <summary>
		/// Common personal honorific titles
		/// </summary>
		public static string[] Common = new string[] {
			"Master",
			"Mr.",
			"Mister",
			"Miss",
			"Mrs.",
			"Missus",
			"Ms.",
			"Mx.",
			"M."
		};

		/// <summary>
		/// Formal (UK) honorific titles.
		/// </summary>
		public static string[] UKFormal = new string[] {
			"Sir",
			"Madam",
			"Dame",
			"Lord",
			"Lady",
			"Her Excellency",
			"His Excellency",
			"Her Honour",
			"His Honour",
			"The Hon.",
			"Hon.",
			"The",
			"The Honourable",
			"The Right Honourable",
			"The Most Honourable",
			"M.P.",
		};

		/// <summary>
		/// Formal (US) honorific titles.
		/// </summary>
		public static string[] USFormal = new string[] {
			"Sir",
			"Madam",
			"Her Honor",
			"His Honor",
			"The Honorable",
			"Rep.",
			"Representative",
			"Sen.",
			"Senator",
		};

		/// <summary>
		/// Academic personal titles.
		/// </summary>
		public static string[] Academic = new string[] {
			"Dr.",
			"Doctor",
			"Prof.",
			"Professor",
			"Chancellor",
			"Vice-Chancellor",
			"Principal",
			"President",
			"Warden",
			"Dean",
			"Regent",
			"Rector",
			"Provost",
			"Director",
			"Chief Executive"
		};

		/// <summary>
		/// Professional personal titles (US & EU)
		/// </summary>
		public static string[] Professional = new string[] {
			"Dr.",
			"Doctor",
			"Cl",
			"Counsel",
			"SCl",
			"Senior Counsel",
			"Eur Ing",
			"European Engineer",
			"President",
			"Director",
			"Chief Executive"
		};

		/// <summary>
		/// Roman Catholic & Orthodox personal titles.
		/// </summary>
		public static string[] Catholic = new string[] {
			"HH",
			"His Holiness",
			"HAH",
			"His All Holiness",
			"His Beatitude",
			"HE",
			"His Excellency",
			"HMEH",
			"His Most Eminent Highness",
			"His Eminence",
			"The Most Rev.",
			"The Most Reverend",
			"His Grace",
			"The Rt. Rev.",
			"The Right Reverend",
			"The Rev.",
			"The Reverend",
			"Fr.",
			"Father",
			"Br.",
			"Brother",
			"Sr.",
			"Sister"
		};

		/// <summary>
		/// Protestant Christian personal titles.
		/// </summary>
		public static string[] Protestant = new string[] {
			"Fr.",
			"Father",
			"Pr.",
			"Pastor",
			"Br.",
			"Brother",
			"Rev.",
			"Reverend",
			"Elder"
		};

		/// <summary>
		/// Judaic personal titles.
		/// </summary>
		public static string[] Judaic = new string[] {
			"Rabbi"
		};

		/// <summary>
		/// Islamic personal titles.
		/// </summary>
		public static string[] Islamic = new string[] {
			"Imām",
			"Shaykh",
			"Muftī",
			"Hāfiz",
			"Hāfizah",
			"Qārī",
			"Mawlānā",
			"Hājī",
			"Sayyid",
			"Sayyidah",
			"Sharif"
		};
	}
}