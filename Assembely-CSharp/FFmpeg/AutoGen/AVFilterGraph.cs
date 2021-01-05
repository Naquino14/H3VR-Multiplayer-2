using System;

namespace FFmpeg.AutoGen
{
	public struct AVFilterGraph
	{
		public unsafe AVClass* av_class;

		public unsafe AVFilterContext** filters;

		public uint nb_filters;

		public unsafe sbyte* scale_sws_opts;

		public unsafe sbyte* resample_lavr_opts;

		public int thread_type;

		public int nb_threads;

		public unsafe AVFilterGraphInternal* @internal;

		public unsafe void* opaque;

		public IntPtr execute;

		public unsafe sbyte* aresample_swr_opts;

		public unsafe AVFilterLink** sink_links;

		public int sink_links_count;

		public uint disable_auto_convert;
	}
}
