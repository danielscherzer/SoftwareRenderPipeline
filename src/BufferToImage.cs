using ImageMagick;
using System.Numerics;
using Zenseless.Spatial;

namespace RenderPipeline
{
	public static class BufferToImage
	{
		public static void ToImage(this Grid<Vector4> frameBuffer, string fileName)
		{
			var bytes = new byte[frameBuffer.Columns * frameBuffer.Rows * 4];
			var id = 0;
			foreach (var pixel in frameBuffer.AsReadOnly)
			{
				var clampedColor = Vector4.Multiply(255f, Vector4.Max(Vector4.Zero, Vector4.Min(pixel, Vector4.One)));
				bytes[id++] = (byte)clampedColor.X;
				bytes[id++] = (byte)clampedColor.Y;
				bytes[id++] = (byte)clampedColor.Z;
				bytes[id++] = (byte)clampedColor.W;
			}
			var settings = new MagickReadSettings
			{
				Format = MagickFormat.Rgba,
				Width = frameBuffer.Columns,
				Height = frameBuffer.Rows,
			};
			using var image = new MagickImage(bytes, settings);
			image.Flip(); // images have positive y-axis going downwards and our grid is filled right-handed y-axis going upwards
			image.Write(fileName);

		}
	}
}
