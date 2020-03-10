using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace XRD.LibCat.Models {
	public class ListStringConverter : ValueConverter<List<string>, string> {
		public ListStringConverter(ConverterMappingHints mappingHints = default) :
			base(c => TextConcatenator.Concatenate(c),
				c => new List<string>(TextConcatenator.Split(c)),
				mappingHints) { }
	}

	public class ObservableStringCollectionConverter : ValueConverter<ObservableCollection<string>, string> {
		public ObservableStringCollectionConverter(ConverterMappingHints mappingHints = default) :
			base(c=>TextConcatenator.Concatenate(c),
				c=>new ObservableCollection<string>(TextConcatenator.Split(c)),
				mappingHints) { }
	}
}
