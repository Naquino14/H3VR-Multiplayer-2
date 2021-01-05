namespace FFmpeg.AutoGen
{
	public struct AVPixFmtDescriptor
	{
		public unsafe sbyte* name;

		public sbyte nb_components;

		public sbyte log2_chroma_w;

		public sbyte log2_chroma_h;

		public ulong flags;

		public AVComponentDescriptor comp0;

		public AVComponentDescriptor comp1;

		public AVComponentDescriptor comp2;

		public AVComponentDescriptor comp3;

		public unsafe sbyte* alias;
	}
}
