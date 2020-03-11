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
	///     <MyNamespace:NavigationToolbar/>
	///
	/// </summary>
	[TemplatePart(Name ="PART_btnMoveFirst", Type =typeof(Button))]
	[TemplatePart(Name = "PART_btnMovePrev", Type = typeof(Button))]
	[TemplatePart(Name = "PART_btnMoveNext", Type = typeof(Button))]
	[TemplatePart(Name = "PART_btnMoveLast", Type = typeof(Button))]
	[TemplatePart(Name ="PART_cmbJump", Type =typeof(ComboBox))]
	public class NavigationToolbar : ToolBar {
		static NavigationToolbar() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationToolbar), new FrameworkPropertyMetadata(typeof(NavigationToolbar)));
		}

		public NavigationToolbar() {
			Loaded += NavigationToolbar_Loaded;
		}

		private void NavigationToolbar_Loaded(object sender, RoutedEventArgs e) {
			var btnFirst = (Button)Template.FindName("PART_btnMoveFirst", this);
			if(btnFirst != null)
				btnFirst.Click += BtnFirst_Click;

			var btnPrev = (Button)Template.FindName("PART_btnMovePrev", this);
			if(btnPrev != null)
				btnPrev.Click += BtnPrev_Click;

			var btnNext = (Button)Template.FindName("PART_btnMoveNext", this);
			if(btnNext != null)
				btnNext.Click += BtnNext_Click;

			var btnLast = (Button)Template.FindName("PART_btnMoveLast", this);
			if(btnLast != null)
				btnLast.Click += BtnLast_Click;

			var cmbJump = (ComboBox)Template.FindName("PART_btnJump", this);
			if(cmbJump != null)
				cmbJump.SelectionChanged += CmbJump_SelectionChanged;

			Target.Navigated += Target_Navigated;
		}

		private async void CmbJump_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (_isFillingCombo)
				return;

			if(Target != null && Target.CanSearch) {
				if(sender is ComboBox cmb) {
					await Target.JumpToPage(cmb.SelectedIndex + 1);
				}
			}
		}

		private async void BtnLast_Click(object sender, RoutedEventArgs e) {
			if (Target != null && Target.CanMoveNext)
				await Target.MoveLast();
		}
		private async void BtnNext_Click(object sender, RoutedEventArgs e) {
			if (Target != null && Target.CanMoveNext)
				await Target.MoveNext();
		}
		private async void BtnPrev_Click(object sender, RoutedEventArgs e) {
			if (Target != null && Target.CanMovePrevious)
				await Target.MovePrevious();
		}
		private async void BtnFirst_Click(object sender, RoutedEventArgs e) {
			if (Target != null && Target.CanMovePrevious)
				await Target.MoveFirst();
		}


		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
			"Target",
			typeof(Paginated),
			typeof(NavigationToolbar),
			new FrameworkPropertyMetadata(new Paginated(), OnTargetChanged));

		public Paginated Target {
			get => (Paginated)GetValue(TargetProperty);
			set => SetValue(TargetProperty, value);
		}

		private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (!(d is NavigationToolbar nt))
				return;
			if (e.NewValue is Paginated ip) {
				nt.Resources["target"] = ip;
			} else {
				nt.Resources["target"] = new Paginated();
			}
		}

		private bool _isFillingCombo = false;
		private int _curPagCount = 0;
		private void Target_Navigated(object sender, EventArgs e) {
			var cmb = (ComboBox)Template.FindName("PART_cmbJump", this);
			if(cmb != null) {
				if (_curPagCount == Target.TotalPages)
					return;
				
				_isFillingCombo = true;
				cmb.Items.Clear();
				for(int i = 1; i <= Target.TotalPages; i++) {
					cmb.Items.Add(i);
				}
				try {
					cmb.SelectedItem = Target.PageIndex;
				} catch { }
				_isFillingCombo = false;
			}
		}
	}
}
