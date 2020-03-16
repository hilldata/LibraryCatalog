using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using XRD.LibCat.Models;
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
		private readonly LibraryContext _db = App.DbContext;

		private void ClearSearch() {
			if(lvwLocal != null)
				lvwLocal.ItemsSource = null;
			googleSearch?.ClearSearch();
			txtSearchCriteria?.Clear();

			if (tbiLocal != null)
				tbiLocal.IsSelected=true;
			txtSearchCriteria?.Focus();
		}

		private async Task ApplySearch() {
			if (string.IsNullOrWhiteSpace(txtSearchCriteria.Text))
				return;

			txtSearchCriteria.IsEnabled = false;
			if (cmbSearchField.SelectedIndex == 0) { //Search ISBN
				await googleSearch.Search(GoogleBooksApi.SearchFields.ISBN, txtSearchCriteria.Text.Trim());
				lvwLocal.ItemsSource = await _db.AddCatalogIncludes(_db.QueryBooksByIdentifier(txtSearchCriteria.Text.Trim())).ToListAsync();
			} else {// Search Title
				await googleSearch.Search(GoogleBooksApi.SearchFields.InTitle, txtSearchCriteria.Text.Trim());
				lvwLocal.ItemsSource = await _db.AddCatalogIncludes(_db.QueryBooksByTitle(txtSearchCriteria.Text.Trim())).ToListAsync();
			}
			if (lvwLocal.Items.Count > 0) {
				tbiLocal.IsSelected = true;
				lvwLocal.SelectedItem = lvwLocal.Items[0];
			} else if (googleSearch.Client.Items.Count > 0) {
				tbiGoogle.IsSelected = true;
			} else {
				StartManualEntry();
			}
			txtSearchCriteria.IsEnabled = true;
		}

		private void UpdateLocalHeader() {
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
			StartManualEntry();
		}

		private void StartManualEntry() { 
			tbiManual.IsSelected = true;
			manualEditor.SelCatalogEntry = null;
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
