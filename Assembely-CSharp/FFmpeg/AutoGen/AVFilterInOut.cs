namespace FFmpeg.AutoGen
{
	public struct AVFilterInOut
	{
		public unsafe sbyte* name;

		public unsafe AVFilterContext* filter_ctx;

		public int pad_idx;

		public unsafe AVFilterInOut* next;
	}
}
