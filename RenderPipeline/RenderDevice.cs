﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RenderPipeline
{
	class RenderDevice
	{
		public delegate Vertex VertexShaderDelegate(IReadOnlyDictionary<string, object> uniforms, Vertex vertex);
		public delegate IEnumerable<Triangle> GeometryShaderDelegate(IReadOnlyDictionary<string, object> uniforms, Triangle triangle);
		public delegate IEnumerable<Triangle> TessellationShaderDelegate(IReadOnlyDictionary<string, object> uniforms, Triangle triangle);
		public delegate Vector4 FragmentShaderDelegate(IReadOnlyDictionary<string, object> uniforms, Fragment fragment);

		public RenderDevice(int width, int height)
		{
			FrameBuffer = new Buffer2D<Vector4>(width, height);
			FrameBuffer.Clear(new Vector4(0, 0, 0, 1));
			Zbuffer = new Buffer2D<float>(width, height);
			ViewPort = new ViewPort(0, 0, FrameBuffer.Width, FrameBuffer.Height, 0, -1);
			Zbuffer.Clear(ViewPort.MaxDepth);
		}

		public Buffer2D<Vector4> FrameBuffer { get; }
		public Buffer2D<float> Zbuffer { get; }
		public ViewPort ViewPort { get; set; }


		public RenderState RenderState { get; set; } = new RenderState();

		public Handle CopyToVideoRAM(Array data)
		{
			bufferObjects.Add(data);
			return new Handle(bufferObjects.Count - 1);
		}

		public void DrawTrianglesIndexed(Handle indexBuffer, Handle[] attributeBuffers)
		{
			//most of the following operations can be done in parallel
			
			//extract vertices out of the input buffers
			var vertexShaderInputStream = InputAssembler(indexBuffer, attributeBuffers);
			
			//execute vertex shader on each input vertex
			var vertexShaderOutputStream = vertexShaderInputStream.Select(inputVertex => RenderState.VertexShader(RenderState.Uniforms, inputVertex));
			
			// primitive assembly
			var triangles = PrimitiveAssemblyTriangle(vertexShaderOutputStream);
			
			//execute tessellation shader
			triangles = triangles.SelectMany(triangle => RenderState.TessellationShader(RenderState.Uniforms, triangle));

			//execute geometry shader
			triangles = triangles.SelectMany(triangle => RenderState.GeometryShader(RenderState.Uniforms, triangle));

			//TODO: here would come geometrical clipping

			//perspective division
			triangles = triangles.Select(triangle => PerspectiveDivide(triangle));

			// Back face culling would come here.

			// Transform from clip space to screen space. (view-port transform)
			triangles = triangles.Select(triangle => ViewportTransform(triangle));

			var fragments = triangles.SelectMany(triangle => RasterizeTriangle(triangle));
			foreach (var fragment in fragments)
			{
				var color = RenderState.FragmentShader(RenderState.Uniforms, fragment);
				
				//TODO: blending
				//if(RenderState.Blending)

				FrameBuffer[fragment.X, fragment.Y] = color;
			}
		}

		private readonly List<Array> bufferObjects = new List<Array>();

		/// <summary>
		/// Generates a stream of vertices out of the input geometry
		/// </summary>
		/// <param name="indexBuffer"></param>
		/// <param name="attributeBuffers"></param>
		/// <returns></returns>
		private IEnumerable<Vertex> InputAssembler(Handle indexBuffer, Handle[] attributeBuffers)
		{
			foreach (uint index in bufferObjects[indexBuffer.Value])
			{
				yield return new Vertex(attributeBuffers.Select(handle => bufferObjects[handle.Value].GetValue(index)));
			}
		}

		/// <summary>
		/// Create triangles out of a stream of vertices
		/// </summary>
		/// <param name="vertexShaderOutputStream"></param>
		/// <returns></returns>
		private IEnumerable<Triangle> PrimitiveAssemblyTriangle(IEnumerable<Vertex> vertexShaderOutputStream)
		{
			var it = vertexShaderOutputStream.GetEnumerator();
			var triangleVertices = new Vertex[3];
			while (it.MoveNext())
			{
				triangleVertices[0] = it.Current;
				it.MoveNext();
				triangleVertices[1] = it.Current;
				it.MoveNext();
				triangleVertices[2] = it.Current;
				//primitive assembler emits a triangle primitive
				yield return new Triangle(triangleVertices);
			}
		}

		private Triangle PerspectiveDivide(Triangle triangle)
		{
			for (int i = 0; i < 3; ++i)
			{
				var position = triangle[i].Position;
				triangle[i].Position = Vector4.Divide(position, position.W);
			}
			return triangle;
		}

		private Triangle ViewportTransform(Triangle triangle)
		{
			for (int i = 0; i < 3; ++i)
			{
				triangle[i].Position = ViewPort.Transform(triangle[i].Position);
			}
			return triangle;
		}

		private IEnumerable<Fragment> RasterizeTriangle(Triangle triangle)
		{
			// get screen coordinates
			var p = new Vector2[3];
			for (int i = 0; i < 3; ++i)
			{
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
