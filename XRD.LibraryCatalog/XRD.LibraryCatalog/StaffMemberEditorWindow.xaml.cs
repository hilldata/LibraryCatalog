using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XRD.LibCat.Models;

namespace XRD.LibCat {
	/// <summary>
	/// Interaction logic for StaffMemberEditorWindow.xaml
	/// </summary>
	public partial class StaffMemberEditorWindow : Controls.EntityBoundWindow {
		public StaffMemberEditorWindow() {
			InitializeComponent();
		}

		protected override IQueryable<IEntity> Query =>
			(from sm
			 in _db.StaffMembers
			 where sm.Id == EntityId
			 select sm
			).Include(sm=>sm.Students);

		protected override IEntity CreateNewRecord() =>
			new StaffMember(true);

		protected override bool ValidateRecord() {
			if (!(Entity is StaffMember sm))
				return true;

			var err = sm.Validate();
			if (err.Where(a => a.PropertyName == nameof(sm.First)).Any()) {
				editName.Focus();
				editName.SetFocusFirst();
				return false;
			}else if(err.Where(a=>a.PropertyName == nameof(sm.Last)).Any()) {
				editName.Focus();
				editName.SetFocusLast();
				return false;
			}
			if(!(Entity as StaffMember).Email.IsValidEmail()) {
				txtEmail.Focus();
				return false;
			}
			return true;
		}

		#region Subjects
		private void lbxSubjects_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Delete)
				dropSubject();
		}

		private void addSubject() {
			if (Entity is StaffMember sm) {
				string sub = txtNewSubject.Text;
				if (!string.IsNullOrWhiteSpace(sub)) {
					sub = sub.TrimTo(50);
					if (!sm.Subjects.Contains(sub))
						sm.Subjects.Add(sub);
				}
			}
			txtNewSubject.Clear();
			txtNewSubject.Focus();
		}
		private void dropSubject() { 
			if (!(Entity is StaffMember sm))
				return;
			if (lbxSubjects.SelectedValue is string sub)
				sm.Subjects.Remove(sub);			
		}

		private void txtNewSubject_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Return)
				addSubject();
		}

		private void btnAddSubj_Click(object sender, RoutedEventArgs e) {
			addSubject();
		}

		private void btnRemSubj_Click(object sender, RoutedEventArgs e) {
			dropSubject();
		}
		#endregion

		#region Students
		private async Task refreshAvailable() {
			if (!(Entity is StaffMember sm))
				return;

			tbiStudents.IsEnabled = false;
			var list = await _db.Patrons.Where(p => p.TeacherId == null).OrderBy(p => p.Last).ThenBy(p => p.First).ToListAsync();
			List<Patron> res;
			if (btnFilterAvail.IsChecked ?? false) {
				res = new List<Patron>();
				foreach (var s in list) {
					if (s.Grade != GradeLevels.NotSet) {
						if (sm.GradesTaught.HasFlag(s.Grade))
							res.Add(s);
					}
				}
			} else {
				res = list;
			}
			lbxAvailStudents.ItemsSource = res;
			tbiStudents.IsEnabled = true;
		}
		
		private async Task refreshAssigned() {
			if (Entity is StaffMember sm) {
				if (sm.Students == null) {
					tbiStudents.IsEnabled = false;
					sm.Students = await _db.Patrons.Where(p => p.Teacher == sm).ToListAsync();
					tbiStudents.IsEnabled = true;
				}
				lbxAssignStudents.ItemsSource = sm.Students.ToList();
			}
		}

		private async Task addAllStudents() {
			if (!(Entity is StaffMember sm))
				return;
			if(lbxAvailStudents.Items.Count > 0) {
				string msg = $"Are you sure that you want to assign ALL available students to this teacher [{sm}]?";
				if (MessageBox.Show(msg, "Confirm Assign All", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
					return;

				foreach (var s in lbxAvailStudents.Items) {
					if (s is Patron p) {
						p.Teacher = sm;
						sm.Students.Add(p);
					}
				}
			}
			await refreshAssigned();
			await refreshAvailable();
		}

		private async Task addSelectedStudent() {
			if (!(Entity is StaffMember sm))
				return;
			if (lbxAvailStudents.SelectedValue == null)
				return;
			if (lbxAvailStudents.SelectedValue is Patron p) {
				p.Teacher = sm;
				sm.Students.Add(p);
			}
			await refreshAssigned();
			await refreshAvailable();
		}

		private async Task dropSelectedStudent() {
			if (!(Entity is StaffMember sm))
				return;
			if (lbxAssignStudents.SelectedValue == null)
				return;
			if(lbxAssignStudents.SelectedValue is Patron p) {
				p.Teacher = null;
				sm.Students.Remove(p);
			}
			await refreshAssigned();
			await refreshAvailable();
		}

		private async Task dropAllStudents() {
			if (!(Entity is StaffMember sm))
				return;
			if (sm.Students.IsNullOrEmpty())
				return;
			string msg = $"Are you sure that you want to drop ALL assigned students from this teacher [{sm}]?";
			if (MessageBox.Show(msg, "Confirm Drop All", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
				return;

			foreach (var p in sm.Students) {
				p.Teacher = null;
			}
			sm.Students.Clear();
			await refreshAssigned();
			await refreshAvailable();
		}
		#endregion

		private async void btnRefreshAvail_Click(object sender, RoutedEventArgs e) {
			await refreshAvailable();
		}

		private async void btnFilterAvail_Click(object sender, RoutedEventArgs e) {
			await refreshAvailable();
		}

		private async void lbxAvailStudents_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			await addSelectedStudent();
		}

		private async void lbxAvailStudents_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter || e.Key == Key.Space)
				await addSelectedStudent();
		}

		private async void btnAddAllStudents_Click(object sender, RoutedEventArgs e) {
			await addAllStudents();
		}

		private async void btnAddSelStudent_Click(object sender, RoutedEventArgs e) {
			await addSelectedStudent();
		}

		private async void btnRemSelStudent_Click(object sender, RoutedEventArgs e) {
			await dropSelectedStudent();
		}

		private async void btnRemAllStudents_Click(object sender, RoutedEventArgs e) {
			await dropAllStudents();
		}

		private async void btnRefreshAssignStudents_Click(object sender, RoutedEventArgs e) {
			await refreshAssigned();
		}

		private async void lbxAssignStudents_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			await dropSelectedStudent();
		}

		private async void lbxAssignStudents_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Back || e.Key == Key.Delete)
				await dropSelectedStudent();
		}
	}
}
