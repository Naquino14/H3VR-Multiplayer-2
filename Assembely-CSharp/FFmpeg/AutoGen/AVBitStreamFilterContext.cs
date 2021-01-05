namespace FFmpeg.AutoGen
{
	public struct AVBitStreamFilterContext
	{
		public unsafe void* priv_data;

		public unsafe AVBitStreamFilter* filter;

		public unsafe AVCodecParserContext* parser;

		public unsafe AVBitStreamFilterContext* next;

		public unsafe sbyte* args;
	}
}
