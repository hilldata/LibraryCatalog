using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace XRD.LibCat.Controls {
	public class Mdl2AssetProperty : DependencyObject {
		public static char GetMdl2Icon(DependencyObject obj) =>
			(char)obj.GetValue(Mdl2IconProperty);
		public static void SetMdl2Icon(DependencyObject obj, char value) =>
			obj.SetValue(Mdl2IconProperty, value);

		public static readonly DependencyProperty Mdl2IconProperty = DependencyProperty.Register(
			"Mdl2Icon",
			typeof(char),
			typeof(Mdl2AssetProperty),
			new UIPropertyMetadata((char)0xe700));

		public static Brush GetMdl2Brush(DependencyObject obj) =>
			(Brush)obj.GetValue(Mdl2BrushProperty);
		public static void SetMdl2Brush(DependencyObject obj, Brush value) =>
			obj.SetValue(Mdl2BrushProperty, value);
		public static readonly DependencyProperty Mdl2BrushProperty = DependencyProperty.Register(
			"Mdl2Brush",
			typeof(Brush),
			typeof(Mdl2AssetProperty),
			new UIPropertyMetadata(Brushes.Black));
	}
}
