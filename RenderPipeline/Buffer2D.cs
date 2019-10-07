using System;
using System.Collections;
using System.Collections.Generic;

namespace RenderPipeline
{
	public class Buffer2D<TPixel> : IEnumerable<TPixel> where TPixel : struct
	{
		public Buffer2D(int width, int height)
		{
			Width = width;
			Height = height;
			frameBuffer = new TPixel[width * height];
		}

		public void Clear(TPixel value)
		{
			Array.Fill(frameBuffer, value);
		}

		public TPixel this[int x, int y]
		{
			get => Lookup(x, y);
			set => Lookup(x, y) = value;
		}

		public int Width { get; }

		public int Height { get; }

		private readonly TPixel[] frameBuffer;

		private ref TPixel Lookup(int x, int y) => ref frameBuffer[x + y * Width];

		public IEnumerator<TPixel> GetEnumerator()
		{
			foreach (var color in frameBuffer)
			{
				yield return color;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
