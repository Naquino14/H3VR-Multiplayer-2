namespace FFmpeg.AutoGen
{
	public struct AVSubtitle
	{
		public ushort format;

		public uint start_display_time;

		public uint end_display_time;

		public uint num_rects;

		public unsafe AVSubtitleRect** rects;

		public long pts;
	}
}
