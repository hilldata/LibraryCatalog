using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace XRD.LibCat.GoogleBooksApi {
	/// <summary>
	/// Class used to download an ImageLink
	/// </summary>
	public class ImageLinkDownloadResult {
		internal ImageLinkDownloadResult(string mimeType, byte[] bits) {
			MimeType = mimeType.Trim();
			ImageBits = bits;
		}

		internal static async Task<ImageLinkDownloadResult> Create(HttpResponseMessage response) {
			if (response == null)
				throw new ArgumentNullException(nameof(response));
			if (!response.IsSuccessStatusCode)
				throw new Exception(response.ReasonPhrase);

			return new ImageLinkDownloadResult(
				response.Content.Headers.ContentType.MediaType,
				await response.Content.ReadAsByteArrayAsync()
				);
		}

		/// <summary>
		/// The image's MIME (media) type.
		/// </summary>
		public readonly string MimeType;
		/// <summary>
		/// The bits for the image.
		/// </summary>
		public readonly byte[] ImageBits;
	}
}