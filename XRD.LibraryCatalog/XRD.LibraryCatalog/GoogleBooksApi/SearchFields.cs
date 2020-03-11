using System.ComponentModel;

namespace XRD.LibCat.GoogleBooksApi {
	/// <summary>
	/// Enumeration of the search fields that Google's Book API recognizes.
	/// </summary>
	[TypeConverter(typeof(EnumDisplayTypeConverter))]
	public enum SearchFields {
		[Description("(All Fields)")]
		NotSet = 0,
		[Description("ISBN")]
		ISBN = 1,
		[Description("Title")]
		InTitle = 2,
		[Description("Author")]
		InAuthor = 3,
		[Description("Publisher")]
		InPublisher = 4,
		[Description("Genre")]
		Subject = 5
	}
}