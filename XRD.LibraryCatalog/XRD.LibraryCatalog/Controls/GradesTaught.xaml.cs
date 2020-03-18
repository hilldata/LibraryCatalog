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
using XRD.LibCat.Models;

namespace XRD.LibCat.Controls {
	/// <summary>
	/// Interaction logic for GradesTaught.xaml
	/// </summary>
	public partial class GradesTaught : UserControl {
		public GradesTaught() {
			InitializeComponent();

			foreach(var child in pnlMain.Children) {
				if(child is CheckBox chk) {
					chk.Checked += Chk_CheckedChanged;
					chk.Unchecked += Chk_CheckedChanged;
				}
			}
			AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(Chk_CheckedChanged), true);
		}

		private bool _isSettingValue = false;
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
			"Value",
			typeof(GradeLevels),
			typeof(GradesTaught),
			new FrameworkPropertyMetadata(GradeLevels.NotSet, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));
		private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is GradesTaught c)
				c.ApplyValue();
		}
		public GradeLevels Value {
			get => (GradeLevels)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		private void ApplyValue() {
			_isSettingValue = true;
			foreach(var control in pnlMain.Children) {
				if(control is CheckBox chk) {
					GradeLevels v = (GradeLevels)chk.Tag;
					chk.IsChecked = Value.HasFlag(v);
				}
			}
			txtValue.Text = Value.ToString();
			btnClear.IsEnabled = Value != GradeLevels.NotSet;
			_isSettingValue = false;
		}

		private void Chk_CheckedChanged(object sender, RoutedEventArgs e) {
			if (_isSettingValue)
				return;
			if(sender is CheckBox chk) {
				GradeLevels v = (GradeLevels)chk.Tag;
				if(chk.IsChecked??false) {
					Value |= v;
				} else {
					Value &= ~v;
				}
			}
		}

		private void btnClear_Click(object sender, RoutedEventArgs e) {
			Value = GradeLevels.NotSet;
		}
	}
}
