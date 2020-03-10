using System;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace XRD.LibCat.GoogleBooksApi {
	/// <summary>
	/// Helper class used to perform calls to the Google Books API.
	/// </summary>
	/// <remarks>As all public methods of this class are web-requests, all public methods are asynchronous.</remarks>
	public class GoogleBooksLookup {
		readonly HttpClient httpClient;
		readonly string rootUrl;

		/// <summary>
		/// The default root URL for search books in the Google Books API (as of 2020/02/13)
		/// </summary>
		public const string DEFAULT_API_URL = "https://www.googleapis.com/books/v1/volumes?q=";

		/// <summary>
		/// Construct a new GoogleBooksLookup helper instance
		/// </summary>
		/// <param name="client">The <see cref="HttpClient"/> instance used by the application (if null, a new instance is created).</param>
		/// <param name="url">The (optional) root URL to the Google Books API, if null/empty/not-well-formed, the <see cref="DEFAULT_API_URL"/> is used.</param>
		public GoogleBooksLookup(HttpClient client, string url = null) {
			if (client == null)
				httpClient = new HttpClient();
			else
				httpClient = client;

			if (!string.IsNullOrWhiteSpace(url)) {
				if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
					rootUrl = url.Trim();
				else
					rootUrl = DEFAULT_API_URL;
			} else {
				rootUrl = DEFAULT_API_URL;
			}
		}

		/// <summary>
		/// Build a search URL to submit to the Google Books API.
		/// </summary>
		/// <param name="searchOn">The field to search on.</param>
		/// <param name="searchCriteria">The critieria to search for.</param>
		/// <returns>A URL-encoded string containing the full URL for the search.</returns>
		private string BuildSearchUrl(SearchParameters searchOn, string searchCriteria) {
			if (string.IsNullOrEmpty(searchCriteria))
				throw new ArgumentNullException(nameof(searchCriteria));

			if (searchOn == SearchParameters.NotSet)
				return $"{rootUrl}{System.Net.WebUtility.UrlDecode(searchCriteria.Trim())}";
			return $"{rootUrl}{searchCriteria.ToString().ToLower()}:{System.Net.WebUtility.UrlDecode(searchCriteria.Trim())}";
		}

		/// <summary>
		/// Submit a search request to the Google Books API and parse the results in a <see cref="SearchResult"/> instance
		/// </summary>
		/// <param name="searchOn">The field to search on.</param>
		/// <param name="searchCriteria">The criteria to search for.</param>
		/// <returns>A <see cref="SearchResult"/> instance containing the interesting data.</returns>
		public async Task<SearchResult> SearchBooksAsync(SearchParameters searchOn, string searchCriteria) {
			string url = BuildSearchUrl(searchOn, searchCriteria);

			var response = await httpClient.GetAsync(url);
			if(!response.IsSuccessStatusCode) {
				Error error = JsonConvert.DeserializeObject<Error>(await response.Content.ReadAsStringAsync());
				throw new Exception(error.ToString());
			}
			return JsonConvert.DeserializeObject<SearchResult>(await response.Content.ReadAsStringAsync());
		}

		/// <summary>
		/// Download the front cover image from the specified <see cref="ImageLink"/>
		/// </summary>
		/// <param name="imageLink">The ImageLink returned by the Google Books API</param>
		/// <param name="includeCurl">Should the page curl glyph be included in the result?</param>
		/// <returns>The front cover image as a <see cref="ImageLinkDownloadResult"/></returns>
		public async Task<ImageLinkDownloadResult> DownloadImage(ImageLink imageLink, bool includeCurl = false) {
			if (imageLink == null)
				throw new ArgumentNullException(nameof(imageLink));

			string url;
			if(string.IsNullOrWhiteSpace(imageLink.Thumbnail)) {
				if (string.IsNullOrWhiteSpace(imageLink.SmallThumbnail))
					return null;
				else
					url = imageLink.SmallThumbnail;
			}else {
				url = imageLink.Thumbnail;
			}
			return await DownloadImage(url, includeCurl);
		}

		private const string GOOGLE_IMAGE_CURL_PARM = "&edge=";

		/// <summary>
		/// Download an image
		/// </summary>
		/// <param name="imageUrl">The URL to the image to be downloaded.</param>
		/// <param name="includeCurl">Should the page curl glyph be included in the result?</param>
		/// <returns>The image as a <see cref="ImageLinkDownloadResult"/></returns>
		public async Task<ImageLinkDownloadResult> DownloadImage(string imageUrl, bool includeCurl =false) {
			if (string.IsNullOrWhiteSpace(imageUrl))
				throw new ArgumentNullException(nameof(imageUrl));

			if(!includeCurl) {
				if(imageUrl.Contains(GOOGLE_IMAGE_CURL_PARM)) {
					int i1 = imageUrl.IndexOf(GOOGLE_IMAGE_CURL_PARM);
					int i2 = imageUrl.IndexOf("&", i1 + 1);
					if (i2 < i1)
						imageUrl = imageUrl.Substring(0, i1);
					else {
						imageUrl = imageUrl.Remove(i1, i2 - i1);
					}
				}
			}
			var response = await httpClient.GetAsync(imageUrl);
			if (response == null)
				throw new Exception("Unknown error in HTTP GET request.");
			if (!response.IsSuccessStatusCode)
				throw new Exception(response.ReasonPhrase);

			return new ImageLinkDownloadResult(
				response.Content.Headers.ContentType.MediaType,
				await response.Content.ReadAsByteArrayAsync()
				);
		}
	}
}
