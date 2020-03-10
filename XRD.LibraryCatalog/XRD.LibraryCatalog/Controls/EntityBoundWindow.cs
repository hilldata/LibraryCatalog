using System;
using Microsoft.EntityFrameworkCore;
using XRD.LibCat.Models;
using System.Linq;
using System.Windows.Controls.Primitives;
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
	///     <MyNamespace:EntityBoundWindow/>
	///
	/// </summary>
	[TemplatePart(Name = SAVE_BUTTON, Type = typeof(Button))]
	[TemplatePart(Name = SAVE_NEW_BUTTON, Type = typeof(Button))]
	[TemplatePart(Name = SAVE_CLOSE_BUTTON, Type = typeof(Button))]
	[TemplatePart(Name = CANCEL_BUTTON, Type = typeof(Button))]
	[TemplatePart(Name = ISDEL_SBAR_ITEM, Type = typeof(StatusBarItem))]
	[TemplatePart(Name = DELETE_TOGGLE_BUTTON, Type = typeof(ToggleButton))]
	[TemplatePart(Name = LASTSAVE_SBAR_ITEM, Type = typeof(StatusBarItem))]
	[TemplatePart(Name = EDITCOUNT_SBAR_ITEM, Type = typeof(StatusBarItem))]
	[TemplatePart(Name =CONTENT, Type =typeof(ContentPresenter))]
	[TemplatePart(Name =PROGRESS_BAR, Type =typeof(ProgressBar))]
	public abstract class EntityBoundWindow : Window {
		#region Protected Members
		protected readonly LibraryContext _db = App.DbContext;
		protected bool _isCancel = false;

		protected abstract bool ValidateRecord();
		protected abstract IEntity CreateNewRecord();
		protected abstract IQueryable<IEntity> Query { get; }
		#endregion

		#region Constants
		public const string SAVE_BUTTON = "PART_btnSave";
		public const string SAVE_NEW_BUTTON = "PART_btnSaveNew";
		public const string SAVE_CLOSE_BUTTON = "PART_btnSaveClose";
		public const string CANCEL_BUTTON = "PART_btnCancel";
		public const string ISDEL_SBAR_ITEM = "PART_sbIsDeleted";
		public const string LASTSAVE_SBAR_ITEM = "PART_sbTs";
		public const string EDITCOUNT_SBAR_ITEM = "PART_sbEc";
		public const string DELETE_TOGGLE_BUTTON = "PART_tglDeleted";
		public const string CONTENT = "PART_content";
		public const string PROGRESS_BAR = "PART_pgrBar";
		#endregion

		#region WindowTitleProperty
		public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.Register(
			"WindowTitle",
			typeof(string),
			typeof(EntityBoundWindow),
			new PropertyMetadata("SomeWindow"));
		public string WindowTitle {
			get => (string)GetValue(WindowStyleProperty);
			private set => SetValue(WindowTitleProperty, value);
		}
		#endregion

		#region EntityTypeProperty 
		public static readonly DependencyProperty EntityTypeProperty = DependencyProperty.Register(
			"EntityType",
			typeof(Type),
			typeof(EntityBoundWindow),
			new FrameworkPropertyMetadata(typeof(IEntity), OnEntityTypeChanged));
		public Type EntityType {
			get => (Type)GetValue(EntityTypeProperty);
			set => SetValue(EntityTypeProperty, value);
		}
		private static void OnEntityTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityBoundWindow wind = (EntityBoundWindow)d;
			wind.updateWindowTitle();
		}
		#endregion

		#region EntityIdProperty 
		public static readonly DependencyProperty EntityIdProperty = DependencyProperty.Register(
			"EntityId",
			typeof(int?),
			typeof(EntityBoundWindow),
			new FrameworkPropertyMetadata(typeof(IEntity), OnEntityIdChanged));
		public int? EntityId {
			get => (int?)GetValue(EntityIdProperty);
			set => SetValue(EntityIdProperty, value);
		}
		private static async void OnEntityIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityBoundWindow wind = (EntityBoundWindow)d;
			if(wind.EntityId.HasValue) {
				wind.setDbAccessUI(true);
				try {
					wind.Entity = await wind.Query.FirstOrDefaultAsync();
					if(wind.Entity == null) {
						string msg = $"No {wind.entityDisplayName} with the specified Id ({wind.EntityId}) was found.{Environment.NewLine}" +
							$"Do you want to create a new {wind.entityDisplayName}?";
						if (MessageBox.Show(msg, $"No {wind.entityDisplayName} Found", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
							wind.Entity = wind.CreateNewRecord();
						}else {
							wind._isCancel = true;
							wind.Close();
						}
					}
				}catch(Exception ex) {
					throw ex;
				}finally {
					wind.setDbAccessUI(false);
				}
			}else {
				wind.Entity = wind.CreateNewRecord();
			}
			wind.updateWindowTitle();
		}
		#endregion

		#region Constructors
		static EntityBoundWindow() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EntityBoundWindow), new FrameworkPropertyMetadata(typeof(EntityBoundWindow)));
		}

		public EntityBoundWindow() : base() {
			Closing += EntityBoundWindow_Closing;
			Loaded += EntityBoundWindow_Loaded;
		}
		#endregion

		private void updateWindowTitle() {
			if (EntityType == null) {
				WindowTitle = "(record type not set)";
				return;
			} else if (Entity == null || EntityId == null) {
				Entity = CreateNewRecord();
				WindowTitle = $"Add New {entityDisplayName}";
			} else {
				WindowTitle = $"{entityDisplayName} [{Entity}]";
			}
		}

		private void setDbAccessUI(bool isAccessing = false) {
			var prg = (ProgressBar)Template.FindName(PROGRESS_BAR, this);
			var content = (ContentPresenter)Template.FindName(CONTENT, this);

			if(isAccessing) {
				if(prg != null)
					prg.Visibility = Visibility.Visible;
				if (content != null)
					content.IsEnabled = false;
			}else {
				if (prg != null)
					prg.Visibility = Visibility.Collapsed;
				if (content != null)
					content.IsEnabled = true;
			}
		}
		private string entityDisplayName =>
			EntityType == null
			? "(no type set)"
			: _db.GetDisplayName(EntityType);

		private async System.Threading.Tasks.Task SaveAsync() {
			if (ValidateRecord()) {
				setDbAccessUI(true);
				try {
					await _db.SaveChangesAsync();
				} catch (Exception ex) {
					throw ex;
				} finally {
					setDbAccessUI(false);
				}
			}
		}

		public IEntity Entity { get; protected set; }

		#region Event Handlers
		private async void EntityBoundWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (_isCancel) {
				_isCancel = false;
				return;
			}

			if(_db.ChangeTracker.HasChanges()) {
				string msg = $"There are unsaved changes. Do you want to:{Environment.NewLine}" +
					$"\tSave the changes (\"Yes\"){Environment.NewLine}" +
					$"\tDiscard the changes (\"No\"){Environment.NewLine}" +
					$"\tKeep the window open (\"Cancel\")?";
				switch (MessageBox.Show(msg, "Unsaved Changes Detected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)) {
					case MessageBoxResult.Yes: {
							if (!ValidateRecord()) {
								e.Cancel = true;
							} else {
								await SaveAsync();
							}
							return;
						}
					case MessageBoxResult.No: { return; }
					default: { e.Cancel = true; return; }
				}
			}
		}

		private void EntityBoundWindow_Loaded(object sender, RoutedEventArgs e) {
			var btnSave = (Button)Template.FindName(SAVE_BUTTON, this);
			if (btnSave != null)
				btnSave.Click += BtnSave_Click;

			var btnSaveNew = (Button)Template.FindName(SAVE_NEW_BUTTON, this);
			if (btnSaveNew != null)
				btnSaveNew.Click += BtnSaveNew_Click;

			var btnSaveClose = (Button)Template.FindName(SAVE_CLOSE_BUTTON, this);
			if (btnSaveClose != null)
				btnSaveClose.Click += BtnSaveClose_Click;

			var btnCancel = (Button)Template.FindName(CANCEL_BUTTON, this);
			if (btnCancel != null)
				btnCancel.Click += BtnCancel_Click;

			var sbIsDel = (StatusBarItem)Template.FindName(ISDEL_SBAR_ITEM, this);
			if (sbIsDel != null) {
				if (typeof(ISoftDeleted).IsAssignableFrom(Entity.GetType()))
					sbIsDel.Visibility = Visibility.Visible;
				else
					sbIsDel.Visibility = Visibility.Collapsed;
			}

			var sbTs = (StatusBarItem)Template.FindName(LASTSAVE_SBAR_ITEM, this);
			var sbEc = (StatusBarItem)Template.FindName(EDITCOUNT_SBAR_ITEM, this);
			if (typeof(Models.Abstract.ModifiableEntity).IsAssignableFrom(Entity.GetType())) {
				if (sbTs != null)
					sbTs.Visibility = Visibility.Visible;
				if (sbEc != null)
					sbEc.Visibility = Visibility.Visible;
			} else {
				if (sbTs != null)
					sbTs.Visibility = Visibility.Collapsed;
				if (sbEc != null)
					sbEc.Visibility = Visibility.Visible;
			}
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e) {
			_isCancel = true;
			if(Entity != null) {
				if (Entity.Id <= 0)
					_db.Remove(Entity);
			}else {
				_db.ChangeTracker.AcceptAllChanges();
			}
			Close();
		}

		private async void BtnSaveClose_Click(object sender, RoutedEventArgs e) {
			await SaveAsync();
			Close();
		}
		private async void BtnSaveNew_Click(object sender, RoutedEventArgs e) {
			await SaveAsync();
			EntityId = null;
		}

		private async void BtnSave_Click(object sender, RoutedEventArgs e) => await SaveAsync();
		#endregion
	}
}
