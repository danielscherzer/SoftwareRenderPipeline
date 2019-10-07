using System.Collections.Generic;
using System.Numerics;

namespace RenderPipeline
{
	public class Triangle
	{
		public Triangle(Vertex[] vertices) => vertices.CopyTo(this.vertices, 0);

		public Vertex this[int i]
		{
			get => vertices[i];
			set => vertices[i] = value;
		}

		public override string ToString() => $"{vertices[0]} {vertices[1]} {vertices[2]}";

		private readonly Vertex[] vertices = new Vertex[3];

		internal List<object> InterpolateAttributes(float u, float v)
		{
			var outputAttributes = new List<object>();
			var count = vertices[0].Attributes.Count;
			for(int i = 1; i < count; ++i)
			{
				var a = vertices[0].Attributes[i];
				var b = vertices[1].Attributes[i];
				var c = vertices[2].Attributes[i];

				switch (vertices[0].Attributes[i])
				{
					case float attrib:
						outputAttributes.Add(Barycentric.Interpolate(u, v, (float)a, (float)b, (float)c));
						break;
					case Vector2 attrib:
						outputAttributes.Add(Barycentric.Interpolate(u, v, (Vector2)a, (Vector2)b, (Vector2)c));
						break;
					case Vector3 attrib:
						outputAttributes.Add(Barycentric.Interpolate(u, v, (Vector3)a, (Vector3)b, (Vector3)c));
						break;
					case Vector4 attrib:
						outputAttributes.Add(Barycentric.Interpolate(u, v, (Vector4)a, (Vector4)b, (Vector4)c));
						break;
				}
			}
			return outputAttributes;
		}

		internal float InterpolateZ(float u, float v) => Barycentric.Interpolate(u, v, this[0].Position.Z, this[1].Position.Z, this[2].Position.Z);
	}
}