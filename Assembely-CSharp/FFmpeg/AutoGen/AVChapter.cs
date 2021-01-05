namespace FFmpeg.AutoGen
{
	public struct AVChapter
	{
		public int id;

		public AVRational time_base;

		public long start;

		public long end;

		public unsafe AVDictionary* metadata;
	}
}
