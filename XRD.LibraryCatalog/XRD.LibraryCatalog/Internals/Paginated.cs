using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace XRD.LibCat {
	public class Paginated : INotifyPropertyChanged {
		public Paginated() { }

		public event PropertyChangedEventHandler PropertyChanged;
		protected void FirePropertyChangedEvent(string propName) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
		public event EventHandler Navigated;
		protected void FireNavigatedEvent() {
			Navigated?.Invoke(this, new EventArgs());
		}

		#region Properties
		protected virtual string ItemDescription { get; } = "result";
		protected virtual int MAX_PAGE_SIZE { get; } = 40;
		public virtual bool CanSearch { get; } = true;

		private int _pageSize;
		public int PageSize {
			get => _pageSize;
			protected set {
				if (value < 1)
					value = 1;
				else if (value > MAX_PAGE_SIZE)
					value = MAX_PAGE_SIZE;
				if(!_pageSize.Equals(value)) {
					_pageSize = value;
					FirePropertyChangedEvent(nameof(PageSize));
				}
			}
		}

		private int _pageIndex = 1;
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
					FirePropertyChangedEvent(nameof(CanMoveNext));
					FirePropertyChangedEvent(nameof(CanMovePrevious));
					FirePropertyChangedEvent(nameof(PageDescription));
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
					FirePropertyChangedEvent(nameof(CanMoveNext));
					FirePropertyChangedEvent(nameof(CanMovePrevious));
					FirePropertyChangedEvent(nameof(PageDescription));
					FirePropertyChangedEvent(nameof(CanSearch));
				}
			}
		}
		public int TotalPages {
			get {
				if (TotalItems == 0)
					return 1;
				return (int)Math.Ceiling(TotalItems / (double)PageSize);
			}
		}

		private bool _isWorking = false;
		public bool IsWorking {
			get => _isWorking;
			protected set {
				if (!_isWorking.Equals(value)) {
					_isWorking = value;
					FirePropertyChangedEvent(nameof(IsWorking));
				}
			}
		}

		private int StartIndex => ((PageIndex - 1) * PageSize) + 1;
		private int EndIndex {
			get {
				int end = StartIndex + PageSize - 1;
				if (end > TotalItems)
					end = TotalItems;
				return end;
			}
		}

		public bool CanMoveNext => PageIndex < TotalPages;
		public bool CanMovePrevious => PageIndex > 1;

		public string PageDescription =>
			TotalItems <= 0
			? $"No {ItemDescription.Pluralize()}"
			: $"Displaying {StartIndex:N0} - {EndIndex:N0} of {TotalItems:N0} total " +
			$"{(TotalItems == 1 ? ItemDescription : ItemDescription.Pluralize())}.";
		#endregion

		protected async virtual Task<bool> PerformWork() => false;

		public async Task<bool> MoveFirst() {
			if (!CanMovePrevious)
				return false;
			PageIndex =1;
			IsWorking = true;
			var res = await PerformWork();
			IsWorking = false;
			return res;
		}

		public async Task<bool> MoveNext() {
			if (!CanMoveNext)
				return false;
			PageIndex++;
			IsWorking = true;
			var res = await PerformWork();
			IsWorking = false;
			return res;
		}

		public async Task<bool> JumpToPage(int pageIndex) {
			if (!CanSearch)
				return false;

			if (pageIndex < 1)
				pageIndex = 1;
			else if (pageIndex > TotalPages)
				pageIndex = TotalPages;

			if (!PageIndex.Equals(pageIndex)) {
				PageIndex = pageIndex;
				IsWorking = true;
				var res = await PerformWork();
				IsWorking = false;
				return res;
			} else
				return false;
		}
		public async Task<bool> MovePrevious() {
			if (!CanMovePrevious)
				return false;
			PageIndex--;
			IsWorking = true;
			var res = await PerformWork();
			IsWorking = false;
			return res;
		}

		public async Task<bool> MoveLast() {
			if (!CanMoveNext)
				return false;
			PageIndex = TotalPages;
			IsWorking = true;
			var res = await PerformWork();
			IsWorking = false;
			return res;
		}
	}
}
