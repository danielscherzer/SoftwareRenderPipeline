using System.Numerics;

namespace RenderPipeline;

public static class VectorExtensions
{
	public static Vector2 XY(this Vector3 v) => new(v.X, v.Y);
	public static Vector2 XY(this Vector4 v) => new(v.X, v.Y);
	public static Vector3 XYZ(this Vector4 v) => new(v.X, v.Y, v.Z);

	public static Vector<int> Vec2i(int x, int y) => new([x, y]);
}
