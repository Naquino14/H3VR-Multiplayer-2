namespace FFmpeg.AutoGen
{
	public struct AVDeviceInfoList
	{
		public unsafe AVDeviceInfo** devices;

		public int nb_devices;

		public int default_device;
	}
}
