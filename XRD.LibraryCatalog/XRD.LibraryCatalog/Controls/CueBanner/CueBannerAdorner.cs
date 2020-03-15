using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace XRD.LibCat.Controls {
	/// <summary>
	/// Adorner for adding a Cue Banner (watermark) to controls
	/// </summary>
	public class CueBannerAdorner : Adorner {
		#region Fields
		private readonly ContentPresenter contentPresenter;
		#endregion

		public CueBannerAdorner(UIElement adornedElement, object watermark) : base(adornedElement) {
			IsHitTestVisible = false;

			contentPresenter = new ContentPresenter() {
				Content = watermark,
				Opacity = 0.5,
				Margin = new Thickness(Control.Margin.Left + Control.Padding.Left,
					Control.Margin.Top + Control.Padding.Top,
					0,
					0)
			};

			if (Control is ItemsControl && !(Control is ComboBox)) {
				contentPresenter.VerticalAlignment = VerticalAlignment.Center;
				contentPresenter.HorizontalAlignment = HorizontalAlignment.Center;
			}

			// Hide the control adorner when the adorned element is hidden
			Binding binding = new Binding("IsVisible") {
				Source = adornedElement,
				Converter = new Converters.BoolToVisibilityConverter()
			};
			SetBinding(VisibilityProperty, binding);
		}

		private Control Control => (Control)AdornedElement;
		protected override int VisualChildrenCount => 1;
		protected override Visual GetVisualChild(int index) => contentPresenter;
		protected override Size MeasureOverride(Size constraint) {
			// Here's the secret to getting the adorner to cover the whole control
			contentPresenter.Measure(Control.RenderSize);
			return Control.RenderSize;
		}

		protected override Size ArrangeOverride(Size finalSize) {
			contentPresenter.Arrange(new Rect(finalSize));
			return finalSize;
		}
	}
}