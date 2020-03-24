using System.Drawing;

namespace XRD.LibCat {
	public class DrawTextToBitmapOptions {
		public DrawTextToBitmapOptions() { }
		public string FontName { get; set; } = "Segoe MDL2 Assets";
		public int Width { get; set; } = 96;
		public int Height { get; set; } = 96;
		public FontStyle FontStyle { get; set; } = FontStyle.Regular;
		public Color Background { get; set; } = Color.Transparent;
		public Color Foreground { get; set; } = Color.Black;

		public static DrawTextToBitmapOptions Default => new DrawTextToBitmapOptions();
		public static DrawTextToBitmapOptions Large => new DrawTextToBitmapOptions() { Width = 48, Height = 48 };
		public static DrawTextToBitmapOptions Medium => new DrawTextToBitmapOptions() { Width = 24, Height = 24 };
		public static DrawTextToBitmapOptions Small => new DrawTextToBitmapOptions() { Width = 16, Height = 16 };
	}
}
