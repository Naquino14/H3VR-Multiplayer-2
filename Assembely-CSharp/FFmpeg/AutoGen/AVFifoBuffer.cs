namespace FFmpeg.AutoGen
{
	public struct AVFifoBuffer
	{
		public unsafe sbyte* buffer;

		public unsafe sbyte* rptr;

		public unsafe sbyte* wptr;

		public unsafe sbyte* end;

		public uint rndx;

		public uint wndx;
	}
}
