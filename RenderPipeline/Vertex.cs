using System.Numerics;

namespace RenderPipeline
{
	public class Vertex
	{
		public Vertex(Vector4 position)
		{
			Position = position;
		}

		public Vector4 Position { get; set; }

		public Vector4 PerspectiveDivide() => Vector4.Divide(Position, Position.W);

		public override string ToString() => $"{Position}";

	}
}