namespace FFmpeg.AutoGen
{
	public struct AVABufferSinkParams
	{
		public unsafe AVSampleFormat* sample_fmts;

		public unsafe long* channel_layouts;

		public unsafe int* channel_counts;

		public int all_channel_counts;

		public unsafe int* sample_rates;
	}
}
