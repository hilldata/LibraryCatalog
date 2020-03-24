using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XRD.GoogleBooksApi.Models {
	/// <summary>
	/// The root of the result response from a Google Books API request.
	/// </summary>
	[DataContract]
	public class SearchResult {
		/// <summary>
		/// The results kind (should be "books#volumes" [plural])
		/// </summary>
		[DataMember(Name ="kind")]
		public string Kind { get; set; }

		[DataMember(Name ="totalItems")]
		string totalItems { get; set; }

		[DataMember(Name ="items")]
		public List<BookVolume> Items { get; set; }

		/// <summary>
		/// The total number of items that matched the search parameters.
		/// </summary>
		public int TotalItems =>
			int.TryParse(totalItems, out int res) ? res : -1;
	}
}
