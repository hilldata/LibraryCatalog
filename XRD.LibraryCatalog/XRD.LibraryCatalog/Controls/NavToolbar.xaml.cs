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
	/// Interaction logic for NavToolbar.xaml
	/// </summary>
	public partial class NavToolbar : ToolBar {
		public NavToolbar() {
			InitializeComponent();
		}

		#region TargetProperty
		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
			"Target",
			typeof(Paginated),
			typeof(NavToolbar),
			new FrameworkPropertyMetadata(new Paginated(), OnTargetChanged));
		public Paginated Target {
			get => (Paginated)GetValue(TargetProperty);
			set => SetValue(TargetProperty, value);
		}
		private static void OnTargetChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
			if (!(dependencyObject is NavToolbar nt))
				return;
			if (e.NewValue is Paginated p)
				nt.Resources["target"] = p;
			else
				nt.Resources["target"] = new Paginated();
			nt._curPageCt = -1;
			nt.Target.Navigated += nt.Paginated_Navigated;
		}

		#endregion
		
		private bool _isFillCmb=false;
		private int _curPageCt;

		private void Paginated_Navigated(object sender, EventArgs e) {
			if(cmbJump != null) {
				_isFillCmb = true;
				if(_curPageCt != Target.TotalPages) {
					cmbJump.Items.Clear();
					_curPageCt = Target.TotalPages;
					for(int i = 1; i<= Target.TotalPages; i++) {
						cmbJump.Items.Add(i);
					}
				}
				try {
					cmbJump.SelectedItem = Target.PageIndex;
				} catch { }
				_isFillCmb = false;
			}
		}

		private async void btnFirst_Click(object sender, RoutedEventArgs e) {
			if (Target != null && Target.CanMovePrevious)
				await Target.MoveFirst();
		}

		private async void btnPrevious_Click(object sender, RoutedEventArgs e) {
			if (Target != null && Target.CanMovePrevious)
				await Target.MovePrevious();
		}

		private async void cmbJump_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (_isFillCmb)
				return;

			if(Target != null && Target.CanSearch)
				await Target.JumpToPage(cmbJump.SelectedIndex + 1);
		}

		private async void btnNext_Click(object sender, RoutedEventArgs e) {
			if (Target != null && Target.CanMoveNext)
				await Target.MoveNext();
		}

		private async void btnLast_Click(object sender, RoutedEventArgs e) {
			if (Target != null && Target.CanMoveNext)
				await Target.MoveLast();
		}
	}
}
