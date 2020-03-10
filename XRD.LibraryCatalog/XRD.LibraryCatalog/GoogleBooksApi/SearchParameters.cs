using System.ComponentModel;

namespace XRD.LibCat.GoogleBooksApi {
	/// <summary>
	/// Enumeration of the search fields that Google's Book API recognizes.
	/// </summary>
	[TypeConverter(typeof(EnumDisplayTypeConverter))]
	public enum SearchParameters {
		[Description("(All Fields)")]
		NotSet = 0,
		[Description("In ISBN")]
		ISBN = 1,
		[Description("In Title")]
		InTitle = 2,
		[Description("In Author")]
		InAuthor = 3,
		[Description("In Publisher")]
		InPublisher = 4,
		[Description("In Subject/Genre")]
		Subject = 5,
		[Description("In LCCN")]
		LCCN = 6
	}
}