using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RenderPipeline
{
	class RenderDevice
	{
		public RenderDevice(int width, int height)
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
		public Dictionary<string, object> Uniforms = new Dictionary<string, object>();

		public int CreateBuffer(Array data)
		{
			bufferObjects.Add(data);
			return bufferObjects.Count - 1;
		}

		internal void DrawTrianglesIndexed(int indexBuffer, int[] attributeBuffers)
		{
			var triangles = new List<Triangle>();
			var vertexShaderOutput = new Vertex[3];
			int i = 0;
			foreach (uint index in bufferObjects[indexBuffer])
			{
				var vertexShaderInputs = new Vertex(attributeBuffers.Select(id => bufferObjects[id].GetValue(index)));
				vertexShaderOutput[i] = ApplyVertexShader(Uniforms, vertexShaderInputs); // on the GPU this can be done in parallel
				++i;
				if (3 == i)
				{
					i = 0;
					//vertex assembler emits a primitive
					triangles.Add(new Triangle(vertexShaderOutput));
				}
			}
			var tessTris = triangles.SelectMany(triangle => ApplyTessellationShader(triangle)); //on GPU parallel
			var geomTris = tessTris.SelectMany(triangle => ApplyGeometryShader(triangle)); //on GPU parallel
			var fragments = geomTris.SelectMany(triangle => RasterizeTriangle(triangle)); //on GPU parallel
			foreach (var fragment in fragments)
			{
				//TODO: blending
				FrameBuffer[fragment.X, fragment.Y] = ApplyFragmentShader(fragment);
			}
		}

		internal void Test(int indexBufferId, int[] attributeBufferId)
		{
			var attributeBuffers = attributeBufferId.Select(id => bufferObjects[id]);
			var indices = bufferObjects[indexBufferId] as int[];
//			indices.Select(index => ExecuteVertexShader(attributeBuffers.Select(attributeBuffer => attributeBuffer.GetValue(index)).ToArray()));
			var vertexShaderOutputs = new List<object[]>(indices.Length);
			foreach (int index in indices)
			{
				var attributes = attributeBuffers.Select(attributeBuffer => attributeBuffer.GetValue(index)).ToArray();
				vertexShaderOutputs.Add(ExecuteVertexShader(attributes));
			}
		}

		private object[] ExecuteVertexShader(object[] attributes)
		{
			var camera = (Matrix4x4)Uniforms["camera"];
			var position = new Vector4((Vector3)attributes[0], 1f);
			var color = attributes[1];
			
			return new object[] { position, color };
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

		private static Vertex ApplyVertexShader(IReadOnlyDictionary<string, object> uniforms, Vertex vertex)
		{
			var camera = (Matrix4x4)uniforms["camera"];
			var position = new Vector4(vertex.GetAttribute<Vector3>(0), 1f);
			position = Vector4.Transform(position, camera);
			var color = vertex.GetAttribute<Vector4>(1);
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
						float z = triangle.InterpolateZ(u, v);
						if (Zbuffer[x, y] < z)
						{
							Zbuffer[x, y] = z;
							//create fragment
							var fragment = new Fragment(x, y, triangle.InterpolateAttributes(u, v));
							yield return fragment;
						}
					}
				}
			}
		}
	}
}
