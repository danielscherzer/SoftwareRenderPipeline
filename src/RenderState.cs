using System.Collections.Generic;
using System.Numerics;

namespace RenderPipeline
{
	class RenderState
	{
		public delegate Vertex VertexShaderDelegate(IReadOnlyDictionary<string, object> uniforms, Vertex vertex);
		public delegate IEnumerable<Triangle> GeometryShaderDelegate(IReadOnlyDictionary<string, object> uniforms, Triangle triangle);
		public delegate IEnumerable<Triangle> TessellationShaderDelegate(IReadOnlyDictionary<string, object> uniforms, Triangle triangle);
		public delegate Vector4 FragmentShaderDelegate(IReadOnlyDictionary<string, object> uniforms, Fragment fragment);

		public Dictionary<string, object> Uniforms = new();
		public VertexShaderDelegate VertexShader { get; set; } = DefaultVertexShader;
		public TessellationShaderDelegate TessellationShader { get; set; } = DefaultTessellationShader;
		public GeometryShaderDelegate GeometryShader { get; set; } = DefaultGeometryShader;
		public FragmentShaderDelegate FragmentShader { get; set; } = (_, fragment) => Vector4.One;
		public bool Blending { get; set; } = false;
		public FaceCullingMode FaceCulling { get; set; } = FaceCullingMode.NONE;
		public bool ZTest { get; set; } = true;

		private static IEnumerable<Triangle> DefaultGeometryShader(IReadOnlyDictionary<string, object> uniforms, Triangle triangle)
		{
			yield return triangle;
		}

		private static IEnumerable<Triangle> DefaultTessellationShader(IReadOnlyDictionary<string, object> uniforms, Triangle triangle)
		{
			yield return triangle;
		}

		private static Vertex DefaultVertexShader(IReadOnlyDictionary<string, object> uniforms, Vertex vertex)
		{
			var position = vertex.GetAttribute<Vector4>(0);
			return new Vertex(new object[] { position });
		}
	}
}
