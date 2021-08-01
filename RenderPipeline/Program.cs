using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;

namespace RenderPipeline
{
	class Program
	{
		static void Main(string[] _)
		{
			var renderer = new RenderDevice(300, 300);

			// load model data
			var triangles = new SimpleDrawable(renderer);
			var suzanne = new FileDrawable(@"Content\suzanne.obj", renderer);

			// set render state
			renderer.RenderState.VertexShader = (uniforms, vertex) =>
			{
				var mtxCamera = (Matrix4x4)uniforms["modelViewProjection"];
				var position = new Vector4(vertex.GetAttribute<Vector3>(0), 1f);
				position = Vector4.Transform(position, mtxCamera);
				var color = vertex.GetAttribute<Vector4>(1);
				return new Vertex(new object[] { position, color });
			};

			renderer.RenderState.FragmentShader = (uniforms, fragment) =>
			{
				var color = (Vector4)fragment.Attributes[0];
				return color;
			};

			renderer.RenderState.Uniforms["modelViewProjection"] = Matrix4x4.Identity;

			var time = Stopwatch.StartNew();

			renderer.FrameBuffer.Fill(new Vector4(0.5f, 0.5f, 0.5f, 1));
			renderer.Zbuffer.Fill(renderer.ViewPort.MaxDepth);

			// draw
			//triangles.Draw(renderer);
			suzanne.Draw(renderer);

			Console.WriteLine($"render time: {time.ElapsedMilliseconds}msec");
			var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			renderer.FrameBuffer.ToImage(Path.Combine(exeDir, "frame.png"));
		}
	}
}
