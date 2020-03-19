using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
using XRD.LibCat.Models;
using System.Threading.Tasks;

namespace XRD.LibCat.Controls {
	/// <summary>
	/// Interaction logic for DataWindow.xaml
	/// </summary>
	public abstract partial class DataWindow : Window {
		#region Protected Members
		protected LibraryContext _db = App.DbContext;
		protected bool _isCancel = false;

		protected abstract bool ValidateRecord();
		protected abstract IEntity CreateNewRecord();
		protected abstract IQueryable<IEntity> Query { get; }
		#endregion

		public DataWindow() {
			InitializeComponent();
		}

		public IEntity Entity { get; protected set; }

		#region IsAccessingDbProperty
		public static readonly DependencyProperty IsAccessingDbProperty = DependencyProperty.Register(
			"IsAccessingDb",
			typeof(bool),
			typeof(DataWindow),
			new PropertyMetadata(false));
		public bool IsAccessingDb {
			get => (bool)GetValue(IsAccessingDbProperty);
			set => SetValue(IsAccessingDbProperty, value);
		}
		#endregion

		#region EntityTypeProperty
		public static readonly DependencyProperty EntityTypeProperty = DependencyProperty.Register(
			"EntityType",
			typeof(Type),
			typeof(DataWindow),
			new FrameworkPropertyMetadata(typeof(IEntity), OnEntityTypeChanged));
		private static void OnEntityTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DataWindow w = (DataWindow)d;
			w.updateTitle();
		}
		public Type EntityType {
			get => (Type)GetValue(EntityTypeProperty);
			set => SetValue(EntityTypeProperty, value);
		}
		#endregion

		private void setNewEntity() {
			Entity = CreateNewRecord();
			_db.Add(Entity);
			updateTitle();
		}

		#region EntityIdProperty
		public static readonly DependencyProperty EntityIdProperty = DependencyProperty.Register(
			"EntityId",
			typeof(int?),
			typeof(DataWindow),
			new FrameworkPropertyMetadata(null, OnEntityIdChanged));
		private static async void OnEntityIdChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
			DataWindow window = (DataWindow)dependencyObject;
			if(e.NewValue == null || !(e.NewValue is int nId)) {
				window.setNewEntity();
				return;
			}
			window.IsAccessingDb = true;
			try {
				window.Entity = await window.Query.FirstOrDefaultAsync();
				if (window.Entity == null) {
					string msg = $"No {window.EntityDisplayName} with the specified Id ({window.EntityId}) was found.{Environment.NewLine}" +
						$"Do you want to create a new {window.EntityDisplayName}?";
					if (MessageBox.Show(msg, $"No {window.EntityDisplayName} Found",
						MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
						window.setNewEntity();
					} else {
						window._isCancel = true;
						window.Close();
					}
				}
			} catch (Exception ex) {
				throw ex;
			} finally {
				window.IsAccessingDb = false;
			}
			window.updateTitle();
		}

		public int? EntityId {
			get => (int?)GetValue(EntityIdProperty);
			set => SetValue(EntityIdProperty, value);
		}
		#endregion

		private void updateTitle() {
			if (EntityType == null)
				Title = "(record type not set)";
			else if ((EntityId ?? 0) <= 0) {
				Title = $"Add New {EntityDisplayName}";
			} else {
				Title = $"{EntityDisplayName} [{Entity}]";
			}
			DataContext = Entity;
		}

		private async Task<bool> SaveAsync() {
			if (ValidateRecord()) {
				int i;
				IsAccessingDb = true;
				try {
					i = await _db.SaveChangesAsync();
				} catch (Exception ex) {
					MessageBox.Show(ex.Message, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error); return false;
				} finally {
					IsAccessingDb = false;
				}
				return i > 0;
			} else
				return false;
		}

		protected string EntityDisplayName =>
			EntityType == null
			? "(no type set)"
			: LibraryContext.GetDisplayName(EntityType);

		private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(_isCancel) {
				_isCancel = false;
				return;
			}
			if (_db.ChangeTracker.HasChanges()) {
				string msg = $"There are unsaved changes. Do you want to:{Environment.NewLine}" +
					$"\tSave the changes (\"Yes\"){Environment.NewLine}" +
					$"\tDiscard the changes (\"No\"){Environment.NewLine}" +
					$"\tKeep the window open (\"Cancel\")?";
				switch (MessageBox.Show(msg, "Unsaved Changes Detected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)) {
					case MessageBoxResult.Yes: {
							if (!await SaveAsync())
								e.Cancel = true;
							return;
						}
					case MessageBoxResult.No: { return; }
					default: { e.Cancel = true; return; }
				}
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			if(Entity == null) {
				setNewEntity();
			}
			updateTitle();
		}

		private async void btnSave_Click(object sender, RoutedEventArgs e) =>
			await SaveAsync();

		private async void btnSaveNew_Click(object sender, RoutedEventArgs e) {
			if (!await SaveAsync())
				return;
			EntityId = null;
		}

		private async void btnSaveClose_Click(object sender, RoutedEventArgs e) {
			if (!await SaveAsync())
				return;
			Close();
		}

		private void btnReload_Click(object sender, RoutedEventArgs e) {
			_db = App.DbContext;
			EntityId = EntityId;
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			_isCancel = true;
			Close();
		}
	}
}
