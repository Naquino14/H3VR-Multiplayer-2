using System;

namespace FFmpeg.AutoGen
{
	public struct AVCodec
	{
		public unsafe sbyte* name;

		public unsafe sbyte* long_name;

		public AVMediaType type;

		public AVCodecID id;

		public int capabilities;

		public unsafe AVRational* supported_framerates;

		public unsafe AVPixelFormat* pix_fmts;

		public unsafe int* supported_samplerates;

		public unsafe AVSampleFormat* sample_fmts;

		public unsafe ulong* channel_layouts;

		public sbyte max_lowres;

		public unsafe AVClass* priv_class;

		public unsafe AVProfile* profiles;

		public int priv_data_size;

		public unsafe AVCodec* next;

		public IntPtr init_thread_copy;

		public IntPtr update_thread_context;

		public unsafe AVCodecDefault* defaults;

		public IntPtr init_static_data;

		public IntPtr init;

		public IntPtr encode_sub;

		public IntPtr encode2;

		public IntPtr decode;

		public IntPtr close;

		public IntPtr flush;

		public int caps_internal;
	}
}
