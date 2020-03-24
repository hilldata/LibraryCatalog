using System;
using System.Collections.Generic;
using System.Linq;

namespace XRD {
	/// <summary>
	/// Class containing commonly-used extension methods for Collections
	/// </summary>
	public static class CollectionExtensions {
		/// <summary>
		/// Is the collection <see langword="null"/> or empty (contains no values)?
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="coll"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> coll) {
			if (coll == null)
				return true;
			else if (coll.Count() < 1)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Adds an item to the collection (but only if the ITEM is not null).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ts">The collection to add to.</param>
		/// <param name="item">The item to add</param>
		/// <param name="enforceUnique">Only add the item if it does not already exist in the collection?</param>
		/// <returns>A boolean indicating whether or not the item was added.</returns>
		public static bool AddIfNotNull<T>(this ICollection<T> ts, T item, bool enforceUnique = false) {
			if (ts == null)
				throw new ArgumentNullException(nameof(ts));
			if (item == null)
				return false;
			if (enforceUnique) {
				if (!ts.Contains(item))
					ts.Add(item);
				else
					return false;
			} else
				ts.Add(item);
			return true;
		}

		/// <summary>
		/// Adds a collection of items to the collection (but only if each item is not null)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ts">The collection to add to.</param>
		/// <param name="collection">The items to add.</param>
		/// <param name="enforceUnique">Only add each item if it does not already exist in the collection?</param>
		/// <returns>The actual number of items that were added to the collection.</returns>
		public static int AddRangeIfNotNull<T>(this ICollection<T> ts, IEnumerable<T> collection, bool enforceUnique = false) {
			if (ts == null)
				throw new ArgumentNullException(nameof(ts));
			if (collection.IsNullOrEmpty())
				return 0;
			int counter = 0;
			foreach (T item in collection) {
				if (ts.AddIfNotNull(item, enforceUnique))
					counter++;
			}
			return counter;
		}
	}
}
