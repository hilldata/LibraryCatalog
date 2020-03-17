﻿using System;
using XRD.LibCat.Models;
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
	///     <MyNamespace:GradeRestrictControl/>
	///
	/// </summary>
	[TemplatePart(Name = "PART_btnReset", Type = typeof(Button))]
	public class GradeRestrictControl : Control {
		static GradeRestrictControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GradeRestrictControl), new FrameworkPropertyMetadata(typeof(GradeRestrictControl)));
		}

		#region MinGradeProperty
		public static readonly DependencyProperty MinGradeProperty = DependencyProperty.Register(
			"MinGrade",
			typeof(GradeLevels),
			typeof(GradeRestrictControl),
			new PropertyMetadata(GradeLevels.NotSet));
		public GradeLevels MinGrade {
			get => (GradeLevels)GetValue(MinGradeProperty);
			set => SetValue(MinGradeProperty, value);
		}
		#endregion

		#region MaxGradeProperty
		public static readonly DependencyProperty MaxGradeProperty = DependencyProperty.Register(
			"MaxGrade",
			typeof(GradeLevels),
			typeof(GradeRestrictControl),
			new PropertyMetadata(GradeLevels.NotSet));

		public GradeLevels MaxGrade {
			get => (GradeLevels)GetValue(MaxGradeProperty);
			set => SetValue(MaxGradeProperty, value);
		}
		#endregion

		#region OrientationProperty
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
			"Orientation",
			typeof(Orientation),
			typeof(GradeRestrictControl),
			new PropertyMetadata(Orientation.Horizontal));
		public Orientation Orientation {
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}
		#endregion

		public GradeRestrictControl() : base() {
			Loaded += GradeRestrictControl_Loaded;
		}

		private void GradeRestrictControl_Loaded(object sender, RoutedEventArgs e) {
			var btnReset = (Button)Template.FindName("PART_btnReset", this);
			if (btnReset != null)
				btnReset.Click += BtnReset_Click;
		}

		private void BtnReset_Click(object sender, RoutedEventArgs e) {
			MinGrade = GradeLevels.NotSet;
			MaxGrade = GradeLevels.NotSet;
		}
	}
}
