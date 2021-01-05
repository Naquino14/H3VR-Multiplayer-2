using System;

namespace FFmpeg.AutoGen
{
	public struct AVBitStreamFilter
	{
		public unsafe sbyte* name;

		public int priv_data_size;

		public IntPtr filter;

		public IntPtr close;

		public unsafe AVBitStreamFilter* next;
	}
}
