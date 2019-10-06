using System.Numerics;

namespace RenderPipeline
{
	public struct ViewPort
	{
		public int TopLeftX;
		public int TopLeftY;
		public int Width;
		public int Height;
		public float MinDepth;
		public float MaxDepth;

		public ViewPort(int x, int y, int width, int height, float minZ, float maxZ)
		{
			TopLeftX = x;
			TopLeftY = y;
			Width = width;
			Height = height;
			MinDepth = minZ;
			MaxDepth = maxZ;
		}

		/// <summary>
		/// Converts from clip space to screen space.
		/// Formula from http://msdn.microsoft.com/en-us/library/bb205126(v=vs.85).aspx
		/// </summary>
		public Vector3 Transform(Vector3 position)
		{
			return new Vector3
			{
				X = (position.X + 1f) * (Width - 1) * 0.5f + TopLeftX,
				Y = (position.Y + 1f) * (Height - 1) * 0.5f + TopLeftY,
				Z = MinDepth + position.Z * (MaxDepth - MinDepth)
			};
		}
	}
}
