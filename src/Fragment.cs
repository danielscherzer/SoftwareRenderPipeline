using System.Collections.Generic;

namespace RenderPipeline;

internal class Fragment(int x, int y, List<object> attributes)
{
	public int X { get; } = x;
	public int Y { get; } = y;

	public IReadOnlyList<object> Attributes { get; } = attributes;
}