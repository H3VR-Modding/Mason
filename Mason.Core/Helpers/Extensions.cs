using System;
using System.IO;
using Mason.Core.Markup;
using YamlDotNet.Core;

namespace Mason.Core
{
	public static class Extensions
	{
		public static void CopyTo(this Stream @this, Stream other)
		{
			@this.CopyTo(other, @this.GetCopyBufferSize());
		}

		// Plucked from net5.0 Stream.GetCopyBufferSize()
		private static int GetCopyBufferSize(this Stream @this)
		{
			int bufferSize = 20 * 4096;

			if (@this.CanSeek)
			{
				long length = @this.Length;
				long position = @this.Position;
				if (length <= position)
				{
					bufferSize = 1;
				}
				else
				{
					long remaining = length - position;
					if (remaining > 0)
						bufferSize = (int) Math.Min(bufferSize, remaining);
				}
			}

			return bufferSize;
		}

		// Plucked from net5.0 Stream.CopyTo(Stream,int)
		private static void CopyTo(this Stream @this, Stream other, int bufferSize)
		{
			var buffer = new byte[bufferSize];
			int read;

			while ((read = @this.Read(buffer, 0, buffer.Length)) != 0)
				other.Write(buffer, 0, read);
		}

		public static MarkupRange GetRange(this YamlException @this)
		{
			return new(@this.Start.GetIndex(), @this.End.GetIndex());
		}
	}
}
