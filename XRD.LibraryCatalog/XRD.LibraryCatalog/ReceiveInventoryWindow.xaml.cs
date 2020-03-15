using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XRD.LibCat {
	/// <summary>
	/// Interaction logic for ReceiveInventoryWindow.xaml
	/// </summary>
	public partial class ReceiveInventoryWindow : Window {
		public ReceiveInventoryWindow() {
			InitializeComponent();
		}

		private void ClearSearch() {
			if(lvwLocal != null)
				lvwLocal.ItemsSource = null;
			googleSearch?.ClearSearch();
			txtSearchCriteria?.Clear();
			txtSearchCriteria?.Focus();
		}

		private async Task ApplySearch() {
			if (string.IsNullOrWhiteSpace(txtSearchCriteria.Text))
				return;

			txtSearchCriteria.IsEnabled = false;
			if(cmbSearchField.SelectedIndex == 0) { //Search ISBN
				await googleSearch.Search(GoogleBooksApi.SearchFields.ISBN, txtSearchCriteria.Text.Trim());
			}
			txtSearchCriteria.IsEnabled = true;
		}

		private async void txtSearchCriteria_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter)
				await ApplySearch();
		}

		private void cmbSearchField_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			ClearSearch();
		}

		private void btnClearSearch_Click(object sender, RoutedEventArgs e) {
			ClearSearch();
		}

		private async void btnSearch_Click(object sender, RoutedEventArgs e) {
			await ApplySearch();
		}

		private void btnAddManually_Click(object sender, RoutedEventArgs e) {
			tbiManual.IsSelected = true;
		}

		private async Task AddBookToInventory() {

		}

		private async void btnAddCopy_Click(object sender, RoutedEventArgs e) {
			await AddBookToInventory();
		}

		private void txtBookNumber_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key.IsKeyErase() || e.Key.IsKeyNumber())
				return;
			e.Handled = true;
		}

		private async void txtBookNumber_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Return)
				await AddBookToInventory();
		}

		private void txtBox_GotFocus(object sender, RoutedEventArgs e) {
			if (sender is TextBox text)
				text.SelectAll();
		}
	}
}
