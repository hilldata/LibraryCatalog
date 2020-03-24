using System;
using System.Collections.Generic;
using System.Text;

namespace XRD {
	public static class FastHashExtensions {
		public static byte[] FastHash(this byte[] vs) {
			if (vs == null)
				return null;
			M3aHash m3AHash = new M3aHash();
			return m3AHash.ComputeHash(vs);
		}

		public static byte[] FastHash(this string vs) =>
			string.IsNullOrWhiteSpace(vs)
			? null
			: Encoding.UTF8.GetBytes(vs).FastHash();

		public static Guid HashGuid(this byte[] vs) =>
			new Guid(vs.FastHash());
		public static Guid HashGuid(this string vs) =>
			new Guid(vs.FastHash());
	}
}
