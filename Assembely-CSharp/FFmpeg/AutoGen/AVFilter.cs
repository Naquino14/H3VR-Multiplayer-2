using System;

namespace FFmpeg.AutoGen
{
	public struct AVFilter
	{
		public unsafe sbyte* name;

		public unsafe sbyte* description;

		public unsafe AVFilterPad* inputs;

		public unsafe AVFilterPad* outputs;

		public unsafe AVClass* priv_class;

		public int flags;

		public IntPtr init;

		public IntPtr init_dict;

		public IntPtr uninit;

		public IntPtr query_formats;

		public int priv_size;

		public unsafe AVFilter* next;

		public IntPtr process_command;

		public IntPtr init_opaque;
	}
}
