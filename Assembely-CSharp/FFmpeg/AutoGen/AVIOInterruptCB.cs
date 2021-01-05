using System;

namespace FFmpeg.AutoGen
{
	public struct AVIOInterruptCB
	{
		public IntPtr callback;

		public unsafe void* opaque;
	}
}
