namespace FFmpeg.AutoGen
{
	public struct AVIODirEntry
	{
		public unsafe sbyte* name;

		public int type;

		public int utf8;

		public long size;

		public long modification_timestamp;

		public long access_timestamp;

		public long status_change_timestamp;

		public long user_id;

		public long group_id;

		public long filemode;
	}
}
