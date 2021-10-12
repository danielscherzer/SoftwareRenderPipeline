using System;
using System.Numerics;
using Zenseless.Patterns;

namespace RenderPipeline
{
	class ColoredTrianglesDrawable
	{
		private readonly Handle<Array> attributePosition;
		private readonly Handle<Array> attributeColor;
		private readonly Handle<Array> indices;

		public ColoredTrianglesDrawable(RenderDevice renderer)
		{
			attributePosition = renderer.CopyToVideoRAM(new Vector3[]
			{
				new Vector3(0, -1f, 1f), new Vector3(1, 0.25f, 0), new Vector3(-0.75f, 1, 0),
				new Vector3(0, 0, -0.5f), new Vector3(1, 0, 0.5f), new Vector3(1, 1, 0.5f),
			});
			attributeColor = renderer.CopyToVideoRAM(new Vector4[]
			{
				new Vector4(0, 0.5f, 0.5f, 1), new Vector4(0, 0.5f, 0.5f, 1), new Vector4(0, 0.5f, 0.5f, 1),
				new Vector4(0, 0, 0.5f, 1), new Vector4(1, 0, 0.5f, 1), new Vector4(1, 1, 0.5f, 1)
			});
			indices = renderer.CopyToVideoRAM(new uint[] { 0, 1, 2, 3, 4, 5 });
		}

		public void Draw(RenderDevice renderer)
		{
			renderer.DrawTrianglesIndexed(indices, new Handle<Array>[] { attributePosition, attributeColor });
		}
	}
}
