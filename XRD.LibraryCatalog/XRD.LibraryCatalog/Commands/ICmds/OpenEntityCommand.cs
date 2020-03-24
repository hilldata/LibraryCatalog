using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace XRD.LibCat.Commands {
	public class OpenEntityCommand<TWindow, TEntity> : ICommand where TWindow:Controls.EntityBoundWindow where TEntity:Models.Abstract.Entity {
		public void Execute(object parameter) {
			if (parameter == null)
				return;
			else if (parameter is TEntity ent) {
				App.OpenWindow<TWindow>(ent.Id);
			} else if (parameter is int id) {
				App.OpenWindow<TWindow>(id);
			}
		}

		public bool CanExecute(object parameter) {
			if (parameter == null)
				return false;
			else if (parameter is TEntity)
				return true;
			else if (parameter is int)
				return true;
			else
				return false;
		}

		public event EventHandler CanExecuteChanged {
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}
	}
}
