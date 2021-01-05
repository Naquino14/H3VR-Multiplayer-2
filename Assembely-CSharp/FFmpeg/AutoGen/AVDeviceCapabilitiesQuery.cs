namespace FFmpeg.AutoGen
{
	public struct AVDeviceCapabilitiesQuery
	{
		public unsafe AVClass* av_class;

		public unsafe AVFormatContext* device_context;

		public AVCodecID codec;

		public AVSampleFormat sample_format;

		public AVPixelFormat pixel_format;

		public int sample_rate;

		public int channels;

		public long channel_layout;

		public int window_width;

		public int window_height;

		public int frame_width;

		public int frame_height;

		public AVRational fps;
	}
}
