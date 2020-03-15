﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XRD.LibCat.Controls {
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:XRD.LibCat.Controls"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:XRD.LibCat.Controls;assembly=XRD.LibCat.Controls"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Browse to and select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file.
	///
	///     <MyNamespace:AgeRestrictControl/>
	///
	/// </summary>
	public class AgeRestrictControl : Control {
		static AgeRestrictControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AgeRestrictControl), new FrameworkPropertyMetadata(typeof(AgeRestrictControl)));
		}

		#region MinAgeProperty
		public static readonly DependencyProperty MinAgeProperty = DependencyProperty.Register(
			"MinAge",
			typeof(int?),
			typeof(AgeRestrictControl),
			new PropertyMetadata(null));
		public int? MinAge {
			get => (int?)GetValue(MinAgeProperty);
			set => SetValue(MinAgeProperty, value);
		}
		#endregion

		#region MaxAgeProperty
		public static readonly DependencyProperty MaxAgeProperty = DependencyProperty.Register(
			"MaxAge",
			typeof(int?),
			typeof(AgeRestrictControl),
			new PropertyMetadata(null));

		public int? MaxAge {
			get => (int?)GetValue(MaxAgeProperty);
			set => SetValue(MaxAgeProperty, value);
		}
		#endregion

		#region OrientationProperty
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
			"Orientation",
			typeof(Orientation),
			typeof(AgeRestrictControl),
			new PropertyMetadata(Orientation.Horizontal));
		public Orientation Orientation {
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}
		#endregion
	}
}
