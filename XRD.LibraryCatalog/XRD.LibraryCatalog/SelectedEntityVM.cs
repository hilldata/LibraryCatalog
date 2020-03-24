using System;
using System.Collections.Generic;
using System.Text;

namespace XRD.LibCat {
	public class SelectedEntityVM<TEntity> where TEntity:Models.Abstract.Entity {
		public SelectedEntityVM(int? id = null) {
			EType = typeof(TEntity);
			Id = id;
		}

		public Type EType { get; set; }
		public int? Id { get; set; }
	}
}
