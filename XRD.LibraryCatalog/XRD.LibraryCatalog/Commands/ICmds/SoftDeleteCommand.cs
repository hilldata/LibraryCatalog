using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace XRD.LibCat.Commands {
	public class SoftDeleteCommand : ICommand {
		public void Execute(object parameter) {
			if (parameter == null)
				return;
			else if (parameter is Models.ISoftDeleted ent) {
				if (!ent.IsDeleted)
					ent.IsDeleted = true;
			} else
				return;
		}

		public bool CanExecute(object parameter) {
			if (parameter == null)
				return false;
			else
				return parameter is Models.ISoftDeleted;
		}

		public event EventHandler CanExecuteChanged {
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}
	}
}
