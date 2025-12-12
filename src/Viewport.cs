using System.Numerics;

namespace RenderPipeline;

/// <summary>
/// Idea from https://docs.microsoft.com/en-us/windows/win32/direct3d11/d3d10-graphics-programming-guide-rasterizer-stage-getting-started?redirectedfrom=MSDN
/// But my origin is in the lower left corner with positive y upwards
/// and my positive z-axis is towards the viewer
/// a right-handed coordinates system
/// </summary>
public readonly struct ViewPort(int minX, int minY, int width, int height, float nearZ = 0f, float farZ = -1f)
{
	public int MinX { get; } = minX;
	public int MinY { get; } = minY;
	public int Width { get; } = width;
	public int Height { get; } = height;
	public float NearZ { get; } = nearZ;
	public float FarZ { get; } = farZ;

	/// <summary>
	/// Converts from clip space to screen space.
	/// Idea from https://docs.microsoft.com/en-us/windows/win32/direct3d11/d3d10-graphics-programming-guide-rasterizer-stage-getting-started?redirectedfrom=MSDN
	/// But my origin is in the lower left corner with positive y upwards
	/// and my positive z-axis is towards the viewer
	/// a right-handed coordinates system
	/// </summary>
	public Vector4 Transform(Vector4 position)
	{
		return new Vector4
		{
			X = MinX + (position.X + 1f) * (Width - 1) * 0.5f,
			Y = MinY + (position.Y + 1f) * (Height - 1) * 0.5f,
			Z = 0.5f * ((NearZ - FarZ) * position.Z + (NearZ + FarZ)),
			W = position.W,
		};
	}
}
