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
			var bytes = File.ReadAllBytes(fileName);
			var mesh = Obj2Mesh.FromObj(bytes);
			mesh = mesh.Transform(Transformation.Combine(Transformation.Rotation(180f, Axis.Y), Transformation.Scale(0.7f)));
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