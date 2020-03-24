using Microsoft.VisualStudio.TestTools.UnitTesting;
using XRD;
using System;
using System.Collections.Generic;
using System.Text;

namespace XRD.Tests {
	[TestClass()]
	public class CollectionExtensionsTests {
		[TestMethod()]
		public void IsNullOrEmpty_TestNull() {
			// arrange 
			List<int> vs = null;
			List<object> list = null;
			// Act and assert
			Assert.IsTrue(vs.IsNullOrEmpty());
			Assert.IsTrue(list.IsNullOrEmpty());
		}

		[TestMethod()]
		public void IsNullOrEmpty_TestEmpty() {
			// Arrange
			List<int> vs = new List<int>();
			List<object> list = new List<object>();
			// Act and assert
			Assert.IsTrue(vs.IsNullOrEmpty());
			Assert.IsTrue(list.IsNullOrEmpty());
		}

		[TestMethod()]
		public void IsNullOrEmpty_TestWith2() {
			// arrange
			List<int> vs = new List<int>() { 1, 2 };
			List<object> list = new List<object>() { "string", 2 };
			//Act and assert
			Assert.IsFalse(vs.IsNullOrEmpty());
			Assert.IsFalse(list.IsNullOrEmpty());
		}

		[TestMethod()]
		public void AddIfNotNull_ThrowsOnNull() {
			// Arrange
			List<int> vs = null;
			// Act and assert
			Assert.ThrowsException<ArgumentNullException>(() => vs.AddIfNotNull(1));
		}

		[TestMethod()]
		public void AddIfNotNull_TestNullity() {
			// Arrange
			List<object> vs = new List<object>() { 1, 2, 3 };
			// Act
			var res = vs.AddIfNotNull(null);
			// Assert
			Assert.IsFalse(res);

			// Act
			res = vs.AddIfNotNull("string");
			// Assert
			Assert.IsTrue(res);
		}

		[TestMethod()]
		public void AddIfNotNull_TestEnforceUnique() {
			// Arrange
			List<int> vs = new List<int>() { 1, 2, 3 };
			// Act
			var res = vs.AddIfNotNull(1, true);
			// Assert
			Assert.IsFalse(res);
			
			//Act
			res = vs.AddIfNotNull(5, true);
			//Assert
			Assert.IsTrue(res);

			// Act
			res = vs.AddIfNotNull(1, false);
			Assert.IsTrue(res);

		}

		private static object[] _testToAdd = new object[] { 1, 2, "String", null };
		[TestMethod()]
		public void AddRangeIfNotNull_ThrowsOnNullThis() {
			// Arrange
			List<object> vs = null;

			//Act and Assert
			Assert.ThrowsException<ArgumentNullException>(() => vs.AddRangeIfNotNull(_testToAdd));
		}

		[TestMethod()]
		public void AddRangeIfNotNull_TestNullity() {
			// Arrange 
			List<object> vs = new List<object>();
			// Act
			var res = vs.AddRangeIfNotNull(_testToAdd);
			// Assert
			Assert.AreEqual(3, res);
		}

		[TestMethod()]
		public void AddRangeIfNotNull_TestEnforceUnique() {
			// Arrange
			List<object> vs = new List<object>();
			// Act
			var res = vs.AddRangeIfNotNull(_testToAdd);
			// Assert
			Assert.AreEqual(3, res);

			// Act
			res = vs.AddRangeIfNotNull(_testToAdd, true);
			// Assert
			Assert.AreEqual(0, res);
		}
	}
}