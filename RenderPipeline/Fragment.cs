using System.Collections.Generic;

namespace RenderPipeline
{
	internal class Fragment
	{
		public Fragment(int x, int y)
		{
			X = x;
			Y = y;
		}

		public int X { get; }
		public int Y { get; }

		public List<object> Attributes;
	}
}