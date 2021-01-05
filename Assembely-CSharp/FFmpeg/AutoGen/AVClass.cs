using System;

namespace FFmpeg.AutoGen
{
	public struct AVClass
	{
		public unsafe sbyte* class_name;

		public IntPtr item_name;

		public unsafe AVOption* option;

		public int version;

		public int log_level_offset_offset;

		public int parent_log_context_offset;

		public IntPtr child_next;

		public IntPtr child_class_next;

		public AVClassCategory category;

		public IntPtr get_category;

		public IntPtr query_ranges;
	}
}
