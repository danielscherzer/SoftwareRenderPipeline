using System;
using System.Diagnostics;
using System.Numerics;

namespace RenderPipeline
{
	class Program
	{
		static Triangle Tri(Vector3 p0, Vector3 p1, Vector3 p2)
		{
			var v0 = new Vertex(new Vector4(p0, 1));
			var v1 = new Vertex(new Vector4(p1, 1));
			var v2 = new Vertex(new Vector4(p2, 1));
			return new Triangle(v0, v1, v2);
		}

		static void Main(string[] args)
		{
			var renderer = new Renderer(400, 400);

			var attributePosition = renderer.CreateBuffer(new Vector3[] 
			{
				new Vector3(0, -1f, 0), new Vector3(1, 0.25f, 0), new Vector3(-0.75f, 1, 0),
				new Vector3(0, 0, 0.5f), new Vector3(1, 0, 0.5f), new Vector3(1, 1, 0.5f)
			});
			var attributeColor = renderer.CreateBuffer(new Vector4[] 
			{
				new Vector4(1, 0.5f, 0.5f, 1), new Vector4(1, 0.5f, 0.5f, 1), new Vector4(1, 0.5f, 0.5f, 1),
				new Vector4(0, 0, 0.5f, 1), new Vector4(1, 0, 0.5f, 1), new Vector4(1, 1, 0.5f, 1)
			});
			var indices = renderer.CreateBuffer(new int[] { 0, 1, 2, 3, 4, 5 });

			var v0 = new Vertex(new Vector4(0, -1f, 0, 1));
			var v1 = new Vertex(new Vector4(1, 0.25f, 0, 1));
			var v2 = new Vertex(new Vector4(-0.75f, 1, 0, 1));

			var time = Stopwatch.StartNew();
			renderer.FrameBuffer.Clear(new Vector4(0, 0.5f, 0, 1));
			renderer.Zbuffer.Clear(renderer.ViewPort.MaxDepth);

			renderer.DrawTrianglesIndexed(indices, new int[] { attributePosition, attributeColor });
			Console.WriteLine(time.ElapsedMilliseconds);
			time.Restart();
			renderer.FrameBuffer.ToImage(@"d:\frame.png");
			Console.WriteLine(time.ElapsedMilliseconds);
		}
	}
}
