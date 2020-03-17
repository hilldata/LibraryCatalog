using System;
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
	///     <MyNamespace:AgeControl/>
	///
	/// </summary>
	public class AgeControl : Control {
		static AgeControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AgeControl), new FrameworkPropertyMetadata(typeof(AgeControl)));
		}

		#region Label Property
		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
			"Label",
			typeof(string),
			typeof(AgeControl),
			new PropertyMetadata("Age:"));
		public string Label {
			get => (string)GetValue(LabelProperty);
			set => SetValue(LabelProperty, value.Trim());
		}
		#endregion

		#region Value Property
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
			"Value",
			typeof(int?),
			typeof(AgeControl),
			new PropertyMetadata(0));
		public int? Value {
			get {
				var v = (int)GetValue(ValueProperty);
				if (v == 0)
					return null;
				else
					return v;
			}
			set {
				if (value.HasValue) {
					if (value < 2)
						value = 0;
					else if (value > 14)
						value = 14;
				} else
					value = 0;
				SetValue(ValueProperty, value);
			}
		}
		#endregion
	}
}
