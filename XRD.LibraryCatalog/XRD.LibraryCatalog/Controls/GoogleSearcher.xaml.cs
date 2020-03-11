using System;
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

		public ApiClient apiClient => (ApiClient)Resources["apiClient"];

		private void tglExactMatch_Checked(object sender, RoutedEventArgs e) {
			if (apiClient == null || apiClient.ExactIdMatch == null)
				return;

			if (tglExactMatch.IsChecked ?? false)
				lvw.ItemsSource = apiClient.ExactIdMatch;
			else
				lvw.ItemsSource = apiClient.Items;

		}
	}
}
