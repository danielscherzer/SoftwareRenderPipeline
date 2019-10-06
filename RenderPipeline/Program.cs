using System;
using System.Diagnostics;
using System.Numerics;

namespace RenderPipeline
{
	class Program
	{

		static void Main(string[] args)
		{
			var renderer = new Renderer(400, 400);

			var v0 = new Vertex(new Vector4(0, -1f, 0, 1));
			var v1 = new Vertex(new Vector4(1, 0.25f, 0, 1));
			var v2 = new Vertex(new Vector4(-0.75f, 1, 0, 1));

			var time = Stopwatch.StartNew();
			renderer.FrameBuffer.Clear(new Vector4(0, 0.5f, 0, 1));
			renderer.DrawTriangle(new Triangle(v0, v1, v2));
			Console.WriteLine(time.ElapsedMilliseconds);
			renderer.FrameBuffer.ToImage(@"d:\frame.png");
			Console.WriteLine(time.ElapsedMilliseconds);
		}
	}
}
