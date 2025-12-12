using System;
using System.IO;
using System.Reflection;
using Zenseless.Resources;

internal class Resource
{
	internal static byte[] Load(string name)
	{
		using var stream = resDir.Open(name);
		byte[] bytes = new byte[stream.Length];
		stream.ReadExactly(bytes, 0, bytes.Length);
		return bytes;
	}

	private static readonly string assemblyName = Assembly.GetExecutingAssembly().Location;
	public static readonly string AssemblyDir = Path.GetDirectoryName(assemblyName) ?? assemblyName;
	private static IResourceDirectory resDir = new ShortestMatchResourceDirectory(new EmbeddedResourceDirectory());
}