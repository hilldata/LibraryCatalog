using System;
using System.ComponentModel;
using System.Threading.Tasks;

using XRD.Text;

namespace XRD {
	public abstract class PaginatedSet : INotifyPropertyChanged {
		protected PaginatedSet() { }

		#region Events
		/// <summary>
		/// Required event for INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		protected void FirePropertyChangedEvent(string pName) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));

		/// <summary>
		/// Event fired when the PaginatedSet has navigated to a different page in the result set.
		/// </summary>
		public event EventHandler Navigated;
		protected void FireNavigatedEvent() =>
			Navigated?.Invoke(this, new EventArgs());
		#endregion

		#region Properties
		/// <summary>
		/// Description of the set's items to be used in generating a UI string description of the current set page.
		/// </summary>
		protected virtual string ItemDescription { get; } = "result";
		/// <summary>
		/// The Maximum page size allowed.
		/// </summary>
		public virtual int MaxPageSize { get; } = 40;

		private int _pageSize;
		/// <summary>
		/// The current page size to be submitted in the search.
		/// </summary>
		public int PageSize {
			get => _pageSize;
			protected set {
				if (value < 1)
					value = 1;
				else if (value > MaxPageSize)
					value = MaxPageSize;
				if (!_pageSize.Equals(value)) {
					_pageSize = value;
					FirePropertyChangedEvent(nameof(PageSize));
				}
			}
		}

		private int _pageIndex = 1;
		/// <summary>
		/// The current page index in the results set (*this number is 1-based, not 0-based*; for human readability).
		/// </summary>
		public int PageIndex {
			get => _pageIndex;
			set {
				if (value < 1)
					value = 1;
				else if (value > TotalPages)
					value = TotalPages;

				if(!_pageIndex.Equals(value)) {
					_pageIndex = value;
					FirePropertyChangedEvent(nameof(PageIndex));
				}
			}

		}

		private int _totalItems = 0;
		public int TotalItems {
			get => _totalItems;
			set {
				if (value < 0)
					value = 0;
				if(!_totalItems.Equals(value)) {
					_totalItems = value;
					FirePropertyChangedEvent(nameof(TotalItems));
					FirePropertyChangedEvent(nameof(TotalPages));
					FirePropertyChangedEvent(nameof(CanMoveBack));
					FirePropertyChangedEvent(nameof(CanMoveForward));
					FirePropertyChangedEvent(nameof(CanSearch));
					FirePropertyChangedEvent(nameof(PageDescription));
				}
			}
		}

		private bool _isWorking = false;
		public bool IsWorking {
			get => _isWorking;
			protected set {
				if(_isWorking != value) {
					_isWorking = value;
					FirePropertyChangedEvent(nameof(IsWorking));
				}
			}
		}
		#endregion

		#region Calculated Properties
		/// <summary>
		/// Boolean indicating whether or not the implementing instance is prepared to search, or if more parameters are required.
		/// </summary>
		public virtual bool CanSearch { get; } = true;

		/// <summary>
		/// The total number of pages in the result set.
		/// </summary>
		public int TotalPages {
			get {
				if (TotalItems <= 0)
					return 1;
				return (int)Math.Ceiling(TotalItems / (double)PageSize);
			}
		}

		/// <summary>
		/// Is it possible to move backwards in the set (are we NOT on page 1)?
		/// </summary>
		public bool CanMoveBack =>
			PageIndex > 1;

		/// <summary>
		/// Is it possible to move forwards in the set (are we NOT on the last page)?
		/// </summary>
		public bool CanMoveForward =>
			PageIndex < TotalPages;

		private int startIndex => ((PageIndex - 1) * PageSize) + 1;
		private int endIndex {
			get {
				int end = startIndex + PageSize - 1;
				if (end > TotalItems)
					end = TotalItems;
				return end;
			}
		}

		public string PageDescription =>
			TotalItems <= 0
			? $"No {ItemDescription.Pluralize()}"
			: $"Displaying {startIndex:N0} - {endIndex:N0} of {TotalItems:N0} total " +
			$"{(TotalItems == 1 ? ItemDescription : ItemDescription.Pluralize())}.";
		#endregion

		protected abstract Task<bool> PerformWork();

		public async Task<bool> MoveFirst() {
			if (!CanMoveBack)
				return false;
			IsWorking = true;
			PageIndex = 1;
			var res = await PerformWork();
			IsWorking = false;
			return res;
		}

		public async Task<bool> MovePrevious() {
			if (!CanMoveBack)
				return false;
			IsWorking = true;
			PageIndex--;
			var res = await PerformWork();
			IsWorking = true;
			return res;
		}

		public async Task<bool> JumpToPage(int pageIndex) {
			if (!CanSearch)
				return false;
			if (TotalItems <= 0)
				return false;

			if (pageIndex < 1)
				pageIndex = 1;
			if (pageIndex > TotalPages)
				pageIndex = TotalPages;

			if (!PageIndex.Equals(pageIndex)) {
				IsWorking = true;
				PageIndex = pageIndex;
				var res = await PerformWork();
				IsWorking = false;
				return res;
			} else
				return false;
		}

		public async Task<bool> MoveNext() {
			if (!CanMoveForward)
				return false;
			IsWorking = true;
			PageIndex++;
			var res = await PerformWork();
			IsWorking = false;
			return res;
		}

		public async Task<bool> MoveLast() {
			if (!CanMoveForward)
				return false;
			IsWorking = true;
			PageIndex = TotalPages;
			var res = await PerformWork();
			IsWorking = false;
			return res;
		}
	}
}
