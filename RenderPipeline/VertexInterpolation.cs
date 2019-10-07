using System.Collections.Generic;
using System.Numerics;

namespace RenderPipeline
{
	public static class VertexInterpolation
	{
		internal static List<object> InterpolateAttributes(float u, float v, Vertex a, Vertex b, Vertex c)
		{
			var outputAttributes = new List<object>();
			var count = a.Attributes.Count;
			for (int i = 1; i < count; ++i)
			{
				var aAttr = a.Attributes[i];
				var bAttr = b.Attributes[i];
				var cAttr = c.Attributes[i];

				switch (aAttr)
				{
					case float attrib:
						outputAttributes.Add(Barycentric.Interpolate(u, v, (float)aAttr, (float)bAttr, (float)cAttr));
						break;
					case Vector2 attrib:
						outputAttributes.Add(Barycentric.Interpolate(u, v, (Vector2)aAttr, (Vector2)bAttr, (Vector2)cAttr));
						break;
					case Vector3 attrib:
						outputAttributes.Add(Barycentric.Interpolate(u, v, (Vector3)aAttr, (Vector3)bAttr, (Vector3)cAttr));
						break;
					case Vector4 attrib:
						outputAttributes.Add(Barycentric.Interpolate(u, v, (Vector4)aAttr, (Vector4)bAttr, (Vector4)cAttr));
						break;
				}
			}
			return outputAttributes;
		}

		internal static float InterpolateZ(float u, float v, Vertex a, Vertex b, Vertex c) => Barycentric.Interpolate(u, v, a.Position.Z, b.Position.Z, c.Position.Z);
	}
}
