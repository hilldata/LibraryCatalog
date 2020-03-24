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
	///     <MyNamespace:EntityExplorer/>
	///
	/// </summary>
	public class EntityExplorer : Control {
		protected static readonly Models.LibraryContext _db = App.DbContext;

		static EntityExplorer() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EntityExplorer), new FrameworkPropertyMetadata(typeof(EntityExplorer)));
		}

		public static readonly DependencyProperty EntityTypeProperty = DependencyProperty.Register(
			"EntityType",
			typeof(Type),
			typeof(EntityExplorer),
			new PropertyMetadata(typeof(Models.IEntity)));
		public Type EntityType {
			get => (Type)GetValue(EntityTypeProperty);
			set => SetValue(EntityTypeProperty, value);
		}

		public static readonly DependencyProperty EditorWindowTypeProperty = DependencyProperty.Register(
			"EditorWindowProperty",
			typeof(Type),
			typeof(EntityExplorer),
			new PropertyMetadata(null));
		public Type EditorWindowType {
			get => (Type)GetValue(EditorWindowTypeProperty);
			set => SetValue(EditorWindowTypeProperty, value);
		}

		public static readonly DependencyProperty AdditionalFilterItemsProperty = DependencyProperty.Register(
			"AdditionalFilterItems",
			typeof(ItemCollection),
			typeof(EntityExplorer));

		public ItemCollection AdditionalFilterItems {
			get => (ItemCollection)GetValue(AdditionalFilterItemsProperty);
			set => SetValue(AdditionalFilterItemsProperty, value);
		}
	}
}
