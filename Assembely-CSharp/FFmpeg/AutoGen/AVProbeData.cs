namespace FFmpeg.AutoGen
{
	public struct AVProbeData
	{
		public unsafe sbyte* filename;

		public unsafe sbyte* buf;

		public int buf_size;

		public unsafe sbyte* mime_type;
	}
}
