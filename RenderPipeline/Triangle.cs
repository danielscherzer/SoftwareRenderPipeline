using System.Diagnostics;
using System.Numerics;

namespace RenderPipeline
{
	[DebuggerDisplay("{vertices}")]
	internal class Triangle
	{
		public Triangle(Vertex v0, Vertex v1, Vertex v2)
		{
			vertices[0] = v0;
			vertices[1] = v1;
			vertices[2] = v2;
			//V0 = v0;
			//V1 = v1;
			//V2 = v2;
		}

		// Define the indexer to allow client code to use [] notation.
		public Vertex this[int i]
		{
			get => vertices[i];
			set => vertices[i] = value;
		}

		//internal Vertex V0 { get; set; }
		//internal Vertex V1 { get; set; }
		//internal Vertex V2 { get; set; }

		private readonly Vertex[] vertices = new Vertex[3];
	}
}