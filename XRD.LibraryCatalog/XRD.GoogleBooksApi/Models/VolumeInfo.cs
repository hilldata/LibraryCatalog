using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XRD.GoogleBooksApi.Models {
	/// <summary>
	/// Class containing the interesting data for a specified Book Volume (many fields of the Google Books API are ignored as non-useful for the local store).
	/// </summary>
	[DataContract]
	public class VolumeInfo {
		/// <summary>
		/// The volume's Title
		/// </summary>
		[DataMember(Name ="title")]
		public string Title { get; set; }

		/// <summary>
		/// The volume's Subtitle.
		/// </summary>
		[DataMember(Name ="subtitle")]
		public string Subtitle { get; set; }

		/// <summary>
		/// List of author(s) as a single string per author.
		/// </summary>
		[DataMember(Name ="authors")]
		public List<string> Authors { get; set; }

		/// <summary>
		/// The name of the organization that published the volume
		/// </summary>
		[DataMember(Name ="publisher")]
		public string Publisher { get; set; }

		/// <summary>
		/// The volume's publication date as a string.
		/// </summary>
		[DataMember(Name ="publishedDate")]
		public string PublishedDate { get; set; }

		/// <summary>
		/// If the <see cref="PublishedDate"/> can be parsed, a DateTime structure, else NULL
		/// </summary>
		public DateTime? PubDate =>
			DateTime.TryParse(PublishedDate, out DateTime res) ? res : (DateTime?)null;

		/// <summary>
		/// Brief description of the volume
		/// </summary>
		[DataMember(Name ="description")]
		public string Description { get; set; }

		/// <summary>
		/// List of Industry Identifiers (ISBNs)
		/// </summary>
		[DataMember(Name ="industryIdentifiers")]
		public List<IndustryIdentifier> IndustryIdentifiers { get; set; }

		[DataMember(Name ="pageCount")]
		string pageCount { get; set; }

		/// <summary>
		/// The number of pages in the volume.
		/// </summary>
		public int? PageCount =>
			int.TryParse(pageCount, out int res) ? res : (int?)null;

		/// <summary>
		/// Categories (subjects/genres) the volume is listed under.
		/// </summary>
		[DataMember(Name ="categories")]
		public List<string> Categories { get; set; }

		/// <summary>
		/// ImageLink subclass (contains thumbnail image URLs).
		/// </summary>
		[DataMember(Name ="imageLink")]
		public ImageLink ImageLink { get; set; }
	}
}
