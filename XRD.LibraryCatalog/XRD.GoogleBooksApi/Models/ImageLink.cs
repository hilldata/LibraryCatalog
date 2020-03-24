using System.Runtime.Serialization;

namespace XRD.GoogleBooksApi.Models {
	/// <summary>
	/// Class representing the image links (front cover thumbnails) for a volume.
	/// </summary>
	[DataContract]
	public class ImageLink {
		[DataMember(Name ="smallThumbnail")]
		string smallThumbnail { get; set; }
		/// <summary>
		/// The URL to the small thumbnail for the volume.
		/// </summary>
		public string SmallThumbnail => removeCurl(smallThumbnail);

		[DataMember(Name = "thumbnail")]
		string thumbnail { get; set; }
		/// <summary>
		/// The URL to the thumbnail for the volume.
		/// </summary>
		public string Thumbnail => removeCurl(thumbnail);

		private const string GOOGLE_IMAGE_CURL_PARM = "&edge=";
		private static string removeCurl(string url) {
			if (string.IsNullOrWhiteSpace(url))
				return string.Empty;

			if (url.Contains(GOOGLE_IMAGE_CURL_PARM)) {
				int i1 = url.IndexOf(GOOGLE_IMAGE_CURL_PARM);
				int i2 = url.IndexOf("&", i1 + 1);
				if (i2 < i1)// No further ampersands were found after the curl param.
					return url.Substring(0, i1);
				else
					return url.Remove(i1, i2 - i1);
			} else
				return url;
		}
	}
}
