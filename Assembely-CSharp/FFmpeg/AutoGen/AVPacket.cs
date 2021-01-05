namespace FFmpeg.AutoGen
{
	public struct AVPacket
	{
		public unsafe AVBufferRef* buf;

		public long pts;

		public long dts;

		public unsafe sbyte* data;

		public int size;

		public int stream_index;

		public int flags;

		public unsafe AVPacketSideData* side_data;

		public int side_data_elems;

		public long duration;

		public long pos;

		public long convergence_duration;
	}
}
