using System.Collections.Generic;

namespace RenderPipeline
{
	internal class Fragment
	{
		public Fragment(int x, int y, List<object> attributes)
		{
			X = x;
			Y = y;
			Attributes = attributes;
		}

		public int X { get; }
		public int Y { get; }

		public IReadOnlyList<object> Attributes { get; }
	}
}