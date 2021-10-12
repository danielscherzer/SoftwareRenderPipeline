using RenderPipeline;
using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

var renderer = new RenderDevice(300, 300);

// load model data
var triangles = new ColoredTrianglesDrawable(renderer);
var assemblyName = Assembly.GetExecutingAssembly().Location;
var assemblyDir = Path.GetDirectoryName(assemblyName) ?? assemblyName;
var suzanne = new FileDrawable(Path.Combine(assemblyDir, "Content", "suzanne.obj"), renderer);

// set render state
renderer.RenderState.VertexShader = (uniforms, vertex) =>
{
	var mtxMVP = (Matrix4x4)uniforms["modelViewProjection"];
	var position = new Vector4(vertex.GetAttribute<Vector3>(0), 1f);
	position = Vector4.Transform(position, mtxMVP);
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
renderer.FrameBuffer.Fill(new Vector4(0.5f, 0f, 0f, 1));
renderer.Zbuffer.Fill(renderer.ViewPort.FarZ);

var time = Stopwatch.StartNew();
// draw
triangles.Draw(renderer);
suzanne.Draw(renderer);
Console.WriteLine($"render time: {time.ElapsedMilliseconds}msec");

var output = "frame.png";
renderer.FrameBuffer.ToImage(Path.Combine(assemblyDir, output));

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
	Process.Start("explorer.exe", output); // for debug output
}
