namespace FFmpeg.AutoGen
{
	public struct AVCodecDescriptor
	{
		public AVCodecID id;

		public AVMediaType type;

		public unsafe sbyte* name;

		public unsafe sbyte* long_name;

		public int props;

		public unsafe sbyte** mime_types;

		public unsafe AVProfile* profiles;
	}
}
