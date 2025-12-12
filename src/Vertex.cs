using System;
using System.Collections.Generic;
using System.Numerics;

namespace RenderPipeline;

public class Vertex
{
	public Vertex(Vector4 position)
	{
		Attributes =
		[
			position
		];
	}

	public Vertex(IEnumerable<object> attributes)
	{
		Attributes = [.. attributes];
	}

	public TYPE GetAttribute<TYPE>(int index)
	{
		var attrib = Attributes[index];
		if (attrib is not TYPE) throw new ArgumentException($"Given argument type {typeof(TYPE).Name} incompatible with real type {attrib.GetType().Name}");
		return (TYPE)attrib;
	}

	public Vector4 Position { get => (Vector4)Attributes[0]; set => Attributes[0] = value; }

	public override string ToString() => $"{Position.XYZ()}";

	public IList<object> Attributes { get; }
}