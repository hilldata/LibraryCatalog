using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace XRD.LibCat.Commands {
	public static class StaffMemberCommands {
		private static ICommand _add;
		public static ICommand Add {
			get {
				if (_add == null)
					_add = new AddEntityCommand<StaffMemberEditorWindow>();
				return _add;
			}
		}

		private static ICommand _open;
		public static ICommand Open {
			get {
				if (_open == null)
					_open = new OpenEntityCommand<StaffMemberEditorWindow, Models.StaffMember>();
				return _open;
			}
		}

		private static ICommand _softDelete;
		public static ICommand SoftDelete {
			get {
				if (_softDelete == null)
					_softDelete = new SoftDeleteCommand();
				return _softDelete;
			}
		}

		private static ICommand _unDelete;
		public static ICommand UnDelete {
			get {
				if (_unDelete == null)
					_unDelete = new UnDeleteCommand();
				return _unDelete;
			}
		}
	}
}