using System.Numerics;

namespace RenderPipeline
{
	public static class Barycentric
	{
		public static float Interpolate(float u, float v, float a, float b, float c) => u * a + v * b + (1 - u - v) * c;
	}
}
