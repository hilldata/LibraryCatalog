using System;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace XRD.LibCat.GoogleBooksApi {
	public class ApiClient : Paginated {
		public ApiClient() : base() { }

		#region Private Fields
		private SearchFields _field = SearchFields.NotSet;
		private string _criteria = string.Empty;
		#endregion

		#region Properties
		public SearchFields Field {
			get => _field;
			private set {
				if (!_field.Equals(value)) {
					_field = value;
					FirePropertyChangedEvent(nameof(Field));
				}
			}
		}

		public string Criteria {
			get => _criteria;
			private set {
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException(nameof(Criteria));

				value = value.Trim();

				// Remove any wild cards from the criteria as Google Books does not appear to support them.
				if(value.ContainsWildcard()) {
					value = value
						.Replace(TextExtensions.MULTI_CHAR_WILDCARD.ToString(), string.Empty)
						.Replace(TextExtensions.SINGLE_CHAR_WILDCARD.ToString(), string.Empty);
				}

				if(!_criteria.Equals(value)) {
					_criteria = value;
					FirePropertyChangedEvent(nameof(Criteria));
				}
			}
		}

		private List<VolumeInfo> _items;
		public List<VolumeInfo> Items { 
			get => _items;
			private set {
				_items = value;
				FirePropertyChangedEvent(nameof(Items));
			}
		}
		public override bool CanSearch => !string.IsNullOrWhiteSpace(Criteria);
		#endregion

		public List<VolumeInfo> ExactIdMatch {
			get {
				// If the current List contains no items, stop.
				if (Items.IsNullOrEmpty())
					return null;

				// Only search for exact matches ISBNs 
				if(Field == SearchFields.ISBN) {
					return Items
						.Where(
							(vi => vi.IndustryIdentifiers
								.Where(i => i.Identifier.Trim().ToLower().Equals(Criteria.Trim().ToLower())
							).Any())
						).ToList();
				}
				return null;
			}
		}
		// Build a URL to be submitted to the API.
		private string searchUrl {
			get {
				if (!CanSearch)
					throw new Exception("You must provide search criteria text before searching.");

				// Start with the root URL defined in the app's resource manager.
				StringBuilder sb = new StringBuilder(Properties.Resources.GoogleApiUrl);

				// Only include the field name if it is not "not set";
				// Make sure that the search criteria is url-encoded.
				if (Field == SearchFields.NotSet)
					sb.Append(WebUtility.UrlEncode(Criteria));
				else
					sb.Append($"{Field}:{WebUtility.UrlEncode(Criteria)}");

				// Only include the startindex if it is greater than 1
				if (PageIndex > 1)
					sb.Append($"&startIndex={PageIndex - 1}"); // Subtract by 1 as google's api is 0-based.
				
				// Only include the maxresults if it is not the default of 10.
				if (PageSize != 10)
					sb.Append($"&maxResults={PageSize}");

				// Return the url.
				return sb.ToString();
			}
		}

		protected async override Task<bool> PerformWork() { 
			IsWorking = true;
			// Submit the request to the API
			var response = await App.HttpClient.GetAsync(searchUrl);

			// Check to see if the response was unsuccessful.
			if(!response.IsSuccessStatusCode) {
				Error err = JsonConvert.DeserializeObject<Error>(await response.Content.ReadAsStringAsync());
				System.Windows.MessageBox.Show(err.Message, "Google Books API Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
				Items = null;
				IsWorking = false;
				return false;
			}

			// Deserialize the results.
			SearchResult res = JsonConvert.DeserializeObject<SearchResult>(await response.Content.ReadAsStringAsync());

			// Only reset the TotalItems for a new request; TotalItems is set to zero by SearchAsync(...)
			if (TotalItems == 0) {
				TotalItems = res.TotalItems ?? res.Items.Count;
			}

			// Ignore the BookVolume entity and only add the VolumeInfo to the Items list.
			_items = new List<VolumeInfo>();
			foreach(var i in res.Items) {
				_items.Add(i.VolumeInfo);
			}
			
			// Fire the property changed event after all items are loaded.
			FirePropertyChangedEvent(nameof(Items));
			IsWorking = false;
			return true;
		}

		public async Task<bool> SearchAsync(SearchFields field, string criteria, int pageSize = 10) {
			Field = field;
			Criteria = criteria;
			PageSize = pageSize;
			TotalItems = 0;
			PageIndex = 1;
			FireNavigatedEvent();
			return await PerformWork();
		}

		public void Clear() {
			Field = SearchFields.NotSet;
			Criteria = null;
			TotalItems = 0;
			PageIndex = 1;
			Items = new List<VolumeInfo>();
			FireNavigatedEvent();
		}
	}
}
