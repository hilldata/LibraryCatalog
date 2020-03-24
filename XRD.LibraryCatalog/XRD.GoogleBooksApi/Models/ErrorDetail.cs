using System.Runtime.Serialization;

namespace XRD.GoogleBooksApi.Models {
	[DataContract]
	public class ErrorDetail {
		[DataMember(Name ="domain")]
		public string Domain { get; set; }

		[DataMember(Name ="reason")]
		public string Reason { get; set; }

		[DataMember(Name ="message")]
		public string Message { get; set; }

		[DataMember(Name ="locationType")]
		public string LocationType { get; set; }

		[DataMember(Name ="location")]
		public string Location { get; set; }

		public override string ToString() =>
			$"{Message} AT {LocationType} [{Location}].";
	}
}
