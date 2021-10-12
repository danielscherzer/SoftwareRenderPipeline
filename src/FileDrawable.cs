using System.IO;
using System.Linq;
using System.Numerics;
using Zenseless.Geometry;

namespace RenderPipeline
{
	internal class FileDrawable
	{
		private readonly Handle indices;
		private readonly Handle attributeColor;
		private readonly Handle attributePosition;

		public FileDrawable(string fileName, RenderDevice renderer)
		{
			byte[] bytes = File.ReadAllBytes(fileName);
			DefaultMesh mesh = Obj2Mesh.FromObj(bytes);
			indices = renderer.CopyToVideoRAM(mesh.IDs.ToArray());
			var positions = mesh.GetAttribute("position").ToArray();
			var normals = mesh.GetAttribute("normal").GetList<Vector3>();
			attributePosition = renderer.CopyToVideoRAM(positions);

			var normalsAsColors = normals.Select(n => new Vector4(n, 1));
			attributeColor = renderer.CopyToVideoRAM(normalsAsColors.ToArray());
		}

		internal void Draw(RenderDevice renderer)
		{
			renderer.DrawTrianglesIndexed(indices, new Handle[] { attributePosition, attributeColor });
		}
	}
}