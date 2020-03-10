using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRD.LibCat.Models;

namespace XRD.LibCat {
	internal static class ExtensionMethods {
		internal static bool IsNullOrEmpty<T>(this IEnumerable<T> ts) {
			if (ts == null)
				return true;
			else if (ts.Count() < 1)
				return true;
			return false;
		}

		internal static bool AddIfNotNull<T>(this ICollection<T> ts, T item, bool enforceUnique = false) {
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

		internal static int AddRangeIfNotNull<T>(this ICollection<T> ts, IEnumerable<T> collection, bool enforceUnique = false) {
			if (ts == null)
				throw new ArgumentNullException(nameof(ts));
			if (collection == null)
				return 0;
			if (collection.Count() < 1)
				return 0;
			int counter = 0;
			foreach (T item in collection) {
				if (ts.AddIfNotNull(item, enforceUnique))
					counter++;
			}
			return counter;
		}

		internal static byte[] FastHash(this byte[] vs) {
			if (vs == null)
				return null;
			M3aHash m3AHash = new M3aHash();
			return m3AHash.ComputeHash(vs);
		}
		internal static byte[] FastHash(this char[] vs) =>
			Encoding.Unicode.GetBytes(vs).FastHash();
		internal static byte[] FastHash(this string vs) =>
			string.IsNullOrWhiteSpace(vs)
			? null
			: Encoding.Unicode.GetBytes(vs).FastHash();

		internal static Guid HashGuid(this byte[] vs) =>
			vs.IsNullOrEmpty()
			? Guid.Empty
			: new Guid(vs.FastHash());
		internal static Guid HashGuid(this char[] vs) =>
			vs.IsNullOrEmpty()
			? Guid.Empty
			: new Guid(vs.FastHash());
		internal static Guid HashGuid(this string vs) =>
			string.IsNullOrWhiteSpace(vs)
			? Guid.Empty
			: new Guid(vs.FastHash());

		internal static DateTime? GetCreateTime(this Guid sequentialGuid) {
			if (sequentialGuid == Guid.Empty)
				return null;

			byte[] arrGuid = sequentialGuid.ToByteArray();
			byte[] arrDays = new byte[4];
			byte[] arrMSec = new byte[8];
			Array.Copy(arrGuid, arrGuid.Length - 2, arrDays, 2, 2);
			Array.Copy(arrGuid, arrGuid.Length - 6, arrMSec, 4, 4);

			Array.Reverse(arrDays);
			Array.Reverse(arrMSec);
			try {
				return new DateTime(GuidSequential.BASE_DATE_TICKS, DateTimeKind.Utc)
					+ new TimeSpan(BitConverter.ToInt32(arrDays, 0), 0, 0, 0, (int)(BitConverter.ToInt32(arrMSec, 0) * 3.33333));
			} catch {
				return null;
			}
		}

		internal static void ValidateNewRestrictions(this IHasRestrictions entity) {
			if (entity == null)
				return;
			if ((entity.MinAge ?? 0) > (entity.MaxAge ?? 100))
				throw new Exception($"The Min. Age [{entity.MinAge}] is greater than the Max. Age [{entity.MaxAge}].");
			if ((entity.MinGrade != GradeLevels.NotSet) && (entity.MaxGrade != GradeLevels.NotSet) && entity.MinGrade > entity.MinGrade)
				throw new Exception($"The Min. Grade [{entity.MinGrade}] is greater than the Max. Grade [{entity.MaxGrade}].");
		}

		internal static bool CanCheckOut(this IHasRestrictions volume, int? age = null, GradeLevels grade = GradeLevels.NotSet) {
			if (volume.MinAge.HasValue && (age ?? 100) < volume.MinAge.Value)
				return false;
			if (volume.MaxAge.HasValue && (age ?? 0) > volume.MaxAge.Value)
				return false;
			if (volume.MinGrade != GradeLevels.NotSet && grade != GradeLevels.NotSet && volume.MinGrade > grade)
				return false;
			if (volume.MaxGrade != GradeLevels.NotSet && grade != GradeLevels.NotSet && volume.MaxGrade < grade)
				return false;
			return true;
		}

		internal static bool CanCheckOut(this IHasRestrictions volume, IPatron patron) {
			if (volume == null || patron == null)
				return false;

			if (volume.MinAge.HasValue && patron.MaxAge.HasValue) {
				if (patron.MaxAge < volume.MinAge)
					return false;
			}
			if (volume.MaxAge.HasValue && patron.MinAge.HasValue) {
				if (patron.MinAge > volume.MaxAge)
					return false;
			}
			if (volume.MinGrade != GradeLevels.NotSet && patron.MaxGrade != GradeLevels.NotSet) {
				if (patron.MaxGrade < volume.MinGrade)
					return false;
			}
			if (volume.MaxGrade != GradeLevels.NotSet && patron.MinGrade != GradeLevels.NotSet) {
				if (patron.MinGrade > volume.MaxGrade)
					return false;
			}
			return volume.CanCheckOut(patron.Age, patron.Grade);
		}

		internal static bool IsKeyErase(this Key key) {
			switch(key) {
				case Key.Enter:
				case Key.Escape:
				case Key.Back:
				case Key.Delete:
					return true;
				default: return false;
			}
		}

		internal static bool IsKeyNumber(this Key key, bool allowDecimal = false) {
			switch(key) {
				case Key.NumPad0:
				case Key.NumPad1:
				case Key.NumPad2:
				case Key.NumPad3:
				case Key.NumPad4:
				case Key.NumPad5:
				case Key.NumPad6:
				case Key.NumPad7:
				case Key.NumPad8:
				case Key.NumPad9:
				case Key.D0:
				case Key.D1:
				case Key.D2:
				case Key.D3:
				case Key.D4:
				case Key.D5:
				case Key.D6:
				case Key.D7:
				case Key.D8:
				case Key.D9:
				case Key.OemComma:
					return true;
				case Key.OemPeriod:
					return allowDecimal;
				default:
					return false;
			}
		}
	}
}
