using System;

namespace FFmpeg.AutoGen
{
	public struct AVCodecContext
	{
		public unsafe AVClass* av_class;

		public int log_level_offset;

		public AVMediaType codec_type;

		public unsafe AVCodec* codec;

		public unsafe fixed sbyte codec_name[32];

		public AVCodecID codec_id;

		public uint codec_tag;

		public uint stream_codec_tag;

		public unsafe void* priv_data;

		public unsafe AVCodecInternal* @internal;

		public unsafe void* opaque;

		public long bit_rate;

		public int bit_rate_tolerance;

		public int global_quality;

		public int compression_level;

		public int flags;

		public int flags2;

		public unsafe sbyte* extradata;

		public int extradata_size;

		public AVRational time_base;

		public int ticks_per_frame;

		public int delay;

		public int width;

		public int height;

		public int coded_width;

		public int coded_height;

		public int gop_size;

		public AVPixelFormat pix_fmt;

		public int me_method;

		public IntPtr draw_horiz_band;

		public IntPtr get_format;

		public int max_b_frames;

		public float b_quant_factor;

		public int rc_strategy;

		public int b_frame_strategy;

		public float b_quant_offset;

		public int has_b_frames;

		public int mpeg_quant;

		public float i_quant_factor;

		public float i_quant_offset;

		public float lumi_masking;

		public float temporal_cplx_masking;

		public float spatial_cplx_masking;

		public float p_masking;

		public float dark_masking;

		public int slice_count;

		public int prediction_method;

		public unsafe int* slice_offset;

		public AVRational sample_aspect_ratio;

		public int me_cmp;

		public int me_sub_cmp;

		public int mb_cmp;

		public int ildct_cmp;

		public int dia_size;

		public int last_predictor_count;

		public int pre_me;

		public int me_pre_cmp;

		public int pre_dia_size;

		public int me_subpel_quality;

		public int dtg_active_format;

		public int me_range;

		public int intra_quant_bias;

		public int inter_quant_bias;

		public int slice_flags;

		public int xvmc_acceleration;

		public int mb_decision;

		public unsafe ushort* intra_matrix;

		public unsafe ushort* inter_matrix;

		public int scenechange_threshold;

		public int noise_reduction;

		public int me_threshold;

		public int mb_threshold;

		public int intra_dc_precision;

		public int skip_top;

		public int skip_bottom;

		public float border_masking;

		public int mb_lmin;

		public int mb_lmax;

		public int me_penalty_compensation;

		public int bidir_refine;

		public int brd_scale;

		public int keyint_min;

		public int refs;

		public int chromaoffset;

		public int scenechange_factor;

		public int mv0_threshold;

		public int b_sensitivity;

		public AVColorPrimaries color_primaries;

		public AVColorTransferCharacteristic color_trc;

		public AVColorSpace colorspace;

		public AVColorRange color_range;

		public AVChromaLocation chroma_sample_location;

		public int slices;

		public AVFieldOrder field_order;

		public int sample_rate;

		public int channels;

		public AVSampleFormat sample_fmt;

		public int frame_size;

		public int frame_number;

		public int block_align;

		public int cutoff;

		public ulong channel_layout;

		public ulong request_channel_layout;

		public AVAudioServiceType audio_service_type;

		public AVSampleFormat request_sample_fmt;

		public IntPtr get_buffer2;

		public int refcounted_frames;

		public float qcompress;

		public float qblur;

		public int qmin;

		public int qmax;

		public int max_qdiff;

		public float rc_qsquish;

		public float rc_qmod_amp;

		public int rc_qmod_freq;

		public int rc_buffer_size;

		public int rc_override_count;

		public unsafe RcOverride* rc_override;

		public unsafe sbyte* rc_eq;

		public long rc_max_rate;

		public long rc_min_rate;

		public float rc_buffer_aggressivity;

		public float rc_initial_cplx;

		public float rc_max_available_vbv_use;

		public float rc_min_vbv_overflow_use;

		public int rc_initial_buffer_occupancy;

		public int coder_type;

		public int context_model;

		public int lmin;

		public int lmax;

		public int frame_skip_threshold;

		public int frame_skip_factor;

		public int frame_skip_exp;

		public int frame_skip_cmp;

		public int trellis;

		public int min_prediction_order;

		public int max_prediction_order;

		public long timecode_frame_start;

		public IntPtr rtp_callback;

		public int rtp_payload_size;

		public int mv_bits;

		public int header_bits;

		public int i_tex_bits;

		public int p_tex_bits;

		public int i_count;

		public int p_count;

		public int skip_count;

		public int misc_bits;

		public int frame_bits;

		public unsafe sbyte* stats_out;

		public unsafe sbyte* stats_in;

		public int workaround_bugs;

		public int strict_std_compliance;

		public int error_concealment;

		public int debug;

		public int debug_mv;

		public int err_recognition;

		public long reordered_opaque;

		public unsafe AVHWAccel* hwaccel;

		public unsafe void* hwaccel_context;

		public unsafe fixed ulong error[8];

		public int dct_algo;

		public int idct_algo;

		public int bits_per_coded_sample;

		public int bits_per_raw_sample;

		public int lowres;

		public unsafe AVFrame* coded_frame;

		public int thread_count;

		public int thread_type;

		public int active_thread_type;

		public int thread_safe_callbacks;

		public IntPtr execute;

		public IntPtr execute2;

		public int nsse_weight;

		public int profile;

		public int level;

		public AVDiscard skip_loop_filter;

		public AVDiscard skip_idct;

		public AVDiscard skip_frame;

		public unsafe sbyte* subtitle_header;

		public int subtitle_header_size;

		public int error_rate;

		public ulong vbv_delay;

		public int side_data_only_packets;

		public int initial_padding;

		public AVRational framerate;

		public AVPixelFormat sw_pix_fmt;

		public AVRational pkt_timebase;

		public unsafe AVCodecDescriptor* codec_descriptor;

		public long pts_correction_num_faulty_pts;

		public long pts_correction_num_faulty_dts;

		public long pts_correction_last_pts;

		public long pts_correction_last_dts;

		public unsafe sbyte* sub_charenc;

		public int sub_charenc_mode;

		public int skip_alpha;

		public int seek_preroll;

		public unsafe ushort* chroma_intra_matrix;

		public unsafe sbyte* dump_separator;

		public unsafe sbyte* codec_whitelist;

		public uint properties;

		public unsafe AVPacketSideData* coded_side_data;

		public int nb_coded_side_data;
	}
}
