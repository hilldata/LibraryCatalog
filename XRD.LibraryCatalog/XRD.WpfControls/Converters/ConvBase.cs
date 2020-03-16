using System;
using System.Windows.Markup;

namespace XRD.Wpf.Converters {
	public abstract class ConvBase : MarkupExtension{
		public override object ProvideValue(IServiceProvider serviceProvider) => this;
	}
}
