using System;
using System.Windows.Markup;

namespace XRD.LibCat.Converters {
	public abstract class BaseConv : MarkupExtension {
		public override object ProvideValue(IServiceProvider serviceProvider) => this;
	}
}
