using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
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
using XRD.LibCat.GoogleBooksApi;

namespace XRD.LibCat.Controls {
	/// <summary>
	/// Interaction logic for GoogleSearcher.xaml
	/// </summary>
	public partial class GoogleSearcher : UserControl {
		public GoogleSearcher() {
			InitializeComponent();
		}

		public void ClearSearch() {
			Client.Clear();
		}

		public static readonly DependencyProperty IsHeaderVisibleProperty = DependencyProperty.Register(
			"IsHeaderVisible",
			typeof(bool),
			typeof(GoogleSearcher),
			new FrameworkPropertyMetadata(true, IsHeaderVisibilityChanged));
		public bool IsHeaderVisible {
			get => (bool)GetValue(IsHeaderVisibleProperty);
			set => SetValue(IsHeaderVisibleProperty, value);
		}
		private static void IsHeaderVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is GoogleSearcher me) {
				if (!(bool)e.NewValue)
					me.lblHeader.Visibility = Visibility.Collapsed;
				else
					me.lblHeader.Visibility = Visibility.Visible;
			}
		}

		public async Task Search(SearchFields field, string criteria, int pageSize = 10) =>
			await Client.SearchAsync(field, criteria, pageSize);

		public ApiClient Client => (ApiClient)Resources["apiClient"];

		private System.Windows.Controls.Primitives.ToggleButton tglExactMatch =>
			navTb.Items.SourceCollection.OfType<System.Windows.Controls.Primitives.ToggleButton>().First();
		private void tglExactMatch_Checked(object sender, RoutedEventArgs e) {
			if (Client == null || _navigating)
				return;

			if (tglExactMatch.IsChecked ?? false)
				lvw.ItemsSource = Client.ExactIdMatch;
			else
				lvw.ItemsSource = Client.Items;
		}

		private bool _navigating = false;
		private void ApiClient_Navigated(object sender, EventArgs e) {
			if(sender is ApiClient client) {
				_navigating = true;
				if (!client.ExactIdMatch.IsNullOrEmpty()) {
					tglExactMatch.IsChecked = true;
					lvw.ItemsSource = client.ExactIdMatch;
				} else {
					tglExactMatch.IsChecked = false;
					lvw.ItemsSource = client.Items;
				}
				_navigating = false;
			}
		}
	}
}
