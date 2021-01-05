namespace FFmpeg.AutoGen
{
	public struct AVFrameSideData
	{
		public AVFrameSideDataType type;

		public unsafe sbyte* data;

		public int size;

		public unsafe AVDictionary* metadata;

		public unsafe AVBufferRef* buf;
	}
}
