using System;
using System.Numerics;
using Zenseless.Patterns;

namespace RenderPipeline;

class ColoredTrianglesDrawable(RenderDevice renderer)
{
	private readonly Handle<Array> attributePosition = renderer.CopyToVideoRAM(new Vector3[]
		{
			new(0, -1f, 1f), new(1, 0.25f, 0), new(-0.75f, 1, 0),
			new(0, 0, -0.5f), new(1, 0, 0.5f), new(1, 1, 0.5f),
		});
	private readonly Handle<Array> attributeColor = renderer.CopyToVideoRAM(new Vector4[]
		{
			new(0, 0.5f, 0.5f, 1), new(0, 0.5f, 0.5f, 1), new(0, 0.5f, 0.5f, 1),
			new(0, 0, 0.5f, 1), new(1, 0, 0.5f, 1), new(1, 1, 0.5f, 1)
		});
	private readonly Handle<Array> indices = renderer.CopyToVideoRAM(new uint[] { 0, 1, 2, 3, 4, 5 });

	public void Draw(RenderDevice renderer)
	{
		renderer.DrawTrianglesIndexed(indices, [attributePosition, attributeColor]);
	}
}
