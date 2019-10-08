using System.IO;
using System.Linq;
using System.Numerics;
using Zenseless.Geometry;

namespace RenderPipeline
{
	internal class FileDrawable
	{
		private readonly int indices;
		private readonly int attributeColor;
		private readonly int attributePosition;

		public FileDrawable(string fileName, RenderDevice renderer)
		{
			var bytes = File.ReadAllBytes(fileName);
			var mesh = Obj2Mesh.FromObj(bytes);
			indices = renderer.CreateBuffer(mesh.IDs.ToArray());
			var positions = mesh.GetAttribute("position").ToArray();
			var normals = mesh.GetAttribute("normal").GetList<Vector3>();
			attributePosition = renderer.CreateBuffer(positions);

			var normalsAsColors = normals.Select(n => new Vector4(n, 1));
			attributeColor = renderer.CreateBuffer(normalsAsColors.ToArray());
		}

		internal void Draw(RenderDevice renderer)
		{
			renderer.DrawTrianglesIndexed(indices, new int[] { attributePosition, attributeColor });
		}
	}
}