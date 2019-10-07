using System;
using System.Collections.Generic;
using System.Numerics;

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

		public Triangle(Vertex[] vertices) => this.vertices = vertices;

		public Vertex this[int i]
		{
			get => vertices[i];
			set => vertices[i] = value;
		}

		public override string ToString() => $"{vertices[0]} {vertices[1]} {vertices[2]}";

		private readonly Vertex[] vertices = new Vertex[3];

		internal List<object> Interpolate(float u, float v)
		{
			var outputAttributes = new List<object>();
			var count = vertices[0].Attributes.Count;
			for(int i = 1; i < count; ++i)
			{
				switch (vertices[0].Attributes[i])
				{
					case Vector4 attrib:
						outputAttributes.Add(Barycentric.Interpolate(u, v, (Vector4)vertices[0].Attributes[i], (Vector4)vertices[1].Attributes[i], (Vector4)vertices[2].Attributes[i]));
						break;
				}
			}
			return outputAttributes;
		}
	}
}