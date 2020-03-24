using System.Runtime.Serialization;

namespace XRD.GoogleBooksApi.Models {
	/// <summary>
	/// The root item in the result response.
	/// </summary>
	[DataContract]
	public class BookVolume {
		/// <summary>
		/// The result kind (should be "books#volume" [singular]).
		/// </summary>
		[DataMember(Name ="kind")]
		public string Kind { get; set; }

		/// <summary>
		/// The GoogleId for the volume.
		/// </summary>
		[DataMember(Name ="id")]
		public string GoogleId { get; set; }

		/// <summary>
		/// Cubclass containing the details of the volume.
		/// </summary>
		[DataMember(Name ="volumeInfo")]
		public VolumeInfo VolumeInfo { get; set; }
	}
}
