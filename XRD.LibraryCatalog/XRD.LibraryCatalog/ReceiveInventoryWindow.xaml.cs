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

			if (tbiLocal != null) {
				tbiLocal.IsSelected = true;
				tbiManual.IsEnabled = false;
			}
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
				lvwLocal.SelectedValue = lvwLocal.Items[0];
				lvwLocal.Focus();
			} else if (googleSearch.Client.Items.Count > 0) {
				tbiGoogle.IsSelected = true;
				googleSearch.Focus();
			} else {
				StartManualEntry();
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
			StartManualEntry();
		}

		private void StartManualEntry() {
			tbiManual.IsEnabled = true;
			tbiManual.IsSelected = true;
			manualEditor.SelCatalogEntry = null;
			manualEditor.Focus();
		}

		private async Task<int?> validateBookNumber() {
			TextBox txt = txtBookNumber;
			Tuple<string, string> msg = null;
			if (!int.TryParse(txt.Text, out int val)) {
				msg = new Tuple<string, string>(
					$"The value provided for the \"New Copy ID\" [{txt.Text}] is not a valid number.",
					"Invalid \"New Copy ID\"");
			} else {
				var qry = from ob
						  in _db.OwnedBooks
						  where ob.BookNumber == val
						  select ob;
				if(await qry.AnyAsync()) {
					msg = new Tuple<string, string>(
						$"The value provided for the \"New Copy ID\" [{val}] is already in use.",
						"Duplicate \"Copy ID\"");
				}
			}
			if (msg == null)
				return val;
			else {
				MessageBox.Show(msg.Item1, msg.Item2, MessageBoxButton.OK, MessageBoxImage.Warning);
				return null;
			}
		}

		private bool validateAgeRestrictions() {
			int? min = ageRestrict.MinAge;
			int? max = ageRestrict.MaxAge;
			if (ageHasValue(min) && ageHasValue(max)) {
				if (min > max) {
					MessageBox.Show($"The Minimum [{min}] is greater than the Maximum [{max}] Age restriction. Please correct.",
						"Invalid Age Restrictions", MessageBoxButton.OK, MessageBoxImage.Warning);
					ageRestrict.Focus();
					return false;
				}
			}
			return true;
		}

		private bool ageHasValue(int? v) => v.HasValue && v.Value > 0;

		private bool validateGradeRestrictions() {
			GradeLevels min = gradeRestrict.MinGrade;
			GradeLevels max = gradeRestrict.MaxGrade;
			if (gradeHasValue(min) && gradeHasValue(max)) {
				if (min > max) {
					MessageBox.Show($"The Minimum [{min}] is greater than the Maximum [{max}] Grade-Level restriction. Please correct.",
						"Invalid Grade-Level Restrictions", MessageBoxButton.OK, MessageBoxImage.Warning);
					gradeRestrict.Focus();
					return false;
				}
			}
			return true;
		}
		
		private bool gradeHasValue(GradeLevels v) => v > GradeLevels.NotSet;

		private async Task AddBookToInventory() {
			int? bookNum = await validateBookNumber();
			if(!bookNum.HasValue) {
				txtBookNumber.SelectAll();
				txtBookNumber.Focus();
				return;
			}
			if (!validateAgeRestrictions())
				return;
			if (!validateGradeRestrictions())
				return;

			CatalogEntry entry;
			if (tbiLocal.IsSelected) {
				if (lvwLocal.SelectedValue == null || !(lvwLocal.SelectedValue is CatalogEntry catalogEntry))
					return;
				entry = catalogEntry;
			} else if (tbiGoogle.IsSelected) {
				if (googleSearch.SelectedVolume == null)
					return;
				entry = CreateEntryFromGoogle(googleSearch.SelectedVolume);
			} else if (tbiManual.IsSelected) {
				if (!manualEditor.Validate()) {
					manualEditor.Focus();
					return;
				}
				entry = manualEditor.SelCatalogEntry;
			} else
				return;
			if (!tbiLocal.IsSelected)
				_db.Add(entry);
			entry.MaxAge = ageRestrict.MaxAge;
			entry.MinAge = ageRestrict.MinAge;
			entry.MinGrade = gradeRestrict.MinGrade;
			entry.MaxGrade = gradeRestrict.MaxGrade;
			entry.ShelfLocation = txtShelf.Text.Trim();
			entry.AddOwnedBook(bookNum.Value);
			await _db.SaveChangesAsync();
			await updateMaxBookNumber();
			txtBookNumber.Clear();
			ClearSearch();
		}

		private CatalogEntry CreateEntryFromGoogle(GoogleBooksApi.VolumeInfo source) {
			List<string> ids = new List<string>();
			if(!source.IndustryIdentifiers.IsNullOrEmpty()) {
				foreach (var i in source.IndustryIdentifiers)
					ids.Add(i.Identifier);
			}
			List<string> genres = new List<string>();
			if(!source.Categories.IsNullOrEmpty()) {
				foreach (var g in source.Categories)
					genres.Add(g);
			}
			CatalogEntry res = new CatalogEntry(
				source.Title,
				source.Subtitle,
				ids,
				source.Publisher,
				source.PublishedDateAsString,
				source.PageCount,
				source.Description,
				txtShelf.Text,
				ageRestrict.MinAge,
				ageRestrict.MaxAge,
				gradeRestrict.MinGrade,
				gradeRestrict.MaxGrade,
				genres.ToArray()
			);
			if(!source.Authors.IsNullOrEmpty()) {
				foreach (var a in source.Authors)
					res.AddAuthor(a);
			}
			return res;
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

		private void lvwLocal_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (lvwLocal.SelectedValue == null)
				return;
			if(lvwLocal.SelectedValue is CatalogEntry cat) {
				if(ageHasValue(cat.MaxAge))
					ageRestrict.MaxAge = cat.MaxAge;
				if (ageHasValue(cat.MinAge))
					ageRestrict.MinAge = cat.MinAge;
				if (gradeHasValue(cat.MaxGrade))
					gradeRestrict.MaxGrade = cat.MaxGrade;
				if (gradeHasValue(cat.MinGrade))
					gradeRestrict.MinGrade = cat.MinGrade;
			}
		}

		private void btnClearShelf_Click(object sender, RoutedEventArgs e) {
			txtShelf.Clear();
		}

		private async Task updateMaxBookNumber() {
			var qry = from ob
					  in _db.OwnedBooks
					  select ob.BookNumber;
			if (!await qry.AnyAsync())
				txtMaxBookNum.Text = "n/a";
			else
				txtMaxBookNum.Text = (await qry.OrderByDescending(a => a).FirstOrDefaultAsync()).ToString();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e) {
			await updateMaxBookNumber();
		}
	}
}
