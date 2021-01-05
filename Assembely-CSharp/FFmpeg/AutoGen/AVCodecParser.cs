using System;

namespace FFmpeg.AutoGen
{
	public struct AVCodecParser
	{
		public unsafe fixed int codec_ids[5];

		public int priv_data_size;

		public IntPtr parser_init;

		public IntPtr parser_parse;

		public IntPtr parser_close;

		public IntPtr split;

		public unsafe AVCodecParser* next;
	}
}
