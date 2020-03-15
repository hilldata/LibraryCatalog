using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace XRD.LibCat.Controls {
	/// <summary>
	/// Class that provides the CueBanner (watermark) attached property
	/// </summary>
	public class CueBannerService {
		/// <summary>
		/// CueBanner (watermark) attached dependency property.
		/// </summary>
		public static readonly DependencyProperty CueBannerProperty = DependencyProperty.RegisterAttached(
			"CueBanner",
			typeof(object),
			typeof(CueBannerService),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCueBannerChanged)));

		private static readonly Dictionary<object, ItemsControl> itemsControls = new Dictionary<object, ItemsControl>();

		/// <summary>
		/// Gets the CueBanner (watermark) property.
		/// </summary>
		/// <param name="d">The object to get the property from.</param>
		/// <returns>The value of the CueBanner property.</returns>
		public static object GetCueBanner(DependencyObject d) =>
			d.GetValue(CueBannerProperty);

		/// <summary>
		/// Sets the CueBanner (watermark) property.
		/// </summary>
		/// <param name="d">The object to set the property on.</param>
		/// <param name="value">The value of the CueBanner</param>
		public static void SetCueBanner(DependencyObject d, object value) =>
			d.SetValue(CueBannerProperty, value);

		private static void OnCueBannerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Control control = (Control)d;
			control.Loaded += Control_Loaded;

			if (d is ComboBox || d is TextBox) {
				control.GotKeyboardFocus += Control_GotKeyboardFocus;
				control.LostKeyboardFocus += Control_LostKeyboardFocus;
			}
			if (d is ItemsControl && !(d is ComboBox)) {
				ItemsControl i = (ItemsControl)d;

				// for Items Property
				i.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
				itemsControls.Add(i.ItemContainerGenerator, i);

				// for ItemsSource property
				DependencyPropertyDescriptor prop = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, i.GetType());
				prop.AddValueChanged(i, ItemsSourceChanged);
			}
		}

		private static void ItemsSourceChanged(object sender, EventArgs e) {
			ItemsControl c = (ItemsControl)sender;
			if (c.ItemsSource != null) {
				if (ShouldShowCueBanner(c))
					ShowCueBanner(c);
				else
					RemoveCueBanner(c);
			} else {
				ShowCueBanner(c);
			}
		}

		private static void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			if (itemsControls.TryGetValue(sender, out ItemsControl control)) {
				if (ShouldShowCueBanner(control))
					ShowCueBanner(control);
				else
					RemoveCueBanner(control);
			}
		}
		private static void Control_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e) {
			Control c = (Control)sender;
			if (ShouldShowCueBanner(c))
				ShowCueBanner(c);
		}
		private static void Control_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e) {
			Control c = (Control)sender;
			if (ShouldShowCueBanner(c))
				RemoveCueBanner(c);
		}

		private static void Control_Loaded(object sender, RoutedEventArgs e) {
			Control control = (Control)sender;
			if (ShouldShowCueBanner(control))
				ShowCueBanner(control);
		}

		#region Helper Methods
		private static void RemoveCueBanner(UIElement control) {
			AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);

			//Layer could be null if element is no longer in the visual tree
			if (layer != null) {
				Adorner[] adorners = layer.GetAdorners(control);
				if (adorners == null)
					return;

				foreach (Adorner a in adorners) {
					if (a is CueBannerAdorner) {
						a.Visibility = Visibility.Hidden;
						layer.Remove(a);
					}
				}
			}
		}

		private static void ShowCueBanner(Control control) {
			AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);
			// Layer could be null if element is no longer in the visual tree
			if (layer != null)
				layer.Add(new CueBannerAdorner(control, GetCueBanner(control)));
		}

		private static bool ShouldShowCueBanner(Control control) {
			if (control is ComboBox)
				return string.IsNullOrWhiteSpace((control as ComboBox).Text);
			else if (control is TextBoxBase)
				return string.IsNullOrWhiteSpace((control as TextBox).Text);
			else if (control is ItemsControl)
				return (control as ItemsControl).Items.Count < 1;
			return false;
		}
		#endregion
	}
}