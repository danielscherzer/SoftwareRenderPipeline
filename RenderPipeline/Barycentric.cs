using System.Numerics;

namespace RenderPipeline
{
	public static class Barycentric
	{
		public static float Interpolate(float u, float v, float a, float b, float c) => u * a + v * b + (1 - u - v) * c;
		public static Vector4 Interpolate(float u, float v, Vector4 a, Vector4 b, Vector4 c) => u * a + v * b + (1 - u - v) * c;
	}
}
