namespace FFmpeg.AutoGen
{
	public struct AVSubtitleRect
	{
		public int x;

		public int y;

		public int w;

		public int h;

		public int nb_colors;

		public AVPicture pict;

		public unsafe sbyte* data0;

		public unsafe sbyte* data1;

		public unsafe sbyte* data2;

		public unsafe sbyte* data3;

		public unsafe fixed int linesize[4];

		public AVSubtitleType type;

		public unsafe sbyte* text;

		public unsafe sbyte* ass;

		public int flags;
	}
}
