namespace FFmpeg.AutoGen
{
	public struct AVFilterContext
	{
		public unsafe AVClass* av_class;

		public unsafe AVFilter* filter;

		public unsafe sbyte* name;

		public unsafe AVFilterPad* input_pads;

		public unsafe AVFilterLink** inputs;

		public uint nb_inputs;

		public unsafe AVFilterPad* output_pads;

		public unsafe AVFilterLink** outputs;

		public uint nb_outputs;

		public unsafe void* priv;

		public unsafe AVFilterGraph* graph;

		public int thread_type;

		public unsafe AVFilterInternal* @internal;

		public unsafe AVFilterCommand* command_queue;

		public unsafe sbyte* enable_str;

		public unsafe void* enable;

		public unsafe double* var_values;

		public int is_disabled;
	}
}
