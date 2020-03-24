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
	[TemplatePart(Name = CONTENT, Type = typeof(ContentPresenter))]
	[TemplatePart(Name = RELOAD_BUTTON, Type =typeof(Button))]
	public abstract class EntityBoundWindow : Window {
		#region Protected Members
		protected LibraryContext _db = App.DbContext;
		protected bool _isCancel = false;

		protected abstract bool ValidateRecord();
		protected abstract IEntity CreateNewRecord();
		protected abstract IQueryable<IEntity> Query { get; }
		protected virtual void OnEntityChanged() {
			var sb = (StatusBar)Template.FindName("PART_sbar", this);
			if (sb != null)
				sb.DataContext = Entity;
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

		#region Constants
		public const string SAVE_BUTTON = "PART_btnSave";
		public const string SAVE_NEW_BUTTON = "PART_btnSaveNew";
		public const string SAVE_CLOSE_BUTTON = "PART_btnSaveClose";
		public const string CANCEL_BUTTON = "PART_btnCancel";
		public const string RELOAD_BUTTON = "PART_btnReload";
		public const string CONTENT = "PART_content";
		#endregion

		public IEntity Entity { get; private set; }

		#region WindowIconCharProperty 
		public static readonly DependencyProperty WindowIconCharProperty = DependencyProperty.Register(
			"WindowIconChar",
			typeof(char?),
			typeof(EntityBoundWindow),
			new FrameworkPropertyMetadata(null, onWindowIconCharChanged));
		private static void onWindowIconCharChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityBoundWindow wind = (EntityBoundWindow)d;
			if (e.NewValue == null || !(e.NewValue is char c))
				return;
			var bmp = DrawingHelper.CreateBitmapSource(DrawingHelper.DrawTextToFixedBitmap(c.ToString()));
			wind.Icon = BitmapFrame.Create(bmp);
		}
		public string WindowIconChar {
			get => (string)GetValue(WindowIconCharProperty);
			set => SetValue(WindowIconCharProperty, value);
		}
		#endregion

		#region IsAccessingDbProperty
		public static readonly DependencyProperty IsAccessingDbProperty = DependencyProperty.Register(
			"IsAccessingDb",
			typeof(bool),
			typeof(EntityBoundWindow),
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
			typeof(EntityBoundWindow),
			new FrameworkPropertyMetadata(typeof(IEntity), OnEntityTypeChanged));
		public Type EntityType {
			get => (Type)GetValue(EntityTypeProperty);
			set => SetValue(EntityTypeProperty, value);
		}
		private static void OnEntityTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityBoundWindow wind = (EntityBoundWindow)d;
			wind.updateTitle();
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
			typeof(EntityBoundWindow),
			new FrameworkPropertyMetadata(null, OnEntityIdChanged));
		public int? EntityId {
			get => (int?)GetValue(EntityIdProperty);
			set => SetValue(EntityIdProperty, value);
		}
		private static async void OnEntityIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityBoundWindow wind = (EntityBoundWindow)d;
			if(e.NewValue == null || !(e.NewValue is int nId)) {
				wind.setNewEntity();
				return;
			}
			wind.IsAccessingDb = true;
			try {
				wind.Entity = await wind.Query.FirstOrDefaultAsync();
				if(wind.Entity == null) {
					string msg = $"No {wind.entityDisplayName} with the specified Id ({nId}) was found.{Environment.NewLine}" +
						$"Do you want to create a new {wind.entityDisplayName}?";
					if(MessageBox.Show(msg, $"No {wind.entityDisplayName} Found",
						MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
						wind.setNewEntity();
					}else {
						wind._isCancel = true;
						wind.Close();
					}
				}
			}catch(Exception ex) {
				throw ex;
			}finally {
				wind.IsAccessingDb = false;
			}
			wind.updateTitle();
		}
		#endregion


		private void updateTitle() {
			if (EntityType == null) {
				Title = "(record type not set)";
			} else if (Entity == null || (EntityId??0)<=0) {
				Title = $"Add new {entityDisplayName}";
			} else {
				Title = $"{entityDisplayName} [{Entity}]";
			}
			DataContext = Entity;
		}

		private string entityDisplayName =>
			EntityType == null
			? "(no type set)"
			: LibraryContext.GetDisplayName(EntityType);

		private async System.Threading.Tasks.Task<bool> SaveAsync() {
			if (ValidateRecord()) {
				int i =-1;
				IsAccessingDb = true;
				try {
					i = await _db.SaveChangesAsync();
				} catch (Exception ex) {
					MessageBox.Show(ex.Message, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
				} finally {
					IsAccessingDb = false;
				}
				return i > 0;
			} else {
				return false;
			}
		}

		#region Event Handlers
		private async void EntityBoundWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (_isCancel) {
				_isCancel = false;
				return;
			}

			if(_db.ChangeTracker.HasChanges()) {
				string msg = $"There are unsaved changes to this {entityDisplayName}. Do you want to:{Environment.NewLine}" +
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
			if (Entity == null) {
				setNewEntity();
			}

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

			var btnReload = (Button)Template.FindName(RELOAD_BUTTON, this);
			if(btnReload != null)
				btnReload.Click += BtnReload_Click;
		}

		private void BtnReload_Click(object sender, RoutedEventArgs e) {
			_db = App.DbContext;
			EntityId = EntityId;
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e) {
			_isCancel = true;
			Close();
		}

		private async void BtnSaveClose_Click(object sender, RoutedEventArgs e) {
			if (!await SaveAsync())
				return;
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
