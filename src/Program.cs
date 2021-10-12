using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using Zenseless.Geometry;
using Zenseless.Patterns;

namespace RenderPipeline
{
	class Program
	{
		static void Main(string[] _)
		{
			var renderer = new RenderDevice(300, 300);

			// load model data
			var triangles = new ColoredTrianglesDrawable(renderer);
			var suzanne = new FileDrawable(Path.Combine(PathTools.GetCurrentProcessDir(), "Content", "suzanne.obj"), renderer);

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
			renderer.RenderState.FaceCulling = FaceCullingMode.CW;
			renderer.FrameBuffer.Fill(new Vector4(0.5f, 0.5f, 0.5f, 1));
			renderer.Zbuffer.Fill(renderer.ViewPort.FarZ);

			var time = Stopwatch.StartNew();
			// draw
			//triangles.Draw(renderer);
			suzanne.Draw(renderer);

			Console.WriteLine($"render time: {time.ElapsedMilliseconds}msec");
			var assemblyName = Assembly.GetExecutingAssembly().Location;
			var assemblyDir = Path.GetDirectoryName(assemblyName) ?? assemblyName;
			renderer.FrameBuffer.ToImage(Path.Combine(assemblyDir, "frame.png"));

			Process.Start("explorer.exe", "frame.png"); // for debug output
		}
	}
}
