using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace XRD.LibCat {
	public class PaginatedList<T> : List<T> where T : class {
		#region Properties
		public int PageIndex { get; private set; }
		public int TotalPages { get; private set; }
		public string Description { get; private set; }
		#endregion

		private PaginatedList(List<T> items, int count, int pageIndex, int pageSize) {
			PageIndex = pageIndex;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			AddRange(items);

			int start = ((pageIndex - 1) * pageSize) + 1;
			int end = start + pageSize - 1;
			if (end > count)
				end = count;
			if (count == 0)
				start = 0;
			Description = $"Displaying {start} - {end} of {count} total record{(count == 1 ? string.Empty : "s")}";
		}

		#region Calculated Properties
		public bool CanMoveNext => PageIndex < TotalPages;
		public bool CanMovePrevious => PageIndex > 1;
		#endregion

		public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize) {
			int count = source.Count();
			var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
			return new PaginatedList<T>(items, count, pageIndex, pageSize);
		}

		public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize) {
			int count = await source.CountAsync();
			var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
			return new PaginatedList<T>(items, count, pageIndex, pageSize);
		}
	}
}