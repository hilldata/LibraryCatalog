using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace XRD.LibCat {
	public static class DrawingHelper {
		public static Bitmap DrawTextToVariableBitmap(string text, string fontName, float fontSize, FontStyle fontStyle = FontStyle.Regular, Color? bgColor = null, Color? fgColor = null) {
			if (string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException(nameof(text));
			if (string.IsNullOrWhiteSpace(fontName))
				fontName = "Segoe MDL2 Assets";
			if (fontSize < 5)
				throw new ArgumentOutOfRangeException(nameof(fontSize));

			using Bitmap bmp = new Bitmap(1, 1);
			using Graphics temp = Graphics.FromImage(bmp);
			using Font font = new Font(fontName, fontSize, fontStyle);
			SizeF size = temp.MeasureString(text, font);
			using Bitmap res = new Bitmap((int)size.Width, (int)size.Height);
			using Graphics final = Graphics.FromImage(res);
			if (bgColor != null)
				final.FillRectangle(new SolidBrush(bgColor.Value), 0, 0, res.Width, res.Height);

			final.DrawString(text, font, new SolidBrush(fgColor ?? Color.Black), 0, 0);
			final.Flush();
			return res;
		}

		public static Bitmap DrawTextToFixedBitmap(string text, string fontName, int width = 64, int height = 64, FontStyle fontStyle = FontStyle.Regular, Color? bgColor = null, Color? fgColor = null) {
			if (string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException(nameof(text));
			if (string.IsNullOrWhiteSpace(fontName))
				fontName = "Segoe MDL2 Assets";
			if (width < 5)
				throw new ArgumentOutOfRangeException(nameof(width));
			if (height < 5)
				throw new ArgumentOutOfRangeException(nameof(height));

			Bitmap res = new Bitmap(width, height);
			using Graphics gr = Graphics.FromImage(res);
			if (bgColor.HasValue)
				gr.FillRectangle(new SolidBrush(bgColor.Value), 0, 0, width, height);
			int fontSize = height;
			bool tooBig = true;
			while (tooBig && fontSize > 0) {
				if (fontSize < 1)
					throw new Exception($"The text will not fix in the size specified [{width} x {height}].");
				using Font font = new Font(fontName, fontSize, fontStyle);
				SizeF size = gr.MeasureString(text, font);
				tooBig = size.Width > width || size.Height > height;
				if (!tooBig) {
					// Center text;
					float fW = (width - size.Width > 0) ? (width - size.Width) / 2 : 0;
					float fH = (height - size.Height > 0) ? (height - size.Height) / 2 : 0;
					gr.DrawString(text, font, new SolidBrush(fgColor ?? Color.Black), fW, fH);
					gr.Flush();
					return res;
				} else
					fontSize -= 1;
			}
			return null;
		}

		public static Bitmap DrawTextToFixedBitmap(string text, DrawTextToBitmapOptions options = default) {
			if (options == default)
				options = new DrawTextToBitmapOptions();
			return DrawTextToFixedBitmap(text, options.FontName, options.Width, options.Height, options.FontStyle, options.Background, options.Foreground);
		}

		public static BitmapSource CreateBitmapSource(Bitmap bmp) {
			if (bmp == null)
				throw new ArgumentNullException(nameof(bmp));
			var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
			var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			try {
				var size = (rect.Width * rect.Height) * 4;
				return BitmapSource.Create(
					bmp.Width,
					bmp.Height,
					bmp.HorizontalResolution,
					bmp.VerticalResolution,
					System.Windows.Media.PixelFormats.Bgra32,
					null,
					bmpData.Scan0,
					size,
					bmpData.Stride);
			} catch (Exception ex) {
				throw ex;
			} finally {
				bmp.UnlockBits(bmpData);
			}
		}
	}
}