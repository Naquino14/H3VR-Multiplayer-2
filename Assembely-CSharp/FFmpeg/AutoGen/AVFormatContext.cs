using System;

namespace FFmpeg.AutoGen
{
	public struct AVFormatContext
	{
		public unsafe AVClass* av_class;

		public unsafe AVInputFormat* iformat;

		public unsafe AVOutputFormat* oformat;

		public unsafe void* priv_data;

		public unsafe AVIOContext* pb;

		public int ctx_flags;

		public uint nb_streams;

		public unsafe AVStream** streams;

		public unsafe fixed sbyte filename[1024];

		public long start_time;

		public long duration;

		public long bit_rate;

		public uint packet_size;

		public int max_delay;

		public int flags;

		public long probesize;

		public long max_analyze_duration;

		public unsafe sbyte* key;

		public int keylen;

		public uint nb_programs;

		public unsafe AVProgram** programs;

		public AVCodecID video_codec_id;

		public AVCodecID audio_codec_id;

		public AVCodecID subtitle_codec_id;

		public uint max_index_size;

		public uint max_picture_buffer;

		public uint nb_chapters;

		public unsafe AVChapter** chapters;

		public unsafe AVDictionary* metadata;

		public long start_time_realtime;

		public int fps_probe_size;

		public int error_recognition;

		public AVIOInterruptCB interrupt_callback;

		public int debug;

		public long max_interleave_delta;

		public int strict_std_compliance;

		public int event_flags;

		public int max_ts_probe;

		public int avoid_negative_ts;

		public int ts_id;

		public int audio_preload;

		public int max_chunk_duration;

		public int max_chunk_size;

		public int use_wallclock_as_timestamps;

		public int avio_flags;

		public AVDurationEstimationMethod duration_estimation_method;

		public long skip_initial_bytes;

		public uint correct_ts_overflow;

		public int seek2any;

		public int flush_packets;

		public int probe_score;

		public int format_probesize;

		public unsafe sbyte* codec_whitelist;

		public unsafe sbyte* format_whitelist;

		public unsafe AVFormatInternal* @internal;

		public int io_repositioned;

		public unsafe AVCodec* video_codec;

		public unsafe AVCodec* audio_codec;

		public unsafe AVCodec* subtitle_codec;

		public unsafe AVCodec* data_codec;

		public int metadata_header_padding;

		public unsafe void* opaque;

		public IntPtr control_message_cb;

		public long output_ts_offset;

		public unsafe sbyte* dump_separator;

		public AVCodecID data_codec_id;

		public IntPtr open_cb;

		public unsafe sbyte* protocol_whitelist;

		public IntPtr io_open;

		public IntPtr io_close;
	}
}
