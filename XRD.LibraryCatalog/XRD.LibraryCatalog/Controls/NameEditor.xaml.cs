using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace XRD.LibCat.Controls {
	/// <summary>
	/// Interaction logic for NameEditor.xaml
	/// </summary>
	public partial class NameEditor : UserControl {
		public NameEditor() {
			InitializeComponent();
		}

		public void SetFocusFirst() => txtFirst.Focus();
		public void SetFocusLast() => txtLast.Focus();
		public void SetFocusPrefix() => cmbPrefix.Focus();

		private void UserControl_Loaded(object sender, RoutedEventArgs e) {
			cmbPrefix.ItemsSource = Honorifics.GetAll();
			cmbSuffix.ItemsSource = CommonSuffices;
		}
		public static List<string> CommonSuffices =>
			new List<string>() {
				"Jr.",
				"Sr.",
				"MD",
				"DO",
				"I",
				"II",
				"III",
				"IV"
			};
	}
}
