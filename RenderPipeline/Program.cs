using System;
using System.Diagnostics;
using System.Numerics;
using Zenseless.Geometry;

namespace RenderPipeline
{
	class Program
	{
		static void Main(string[] args)
		{
			var renderer = new RenderDevice(300, 300);

			var triangles = new SimpleDrawable(renderer);
			var suzanne = new FileDrawable(@"Content\suzanne.obj", renderer);

			var time = Stopwatch.StartNew();

			renderer.FrameBuffer.Clear(new Vector4(0.5f, 0.5f, 0, 1));
			renderer.Zbuffer.Clear(renderer.ViewPort.MaxDepth);

			var camera = new Rotation(180f, Axis.Y);

			renderer.Uniforms["camera"] = camera.Matrix;
			//triangles.Draw(renderer);
			suzanne.Draw(renderer);

			Console.WriteLine(time.ElapsedMilliseconds);
			time.Restart();
			renderer.FrameBuffer.ToImage(@"d:\frame.png");
			Console.WriteLine(time.ElapsedMilliseconds);
		}
	}
}
