using System;
using System.Globalization;
using System.Windows.Data;
using System.Collections.Generic;
using System.Text;
using XRD.LibCat.Models;

namespace XRD.LibCat.Converters {
	[ValueConversion(typeof(ICollection<Author>), typeof(bool))]
	public class CanMoveDownConverter : BaseConv, IValueConverter {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">The selected Author</param>
		/// <param name="targetType"></param>
		/// <param name="parameter">The collection of Authors</param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null || parameter == null)
				return false;

			if(value is Author sel) {
				if (sel.Book.Authors.Count < 2)
					return false;
				return sel.OrdIndex < sel.Book.Authors.Count;
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}

	[ValueConversion(typeof(ICollection<Author>), typeof(bool))]
	public class CanMoveUpConverter : BaseConv, IValueConverter {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">The selected Author</param>
		/// <param name="targetType"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return false;
			if (value is Author sel)
				return sel.OrdIndex > 1;
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
