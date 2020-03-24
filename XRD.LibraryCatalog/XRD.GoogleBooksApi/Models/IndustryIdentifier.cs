using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace XRD.GoogleBooksApi.Models {
	/// <summary>
	/// Class representing an item of a list of Industry Indentifier values.
	/// </summary>
	[DataContract]
	public class IndustryIdentifier {
		/// <summary>
		/// The type of identifier
		/// </summary>
		[DataMember(Name ="type")]
		public string Type { get; set; }

		/// <summary>
		/// The value of the identifier
		/// </summary>
		[DataMember(Name ="identifier")]
		public string Identifier { get; set; }
	}
}
