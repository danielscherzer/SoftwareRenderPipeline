using System.Collections.Generic;
using System.Numerics;

namespace RenderPipeline
{
	class Renderer
	{
		public Renderer(int width, int height)
		{
			FrameBuffer = new FrameBuffer<Vector4>(width, height);
			zBuffer = new FrameBuffer<float>(width, height);
			viewPort = new ViewPort(0, 0, FrameBuffer.Width, FrameBuffer.Height, 0, 1);
		}
		public FrameBuffer<Vector4> FrameBuffer { get; }
		private FrameBuffer<float> zBuffer { get; }

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
						//TODO: blending
						FrameBuffer[fragment.X, fragment.Y] = new Vector4(1, 0.5f, 0.5f, 1);
				}
			}
		}

		private readonly ViewPort viewPort;

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
			// perspective division
			for (int i = 0; i < 3; ++i)
			{
				triangle[i].Position = triangle[i].PerspectiveDivide();
			}

			// Back face culling would come here.

			// Transform from clip space to screen space. (view-port transform)
			var p = new Vector2[3];
			for (int i = 0; i < 3; ++i)
			{
				triangle[i].Position = viewPort.Transform(triangle[i].Position);
				p[i] = triangle[i].Position.XY();
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
			// rasterization
			for (int x = (int)min.X; x <= (int)max.X; x++)
			{
				for (int y = (int)min.Y; y <= (int)max.Y; y++)
				{
					var pos = new Vector2(x, y);
					var cp = pos - p[2];
					var u = fact * Det(cp, cb);
					var v = fact * Det(ca, cp);
					if ((u >= -eps) && (v >= -eps) && (u + v <= 1 + eps))
					{
						/* inside triangle */
						//early z-test
						float z = triangle.InterpolateZ(u, v);
						//if (zBuffer[x, y] < z)
						{
							//create fragment
							yield return new Fragment(x, y);
						}
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
