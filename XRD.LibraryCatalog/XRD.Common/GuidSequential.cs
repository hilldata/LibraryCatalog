using System;

namespace XRD {
	/// <summary>
	/// Static class used to generate "Sequential" guids.
	/// </summary>
	public static class GuidSequential {
		internal static readonly long BASE_DATE_TICKS = new DateTime(1900, 1, 1).Ticks;

		/// <summary>
		/// Generate a new Sequential Guid for storage in SQL Server.
		/// </summary>
		/// <param name="seed">A seed to use to individuate the result.</param>
		/// <returns>A "sequential" guid.</returns>
		public static Guid New(int? seed = null) {
			byte[] arrGuid = Guid.NewGuid().ToByteArray();
			DateTime now = DateTime.UtcNow;

			// get the days and milliseconds, which will be used to build the byte array
			TimeSpan days = new TimeSpan(now.Ticks - BASE_DATE_TICKS);
			TimeSpan mSec = now.TimeOfDay;

			// Convert to a byte array.
			// Note that SqlServer is accuration to 1/300th of a millisecond, so we divide by 3.33333
			byte[] arrDays = BitConverter.GetBytes(days.Days);
			long vMSec = (long)(mSec.TotalMilliseconds / 3.33333);
			byte[] arrMSec = BitConverter.GetBytes(vMSec);

			// Reverse the bytes to match SqlServer's ordering
			Array.Reverse(arrDays);
			Array.Reverse(arrMSec);

			// Copy the Day/MSec arrays to the result
			Array.Copy(arrDays, arrDays.Length - 2, arrGuid, arrGuid.Length - 2, 2);
			Array.Copy(arrMSec, arrMSec.Length - 4, arrGuid, arrGuid.Length - 6, 4);

			//Include the seed value in result (if provided)
			if ((seed ?? 0) != 0) {
				byte[] arrSeed = BitConverter.GetBytes(seed.Value);
				Array.Copy(arrSeed, 0, arrGuid, arrGuid.Length - 10, 4);
			}

			// Return the result.
			return new Guid(arrGuid);
		}

		/// <summary>
		/// Generate a new Sequential Guid for storage in SQL Server.
		/// </summary>
		/// <param name="cTime">The Date/Time (preferrable UTC) to base the Sequential Guid on (e.g. the Date/Time the associated entity was created.</param>
		/// <param name="seed">A seed to use to individuate the result.</param>
		/// <returns>A "sequential" guid.</returns>
		public static Guid New(DateTime cTime, int? seed = null) {
			byte[] arrGuid = Guid.NewGuid().ToByteArray();

			// Get the days and milliseconds from the provided cTime, which will be used to build the byte array
			TimeSpan days = new TimeSpan(cTime.Ticks - BASE_DATE_TICKS);
			TimeSpan mSec = cTime.TimeOfDay;

			// Convert a byte array.
			// Not that SQL server is accurate to 1/300th of a millisecond, so we divide by 3.33333
			byte[] arrDays = BitConverter.GetBytes(days.Days);
			long vMSec = (long)(mSec.TotalMilliseconds / 3.33333);
			byte[] arrMSec = BitConverter.GetBytes(vMSec);

			// Reverse the bytes to match SqlServer's ordering.
			Array.Reverse(arrDays);
			Array.Reverse(arrMSec);

			// Copy the Day/MSec arrays to the result.
			Array.Copy(arrDays, arrDays.Length - 2, arrGuid, arrGuid.Length - 2, 2);
			Array.Copy(arrMSec, arrMSec.Length - 4, arrGuid, arrGuid.Length - 6, 4);

			// Include the see value in the result (if provided)
			if ((seed ?? 0) != 0) {
				byte[] arrSeed = BitConverter.GetBytes(seed.Value);
				Array.Copy(arrSeed, 0, arrGuid, arrGuid.Length - 10, 4);
			}

			// Return the result.
			return new Guid(arrGuid);
		}

		public static Guid New(DateTime cTime, Guid source) {
			byte[] arrGuid = source.ToByteArray();
			TimeSpan days = new TimeSpan(cTime.Ticks - BASE_DATE_TICKS);
			TimeSpan mSec = cTime.TimeOfDay;

			byte[] arrDays = BitConverter.GetBytes(days.Days);
			long vMSec = (long)(mSec.TotalMilliseconds / 3.33333);
			byte[] arrMSec = BitConverter.GetBytes(vMSec);

			Array.Reverse(arrDays);
			Array.Reverse(arrMSec);

			Array.Copy(arrDays, arrDays.Length - 2, arrGuid, arrGuid.Length - 2, 2);
			Array.Copy(arrMSec, arrMSec.Length - 4, arrGuid, arrGuid.Length - 6, 4);
			return new Guid(arrGuid);
		}

		public static DateTime? GetCreateTime(this Guid sequentialGuid) {
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
				return new DateTime(BASE_DATE_TICKS, DateTimeKind.Utc)
					+ new TimeSpan(BitConverter.ToInt32(arrDays, 0), 0, 0, 0, (int)(BitConverter.ToInt32(arrMSec) * 3.33333));
			} catch {
				return null;
			}
		}
	}
}
