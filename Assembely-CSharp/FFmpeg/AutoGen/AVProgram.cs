namespace FFmpeg.AutoGen
{
	public struct AVProgram
	{
		public int id;

		public int flags;

		public AVDiscard discard;

		public unsafe uint* stream_index;

		public uint nb_stream_indexes;

		public unsafe AVDictionary* metadata;

		public int program_num;

		public int pmt_pid;

		public int pcr_pid;

		public long start_time;

		public long end_time;

		public long pts_wrap_reference;

		public int pts_wrap_behavior;
	}
}
