namespace FFmpeg.AutoGen
{
	public struct AVOption
	{
		public unsafe sbyte* name;

		public unsafe sbyte* help;

		public int offset;

		public AVOptionType type;

		public default_val default_val;

		public double min;

		public double max;

		public int flags;

		public unsafe sbyte* unit;
	}
}
