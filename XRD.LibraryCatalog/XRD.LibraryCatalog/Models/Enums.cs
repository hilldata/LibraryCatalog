using System;
using System.ComponentModel;

namespace XRD.LibCat.Models {
	/// <summary>
	/// Enumeration of various grade levels (flags so that teachers can be assigned multiple grades)
	/// </summary>
	[Flags]
	[TypeConverter(typeof(EnumDisplayTypeConverter))]
	public enum GradeLevels : long {
		[Description("Not Set")]
		NotSet = 0x_0000_0000_0000_0000,
		[Description("Toddler")]
		Toddler = 0x_0000_0000_0000_0001,
		[Description("Preschool (3-year-old)")]
		Preschool3 = 0x_0000_0000_0000_0010,
		[Description("Preschool (4-year-old)")]
		Preschool4 = 0x_0000_0000_0000_0100,
		[Description("Kindergarden")]
		Kindergarden = 0x_0000_0000_0000_1000,
		[Description("First Grade")]
		First = 0x_0000_0000_0001_0000,
		[Description("Second Grade")]
		Second = 0x_0000_0000_0010_0000,
		[Description("Third Grade")]
		Third = 0x_0000_0000_0100_0000,
		[Description("Fourth Grade")]
		Fourth = 0x_0000_0000_1000_0000,
		[Description("Fifth Grade")]
		Fifth = 0x_0000_0001_0000_0000,
		[Description("Sixth Grade")]
		Sixth = 0x_0000_0010_0000_0000,
		[Description("Seventh Grade")]
		Seventh = 0x_0000_0100_0000_0000,
		[Description("Eighth Grade")]
		Eighth = 0x_0000_1000_0000_0000,
		[Description("Ninth Grade")]
		Ninth = 0x_0001_0000_0000_0000,
		[Description("Tenth Grade")]
		Tenth = 0x_0010_0000_0000_0000,
		[Description("Eleventh Grade")]
		Eleventh = 0x_0100_0000_0000_0000,
		[Description("Twelfth Grade")]
		Twelfth = 0x_1000_0000_0000_0000
	}
}