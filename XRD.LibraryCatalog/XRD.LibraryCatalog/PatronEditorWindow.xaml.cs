using System;
using Microsoft.EntityFrameworkCore;
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
	/// Interaction logic for PatronEditorWindow.xaml
	/// </summary>
	public partial class PatronEditorWindow : Controls.EntityBoundWindow {
		public PatronEditorWindow() {
			InitializeComponent();
		}

		protected override IQueryable<IEntity> Query =>
			(from p
			 in _db.Patrons
			 where p.Id == EntityId
			 select p
			 ).Include(p => p.Teacher);

		protected override IEntity CreateNewRecord() => new Patron(true);
		protected override bool ValidateRecord() => throw new NotImplementedException();
	}
}
