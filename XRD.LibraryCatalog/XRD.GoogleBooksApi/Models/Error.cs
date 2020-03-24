using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace XRD.GoogleBooksApi.Models {
	[DataContract]
	public class Error {
		[DataMember(Name ="errors")]
		public List<ErrorDetail> Errors { get; set; }

		[DataMember(Name ="code")]
		public string Code { get; set; }

		[DataMember(Name ="message")]
		public string Message { get; set; }

		public override string ToString() {
			StringBuilder sb = new StringBuilder(Message);
			if (Errors != null && Errors.Count > 0) {
				foreach (var e in Errors) {
					sb.Append(Environment.NewLine);
					sb.Append(e);
				}
			}
			return sb.ToString();
		}
	}
}
