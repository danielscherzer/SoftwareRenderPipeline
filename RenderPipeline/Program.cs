using ImageMagick;
using System;
using System.Diagnostics;
using System.Numerics;

namespace RenderPipeline
{
	class Program
	{

		static void Main(string[] args)
		{
			var renderer = new Renderer(128, 128);

			var v0 = new Vertex(new Vector4(0, 0, 0, 1));
			var v1 = new Vertex(new Vector4(1, 0, 0, 1));
			var v2 = new Vertex(new Vector4(1, 1, 0, 1));

			var time = Stopwatch.StartNew();
			renderer.DrawTriangle(new Triangle(v0, v1, v2));

			var bytes = new byte[renderer.FrameBuffer.Length * 4];
			var id = 0;
			foreach(var pixel in renderer.FrameBuffer)
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
				Width = renderer.FrameBuffer.GetLength(0),
				Height = renderer.FrameBuffer.GetLength(1)
			};
			using (var image = new MagickImage(bytes, settings))
			{
				image.Write(@"d:\frame.png");
			}
			Console.WriteLine(time.ElapsedMilliseconds);
		}
	}
}
