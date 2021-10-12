using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Zenseless.Geometry;
using Zenseless.Patterns;

namespace RenderPipeline
{
	internal class FileDrawable
	{
		private readonly Handle<Array> indices;
		private readonly Handle<Array> attributeColor;
		private readonly Handle<Array> attributePosition;

		public FileDrawable(string fileName, RenderDevice renderer)
		{
			byte[] bytes = File.ReadAllBytes(fileName);
			DefaultMesh mesh = Obj2Mesh.FromObj(bytes);
			indices = renderer.CopyToVideoRAM(mesh.IDs.ToArray());
			List<Vector3> positions = mesh.Position;
			List<Vector3> normals = mesh.Normal;
			attributePosition = renderer.CopyToVideoRAM(positions.ToArray());

			var normalsAsColors = normals.Select(n => new Vector4(n, 1));
			attributeColor = renderer.CopyToVideoRAM(normalsAsColors.ToArray());
		}

		internal void Draw(RenderDevice renderer)
		{
			renderer.DrawTrianglesIndexed(indices, new Handle<Array>[] { attributePosition, attributeColor });
		}
	}
}