using System;

namespace FFmpeg.AutoGen
{
	public struct AVHWAccel
	{
		public unsafe sbyte* name;

		public AVMediaType type;

		public AVCodecID id;

		public AVPixelFormat pix_fmt;

		public int capabilities;

		public unsafe AVHWAccel* next;

		public IntPtr alloc_frame;

		public IntPtr start_frame;

		public IntPtr decode_slice;

		public IntPtr end_frame;

		public int frame_priv_data_size;

		public IntPtr decode_mb;

		public IntPtr init;

		public IntPtr uninit;

		public int priv_data_size;
	}
}
