using ImageMagick;
using System.Numerics;

namespace RenderPipeline
{
	public static class FrameBufferToImage
	{
		public static void ToImage(this FrameBuffer<Vector4> frameBuffer, string fileName)
		{
			var bytes = new byte[frameBuffer.Width * frameBuffer.Height * 4];
			var id = 0;
			foreach (var pixel in frameBuffer)
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
				Width = frameBuffer.Width,
				Height = frameBuffer.Height
			};
			using (var image = new MagickImage(bytes, settings))
			{
				image.Write(fileName);
			}

		}
	}
}
