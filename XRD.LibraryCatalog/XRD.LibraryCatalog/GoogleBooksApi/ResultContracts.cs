using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XRD.LibCat.GoogleBooksApi {
	/// <summary>
	/// The root of the result response from a Google Books API request.
	/// </summary>
	[DataContract]
	public class SearchResult {
		/// <summary>
		/// The results kind (should be "books#volumes" [plural])
		/// </summary>
		[DataMember(Name = "kind")]
		public string Kind { get; set; }

		[DataMember(Name = "totalItems")]
		string totalItems { get; set; }

		[DataMember(Name = "items")]
		public List<BookVolume> Items { get; set; }

		/// <summary>
		/// The total number of items that matched the search parameters.
		/// </summary>
		public int? TotalItems => int.TryParse(totalItems, out int res) ? res : (int?)null;
	}

	/// <summary>
	/// The root item in the list.
	/// </summary>
	[DataContract]
	public class BookVolume {
		/// <summary>
		/// The result kind (should be "books#volume" [singular]).
		/// </summary>
		[DataMember(Name = "kind")]
		public string Kind { get; set; }

		/// <summary>
		/// The GoogleID for the book.
		/// </summary>
		[DataMember(Name = "id")]
		public string GoogleId { get; set; }

		/// <summary>
		/// Subclass containing the details of the book.
		/// </summary>
		[DataMember(Name = "volumeInfo")]
		public VolumeInfo VolumeInfo { get; set; }

		public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
	}

	/// <summary>
	/// Class containing the interesting data for a specific Book Volume (many fields of the Google Books API result are ignored).
	/// </summary>
	[DataContract]
	public class VolumeInfo {
		/// <summary>
		/// The Title of the book.
		/// </summary>
		[DataMember(Name = "title")]
		public string Title { get; set; }

		/// <summary>
		/// The Subtitle for the book.
		/// </summary>
		[DataMember(Name = "subtitle")]
		public string Subtitle { get; set; }

		/// <summary>
		/// List of Authors
		/// </summary>
		[DataMember(Name = "authors")]
		public List<string> Authors { get; set; }

		/// <summary>
		/// The name of the publisher
		/// </summary>
		[DataMember(Name = "publisher")]
		public string Publisher { get; set; }

		[DataMember(Name = "publishedDate")]
		public string publishedDate { get; set; }

		/// <summary>
		/// Brief description of the book.
		/// </summary>
		[DataMember(Name = "description")]
		public string Description { get; set; }

		/// <summary>
		/// List of Industry Identifiers (ISBNs)
		/// </summary>
		[DataMember(Name = "industryIdentifiers")]
		public List<IndustryIdentifier> IndustryIdentifiers { get; set; }

		[DataMember(Name = "pageCount")]
		string pageCount { get; set; }

		/// <summary>
		/// Categories (subjects) the book is listed under.
		/// </summary>
		[DataMember(Name = "categories")]
		public List<string> Categories { get; set; }

		/// <summary>
		/// ImageLink subclass (contains thumbnail URLs)
		/// </summary>
		[DataMember(Name = "ImageLink")]
		public List<ImageLink> ImageLink { get; set; }

		#region Read-Only Properties
		/// <summary>
		/// The date the book was published.
		/// </summary>
		/// <remarks>
		/// Some sample dates could not be parsed, so this performs a TryParse on the "publishedDate" string value.
		/// </remarks>
		public DateTime? PublishedDate => DateTime.TryParse(publishedDate, out DateTime res) ? res : (DateTime?)null;

		/// <summary>
		/// The actual string value of the "publishedDate" as was read from the API.
		/// </summary>
		public string PublishedDateAsString => publishedDate;

		/// <summary>
		/// A single string containing all authors.
		/// </summary>
		public string AuthorDisplay {
			get {
				if (Authors == null || Authors.Count < 1)
					return null;
				if (Authors.Count == 1)
					return Authors[0];
				System.Text.StringBuilder sb = new System.Text.StringBuilder(Authors[0]);
				for (int i = 1; i < Authors.Count; i++) {
					sb.Append("; ");
					sb.Append(Authors[i]);
				}
				return sb.ToString();
			}
		}

		/// <summary>
		/// A single string containing all ISBNs
		/// </summary>
		public string IsbnDisplay {
			get {
				if (IndustryIdentifiers == null || IndustryIdentifiers.Count < 1)
					return null;
				if (IndustryIdentifiers.Count == 1)
					return IndustryIdentifiers[0].Identifier;
				System.Text.StringBuilder sb = new System.Text.StringBuilder(IndustryIdentifiers[0].Identifier);
				for (int i = 1; i < IndustryIdentifiers.Count; i++) {
					sb.Append("; ");
					sb.Append(IndustryIdentifiers[i].Identifier);
				}
				return sb.ToString();
			}
		}

		/// <summary>
		/// Get a URL to use as the source for the volume's image.
		/// </summary>
		public string ImageUrl {
			get {
				if (ImageLink == null || ImageLink.Count < 1)
					return null;
				for (int i = 0; i < ImageLink.Count; i++) {
					if (!string.IsNullOrWhiteSpace(ImageLink[i].Thumbnail))
						return ImageLink[i].Thumbnail;
					if (!string.IsNullOrWhiteSpace(ImageLink[i].SmallThumbnail))
						return ImageLink[i].SmallThumbnail;
				}
				return null;
			}
		}

		/// <summary>
		/// The number of pages in the book.
		/// </summary>
		/// <remarks>
		/// Performs a TryParse on the value read from the API. This is due to the fact that several samples were either null or empty strings or contained invalid characters.
		/// </remarks>
		public int? PageCount => int.TryParse(pageCount, out int res) ? res : (int?)null;
		#endregion
	}

	/// <summary>
	/// Class representing the ImageLinks (front cover thumbs) for a book.
	/// </summary>
	[DataContract]
	public class ImageLink {
		/// <summary>
		/// The URL to the small thumbnail for the book.
		/// </summary>
		[DataMember(Name = "smallThumbnail")]
		public string SmallThumbnail { get; set; }

		/// <summary>
		/// The URL to the thumbnail for the book.
		/// </summary>
		[DataMember(Name = "thumbnail")]
		public string Thumbnail { get; set; }

		private const string GOOGLE_IMAGE_CURL_PARM = "&edge=";

		public string ImageUrl {
			get {
				string url = string.Empty;
				if (string.IsNullOrWhiteSpace(Thumbnail)) {
					if (string.IsNullOrWhiteSpace(SmallThumbnail))
						return null;
					else
						url = SmallThumbnail;
				} else {
					url = Thumbnail;
				}
				if (string.IsNullOrWhiteSpace(url))
					return null;

				if (url.Contains(GOOGLE_IMAGE_CURL_PARM)) {
					int i1 = url.IndexOf(GOOGLE_IMAGE_CURL_PARM);
					int i2 = url.IndexOf("&", i1 + 1);
					if (i2 < i1) // no further ampersands were found after the curl.
						return url.Substring(0, 1);
					else
						return url.Remove(i1, i2 - i1);
				} else
					return url;
			}
		}
	}

	/// <summary>
	/// Class representing an item of a list of Industry Identifier values.
	/// </summary>
	[DataContract]
	public class IndustryIdentifier {
		/// <summary>
		/// The type of identifier
		/// </summary>
		[DataMember(Name = "type")]
		public string Type { get; set; }

		/// <summary>
		/// The value of the identifier
		/// </summary>
		[DataMember(Name = "identifier")]
		public string Identifier { get; set; }

		/// <summary>
		/// The type value for ISBN-13 identifiers
		/// </summary>
		public const string ISBN13 = "ISBN_13";

		/// <summary>
		/// The type value for ISBN-10 identifiers.
		/// </summary>
		public const string ISBN10 = "ISBN_10";
	}
}