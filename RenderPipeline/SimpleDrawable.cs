using System.Numerics;

namespace RenderPipeline
{
	class SimpleDrawable
	{
		private readonly int attributePosition;
		private readonly int attributeColor;
		private readonly int indices;

		public SimpleDrawable(RenderDevice renderer)
		{
			attributePosition = renderer.CreateBuffer(new Vector3[]
			{
				new Vector3(0, -1f, 0), new Vector3(1, 0.25f, 0), new Vector3(-0.75f, 1, 0),
				new Vector3(0, 0, -0.5f), new Vector3(1, 0, 0.5f), new Vector3(1, 1, 0.5f),
			});
			attributeColor = renderer.CreateBuffer(new Vector4[]
			{
				new Vector4(0, 0.5f, 0.5f, 1), new Vector4(0, 0.5f, 0.5f, 1), new Vector4(0, 0.5f, 0.5f, 1),
				new Vector4(0, 0, 0.5f, 1), new Vector4(1, 0, 0.5f, 1), new Vector4(1, 1, 0.5f, 1)
			});
			indices = renderer.CreateBuffer(new uint[] { 0, 1, 2, 3, 4, 5 });
		}

		public void Draw(RenderDevice renderer)
		{
			renderer.DrawTrianglesIndexed(indices, new int[] { attributePosition, attributeColor });
		}
	}
}
