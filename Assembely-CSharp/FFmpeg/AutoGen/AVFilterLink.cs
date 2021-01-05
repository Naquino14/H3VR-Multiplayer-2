namespace FFmpeg.AutoGen
{
	public struct AVFilterLink
	{
		public unsafe AVFilterContext* src;

		public unsafe AVFilterPad* srcpad;

		public unsafe AVFilterContext* dst;

		public unsafe AVFilterPad* dstpad;

		public AVMediaType type;

		public int w;

		public int h;

		public AVRational sample_aspect_ratio;

		public ulong channel_layout;

		public int sample_rate;

		public int format;

		public AVRational time_base;

		public unsafe AVFilterFormats* in_formats;

		public unsafe AVFilterFormats* out_formats;

		public unsafe AVFilterFormats* in_samplerates;

		public unsafe AVFilterFormats* out_samplerates;

		public unsafe AVFilterChannelLayouts* in_channel_layouts;

		public unsafe AVFilterChannelLayouts* out_channel_layouts;

		public int request_samples;

		public init_state init_state;

		public unsafe AVFilterGraph* graph;

		public long current_pts;

		public long current_pts_us;

		public int age_index;

		public AVRational frame_rate;

		public unsafe AVFrame* partial_buf;

		public int partial_buf_size;

		public int min_samples;

		public int max_samples;

		public int status;

		public int channels;

		public uint flags;

		public long frame_count;

		public unsafe void* video_frame_pool;

		public int frame_wanted_in;

		public int frame_wanted_out;
	}
}
