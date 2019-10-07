using System.Numerics;

namespace RenderPipeline
{
	public struct ViewPort
	{
		public ViewPort(int x, int y, int width, int height, float minZ, float maxZ)
		{
			TopLeftX = x;
			TopLeftY = y;
			Width = width;
			Height = height;
			MinDepth = minZ;
			MaxDepth = maxZ;
		}
		public int TopLeftX { get; }
		public int TopLeftY { get; }
		public int Width { get; }
		public int Height { get; }
		public float MinDepth { get; }
		public float MaxDepth { get; }

		/// <summary>
		/// Converts from clip space to screen space.
		/// Formula from http://msdn.microsoft.com/en-us/library/bb205126(v=vs.85).aspx
		/// </summary>
		public Vector4 Transform(Vector4 position)
		{
			return new Vector4
			{
				X = (position.X + 1f) * (Width - 1) * 0.5f + TopLeftX,
				Y = (1f - position.Y) * (Height - 1) * 0.5f + TopLeftY,
				Z = MinDepth + position.Z * (MaxDepth - MinDepth),
				W = position.W,
			};
		}
	}
}
