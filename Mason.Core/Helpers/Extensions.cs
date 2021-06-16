using System;
using System.IO;

namespace Mason.Core
{
	public static class Extensions
	{
		public static void CopyTo(this Stream @this, Stream other) => @this.CopyTo(other, @this.GetCopyBufferSize());

		// Plucked from net5.0 Stream.GetCopyBufferSize()
		private static int GetCopyBufferSize(this Stream @this)
		{
			var bufferSize = 20 * 4096;

			if (@this.CanSeek)
			{
				var length = @this.Length;
				var position = @this.Position;
				if (length <= position)
				{
					bufferSize = 1;
				}
				else
				{
					var remaining = length - position;
					if (remaining > 0)
					{
						bufferSize = (int) Math.Min(bufferSize, remaining);
					}
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
	}
}
