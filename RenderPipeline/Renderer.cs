using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RenderPipeline
{
	class Renderer
	{
		public Renderer(int width, int height)
		{
			FrameBuffer = new Buffer2D<Vector4>(width, height);
			FrameBuffer.Clear(new Vector4(0, 0, 0, 1));
			Zbuffer = new Buffer2D<float>(width, height);
			ViewPort = new ViewPort(0, 0, FrameBuffer.Width, FrameBuffer.Height, 0, -1);
			Zbuffer.Clear(ViewPort.MaxDepth);
		}

		public Buffer2D<Vector4> FrameBuffer { get; }
		public ViewPort ViewPort { get; }
		public Buffer2D<float> Zbuffer { get; }

		public int CreateBuffer<TYPE>(TYPE[] data) where TYPE : struct
		{
			bufferObjects.Add(data);
			return bufferObjects.Count - 1;
		}

		internal void DrawTrianglesIndexed(int indexBuffer, int[] attributeBuffers)
		{
			var vertexShaderOutput = new Vertex[3];
			int i = 0;
			foreach (int index in bufferObjects[indexBuffer])
			{
				var vertexShaderInputs = new Vertex(attributeBuffers.Select(id => bufferObjects[id].GetValue(index)));
				vertexShaderOutput[i] = ApplyVertexShader(vertexShaderInputs); // on the GPU this can be done in parallel
				++i;
				if (3 == i)
				{
					i = 0;
					//TODO: VertexAssembler emits a primitive
					foreach (var fragment in RasterizeTriangle(new Triangle(vertexShaderOutput)))
					{
						//TODO: blending
						FrameBuffer[fragment.X, fragment.Y] = ApplyFragmentShader(fragment);
					}
				}
			}
		}

		private readonly List<Array> bufferObjects = new List<Array>();

		private Vector4 ApplyFragmentShader(Fragment fragment)
		{
			var color = (Vector4)fragment.Attributes[0];
			return color;
		}

		private static IEnumerable<Triangle> ApplyGeometryShader(Triangle triangle)
		{
			yield return triangle;
		}

		private static IEnumerable<Triangle> ApplyTessellationShader(Triangle triangle)
		{
			yield return triangle;
		}

		private static Vertex ApplyVertexShader(Vertex vertex)
		{
			
			var position = new Vector4((Vector3)vertex.Attributes[0], 1f);
			var color = (Vector4)vertex.Attributes[1];
			return new Vertex(new object[] { position, color });
		}

		private IEnumerable<Fragment> RasterizeTriangle(Triangle triangle)
		{
			// perspective division
			for (int i = 0; i < 3; ++i)
			{
				var position = triangle[i].Position;
				triangle[i].Position = Vector4.Divide(position, position.W);
			}

			// Back face culling would come here.

			// Transform from clip space to screen space. (view-port transform)
			var p = new Vector2[3];
			for (int i = 0; i < 3; ++i)
			{
				triangle[i].Position = ViewPort.Transform(triangle[i].Position);
				p[i] = triangle[i].Position.XY();
			}

			// get the bounding box of the 2D triangle
			var min = Vector2.Min(p[0], Vector2.Min(p[1], p[2]));
			var max = Vector2.Max(p[0], Vector2.Max(p[1], p[2]));

			// Clipping of box to view-port bounds
			var viewPortMin = new Vector2(ViewPort.TopLeftX, ViewPort.TopLeftY);
			var viewPortMax = viewPortMin + new Vector2(ViewPort.Width, ViewPort.Height) - Vector2.One;
			min = Vector2.Max(min, viewPortMin);
			max = Vector2.Min(max, viewPortMax);

			// rasterize - triangle setup
			float Det(Vector2 a, Vector2 b) => a.X * b.Y - b.X * a.Y;

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
						float z = Barycentric.Interpolate(u, v, triangle[0].Position.Z, triangle[1].Position.Z, triangle[2].Position.Z);
						if (Zbuffer[x, y] < z)
						{
							Zbuffer[x, y] = z;
							//create fragment
							var fragment = new Fragment(x, y);
							fragment.Attributes = triangle.Interpolate(u, v);
							yield return fragment;
						}
					}
				}
			}
		}
	}
}
