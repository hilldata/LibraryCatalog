using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace XRD.LibCat.Commands {
	public class AddEntityCommand<TWindow> : ICommand where TWindow:Controls.EntityBoundWindow {
		public void Execute(object parameter) {
			App.OpenWindow<TWindow>(null);
		}
		public bool CanExecute(object parameter) => true;
		public event EventHandler CanExecuteChanged;
	}
}
