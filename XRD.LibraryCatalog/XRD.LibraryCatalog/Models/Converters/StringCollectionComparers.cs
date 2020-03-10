using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace XRD.LibCat.Models {
	public class ListStringValueComparer : ValueComparer<List<string>> {
		public ListStringValueComparer() :
			base(
				(t1, t2) => doEquals(t1, t2),
				t => doGetHashCode(t),
				t => doGetSnapshot(t)) { }

		private static bool doEquals(List<string> l, List<string> r) =>
			doGetHashCode(l).Equals(doGetHashCode(r));

		private static int doGetHashCode(List<string> vs) {
			if (vs.IsNullOrEmpty())
				return 0;
			return TextConcatenator.Concatenate(vs).GetHashCode();
		}

		private static List<string> doGetSnapshot(List<string> vs) {
			if (vs.IsNullOrEmpty())
				return null;
			return new List<string>(vs);
		}
	}

	public class ObservableStringCollectionValueComparer : ValueComparer<ObservableCollection<string>> {
		public ObservableStringCollectionValueComparer() :
			base(
				(t1, t2) => doEquals(t1, t2),
				t => doGetHashCode(t),
				t => doGetSnapshot(t)) { }

		private static bool doEquals(ObservableCollection<string> l, ObservableCollection<string> r) =>
			doGetHashCode(l).Equals(doGetHashCode(r));

		private static int doGetHashCode(ObservableCollection<string> vs) {
			if (vs.IsNullOrEmpty())
				return 0;
			return TextConcatenator.Concatenate(vs).GetHashCode();
		}

		private static ObservableCollection<string> doGetSnapshot(ObservableCollection<string> vs) {
			if (vs.IsNullOrEmpty())
				return null;
			return new ObservableCollection<string>(vs);
		}
	}
}
