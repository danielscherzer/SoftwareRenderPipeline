using System;

namespace RenderPipeline
{
	public class Triangle
	{
		public Triangle(Vertex v0, Vertex v1, Vertex v2)
		{
			vertices[0] = v0;
			vertices[1] = v1;
			vertices[2] = v2;
		}

		public Vertex this[int i]
		{
			get => vertices[i];
			set => vertices[i] = value;
		}

		public float InterpolateZ(float u, float v) => Barycentric.Interpolate(u, v, vertices[0].Position.Z, vertices[1].Position.Z, vertices[2].Position.Z);

		public override string ToString() => $"{vertices[0]} {vertices[1]} {vertices[2]}";

		private readonly Vertex[] vertices = new Vertex[3];
	}
}