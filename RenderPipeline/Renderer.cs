using System.Collections.Generic;
using System.Numerics;

namespace RenderPipeline
{
	class Renderer
	{
		private readonly ViewPort viewPort;

		public Renderer(int width, int height)
		{
			FrameBuffer = new Vector4[width, height];
			viewPort = new ViewPort(0, 0, FrameBuffer.GetLength(0), FrameBuffer.GetLength(1), 0, 1);
		}

		public Vector4[,] FrameBuffer { get; }

		public void DrawTriangle(Triangle triangle)
		{
			for(int i = 0; i < 3; ++i)
			{
				triangle[i] = ApplyVertexShader(triangle[i]);
			}
			foreach (var tessTri in ApplyTessellationShader(triangle))
			{
				foreach (var geomTri in ApplyGeometryShader(tessTri))
				{
					foreach (var fragment in RasterizeTriangle(geomTri))
						//TODO: z test
						//TODO: blending
						FrameBuffer[fragment.X, fragment.Y] = new Vector4(1, 0, 0, 1);
				}
			}
		}

		private static Vertex ApplyVertexShader(Vertex vertex)
		{
			return vertex;
		}

		private static IEnumerable<Triangle> ApplyTessellationShader(Triangle triangle)
		{
			yield return triangle;
		}

		private static IEnumerable<Triangle> ApplyGeometryShader(Triangle triangle)
		{
			yield return triangle;
		}

		private IEnumerable<Fragment> RasterizeTriangle(Triangle triangle)
		{
			for (int i = 0; i < 3; ++i)
			{
				triangle[i].Position = triangle[i].PerspectiveDivide();
			}

			// Back face culling would come here.

			// Transform from clip space to screen space. (view-port transform)
			var p_ss = new Vector3[3];
			var p = new Vector2[3];
			for (int i = 0; i < 3; ++i)
			{
				p_ss[i] = viewPort.Transform(triangle[i].Position.XYZ());
				p[i] = p_ss[i].XY();
			}

			// get the bounding box of the 2D triangle
			var min = Vector2.Min(p[0], Vector2.Min(p[1], p[2]));
			var max = Vector2.Max(p[0], Vector2.Max(p[1], p[2]));

			// Clipping of box to view-port bounds
			var viewPortMin = new Vector2(viewPort.TopLeftX, viewPort.TopLeftY);
			var viewPortMax = viewPortMin + new Vector2(viewPort.Width, viewPort.Height) - Vector2.One;
			min = Vector2.Max(min, viewPortMin);
			max = Vector2.Min(max, viewPortMax);

			// rasterize - triangle setup
			var ca = p[0] - p[2];
			var cb = p[1] - p[2];
			var fact = 1.0f / Det(ca, cb);
			const float eps = 0.0001f;
			for (int x = (int)min.X; x <= (int)max.X; x++)
			{
				for (int y = (int)min.Y; y <= (int)max.Y; y++)
				{
					var pos = new Vector2(x, y);
					var cp = pos - p[2];
					var u = fact * Det(cp, cb);
					var v = fact * Det(ca, cb);
					if ((u >= -eps) && (v >= -eps) && (u + v <= 1 + eps))
					{
						/* inside triangle */
						//todo: interpolate
						//float z = u * z0 + v * z1 + (1 - u - v) * z2;
						//draw pixel
						yield return new Fragment(x, y);
					}
				}
			}
		}

		private static int Det(Vector<int> a, Vector<int> b)
		{
			return a[0] * b[1] - b[0] * a[1];
		}

		private static float Det(Vector2 a, Vector2 b)
		{
			return a.X * b.Y - b.X * a.Y;
		}

		private static Vector3 PerspectiveDivide(Vector4 position)
		{
			return Vector3.Divide(position.XYZ(), position.W);
		}
	}
}
