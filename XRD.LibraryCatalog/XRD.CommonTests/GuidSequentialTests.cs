using Microsoft.VisualStudio.TestTools.UnitTesting;
using XRD;
using System;
using System.Collections.Generic;
using System.Text;

namespace XRD.Tests {
	[TestClass()]
	public class GuidSequentialTests {
		private static string _bytesToString(byte[] vs) {
			StringBuilder sb = new StringBuilder();
			foreach(var b in vs) {
				if (sb.Length > 0)
					sb.Append(" ");
				sb.Append(b.ToString("X3"));
			}
			return sb.ToString();
		}
		/// <summary>
		/// Asserts that 2 GuidSequentials with the same seed, 
		/// created at the same time, have the same last 8 bytes.
		/// </summary>
		[TestMethod()]
		public void NewTest() {
			// Arrange
			Random r = new Random();
			int seed = r.Next();

			//Act 
			Guid g1 = GuidSequential.New(seed);
			Guid g2 = GuidSequential.New(seed);

			byte[] b1 = new byte[8];
			byte[] b2 = new byte[8];
			Array.Copy(g1.ToByteArray(), 8, b1, 0, 8);
			Array.Copy(g2.ToByteArray(), 8, b2, 0, 8);
			string s1 = _bytesToString(b1);
			string s2 = _bytesToString(b2);

			// Assert
			Assert.AreEqual(s1, s2);
		}

		/// <summary>
		/// Asserts that 2 GuidSEquentials created at the same time
		/// have the same last 6 bytes
		/// </summary>
		[TestMethod()]
		public void NewTest1() {
			// Arrange
			Guid g1 = GuidSequential.New();
			Guid g2 = GuidSequential.New();

			byte[] b1 = new byte[6];
			byte[] b2 = new byte[6];
			Array.Copy(g1.ToByteArray(), 10, b1, 0, 6);
			Array.Copy(g2.ToByteArray(), 10, b2, 0, 6);
			string s1 = _bytesToString(b1);
			string s2 = _bytesToString(b2);

			// Assert
			Assert.AreEqual(s1, s2);
		}

		/// <summary>
		/// Asserts that 2 GuidSequentials created 1 second apart 
		/// do NOT have the same last 6 bytes.
		/// </summary>
		[TestMethod()]
		public void NewTest2() {
			// Arrange & act
			Guid g1 = GuidSequential.New();

			System.Threading.Thread.Sleep(1000);
			Guid g2 = Guid.NewGuid();

			byte[] b1 = new byte[6];
			byte[] b2 = new byte[6];
			Array.Copy(g1.ToByteArray(), 10, b1, 0, 6);
			Array.Copy(g2.ToByteArray(), 10, b2, 0, 6);
			string s1 = _bytesToString(b1);
			string s2 = _bytesToString(b2);

			// Assert
			Assert.AreNotEqual(s1, s2);

		}

		/// <summary>
		/// Asserts that the CreateTime can be calculated from a GuidSequential 
		/// (that it has a value) and that the Date, Hour, Minute, Second match 
		/// the DateTime provided to the constructor
		/// </summary>
		[TestMethod()]
		public void GetCreateTimeTest() {
			// Arrange
			DateTime ctime = DateTime.UtcNow;
			Guid g1 = GuidSequential.New(ctime);

			//Act
			DateTime? c1 = g1.GetCreateTime();
			DateTime c2 = g1.GetCreateTime().Value;

			//Assert
			Assert.IsTrue(c1.HasValue);

			Assert.AreEqual(ctime.Date, c2.Date);
			Assert.AreEqual(ctime.Hour, c2.Hour);
			Assert.AreEqual(ctime.Minute, c2.Minute);
			Assert.AreEqual(ctime.Second, c2.Second);
		}
	}
}