using System;
using System.Linq;
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
	/// Interaction logic for CatalogEntryEditor.xaml
	/// </summary>
	public partial class CatalogEntryEditor : UserControl {
		public CatalogEntryEditor() {
			InitializeComponent();
		}

		public static readonly DependencyProperty SelCatalogEntryProperty = DependencyProperty.Register(
			"SelCatalogEntry",
			typeof(Models.CatalogEntry),
			typeof(CatalogEntryEditor),
			new FrameworkPropertyMetadata(new Models.CatalogEntry(true), OnSelCatalogEntryChanged));

		private static void OnSelCatalogEntryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is CatalogEntryEditor editor) {
				editor.Resources["selEntry"] = e.NewValue;
				editor.SelCatalogEntry.PropertyChanged += editor.SelCatalogEntry_PropertyChanged;
			}
		}

		private static void CatalogEntryEditor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			var cont = sender;
		}

		public Models.CatalogEntry SelCatalogEntry {
			get => (Models.CatalogEntry)GetValue(SelCatalogEntryProperty);
			set => SetValue(SelCatalogEntryProperty, value ?? new Models.CatalogEntry(true));
		}

		private Models.CatalogEntry _model =>
			Resources["selEntry"] as Models.CatalogEntry;

		private void SelCatalogEntry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if (sender is Models.CatalogEntry entry) {
				switch (e.PropertyName) {
					case nameof(Models.CatalogEntry.Authors): {
							lvwAuthors.ItemsSource = entry.Authors.OrderBy(a => a.OrdIndex);
							btnMoveAuthUp.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
							btnMoveAuthDown.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
							break;
						}
					case nameof(Models.CatalogEntry.Genres): {
							lvwGenre.ItemsSource = entry.Genres.OrderBy(a => a.Value);
							break;
						}
					case nameof(Models.CatalogEntry.Identifiers): {
							lvwIsbn.ItemsSource = entry.Identifiers.OrderBy(a => a.Value);
							break;
						}
					default: return;
				}
			}
		}

		#region Author Methods
		private void AddAuthor() {
			if (string.IsNullOrWhiteSpace(txtNewAuthName.Text))
				return;
			var item = _model.AddAuthor(txtNewAuthName.Text, txtNewAuthRole.Text);
			txtNewAuthName.Clear();
			txtNewAuthRole.Clear();
			lvwAuthors.SelectedItem = item;
			txtNewAuthName.Focus();
		}

		private Models.Author selAuthor {
			get {
				if (lvwAuthors != null || lvwAuthors.SelectedItem != null) {
					if (lvwAuthors.SelectedItem is Models.Author auth)
						return auth;
				}
				return null;
			}
		}

		private void DelAuthor() {
			var auth = selAuthor;
			if (auth == null)
				return;
			if (MessageBox.Show($"Are you sure that you want to delete author [{auth}]?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				_model.DelAuthor(auth);
		}
		private void MoveAuthorUp() {
			var auth = selAuthor;
			if (auth == null)
				return;
			_model.MoveAuthorUp(auth);
		}

		private void MoveAuthorDown() {
			var auth = selAuthor;
			if (auth == null)
				return;
			_model.MoveAuthorDown(auth);
		}
		#endregion

		private void btnAddAuthor_Click(object sender, RoutedEventArgs e) => AddAuthor();

		private void txtNewAuthName_KeyUp(object sender, KeyEventArgs e) {
			if(e.Key == Key.Enter)
				AddAuthor();
		}

		private void btnDelAuthor_Click(object sender, RoutedEventArgs e) => DelAuthor();

		private void btnMoveAuthUp_Click(object sender, RoutedEventArgs e) => MoveAuthorUp();

		private void btnMoveAuthDown_Click(object sender, RoutedEventArgs e) => MoveAuthorDown();

		private void lvwAuthors_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Delete)
				DelAuthor();
		}

		#region ISBN Methods
		private void AddIsbn() {
			if (string.IsNullOrWhiteSpace(txtNewIsbn.Text))
				return;
			var item = _model.AddIdentifier(txtNewIsbn.Text);
			txtNewIsbn.Clear();
			lvwIsbn.SelectedItem = item;
			txtNewIsbn.Focus();
		}

		private Models.Identifier selIsbn {
			get {
				if (lvwIsbn != null || lvwIsbn.SelectedItem != null) {
					if (lvwIsbn.SelectedItem is Models.Identifier isbn)
						return isbn;
				}
				return null;
			}
		}
		
		private void DelIsbn() {
			var isbn = selIsbn;
			if (isbn == null)
				return;
			if (MessageBox.Show($"Are you sure that you want to delete identifier [{isbn}]?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				_model.DelIdentifier(isbn);
		}
		#endregion

		private void txtNewIsbn_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter)
				AddIsbn();
		}

		private void btnAddIsbn_Click(object sender, RoutedEventArgs e) => AddIsbn();

		private void btnDelISBN_Click(object sender, RoutedEventArgs e) => DelIsbn();

		private void lvwIsbn_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Delete)
				DelIsbn();
		}

		private void txtPageCount_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key.IsKeyErase())
				return;
			else
				e.Handled = !e.Key.IsKeyNumber();
		}

		#region Genre Methods
		private void AddGenre() {
			if (string.IsNullOrWhiteSpace(txtNewGenre.Text))
				return;
			var item = _model.AddGenre(txtNewGenre.Text);
			txtNewGenre.Clear();
			lvwGenre.SelectedItem = item;
			txtNewGenre.Focus();
		}
		private Models.Genre selGenre {
			get {
				if (lvwGenre != null || lvwGenre.SelectedItem != null) {
					if (lvwGenre.SelectedItem is Models.Genre genre)
						return genre;
				}
				return null;
			}
		}
		private void DelGenre() {
			var genre = selGenre;
			if (genre == null)
				return;
			if (MessageBox.Show($"Are you sure that you want to delete genre [{genre}]?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				_model.DelGenre(genre);
		}
		#endregion

		private void txtNewGenre_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter)
				AddGenre();
		}

		private void btnAddGenre_Click(object sender, RoutedEventArgs e) => AddGenre();

		private void btnDelGenre_Click(object sender, RoutedEventArgs e) => DelGenre();

		private void lvwGenre_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Delete)
				DelGenre();
		}
	}
}