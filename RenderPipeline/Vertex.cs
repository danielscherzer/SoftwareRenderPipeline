﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RenderPipeline
{
	public class Vertex
	{
		public Vertex(Vector4 position)
		{
			Attributes = new List<object>
			{
				position
			};
		}

		public Vertex(IEnumerable<object> attributes)
		{
			Attributes = attributes.ToList();
		}
		//public ref TYPE GetAttribute<TYPE>(int index)

		public Vector4 Position { get => (Vector4)Attributes[0]; set => Attributes[0] = value; }

		public override string ToString() => $"{Position.XYZ()}";

		public List<object> Attributes { get; }
	}
}