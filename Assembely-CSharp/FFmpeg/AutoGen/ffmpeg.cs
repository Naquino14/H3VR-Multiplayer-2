using System;
using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen
{
	public static class ffmpeg
	{
		public const int LIBAVCODEC_VERSION_MAJOR = 57;

		public const int LIBAVCODEC_VERSION_MINOR = 24;

		public const int LIBAVCODEC_VERSION_MICRO = 102;

		public const bool FF_API_VIMA_DECODER = true;

		public const bool FF_API_AUDIO_CONVERT = true;

		public const bool FF_API_AVCODEC_RESAMPLE = true;

		public const bool FF_API_GETCHROMA = true;

		public const bool FF_API_MISSING_SAMPLE = true;

		public const bool FF_API_LOWRES = true;

		public const bool FF_API_CAP_VDPAU = true;

		public const bool FF_API_BUFS_VDPAU = true;

		public const bool FF_API_VOXWARE = true;

		public const bool FF_API_SET_DIMENSIONS = true;

		public const bool FF_API_DEBUG_MV = true;

		public const bool FF_API_AC_VLC = true;

		public const bool FF_API_OLD_MSMPEG4 = true;

		public const bool FF_API_ASPECT_EXTENDED = true;

		public const bool FF_API_ARCH_ALPHA = true;

		public const bool FF_API_ERROR_RATE = true;

		public const bool FF_API_QSCALE_TYPE = true;

		public const bool FF_API_MB_TYPE = true;

		public const bool FF_API_MAX_BFRAMES = true;

		public const bool FF_API_NEG_LINESIZES = true;

		public const bool FF_API_EMU_EDGE = true;

		public const bool FF_API_ARCH_SH4 = true;

		public const bool FF_API_ARCH_SPARC = true;

		public const bool FF_API_UNUSED_MEMBERS = true;

		public const bool FF_API_IDCT_XVIDMMX = true;

		public const bool FF_API_INPUT_PRESERVED = true;

		public const bool FF_API_NORMALIZE_AQP = true;

		public const bool FF_API_GMC = true;

		public const bool FF_API_MV0 = true;

		public const bool FF_API_CODEC_NAME = true;

		public const bool FF_API_AFD = true;

		public const bool FF_API_VISMV = true;

		public const bool FF_API_AUDIOENC_DELAY = true;

		public const bool FF_API_VAAPI_CONTEXT = true;

		public const bool FF_API_AVCTX_TIMEBASE = true;

		public const bool FF_API_MPV_OPT = true;

		public const bool FF_API_STREAM_CODEC_TAG = true;

		public const bool FF_API_QUANT_BIAS = true;

		public const bool FF_API_RC_STRATEGY = true;

		public const bool FF_API_CODED_FRAME = true;

		public const bool FF_API_MOTION_EST = true;

		public const bool FF_API_WITHOUT_PREFIX = true;

		public const bool FF_API_SIDEDATA_ONLY_PKT = true;

		public const bool FF_API_VDPAU_PROFILE = true;

		public const bool FF_API_CONVERGENCE_DURATION = true;

		public const bool FF_API_AVPICTURE = true;

		public const bool FF_API_AVPACKET_OLD_API = true;

		public const bool FF_API_RTP_CALLBACK = true;

		public const bool FF_API_VBV_DELAY = true;

		public const bool FF_API_CODER_TYPE = true;

		public const bool FF_API_STAT_BITS = true;

		public const bool FF_API_PRIVATE_OPT = true;

		public const int AV_CODEC_PROP_INTRA_ONLY = 1;

		public const int AV_CODEC_PROP_LOSSY = 2;

		public const int AV_CODEC_PROP_LOSSLESS = 4;

		public const int AV_CODEC_PROP_REORDER = 8;

		public const int AV_CODEC_PROP_BITMAP_SUB = 65536;

		public const int AV_CODEC_PROP_TEXT_SUB = 131072;

		public const int AV_INPUT_BUFFER_PADDING_SIZE = 32;

		public const int AV_INPUT_BUFFER_MIN_SIZE = 16384;

		public const int FF_INPUT_BUFFER_PADDING_SIZE = 32;

		public const int FF_MIN_BUFFER_SIZE = 16384;

		public const int FF_MAX_B_FRAMES = 16;

		public const int AV_CODEC_FLAG_UNALIGNED = 1;

		public const int AV_CODEC_FLAG_QSCALE = 2;

		public const int AV_CODEC_FLAG_4MV = 4;

		public const int AV_CODEC_FLAG_OUTPUT_CORRUPT = 8;

		public const int AV_CODEC_FLAG_QPEL = 16;

		public const int AV_CODEC_FLAG_PASS1 = 512;

		public const int AV_CODEC_FLAG_PASS2 = 1024;

		public const int AV_CODEC_FLAG_LOOP_FILTER = 2048;

		public const int AV_CODEC_FLAG_GRAY = 8192;

		public const int AV_CODEC_FLAG_PSNR = 32768;

		public const int AV_CODEC_FLAG_TRUNCATED = 65536;

		public const int AV_CODEC_FLAG_INTERLACED_DCT = 262144;

		public const int AV_CODEC_FLAG_LOW_DELAY = 524288;

		public const int AV_CODEC_FLAG_GLOBAL_HEADER = 4194304;

		public const int AV_CODEC_FLAG_BITEXACT = 8388608;

		public const int AV_CODEC_FLAG_AC_PRED = 16777216;

		public const int AV_CODEC_FLAG_INTERLACED_ME = 536870912;

		public const uint AV_CODEC_FLAG_CLOSED_GOP = 2147483648u;

		public const int AV_CODEC_FLAG2_FAST = 1;

		public const int AV_CODEC_FLAG2_NO_OUTPUT = 4;

		public const int AV_CODEC_FLAG2_LOCAL_HEADER = 8;

		public const int AV_CODEC_FLAG2_DROP_FRAME_TIMECODE = 8192;

		public const int AV_CODEC_FLAG2_CHUNKS = 32768;

		public const int AV_CODEC_FLAG2_IGNORE_CROP = 65536;

		public const int AV_CODEC_FLAG2_SHOW_ALL = 4194304;

		public const int AV_CODEC_FLAG2_EXPORT_MVS = 268435456;

		public const int AV_CODEC_FLAG2_SKIP_MANUAL = 536870912;

		public const int AV_CODEC_CAP_DRAW_HORIZ_BAND = 1;

		public const int AV_CODEC_CAP_DR1 = 2;

		public const int AV_CODEC_CAP_TRUNCATED = 8;

		public const int AV_CODEC_CAP_DELAY = 32;

		public const int AV_CODEC_CAP_SMALL_LAST_FRAME = 64;

		public const int AV_CODEC_CAP_HWACCEL_VDPAU = 128;

		public const int AV_CODEC_CAP_SUBFRAMES = 256;

		public const int AV_CODEC_CAP_EXPERIMENTAL = 512;

		public const int AV_CODEC_CAP_CHANNEL_CONF = 1024;

		public const int AV_CODEC_CAP_FRAME_THREADS = 4096;

		public const int AV_CODEC_CAP_SLICE_THREADS = 8192;

		public const int AV_CODEC_CAP_PARAM_CHANGE = 16384;

		public const int AV_CODEC_CAP_AUTO_THREADS = 32768;

		public const int AV_CODEC_CAP_VARIABLE_FRAME_SIZE = 65536;

		public const int AV_CODEC_CAP_INTRA_ONLY = 1073741824;

		public const uint AV_CODEC_CAP_LOSSLESS = 2147483648u;

		public const int CODEC_FLAG_UNALIGNED = 1;

		public const int CODEC_FLAG_QSCALE = 2;

		public const int CODEC_FLAG_4MV = 4;

		public const int CODEC_FLAG_OUTPUT_CORRUPT = 8;

		public const int CODEC_FLAG_QPEL = 16;

		public const int CODEC_FLAG_GMC = 32;

		public const int CODEC_FLAG_MV0 = 64;

		public const int CODEC_FLAG_INPUT_PRESERVED = 256;

		public const int CODEC_FLAG_PASS1 = 512;

		public const int CODEC_FLAG_PASS2 = 1024;

		public const int CODEC_FLAG_GRAY = 8192;

		public const int CODEC_FLAG_EMU_EDGE = 16384;

		public const int CODEC_FLAG_PSNR = 32768;

		public const int CODEC_FLAG_TRUNCATED = 65536;

		public const int CODEC_FLAG_NORMALIZE_AQP = 131072;

		public const int CODEC_FLAG_INTERLACED_DCT = 262144;

		public const int CODEC_FLAG_LOW_DELAY = 524288;

		public const int CODEC_FLAG_GLOBAL_HEADER = 4194304;

		public const int CODEC_FLAG_BITEXACT = 8388608;

		public const int CODEC_FLAG_AC_PRED = 16777216;

		public const int CODEC_FLAG_LOOP_FILTER = 2048;

		public const int CODEC_FLAG_INTERLACED_ME = 536870912;

		public const uint CODEC_FLAG_CLOSED_GOP = 2147483648u;

		public const int CODEC_FLAG2_FAST = 1;

		public const int CODEC_FLAG2_NO_OUTPUT = 4;

		public const int CODEC_FLAG2_LOCAL_HEADER = 8;

		public const int CODEC_FLAG2_DROP_FRAME_TIMECODE = 8192;

		public const int CODEC_FLAG2_IGNORE_CROP = 65536;

		public const int CODEC_FLAG2_CHUNKS = 32768;

		public const int CODEC_FLAG2_SHOW_ALL = 4194304;

		public const int CODEC_FLAG2_EXPORT_MVS = 268435456;

		public const int CODEC_FLAG2_SKIP_MANUAL = 536870912;

		public const int CODEC_CAP_DRAW_HORIZ_BAND = 1;

		public const int CODEC_CAP_DR1 = 2;

		public const int CODEC_CAP_TRUNCATED = 8;

		public const int CODEC_CAP_HWACCEL = 16;

		public const int CODEC_CAP_DELAY = 32;

		public const int CODEC_CAP_SMALL_LAST_FRAME = 64;

		public const int CODEC_CAP_HWACCEL_VDPAU = 128;

		public const int CODEC_CAP_SUBFRAMES = 256;

		public const int CODEC_CAP_EXPERIMENTAL = 512;

		public const int CODEC_CAP_CHANNEL_CONF = 1024;

		public const int CODEC_CAP_NEG_LINESIZES = 2048;

		public const int CODEC_CAP_FRAME_THREADS = 4096;

		public const int CODEC_CAP_SLICE_THREADS = 8192;

		public const int CODEC_CAP_PARAM_CHANGE = 16384;

		public const int CODEC_CAP_AUTO_THREADS = 32768;

		public const int CODEC_CAP_VARIABLE_FRAME_SIZE = 65536;

		public const int CODEC_CAP_INTRA_ONLY = 1073741824;

		public const uint CODEC_CAP_LOSSLESS = 2147483648u;

		public const int HWACCEL_CODEC_CAP_EXPERIMENTAL = 512;

		public const int MB_TYPE_INTRA4x4 = 1;

		public const int MB_TYPE_INTRA16x16 = 2;

		public const int MB_TYPE_INTRA_PCM = 4;

		public const int MB_TYPE_16x16 = 8;

		public const int MB_TYPE_16x8 = 16;

		public const int MB_TYPE_8x16 = 32;

		public const int MB_TYPE_8x8 = 64;

		public const int MB_TYPE_INTERLACED = 128;

		public const int MB_TYPE_DIRECT2 = 256;

		public const int MB_TYPE_ACPRED = 512;

		public const int MB_TYPE_GMC = 1024;

		public const int MB_TYPE_SKIP = 2048;

		public const int MB_TYPE_P0L0 = 4096;

		public const int MB_TYPE_P1L0 = 8192;

		public const int MB_TYPE_P0L1 = 16384;

		public const int MB_TYPE_P1L1 = 32768;

		public const int MB_TYPE_L0 = 12288;

		public const int MB_TYPE_L1 = 49152;

		public const int MB_TYPE_L0L1 = 61440;

		public const int MB_TYPE_QUANT = 65536;

		public const int MB_TYPE_CBP = 131072;

		public const int FF_QSCALE_TYPE_MPEG1 = 0;

		public const int FF_QSCALE_TYPE_MPEG2 = 1;

		public const int FF_QSCALE_TYPE_H264 = 2;

		public const int FF_QSCALE_TYPE_VP56 = 3;

		public const int AV_GET_BUFFER_FLAG_REF = 1;

		public const int AV_PKT_FLAG_KEY = 1;

		public const int AV_PKT_FLAG_CORRUPT = 2;

		public const int FF_COMPRESSION_DEFAULT = -1;

		public const int FF_ASPECT_EXTENDED = 15;

		public const int FF_RC_STRATEGY_XVID = 1;

		public const int FF_PRED_LEFT = 0;

		public const int FF_PRED_PLANE = 1;

		public const int FF_PRED_MEDIAN = 2;

		public const int FF_CMP_SAD = 0;

		public const int FF_CMP_SSE = 1;

		public const int FF_CMP_SATD = 2;

		public const int FF_CMP_DCT = 3;

		public const int FF_CMP_PSNR = 4;

		public const int FF_CMP_BIT = 5;

		public const int FF_CMP_RD = 6;

		public const int FF_CMP_ZERO = 7;

		public const int FF_CMP_VSAD = 8;

		public const int FF_CMP_VSSE = 9;

		public const int FF_CMP_NSSE = 10;

		public const int FF_CMP_W53 = 11;

		public const int FF_CMP_W97 = 12;

		public const int FF_CMP_DCTMAX = 13;

		public const int FF_CMP_DCT264 = 14;

		public const int FF_CMP_CHROMA = 256;

		public const int FF_DTG_AFD_SAME = 8;

		public const int FF_DTG_AFD_4_3 = 9;

		public const int FF_DTG_AFD_16_9 = 10;

		public const int FF_DTG_AFD_14_9 = 11;

		public const int FF_DTG_AFD_4_3_SP_14_9 = 13;

		public const int FF_DTG_AFD_16_9_SP_14_9 = 14;

		public const int FF_DTG_AFD_SP_4_3 = 15;

		public const int FF_DEFAULT_QUANT_BIAS = 999999;

		public const int SLICE_FLAG_CODED_ORDER = 1;

		public const int SLICE_FLAG_ALLOW_FIELD = 2;

		public const int SLICE_FLAG_ALLOW_PLANE = 4;

		public const int FF_MB_DECISION_SIMPLE = 0;

		public const int FF_MB_DECISION_BITS = 1;

		public const int FF_MB_DECISION_RD = 2;

		public const int FF_CODER_TYPE_VLC = 0;

		public const int FF_CODER_TYPE_AC = 1;

		public const int FF_CODER_TYPE_RAW = 2;

		public const int FF_CODER_TYPE_RLE = 3;

		public const int FF_CODER_TYPE_DEFLATE = 4;

		public const int FF_BUG_AUTODETECT = 1;

		public const int FF_BUG_OLD_MSMPEG4 = 2;

		public const int FF_BUG_XVID_ILACE = 4;

		public const int FF_BUG_UMP4 = 8;

		public const int FF_BUG_NO_PADDING = 16;

		public const int FF_BUG_AMV = 32;

		public const int FF_BUG_AC_VLC = 0;

		public const int FF_BUG_QPEL_CHROMA = 64;

		public const int FF_BUG_STD_QPEL = 128;

		public const int FF_BUG_QPEL_CHROMA2 = 256;

		public const int FF_BUG_DIRECT_BLOCKSIZE = 512;

		public const int FF_BUG_EDGE = 1024;

		public const int FF_BUG_HPEL_CHROMA = 2048;

		public const int FF_BUG_DC_CLIP = 4096;

		public const int FF_BUG_MS = 8192;

		public const int FF_BUG_TRUNCATED = 16384;

		public const int FF_COMPLIANCE_VERY_STRICT = 2;

		public const int FF_COMPLIANCE_STRICT = 1;

		public const int FF_COMPLIANCE_NORMAL = 0;

		public const int FF_COMPLIANCE_UNOFFICIAL = -1;

		public const int FF_COMPLIANCE_EXPERIMENTAL = -2;

		public const int FF_EC_GUESS_MVS = 1;

		public const int FF_EC_DEBLOCK = 2;

		public const int FF_EC_FAVOR_INTER = 256;

		public const int FF_DEBUG_PICT_INFO = 1;

		public const int FF_DEBUG_RC = 2;

		public const int FF_DEBUG_BITSTREAM = 4;

		public const int FF_DEBUG_MB_TYPE = 8;

		public const int FF_DEBUG_QP = 16;

		public const int FF_DEBUG_MV = 32;

		public const int FF_DEBUG_DCT_COEFF = 64;

		public const int FF_DEBUG_SKIP = 128;

		public const int FF_DEBUG_STARTCODE = 256;

		public const int FF_DEBUG_PTS = 512;

		public const int FF_DEBUG_ER = 1024;

		public const int FF_DEBUG_MMCO = 2048;

		public const int FF_DEBUG_BUGS = 4096;

		public const int FF_DEBUG_VIS_QP = 8192;

		public const int FF_DEBUG_VIS_MB_TYPE = 16384;

		public const int FF_DEBUG_BUFFERS = 32768;

		public const int FF_DEBUG_THREADS = 65536;

		public const int FF_DEBUG_GREEN_MD = 8388608;

		public const int FF_DEBUG_NOMC = 16777216;

		public const int FF_DEBUG_VIS_MV_P_FOR = 1;

		public const int FF_DEBUG_VIS_MV_B_FOR = 2;

		public const int FF_DEBUG_VIS_MV_B_BACK = 4;

		public const int AV_EF_CRCCHECK = 1;

		public const int AV_EF_BITSTREAM = 2;

		public const int AV_EF_BUFFER = 4;

		public const int AV_EF_EXPLODE = 8;

		public const int AV_EF_IGNORE_ERR = 32768;

		public const int AV_EF_CAREFUL = 65536;

		public const int AV_EF_COMPLIANT = 131072;

		public const int AV_EF_AGGRESSIVE = 262144;

		public const int FF_DCT_AUTO = 0;

		public const int FF_DCT_FASTINT = 1;

		public const int FF_DCT_INT = 2;

		public const int FF_DCT_MMX = 3;

		public const int FF_DCT_ALTIVEC = 5;

		public const int FF_DCT_FAAN = 6;

		public const int FF_IDCT_AUTO = 0;

		public const int FF_IDCT_INT = 1;

		public const int FF_IDCT_SIMPLE = 2;

		public const int FF_IDCT_SIMPLEMMX = 3;

		public const int FF_IDCT_ARM = 7;

		public const int FF_IDCT_ALTIVEC = 8;

		public const int FF_IDCT_SH4 = 9;

		public const int FF_IDCT_SIMPLEARM = 10;

		public const int FF_IDCT_IPP = 13;

		public const int FF_IDCT_XVID = 14;

		public const int FF_IDCT_XVIDMMX = 14;

		public const int FF_IDCT_SIMPLEARMV5TE = 16;

		public const int FF_IDCT_SIMPLEARMV6 = 17;

		public const int FF_IDCT_SIMPLEVIS = 18;

		public const int FF_IDCT_FAAN = 20;

		public const int FF_IDCT_SIMPLENEON = 22;

		public const int FF_IDCT_SIMPLEALPHA = 23;

		public const int FF_IDCT_SIMPLEAUTO = 128;

		public const int FF_THREAD_FRAME = 1;

		public const int FF_THREAD_SLICE = 2;

		public const int FF_PROFILE_UNKNOWN = -99;

		public const int FF_PROFILE_RESERVED = -100;

		public const int FF_PROFILE_AAC_MAIN = 0;

		public const int FF_PROFILE_AAC_LOW = 1;

		public const int FF_PROFILE_AAC_SSR = 2;

		public const int FF_PROFILE_AAC_LTP = 3;

		public const int FF_PROFILE_AAC_HE = 4;

		public const int FF_PROFILE_AAC_HE_V2 = 28;

		public const int FF_PROFILE_AAC_LD = 22;

		public const int FF_PROFILE_AAC_ELD = 38;

		public const int FF_PROFILE_MPEG2_AAC_LOW = 128;

		public const int FF_PROFILE_MPEG2_AAC_HE = 131;

		public const int FF_PROFILE_DTS = 20;

		public const int FF_PROFILE_DTS_ES = 30;

		public const int FF_PROFILE_DTS_96_24 = 40;

		public const int FF_PROFILE_DTS_HD_HRA = 50;

		public const int FF_PROFILE_DTS_HD_MA = 60;

		public const int FF_PROFILE_DTS_EXPRESS = 70;

		public const int FF_PROFILE_MPEG2_422 = 0;

		public const int FF_PROFILE_MPEG2_HIGH = 1;

		public const int FF_PROFILE_MPEG2_SS = 2;

		public const int FF_PROFILE_MPEG2_SNR_SCALABLE = 3;

		public const int FF_PROFILE_MPEG2_MAIN = 4;

		public const int FF_PROFILE_MPEG2_SIMPLE = 5;

		public const int FF_PROFILE_H264_CONSTRAINED = 512;

		public const int FF_PROFILE_H264_INTRA = 2048;

		public const int FF_PROFILE_H264_BASELINE = 66;

		public const int FF_PROFILE_H264_CONSTRAINED_BASELINE = 578;

		public const int FF_PROFILE_H264_MAIN = 77;

		public const int FF_PROFILE_H264_EXTENDED = 88;

		public const int FF_PROFILE_H264_HIGH = 100;

		public const int FF_PROFILE_H264_HIGH_10 = 110;

		public const int FF_PROFILE_H264_HIGH_10_INTRA = 2158;

		public const int FF_PROFILE_H264_HIGH_422 = 122;

		public const int FF_PROFILE_H264_HIGH_422_INTRA = 2170;

		public const int FF_PROFILE_H264_HIGH_444 = 144;

		public const int FF_PROFILE_H264_HIGH_444_PREDICTIVE = 244;

		public const int FF_PROFILE_H264_HIGH_444_INTRA = 2292;

		public const int FF_PROFILE_H264_CAVLC_444 = 44;

		public const int FF_PROFILE_VC1_SIMPLE = 0;

		public const int FF_PROFILE_VC1_MAIN = 1;

		public const int FF_PROFILE_VC1_COMPLEX = 2;

		public const int FF_PROFILE_VC1_ADVANCED = 3;

		public const int FF_PROFILE_MPEG4_SIMPLE = 0;

		public const int FF_PROFILE_MPEG4_SIMPLE_SCALABLE = 1;

		public const int FF_PROFILE_MPEG4_CORE = 2;

		public const int FF_PROFILE_MPEG4_MAIN = 3;

		public const int FF_PROFILE_MPEG4_N_BIT = 4;

		public const int FF_PROFILE_MPEG4_SCALABLE_TEXTURE = 5;

		public const int FF_PROFILE_MPEG4_SIMPLE_FACE_ANIMATION = 6;

		public const int FF_PROFILE_MPEG4_BASIC_ANIMATED_TEXTURE = 7;

		public const int FF_PROFILE_MPEG4_HYBRID = 8;

		public const int FF_PROFILE_MPEG4_ADVANCED_REAL_TIME = 9;

		public const int FF_PROFILE_MPEG4_CORE_SCALABLE = 10;

		public const int FF_PROFILE_MPEG4_ADVANCED_CODING = 11;

		public const int FF_PROFILE_MPEG4_ADVANCED_CORE = 12;

		public const int FF_PROFILE_MPEG4_ADVANCED_SCALABLE_TEXTURE = 13;

		public const int FF_PROFILE_MPEG4_SIMPLE_STUDIO = 14;

		public const int FF_PROFILE_MPEG4_ADVANCED_SIMPLE = 15;

		public const int FF_PROFILE_JPEG2000_CSTREAM_RESTRICTION_0 = 0;

		public const int FF_PROFILE_JPEG2000_CSTREAM_RESTRICTION_1 = 1;

		public const int FF_PROFILE_JPEG2000_CSTREAM_NO_RESTRICTION = 2;

		public const int FF_PROFILE_JPEG2000_DCINEMA_2K = 3;

		public const int FF_PROFILE_JPEG2000_DCINEMA_4K = 4;

		public const int FF_PROFILE_VP9_0 = 0;

		public const int FF_PROFILE_VP9_1 = 1;

		public const int FF_PROFILE_VP9_2 = 2;

		public const int FF_PROFILE_VP9_3 = 3;

		public const int FF_PROFILE_HEVC_MAIN = 1;

		public const int FF_PROFILE_HEVC_MAIN_10 = 2;

		public const int FF_PROFILE_HEVC_MAIN_STILL_PICTURE = 3;

		public const int FF_PROFILE_HEVC_REXT = 4;

		public const int FF_LEVEL_UNKNOWN = -99;

		public const int FF_SUB_CHARENC_MODE_DO_NOTHING = -1;

		public const int FF_SUB_CHARENC_MODE_AUTOMATIC = 0;

		public const int FF_SUB_CHARENC_MODE_PRE_DECODER = 1;

		public const int FF_CODEC_PROPERTY_LOSSLESS = 1;

		public const int FF_CODEC_PROPERTY_CLOSED_CAPTIONS = 2;

		public const int AV_HWACCEL_FLAG_IGNORE_LEVEL = 1;

		public const int AV_HWACCEL_FLAG_ALLOW_HIGH_DEPTH = 2;

		public const int AV_SUBTITLE_FLAG_FORCED = 1;

		public const int AV_PARSER_PTS_NB = 4;

		public const int PARSER_FLAG_COMPLETE_FRAMES = 1;

		public const int PARSER_FLAG_ONCE = 2;

		public const int PARSER_FLAG_FETCHED_OFFSET = 4;

		public const int PARSER_FLAG_USE_CODEC_TS = 4096;

		private const string libavcodec = "avcodec-57";

		public const int LIBAVDEVICE_VERSION_MAJOR = 57;

		public const int LIBAVDEVICE_VERSION_MINOR = 0;

		public const int LIBAVDEVICE_VERSION_MICRO = 101;

		private const string libavdevice = "avdevice-57";

		public const int LIBAVFILTER_VERSION_MAJOR = 6;

		public const int LIBAVFILTER_VERSION_MINOR = 31;

		public const int LIBAVFILTER_VERSION_MICRO = 100;

		public const bool FF_API_OLD_FILTER_OPTS = true;

		public const bool FF_API_OLD_FILTER_OPTS_ERROR = true;

		public const bool FF_API_AVFILTER_OPEN = true;

		public const bool FF_API_AVFILTER_INIT_FILTER = true;

		public const bool FF_API_OLD_FILTER_REGISTER = true;

		public const bool FF_API_NOCONST_GET_NAME = true;

		public const int AVFILTER_FLAG_DYNAMIC_INPUTS = 1;

		public const int AVFILTER_FLAG_DYNAMIC_OUTPUTS = 2;

		public const int AVFILTER_FLAG_SLICE_THREADS = 4;

		public const int AVFILTER_FLAG_SUPPORT_TIMELINE_GENERIC = 65536;

		public const int AVFILTER_FLAG_SUPPORT_TIMELINE_INTERNAL = 131072;

		public const int AVFILTER_FLAG_SUPPORT_TIMELINE = 196608;

		public const int AVFILTER_THREAD_SLICE = 1;

		public const int AVFILTER_CMD_FLAG_ONE = 1;

		public const int AVFILTER_CMD_FLAG_FAST = 2;

		public const int AV_BUFFERSINK_FLAG_PEEK = 1;

		public const int AV_BUFFERSINK_FLAG_NO_REQUEST = 2;

		private const string libavfilter = "avfilter-6";

		public const int LIBAVFORMAT_VERSION_MAJOR = 57;

		public const int LIBAVFORMAT_VERSION_MINOR = 25;

		public const int LIBAVFORMAT_VERSION_MICRO = 100;

		public const bool FF_API_LAVF_BITEXACT = true;

		public const bool FF_API_LAVF_FRAC = true;

		public const bool FF_API_LAVF_CODEC_TB = true;

		public const bool FF_API_URL_FEOF = true;

		public const bool FF_API_LAVF_FMT_RAWPICTURE = true;

		public const bool FF_API_COMPUTE_PKT_FIELDS2 = true;

		public const bool FF_API_OLD_OPEN_CALLBACKS = true;

		public const int FF_API_R_FRAME_RATE = 1;

		public const int AVIO_SEEKABLE_NORMAL = 1;

		public const int AVSEEK_SIZE = 65536;

		public const int AVSEEK_FORCE = 131072;

		public const int AVIO_FLAG_READ = 1;

		public const int AVIO_FLAG_WRITE = 2;

		public const int AVIO_FLAG_READ_WRITE = 3;

		public const int AVIO_FLAG_NONBLOCK = 8;

		public const int AVIO_FLAG_DIRECT = 32768;

		public const int AVPROBE_SCORE_RETRY = 25;

		public const int AVPROBE_SCORE_STREAM_RETRY = 24;

		public const int AVPROBE_SCORE_EXTENSION = 50;

		public const int AVPROBE_SCORE_MIME = 75;

		public const int AVPROBE_SCORE_MAX = 100;

		public const int AVPROBE_PADDING_SIZE = 32;

		public const int AVFMT_NOFILE = 1;

		public const int AVFMT_NEEDNUMBER = 2;

		public const int AVFMT_SHOW_IDS = 8;

		public const int AVFMT_RAWPICTURE = 32;

		public const int AVFMT_GLOBALHEADER = 64;

		public const int AVFMT_NOTIMESTAMPS = 128;

		public const int AVFMT_GENERIC_INDEX = 256;

		public const int AVFMT_TS_DISCONT = 512;

		public const int AVFMT_VARIABLE_FPS = 1024;

		public const int AVFMT_NODIMENSIONS = 2048;

		public const int AVFMT_NOSTREAMS = 4096;

		public const int AVFMT_NOBINSEARCH = 8192;

		public const int AVFMT_NOGENSEARCH = 16384;

		public const int AVFMT_NO_BYTE_SEEK = 32768;

		public const int AVFMT_ALLOW_FLUSH = 65536;

		public const int AVFMT_TS_NONSTRICT = 131072;

		public const int AVFMT_TS_NEGATIVE = 262144;

		public const int AVFMT_SEEK_TO_PTS = 67108864;

		public const int AVINDEX_KEYFRAME = 1;

		public const int AV_DISPOSITION_DEFAULT = 1;

		public const int AV_DISPOSITION_DUB = 2;

		public const int AV_DISPOSITION_ORIGINAL = 4;

		public const int AV_DISPOSITION_COMMENT = 8;

		public const int AV_DISPOSITION_LYRICS = 16;

		public const int AV_DISPOSITION_KARAOKE = 32;

		public const int AV_DISPOSITION_FORCED = 64;

		public const int AV_DISPOSITION_HEARING_IMPAIRED = 128;

		public const int AV_DISPOSITION_VISUAL_IMPAIRED = 256;

		public const int AV_DISPOSITION_CLEAN_EFFECTS = 512;

		public const int AV_DISPOSITION_ATTACHED_PIC = 1024;

		public const int AV_DISPOSITION_CAPTIONS = 65536;

		public const int AV_DISPOSITION_DESCRIPTIONS = 131072;

		public const int AV_DISPOSITION_METADATA = 262144;

		public const int AV_PTS_WRAP_IGNORE = 0;

		public const int AV_PTS_WRAP_ADD_OFFSET = 1;

		public const int AV_PTS_WRAP_SUB_OFFSET = -1;

		public const int AVSTREAM_EVENT_FLAG_METADATA_UPDATED = 1;

		public const int MAX_STD_TIMEBASES = 399;

		public const int MAX_REORDER_DELAY = 16;

		public const int AV_PROGRAM_RUNNING = 1;

		public const int AVFMTCTX_NOHEADER = 1;

		public const int AVFMT_FLAG_GENPTS = 1;

		public const int AVFMT_FLAG_IGNIDX = 2;

		public const int AVFMT_FLAG_NONBLOCK = 4;

		public const int AVFMT_FLAG_IGNDTS = 8;

		public const int AVFMT_FLAG_NOFILLIN = 16;

		public const int AVFMT_FLAG_NOPARSE = 32;

		public const int AVFMT_FLAG_NOBUFFER = 64;

		public const int AVFMT_FLAG_CUSTOM_IO = 128;

		public const int AVFMT_FLAG_DISCARD_CORRUPT = 256;

		public const int AVFMT_FLAG_FLUSH_PACKETS = 512;

		public const int AVFMT_FLAG_BITEXACT = 1024;

		public const int AVFMT_FLAG_MP4A_LATM = 32768;

		public const int AVFMT_FLAG_SORT_DTS = 65536;

		public const int AVFMT_FLAG_PRIV_OPT = 131072;

		public const int AVFMT_FLAG_KEEP_SIDE_DATA = 262144;

		public const int AVFMT_FLAG_FAST_SEEK = 524288;

		public const int FF_FDEBUG_TS = 1;

		public const int AVFMT_EVENT_FLAG_METADATA_UPDATED = 1;

		public const int AVFMT_AVOID_NEG_TS_AUTO = -1;

		public const int AVFMT_AVOID_NEG_TS_MAKE_NON_NEGATIVE = 1;

		public const int AVFMT_AVOID_NEG_TS_MAKE_ZERO = 2;

		public const int AVSEEK_FLAG_BACKWARD = 1;

		public const int AVSEEK_FLAG_BYTE = 2;

		public const int AVSEEK_FLAG_ANY = 4;

		public const int AVSEEK_FLAG_FRAME = 8;

		private const string libavformat = "avformat-57";

		public const int FF_LAMBDA_SHIFT = 7;

		public const int FF_LAMBDA_SCALE = 128;

		public const int FF_QP2LAMBDA = 118;

		public const int FF_LAMBDA_MAX = 32767;

		public const int FF_QUALITY_SCALE = 128;

		public const ulong AV_NOPTS_VALUE = 9223372036854775808uL;

		public const int AV_TIME_BASE = 1000000;

		public const int LIBAVUTIL_VERSION_MAJOR = 55;

		public const int LIBAVUTIL_VERSION_MINOR = 17;

		public const int LIBAVUTIL_VERSION_MICRO = 103;

		public const bool FF_API_VDPAU = true;

		public const bool FF_API_XVMC = true;

		public const bool FF_API_OPT_TYPE_METADATA = true;

		public const bool FF_API_DLOG = true;

		public const bool FF_API_VAAPI = true;

		public const bool FF_API_FRAME_QP = true;

		public const bool FF_API_PLUS1_MINUS1 = true;

		public const bool FF_API_ERROR_FRAME = true;

		public const bool FF_API_CRC_BIG_TABLE = true;

		public const int AV_HAVE_BIGENDIAN = 0;

		public const int AV_HAVE_FAST_UNALIGNED = 1;

		public const int AV_HAVE_INCOMPATIBLE_LIBAV_ABI = 0;

		public const int AVERROR_EXPERIMENTAL = -733130664;

		public const int AVERROR_INPUT_CHANGED = -1668179713;

		public const int AVERROR_OUTPUT_CHANGED = -1668179714;

		public const int AV_ERROR_MAX_STRING_SIZE = 64;

		public const double M_E = Math.E;

		public const double M_LN2 = 0.69314718055994529;

		public const double M_LN10 = 2.3025850929940459;

		public const double M_LOG2_10 = 3.3219280948873622;

		public const double M_PHI = 1.6180339887498949;

		public const double M_PI = Math.PI;

		public const double M_PI_2 = Math.PI / 2.0;

		public const double M_SQRT1_2 = 0.70710678118654757;

		public const double M_SQRT2 = 1.4142135623730951;

		public const int AV_LOG_QUIET = -8;

		public const int AV_LOG_PANIC = 0;

		public const int AV_LOG_FATAL = 8;

		public const int AV_LOG_ERROR = 16;

		public const int AV_LOG_WARNING = 24;

		public const int AV_LOG_INFO = 32;

		public const int AV_LOG_VERBOSE = 40;

		public const int AV_LOG_DEBUG = 48;

		public const int AV_LOG_TRACE = 56;

		public const int AV_LOG_MAX_OFFSET = 64;

		public const int AV_LOG_SKIP_REPEATED = 1;

		public const int AV_LOG_PRINT_LEVEL = 2;

		public const int AVPALETTE_SIZE = 1024;

		public const int AVPALETTE_COUNT = 256;

		public const int AV_CH_FRONT_LEFT = 1;

		public const int AV_CH_FRONT_RIGHT = 2;

		public const int AV_CH_FRONT_CENTER = 4;

		public const int AV_CH_LOW_FREQUENCY = 8;

		public const int AV_CH_BACK_LEFT = 16;

		public const int AV_CH_BACK_RIGHT = 32;

		public const int AV_CH_FRONT_LEFT_OF_CENTER = 64;

		public const int AV_CH_FRONT_RIGHT_OF_CENTER = 128;

		public const int AV_CH_BACK_CENTER = 256;

		public const int AV_CH_SIDE_LEFT = 512;

		public const int AV_CH_SIDE_RIGHT = 1024;

		public const int AV_CH_TOP_CENTER = 2048;

		public const int AV_CH_TOP_FRONT_LEFT = 4096;

		public const int AV_CH_TOP_FRONT_CENTER = 8192;

		public const int AV_CH_TOP_FRONT_RIGHT = 16384;

		public const int AV_CH_TOP_BACK_LEFT = 32768;

		public const int AV_CH_TOP_BACK_CENTER = 65536;

		public const int AV_CH_TOP_BACK_RIGHT = 131072;

		public const int AV_CH_STEREO_LEFT = 536870912;

		public const int AV_CH_STEREO_RIGHT = 1073741824;

		public const ulong AV_CH_WIDE_LEFT = 2147483648uL;

		public const ulong AV_CH_WIDE_RIGHT = 4294967296uL;

		public const ulong AV_CH_SURROUND_DIRECT_LEFT = 8589934592uL;

		public const ulong AV_CH_SURROUND_DIRECT_RIGHT = 17179869184uL;

		public const ulong AV_CH_LOW_FREQUENCY_2 = 34359738368uL;

		public const ulong AV_CH_LAYOUT_NATIVE = 9223372036854775808uL;

		public const int AV_CH_LAYOUT_MONO = 4;

		public const int AV_CH_LAYOUT_STEREO = 3;

		public const int AV_CH_LAYOUT_2POINT1 = 11;

		public const int AV_CH_LAYOUT_2_1 = 259;

		public const int AV_CH_LAYOUT_SURROUND = 7;

		public const int AV_CH_LAYOUT_3POINT1 = 15;

		public const int AV_CH_LAYOUT_4POINT0 = 263;

		public const int AV_CH_LAYOUT_4POINT1 = 271;

		public const int AV_CH_LAYOUT_2_2 = 1539;

		public const int AV_CH_LAYOUT_QUAD = 51;

		public const int AV_CH_LAYOUT_5POINT0 = 1543;

		public const int AV_CH_LAYOUT_5POINT1 = 1551;

		public const int AV_CH_LAYOUT_5POINT0_BACK = 55;

		public const int AV_CH_LAYOUT_5POINT1_BACK = 63;

		public const int AV_CH_LAYOUT_6POINT0 = 1799;

		public const int AV_CH_LAYOUT_6POINT0_FRONT = 1731;

		public const int AV_CH_LAYOUT_HEXAGONAL = 311;

		public const int AV_CH_LAYOUT_6POINT1 = 1807;

		public const int AV_CH_LAYOUT_6POINT1_BACK = 319;

		public const int AV_CH_LAYOUT_6POINT1_FRONT = 1739;

		public const int AV_CH_LAYOUT_7POINT0 = 1591;

		public const int AV_CH_LAYOUT_7POINT0_FRONT = 1735;

		public const int AV_CH_LAYOUT_7POINT1 = 1599;

		public const int AV_CH_LAYOUT_7POINT1_WIDE = 1743;

		public const int AV_CH_LAYOUT_7POINT1_WIDE_BACK = 255;

		public const int AV_CH_LAYOUT_OCTAGONAL = 1847;

		public const ulong AV_CH_LAYOUT_HEXADECAGONAL = 6442710839uL;

		public const int AV_CH_LAYOUT_STEREO_DOWNMIX = 1610612736;

		public const uint AV_CPU_FLAG_FORCE = 2147483648u;

		public const int AV_CPU_FLAG_MMX = 1;

		public const int AV_CPU_FLAG_MMXEXT = 2;

		public const int AV_CPU_FLAG_MMX2 = 2;

		public const int AV_CPU_FLAG_3DNOW = 4;

		public const int AV_CPU_FLAG_SSE = 8;

		public const int AV_CPU_FLAG_SSE2 = 16;

		public const int AV_CPU_FLAG_SSE2SLOW = 1073741824;

		public const int AV_CPU_FLAG_3DNOWEXT = 32;

		public const int AV_CPU_FLAG_SSE3 = 64;

		public const int AV_CPU_FLAG_SSE3SLOW = 536870912;

		public const int AV_CPU_FLAG_SSSE3 = 128;

		public const int AV_CPU_FLAG_ATOM = 268435456;

		public const int AV_CPU_FLAG_SSE4 = 256;

		public const int AV_CPU_FLAG_SSE42 = 512;

		public const int AV_CPU_FLAG_AESNI = 524288;

		public const int AV_CPU_FLAG_AVX = 16384;

		public const int AV_CPU_FLAG_AVXSLOW = 134217728;

		public const int AV_CPU_FLAG_XOP = 1024;

		public const int AV_CPU_FLAG_FMA4 = 2048;

		public const int AV_CPU_FLAG_CMOV = 4096;

		public const int AV_CPU_FLAG_AVX2 = 32768;

		public const int AV_CPU_FLAG_FMA3 = 65536;

		public const int AV_CPU_FLAG_BMI1 = 131072;

		public const int AV_CPU_FLAG_BMI2 = 262144;

		public const int AV_CPU_FLAG_ALTIVEC = 1;

		public const int AV_CPU_FLAG_VSX = 2;

		public const int AV_CPU_FLAG_POWER8 = 4;

		public const int AV_CPU_FLAG_ARMV5TE = 1;

		public const int AV_CPU_FLAG_ARMV6 = 2;

		public const int AV_CPU_FLAG_ARMV6T2 = 4;

		public const int AV_CPU_FLAG_VFP = 8;

		public const int AV_CPU_FLAG_VFPV3 = 16;

		public const int AV_CPU_FLAG_NEON = 32;

		public const int AV_CPU_FLAG_ARMV8 = 64;

		public const int AV_CPU_FLAG_VFP_VM = 128;

		public const int AV_CPU_FLAG_SETEND = 65536;

		public const int AV_BUFFER_FLAG_READONLY = 1;

		public const int AV_DICT_MATCH_CASE = 1;

		public const int AV_DICT_IGNORE_SUFFIX = 2;

		public const int AV_DICT_DONT_STRDUP_KEY = 4;

		public const int AV_DICT_DONT_STRDUP_VAL = 8;

		public const int AV_DICT_DONT_OVERWRITE = 16;

		public const int AV_DICT_APPEND = 32;

		public const int AV_NUM_DATA_POINTERS = 8;

		public const int AV_FRAME_FLAG_CORRUPT = 1;

		public const int FF_DECODE_ERROR_INVALID_BITSTREAM = 1;

		public const int FF_DECODE_ERROR_MISSING_REFERENCE = 2;

		public const int AV_OPT_FLAG_ENCODING_PARAM = 1;

		public const int AV_OPT_FLAG_DECODING_PARAM = 2;

		public const int AV_OPT_FLAG_METADATA = 4;

		public const int AV_OPT_FLAG_AUDIO_PARAM = 8;

		public const int AV_OPT_FLAG_VIDEO_PARAM = 16;

		public const int AV_OPT_FLAG_SUBTITLE_PARAM = 32;

		public const int AV_OPT_FLAG_EXPORT = 64;

		public const int AV_OPT_FLAG_READONLY = 128;

		public const int AV_OPT_FLAG_FILTERING_PARAM = 65536;

		public const int AV_OPT_SEARCH_CHILDREN = 1;

		public const int AV_OPT_SEARCH_FAKE_OBJ = 2;

		public const int AV_OPT_ALLOW_NULL = 4;

		public const int AV_OPT_MULTI_COMPONENT_RANGE = 4096;

		public const int AV_OPT_SERIALIZE_SKIP_DEFAULTS = 1;

		public const int AV_OPT_SERIALIZE_OPT_FLAGS_EXACT = 2;

		public const int AV_PIX_FMT_FLAG_BE = 1;

		public const int AV_PIX_FMT_FLAG_PAL = 2;

		public const int AV_PIX_FMT_FLAG_BITSTREAM = 4;

		public const int AV_PIX_FMT_FLAG_HWACCEL = 8;

		public const int AV_PIX_FMT_FLAG_PLANAR = 16;

		public const int AV_PIX_FMT_FLAG_RGB = 32;

		public const int AV_PIX_FMT_FLAG_PSEUDOPAL = 64;

		public const int AV_PIX_FMT_FLAG_ALPHA = 128;

		public const int FF_LOSS_RESOLUTION = 1;

		public const int FF_LOSS_DEPTH = 2;

		public const int FF_LOSS_COLORSPACE = 4;

		public const int FF_LOSS_ALPHA = 8;

		public const int FF_LOSS_COLORQUANT = 16;

		public const int FF_LOSS_CHROMA = 32;

		private const string libavutil = "avutil-55";

		public const int LIBSWRESAMPLE_VERSION_MAJOR = 2;

		public const int LIBSWRESAMPLE_VERSION_MINOR = 0;

		public const int LIBSWRESAMPLE_VERSION_MICRO = 101;

		public const int SWR_FLAG_RESAMPLE = 1;

		private const string libswresample = "swresample-2";

		public const int LIBSWSCALE_VERSION_MAJOR = 4;

		public const int LIBSWSCALE_VERSION_MINOR = 0;

		public const int LIBSWSCALE_VERSION_MICRO = 100;

		public const int SWS_FAST_BILINEAR = 1;

		public const int SWS_BILINEAR = 2;

		public const int SWS_BICUBIC = 4;

		public const int SWS_X = 8;

		public const int SWS_POINT = 16;

		public const int SWS_AREA = 32;

		public const int SWS_BICUBLIN = 64;

		public const int SWS_GAUSS = 128;

		public const int SWS_SINC = 256;

		public const int SWS_LANCZOS = 512;

		public const int SWS_SPLINE = 1024;

		public const int SWS_SRC_V_CHR_DROP_MASK = 196608;

		public const int SWS_SRC_V_CHR_DROP_SHIFT = 16;

		public const int SWS_PARAM_DEFAULT = 123456;

		public const int SWS_PRINT_INFO = 4096;

		public const int SWS_FULL_CHR_H_INT = 8192;

		public const int SWS_FULL_CHR_H_INP = 16384;

		public const int SWS_DIRECT_BGR = 32768;

		public const int SWS_ACCURATE_RND = 262144;

		public const int SWS_BITEXACT = 524288;

		public const int SWS_ERROR_DIFFUSION = 8388608;

		public const double SWS_MAX_REDUCE_CUTOFF = 0.002;

		public const int SWS_CS_ITU709 = 1;

		public const int SWS_CS_FCC = 4;

		public const int SWS_CS_ITU601 = 5;

		public const int SWS_CS_ITU624 = 5;

		public const int SWS_CS_SMPTE170M = 5;

		public const int SWS_CS_SMPTE240M = 7;

		public const int SWS_CS_DEFAULT = 5;

		private const string libswscale = "swscale-4";

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVRational av_codec_get_pkt_timebase(AVCodecContext* avctx);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_codec_set_pkt_timebase(AVCodecContext* avctx, AVRational val);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecDescriptor* av_codec_get_codec_descriptor(AVCodecContext* avctx);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_codec_set_codec_descriptor(AVCodecContext* avctx, AVCodecDescriptor* desc);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint av_codec_get_codec_properties(AVCodecContext* avctx);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_codec_get_lowres(AVCodecContext* avctx);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_codec_set_lowres(AVCodecContext* avctx, int val);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_codec_get_seek_preroll(AVCodecContext* avctx);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_codec_set_seek_preroll(AVCodecContext* avctx, int val);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern ushort* av_codec_get_chroma_intra_matrix(AVCodecContext* avctx);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_codec_set_chroma_intra_matrix(AVCodecContext* avctx, ushort* val);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_codec_get_max_lowres(AVCodec* codec);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodec* av_codec_next(AVCodec* c);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint avcodec_version();

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avcodec_configuration();

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avcodec_license();

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avcodec_register(AVCodec* codec);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern void avcodec_register_all();

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecContext* avcodec_alloc_context3(AVCodec* codec);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avcodec_free_context(AVCodecContext** avctx);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_get_context_defaults3(AVCodecContext* s, AVCodec* codec);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVClass* avcodec_get_class();

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVClass* avcodec_get_frame_class();

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVClass* avcodec_get_subtitle_rect_class();

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_copy_context(AVCodecContext* dest, AVCodecContext* src);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_open2(AVCodecContext* avctx, AVCodec* codec, AVDictionary** options);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_close(AVCodecContext* avctx);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avsubtitle_free(AVSubtitle* sub);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVPacket* av_packet_alloc();

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVPacket* av_packet_clone(AVPacket* src);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_packet_free(AVPacket** pkt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_init_packet(AVPacket* pkt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_new_packet(AVPacket* pkt, int size);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_shrink_packet(AVPacket* pkt, int size);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_grow_packet(AVPacket* pkt, int grow_by);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_packet_from_data(AVPacket* pkt, sbyte* data, int size);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_dup_packet(AVPacket* pkt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_copy_packet(AVPacket* dst, AVPacket* src);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_copy_packet_side_data(AVPacket* dst, AVPacket* src);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_free_packet(AVPacket* pkt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern sbyte* av_packet_new_side_data(AVPacket* pkt, AVPacketSideDataType type, int size);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_packet_add_side_data(AVPacket* pkt, AVPacketSideDataType type, sbyte* data, ulong size);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_packet_shrink_side_data(AVPacket* pkt, AVPacketSideDataType type, int size);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern sbyte* av_packet_get_side_data(AVPacket* pkt, AVPacketSideDataType type, int* size);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_packet_merge_side_data(AVPacket* pkt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_packet_split_side_data(AVPacket* pkt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_packet_side_data_name(AVPacketSideDataType type);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern sbyte* av_packet_pack_dictionary(AVDictionary* dict, int* size);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_packet_unpack_dictionary(sbyte* data, int size, AVDictionary** dict);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_packet_free_side_data(AVPacket* pkt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_packet_ref(AVPacket* dst, AVPacket* src);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_packet_unref(AVPacket* pkt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_packet_move_ref(AVPacket* dst, AVPacket* src);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_packet_copy_props(AVPacket* dst, AVPacket* src);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_packet_rescale_ts(AVPacket* pkt, AVRational tb_src, AVRational tb_dst);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodec* avcodec_find_decoder(AVCodecID id);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodec* avcodec_find_decoder_by_name([MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_default_get_buffer2(AVCodecContext* s, AVFrame* frame, int flags);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint avcodec_get_edge_width();

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avcodec_align_dimensions(AVCodecContext* s, int* width, int* height);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avcodec_align_dimensions2(AVCodecContext* s, int* width, int* height, [MarshalAs(UnmanagedType.LPArray, SizeConst = 8)] int[] linesize_align);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_enum_to_chroma_pos(int* xpos, int* ypos, AVChromaLocation pos);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVChromaLocation avcodec_chroma_pos_to_enum(int xpos, int ypos);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_decode_audio4(AVCodecContext* avctx, AVFrame* frame, int* got_frame_ptr, AVPacket* avpkt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_decode_video2(AVCodecContext* avctx, AVFrame* picture, int* got_picture_ptr, AVPacket* avpkt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_decode_subtitle2(AVCodecContext* avctx, AVSubtitle* sub, int* got_sub_ptr, AVPacket* avpkt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecParser* av_parser_next(AVCodecParser* c);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_register_codec_parser(AVCodecParser* parser);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecParserContext* av_parser_init(int codec_id);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_parser_parse2(AVCodecParserContext* s, AVCodecContext* avctx, sbyte** poutbuf, int* poutbuf_size, sbyte* buf, int buf_size, long pts, long dts, long pos);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_parser_change(AVCodecParserContext* s, AVCodecContext* avctx, sbyte** poutbuf, int* poutbuf_size, sbyte* buf, int buf_size, int keyframe);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_parser_close(AVCodecParserContext* s);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodec* avcodec_find_encoder(AVCodecID id);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodec* avcodec_find_encoder_by_name([MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_encode_audio2(AVCodecContext* avctx, AVPacket* avpkt, AVFrame* frame, int* got_packet_ptr);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_encode_video2(AVCodecContext* avctx, AVPacket* avpkt, AVFrame* frame, int* got_packet_ptr);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_encode_subtitle(AVCodecContext* avctx, sbyte* buf, int buf_size, AVSubtitle* sub);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern ReSampleContext* av_audio_resample_init(int output_channels, int input_channels, int output_rate, int input_rate, AVSampleFormat sample_fmt_out, AVSampleFormat sample_fmt_in, int filter_length, int log2_phase_count, int linear, double cutoff);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int audio_resample(ReSampleContext* s, short* output, short* input, int nb_samples);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void audio_resample_close(ReSampleContext* s);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVResampleContext* av_resample_init(int out_rate, int in_rate, int filter_length, int log2_phase_count, int linear, double cutoff);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_resample(AVResampleContext* c, short* dst, short* src, int* consumed, int src_size, int dst_size, int update_ctx);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_resample_compensate(AVResampleContext* c, int sample_delta, int compensation_distance);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_resample_close(AVResampleContext* c);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avpicture_alloc(AVPicture* picture, AVPixelFormat pix_fmt, int width, int height);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avpicture_free(AVPicture* picture);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avpicture_fill(AVPicture* picture, sbyte* ptr, AVPixelFormat pix_fmt, int width, int height);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avpicture_layout(AVPicture* src, AVPixelFormat pix_fmt, int width, int height, sbyte* dest, int dest_size);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern int avpicture_get_size(AVPixelFormat pix_fmt, int width, int height);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_picture_copy(AVPicture* dst, AVPicture* src, AVPixelFormat pix_fmt, int width, int height);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_picture_crop(AVPicture* dst, AVPicture* src, AVPixelFormat pix_fmt, int top_band, int left_band);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_picture_pad(AVPicture* dst, AVPicture* src, int height, int width, AVPixelFormat pix_fmt, int padtop, int padbottom, int padleft, int padright, int* color);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avcodec_get_chroma_sub_sample(AVPixelFormat pix_fmt, int* h_shift, int* v_shift);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint avcodec_pix_fmt_to_codec_tag(AVPixelFormat pix_fmt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern int avcodec_get_pix_fmt_loss(AVPixelFormat dst_pix_fmt, AVPixelFormat src_pix_fmt, int has_alpha);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVPixelFormat avcodec_find_best_pix_fmt_of_list(AVPixelFormat* pix_fmt_list, AVPixelFormat src_pix_fmt, int has_alpha, int* loss_ptr);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVPixelFormat avcodec_find_best_pix_fmt_of_2(AVPixelFormat dst_pix_fmt1, AVPixelFormat dst_pix_fmt2, AVPixelFormat src_pix_fmt, int has_alpha, int* loss_ptr);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVPixelFormat avcodec_find_best_pix_fmt2(AVPixelFormat dst_pix_fmt1, AVPixelFormat dst_pix_fmt2, AVPixelFormat src_pix_fmt, int has_alpha, int* loss_ptr);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVPixelFormat avcodec_default_get_format(AVCodecContext* s, AVPixelFormat* fmt);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avcodec_set_dimensions(AVCodecContext* s, int width, int height);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong av_get_codec_tag_string(IntPtr buf, ulong buf_size, uint codec_tag);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avcodec_string(IntPtr buf, int buf_size, AVCodecContext* enc, int encode);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern string av_get_profile_name(AVCodec* codec, int profile);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avcodec_profile_name(AVCodecID codec_id, int profile);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_default_execute(AVCodecContext* c, IntPtr* func, void* arg, int* ret, int count, int size);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_default_execute2(AVCodecContext* c, IntPtr* func, void* arg, int* ret, int count);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_fill_audio_frame(AVFrame* frame, int nb_channels, AVSampleFormat sample_fmt, sbyte* buf, int buf_size, int align);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avcodec_flush_buffers(AVCodecContext* avctx);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_get_bits_per_sample(AVCodecID codec_id);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVCodecID av_get_pcm_codec(AVSampleFormat fmt, int be);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_get_exact_bits_per_sample(AVCodecID codec_id);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_get_audio_frame_duration(AVCodecContext* avctx, int frame_bytes);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_register_bitstream_filter(AVBitStreamFilter* bsf);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVBitStreamFilterContext* av_bitstream_filter_init([MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_bitstream_filter_filter(AVBitStreamFilterContext* bsfc, AVCodecContext* avctx, [MarshalAs(UnmanagedType.LPStr)] string args, sbyte** poutbuf, int* poutbuf_size, sbyte* buf, int buf_size, int keyframe);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_bitstream_filter_close(AVBitStreamFilterContext* bsf);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVBitStreamFilter* av_bitstream_filter_next(AVBitStreamFilter* f);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_fast_padded_malloc(void* ptr, uint* size, ulong min_size);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_fast_padded_mallocz(void* ptr, uint* size, ulong min_size);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint av_xiphlacing(sbyte* s, uint v);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_log_missing_feature(void* avc, [MarshalAs(UnmanagedType.LPStr)] string feature, int want_sample);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_log_ask_for_sample(void* avc, [MarshalAs(UnmanagedType.LPStr)] string msg);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_register_hwaccel(AVHWAccel* hwaccel);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVHWAccel* av_hwaccel_next(AVHWAccel* hwaccel);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_lockmgr_register(IntPtr* cb);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVMediaType avcodec_get_type(AVCodecID codec_id);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avcodec_get_name(AVCodecID id);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avcodec_is_open(AVCodecContext* s);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_codec_is_encoder(AVCodec* codec);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_codec_is_decoder(AVCodec* codec);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecDescriptor* avcodec_descriptor_get(AVCodecID id);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecDescriptor* avcodec_descriptor_next(AVCodecDescriptor* prev);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecDescriptor* avcodec_descriptor_get_by_name([MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCPBProperties* av_cpb_properties_alloc(ulong* size);

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint avdevice_version();

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avdevice_configuration();

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avdevice_license();

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern void avdevice_register_all();

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVInputFormat* av_input_audio_device_next(AVInputFormat* d);

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVInputFormat* av_input_video_device_next(AVInputFormat* d);

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVOutputFormat* av_output_audio_device_next(AVOutputFormat* d);

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVOutputFormat* av_output_video_device_next(AVOutputFormat* d);

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avdevice_app_to_dev_control_message(AVFormatContext* s, AVAppToDevMessageType type, void* data, ulong data_size);

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avdevice_dev_to_app_control_message(AVFormatContext* s, AVDevToAppMessageType type, void* data, ulong data_size);

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avdevice_capabilities_create(AVDeviceCapabilitiesQuery** caps, AVFormatContext* s, AVDictionary** device_options);

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avdevice_capabilities_free(AVDeviceCapabilitiesQuery** caps, AVFormatContext* s);

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avdevice_list_devices(AVFormatContext* s, AVDeviceInfoList** device_list);

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avdevice_free_list_devices(AVDeviceInfoList** device_list);

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avdevice_list_input_sources(AVInputFormat* device, [MarshalAs(UnmanagedType.LPStr)] string device_name, AVDictionary* device_options, AVDeviceInfoList** device_list);

		[DllImport("avdevice-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avdevice_list_output_sinks(AVOutputFormat* device, [MarshalAs(UnmanagedType.LPStr)] string device_name, AVDictionary* device_options, AVDeviceInfoList** device_list);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint avfilter_version();

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avfilter_configuration();

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avfilter_license();

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_pad_count(AVFilterPad* pads);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern string avfilter_pad_get_name(AVFilterPad* pads, int pad_idx);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVMediaType avfilter_pad_get_type(AVFilterPad* pads, int pad_idx);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_link(AVFilterContext* src, uint srcpad, AVFilterContext* dst, uint dstpad);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avfilter_link_free(AVFilterLink** link);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_link_get_channels(AVFilterLink* link);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avfilter_link_set_closed(AVFilterLink* link, int closed);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_config_links(AVFilterContext* filter);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_process_command(AVFilterContext* filter, [MarshalAs(UnmanagedType.LPStr)] string cmd, [MarshalAs(UnmanagedType.LPStr)] string arg, IntPtr res, int res_len, int flags);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public static extern void avfilter_register_all();

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public static extern void avfilter_uninit();

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_register(AVFilter* filter);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFilter* avfilter_get_by_name([MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFilter* avfilter_next(AVFilter* prev);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFilter** av_filter_next(AVFilter** filter);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_open(AVFilterContext** filter_ctx, AVFilter* filter, [MarshalAs(UnmanagedType.LPStr)] string inst_name);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_init_filter(AVFilterContext* filter, [MarshalAs(UnmanagedType.LPStr)] string args, void* opaque);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_init_str(AVFilterContext* ctx, [MarshalAs(UnmanagedType.LPStr)] string args);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_init_dict(AVFilterContext* ctx, AVDictionary** options);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avfilter_free(AVFilterContext* filter);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_insert_filter(AVFilterLink* link, AVFilterContext* filt, uint filt_srcpad_idx, uint filt_dstpad_idx);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVClass* avfilter_get_class();

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFilterGraph* avfilter_graph_alloc();

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFilterContext* avfilter_graph_alloc_filter(AVFilterGraph* graph, AVFilter* filter, [MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFilterContext* avfilter_graph_get_filter(AVFilterGraph* graph, [MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_graph_add_filter(AVFilterGraph* graphctx, AVFilterContext* filter);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_graph_create_filter(AVFilterContext** filt_ctx, AVFilter* filt, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string args, void* opaque, AVFilterGraph* graph_ctx);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avfilter_graph_set_auto_convert(AVFilterGraph* graph, uint flags);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_graph_config(AVFilterGraph* graphctx, void* log_ctx);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avfilter_graph_free(AVFilterGraph** graph);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFilterInOut* avfilter_inout_alloc();

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avfilter_inout_free(AVFilterInOut** inout);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_graph_parse(AVFilterGraph* graph, [MarshalAs(UnmanagedType.LPStr)] string filters, AVFilterInOut* inputs, AVFilterInOut* outputs, void* log_ctx);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_graph_parse_ptr(AVFilterGraph* graph, [MarshalAs(UnmanagedType.LPStr)] string filters, AVFilterInOut** inputs, AVFilterInOut** outputs, void* log_ctx);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_graph_parse2(AVFilterGraph* graph, [MarshalAs(UnmanagedType.LPStr)] string filters, AVFilterInOut** inputs, AVFilterInOut** outputs);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_graph_send_command(AVFilterGraph* graph, [MarshalAs(UnmanagedType.LPStr)] string target, [MarshalAs(UnmanagedType.LPStr)] string cmd, [MarshalAs(UnmanagedType.LPStr)] string arg, IntPtr res, int res_len, int flags);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_graph_queue_command(AVFilterGraph* graph, [MarshalAs(UnmanagedType.LPStr)] string target, [MarshalAs(UnmanagedType.LPStr)] string cmd, [MarshalAs(UnmanagedType.LPStr)] string arg, int flags, double ts);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern sbyte* avfilter_graph_dump(AVFilterGraph* graph, [MarshalAs(UnmanagedType.LPStr)] string options);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avfilter_graph_request_oldest(AVFilterGraph* graph);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint av_buffersrc_get_nb_failed_requests(AVFilterContext* buffer_src);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_buffersrc_write_frame(AVFilterContext* ctx, AVFrame* frame);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_buffersrc_add_frame(AVFilterContext* ctx, AVFrame* frame);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_buffersrc_add_frame_flags(AVFilterContext* buffer_src, AVFrame* frame, int flags);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_buffersink_get_frame_flags(AVFilterContext* ctx, AVFrame* frame, int flags);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVBufferSinkParams* av_buffersink_params_alloc();

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVABufferSinkParams* av_abuffersink_params_alloc();

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_buffersink_set_frame_size(AVFilterContext* ctx, uint frame_size);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVRational av_buffersink_get_frame_rate(AVFilterContext* ctx);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_buffersink_get_frame(AVFilterContext* ctx, AVFrame* frame);

		[DllImport("avfilter-6", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_buffersink_get_samples(AVFilterContext* ctx, AVFrame* frame, int nb_samples);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avio_find_protocol_name([MarshalAs(UnmanagedType.LPStr)] string url);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern int avio_check([MarshalAs(UnmanagedType.LPStr)] string url, int flags);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern int avpriv_io_move([MarshalAs(UnmanagedType.LPStr)] string url_src, [MarshalAs(UnmanagedType.LPStr)] string url_dst);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern int avpriv_io_delete([MarshalAs(UnmanagedType.LPStr)] string url);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_open_dir(AVIODirContext** s, [MarshalAs(UnmanagedType.LPStr)] string url, AVDictionary** options);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_read_dir(AVIODirContext* s, AVIODirEntry** next);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_close_dir(AVIODirContext** s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avio_free_directory_entry(AVIODirEntry** entry);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVIOContext* avio_alloc_context(sbyte* buffer, int buffer_size, int write_flag, void* opaque, IntPtr* read_packet, IntPtr* write_packet, IntPtr* seek);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avio_w8(AVIOContext* s, int b);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avio_write(AVIOContext* s, sbyte* buf, int size);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avio_wl64(AVIOContext* s, ulong val);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avio_wb64(AVIOContext* s, ulong val);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avio_wl32(AVIOContext* s, uint val);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avio_wb32(AVIOContext* s, uint val);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avio_wl24(AVIOContext* s, uint val);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avio_wb24(AVIOContext* s, uint val);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avio_wl16(AVIOContext* s, uint val);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avio_wb16(AVIOContext* s, uint val);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_put_str(AVIOContext* s, [MarshalAs(UnmanagedType.LPStr)] string str);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_put_str16le(AVIOContext* s, [MarshalAs(UnmanagedType.LPStr)] string str);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_put_str16be(AVIOContext* s, [MarshalAs(UnmanagedType.LPStr)] string str);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern long avio_seek(AVIOContext* s, long offset, int whence);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern long avio_skip(AVIOContext* s, long offset);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern long avio_size(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_feof(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int url_feof(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_printf(AVIOContext* s, [MarshalAs(UnmanagedType.LPStr)] string fmt);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avio_flush(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_read(AVIOContext* s, sbyte* buf, int size);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_r8(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint avio_rl16(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint avio_rl24(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint avio_rl32(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern ulong avio_rl64(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint avio_rb16(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint avio_rb24(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint avio_rb32(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern ulong avio_rb64(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_get_str(AVIOContext* pb, int maxlen, IntPtr buf, int buflen);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_get_str16le(AVIOContext* pb, int maxlen, IntPtr buf, int buflen);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_get_str16be(AVIOContext* pb, int maxlen, IntPtr buf, int buflen);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_open(AVIOContext** s, [MarshalAs(UnmanagedType.LPStr)] string url, int flags);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_open2(AVIOContext** s, [MarshalAs(UnmanagedType.LPStr)] string url, int flags, AVIOInterruptCB* int_cb, AVDictionary** options);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_close(AVIOContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_closep(AVIOContext** s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_open_dyn_buf(AVIOContext** s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_close_dyn_buf(AVIOContext* s, sbyte** pbuffer);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern string avio_enum_protocols(void** opaque, int output);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_pause(AVIOContext* h, int pause);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern long avio_seek_time(AVIOContext* h, int stream_index, long timestamp, int flags);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_read_to_bprint(AVIOContext* h, AVBPrint* pb, ulong max_size);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_accept(AVIOContext* s, AVIOContext** c);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avio_handshake(AVIOContext* c);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_get_packet(AVIOContext* s, AVPacket* pkt, int size);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_append_packet(AVIOContext* s, AVPacket* pkt, int size);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVRational av_stream_get_r_frame_rate(AVStream* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_stream_set_r_frame_rate(AVStream* s, AVRational r);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecParserContext* av_stream_get_parser(AVStream* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern sbyte* av_stream_get_recommended_encoder_configuration(AVStream* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_stream_set_recommended_encoder_configuration(AVStream* s, IntPtr configuration);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern long av_stream_get_end_pts(AVStream* st);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_format_get_probe_score(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodec* av_format_get_video_codec(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_format_set_video_codec(AVFormatContext* s, AVCodec* c);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodec* av_format_get_audio_codec(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_format_set_audio_codec(AVFormatContext* s, AVCodec* c);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodec* av_format_get_subtitle_codec(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_format_set_subtitle_codec(AVFormatContext* s, AVCodec* c);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodec* av_format_get_data_codec(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_format_set_data_codec(AVFormatContext* s, AVCodec* c);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_format_get_metadata_header_padding(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_format_set_metadata_header_padding(AVFormatContext* s, int c);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_format_get_opaque(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_format_set_opaque(AVFormatContext* s, void* opaque);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern av_format_control_message av_format_get_control_message_cb(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_format_set_control_message_cb(AVFormatContext* s, av_format_control_message callback);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVOpenCallback av_format_get_open_cb(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_format_set_open_cb(AVFormatContext* s, AVOpenCallback callback);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_format_inject_global_side_data(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVDurationEstimationMethod av_fmt_ctx_get_duration_estimation_method(AVFormatContext* ctx);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint avformat_version();

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avformat_configuration();

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avformat_license();

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern void av_register_all();

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_register_input_format(AVInputFormat* format);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_register_output_format(AVOutputFormat* format);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern int avformat_network_init();

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern int avformat_network_deinit();

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVInputFormat* av_iformat_next(AVInputFormat* f);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVOutputFormat* av_oformat_next(AVOutputFormat* f);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFormatContext* avformat_alloc_context();

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avformat_free_context(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVClass* avformat_get_class();

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVStream* avformat_new_stream(AVFormatContext* s, AVCodec* c);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern sbyte* av_stream_new_side_data(AVStream* stream, AVPacketSideDataType type, int size);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern sbyte* av_stream_get_side_data(AVStream* stream, AVPacketSideDataType type, int* size);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVProgram* av_new_program(AVFormatContext* s, int id);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avformat_alloc_output_context2(AVFormatContext** ctx, AVOutputFormat* oformat, [MarshalAs(UnmanagedType.LPStr)] string format_name, [MarshalAs(UnmanagedType.LPStr)] string filename);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVInputFormat* av_find_input_format([MarshalAs(UnmanagedType.LPStr)] string short_name);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVInputFormat* av_probe_input_format(AVProbeData* pd, int is_opened);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVInputFormat* av_probe_input_format2(AVProbeData* pd, int is_opened, int* score_max);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVInputFormat* av_probe_input_format3(AVProbeData* pd, int is_opened, int* score_ret);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_probe_input_buffer2(AVIOContext* pb, AVInputFormat** fmt, [MarshalAs(UnmanagedType.LPStr)] string url, void* logctx, uint offset, uint max_probe_size);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_probe_input_buffer(AVIOContext* pb, AVInputFormat** fmt, [MarshalAs(UnmanagedType.LPStr)] string url, void* logctx, uint offset, uint max_probe_size);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avformat_open_input(AVFormatContext** ps, [MarshalAs(UnmanagedType.LPStr)] string url, AVInputFormat* fmt, AVDictionary** options);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_demuxer_open(AVFormatContext* ic);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avformat_find_stream_info(AVFormatContext* ic, AVDictionary** options);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVProgram* av_find_program_from_stream(AVFormatContext* ic, AVProgram* last, int s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_program_add_stream_index(AVFormatContext* ac, int progid, uint idx);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_find_best_stream(AVFormatContext* ic, AVMediaType type, int wanted_stream_nb, int related_stream, AVCodec** decoder_ret, int flags);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_read_frame(AVFormatContext* s, AVPacket* pkt);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_seek_frame(AVFormatContext* s, int stream_index, long timestamp, int flags);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avformat_seek_file(AVFormatContext* s, int stream_index, long min_ts, long ts, long max_ts, int flags);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avformat_flush(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_read_play(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_read_pause(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void avformat_close_input(AVFormatContext** s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avformat_write_header(AVFormatContext* s, AVDictionary** options);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_write_frame(AVFormatContext* s, AVPacket* pkt);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_interleaved_write_frame(AVFormatContext* s, AVPacket* pkt);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_write_uncoded_frame(AVFormatContext* s, int stream_index, AVFrame* frame);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_interleaved_write_uncoded_frame(AVFormatContext* s, int stream_index, AVFrame* frame);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_write_uncoded_frame_query(AVFormatContext* s, int stream_index);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_write_trailer(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVOutputFormat* av_guess_format([MarshalAs(UnmanagedType.LPStr)] string short_name, [MarshalAs(UnmanagedType.LPStr)] string filename, [MarshalAs(UnmanagedType.LPStr)] string mime_type);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecID av_guess_codec(AVOutputFormat* fmt, [MarshalAs(UnmanagedType.LPStr)] string short_name, [MarshalAs(UnmanagedType.LPStr)] string filename, [MarshalAs(UnmanagedType.LPStr)] string mime_type, AVMediaType type);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_get_output_timestamp(AVFormatContext* s, int stream, long* dts, long* wall);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_hex_dump(_iobuf* f, sbyte* buf, int size);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_hex_dump_log(void* avcl, int level, sbyte* buf, int size);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_pkt_dump2(_iobuf* f, AVPacket* pkt, int dump_payload, AVStream* st);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_pkt_dump_log2(void* avcl, int level, AVPacket* pkt, int dump_payload, AVStream* st);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecID av_codec_get_id(AVCodecTag** tags, uint tag);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint av_codec_get_tag(AVCodecTag** tags, AVCodecID id);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_codec_get_tag2(AVCodecTag** tags, AVCodecID id, uint* tag);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_find_default_stream_index(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_index_search_timestamp(AVStream* st, long timestamp, int flags);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_add_index_entry(AVStream* st, long pos, long timestamp, int size, int distance, int flags);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_url_split(IntPtr proto, int proto_size, IntPtr authorization, int authorization_size, IntPtr hostname, int hostname_size, int* port_ptr, IntPtr path, int path_size, [MarshalAs(UnmanagedType.LPStr)] string url);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_dump_format(AVFormatContext* ic, int index, [MarshalAs(UnmanagedType.LPStr)] string url, int is_output);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_get_frame_filename(IntPtr buf, int buf_size, [MarshalAs(UnmanagedType.LPStr)] string path, int number);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_filename_number_test([MarshalAs(UnmanagedType.LPStr)] string filename);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_sdp_create(AVFormatContext** ac, int n_files, IntPtr buf, int size);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_match_ext([MarshalAs(UnmanagedType.LPStr)] string filename, [MarshalAs(UnmanagedType.LPStr)] string extensions);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avformat_query_codec(AVOutputFormat* ofmt, AVCodecID codec_id, int std_compliance);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecTag* avformat_get_riff_video_tags();

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecTag* avformat_get_riff_audio_tags();

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecTag* avformat_get_mov_video_tags();

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVCodecTag* avformat_get_mov_audio_tags();

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVRational av_guess_sample_aspect_ratio(AVFormatContext* format, AVStream* stream, AVFrame* frame);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVRational av_guess_frame_rate(AVFormatContext* ctx, AVStream* stream, AVFrame* frame);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avformat_match_stream_specifier(AVFormatContext* s, AVStream* st, [MarshalAs(UnmanagedType.LPStr)] string spec);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int avformat_queue_attached_pictures(AVFormatContext* s);

		[DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_apply_bitstream_filters(AVCodecContext* codec, AVPacket* pkt, AVBitStreamFilterContext* bsfc);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint avutil_version();

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_version_info();

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avutil_configuration();

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string avutil_license();

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_get_media_type_string(AVMediaType media_type);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern sbyte av_get_picture_type_char(AVPictureType pict_type);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_log2(uint v);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_log2_16bit(uint v);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_strerror(int errnum, IntPtr errbuf, ulong errbuf_size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_malloc(ulong size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_realloc(void* ptr, ulong size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_realloc_f(void* ptr, ulong nelem, ulong elsize);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_reallocp(void* ptr, ulong size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_realloc_array(void* ptr, ulong nmemb, ulong size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_reallocp_array(void* ptr, ulong nmemb, ulong size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_free(void* ptr);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_mallocz(ulong size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_calloc(ulong nmemb, ulong size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern sbyte* av_strdup([MarshalAs(UnmanagedType.LPStr)] string s);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern sbyte* av_strndup([MarshalAs(UnmanagedType.LPStr)] string s, ulong len);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_memdup(void* p, ulong size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_freep(void* ptr);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_dynarray_add(void* tab_ptr, int* nb_ptr, void* elem);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_dynarray_add_nofree(void* tab_ptr, int* nb_ptr, void* elem);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_dynarray2_add(void** tab_ptr, int* nb_ptr, ulong elem_size, sbyte* elem_data);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern void av_max_alloc(ulong max);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_memcpy_backptr(sbyte* dst, int back, int cnt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_fast_realloc(void* ptr, uint* size, ulong min_size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_fast_malloc(void* ptr, uint* size, ulong min_size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_fast_mallocz(void* ptr, uint* size, ulong min_size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_reduce(int* dst_num, int* dst_den, long num, long den, long max);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVRational av_mul_q(AVRational b, AVRational c);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVRational av_div_q(AVRational b, AVRational c);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVRational av_add_q(AVRational b, AVRational c);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVRational av_sub_q(AVRational b, AVRational c);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVRational av_d2q(double d, int max);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_nearer_q(AVRational q, AVRational q1, AVRational q2);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_find_nearest_q_idx(AVRational q, AVRational* q_list);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint av_q2intfloat(AVRational q);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern long av_gcd(long a, long b);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern long av_rescale(long a, long b, long c);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern long av_rescale_rnd(long a, long b, long c, AVRounding param3);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern long av_rescale_q(long a, AVRational bq, AVRational cq);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern long av_rescale_q_rnd(long a, AVRational bq, AVRational cq, AVRounding param3);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_compare_ts(long ts_a, AVRational tb_a, long ts_b, AVRational tb_b);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern long av_compare_mod(ulong a, ulong b, ulong mod);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern long av_rescale_delta(AVRational in_tb, long in_ts, AVRational fs_tb, int duration, long* last, AVRational out_tb);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern long av_add_stable(AVRational ts_tb, long ts, AVRational inc_tb, long inc);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_log(void* avcl, int level, [MarshalAs(UnmanagedType.LPStr)] string fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_vlog(void* avcl, int level, [MarshalAs(UnmanagedType.LPStr)] string fmt, sbyte* vl);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_log_get_level();

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern void av_log_set_level(int level);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_log_set_callback(IntPtr* callback);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_log_default_callback(void* avcl, int level, [MarshalAs(UnmanagedType.LPStr)] string fmt, sbyte* vl);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern string av_default_item_name(void* ctx);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVClassCategory av_default_get_category(void* ptr);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_log_format_line(void* ptr, int level, [MarshalAs(UnmanagedType.LPStr)] string fmt, sbyte* vl, IntPtr line, int line_size, int* print_prefix);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern void av_log_set_flags(int arg);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_log_get_flags();

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint av_int_list_length_for_size(uint elsize, void* list, ulong term);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern _iobuf* av_fopen_utf8([MarshalAs(UnmanagedType.LPStr)] string path, [MarshalAs(UnmanagedType.LPStr)] string mode);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVRational av_get_time_base_q();

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFifoBuffer* av_fifo_alloc(uint size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFifoBuffer* av_fifo_alloc_array(ulong nmemb, ulong size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_fifo_free(AVFifoBuffer* f);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_fifo_freep(AVFifoBuffer** f);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_fifo_reset(AVFifoBuffer* f);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_fifo_size(AVFifoBuffer* f);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_fifo_space(AVFifoBuffer* f);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_fifo_generic_peek_at(AVFifoBuffer* f, void* dest, int offset, int buf_size, IntPtr* func);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_fifo_generic_peek(AVFifoBuffer* f, void* dest, int buf_size, IntPtr* func);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_fifo_generic_read(AVFifoBuffer* f, void* dest, int buf_size, IntPtr* func);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_fifo_generic_write(AVFifoBuffer* f, void* src, int size, IntPtr* func);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_fifo_realloc2(AVFifoBuffer* f, uint size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_fifo_grow(AVFifoBuffer* f, uint additional_space);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_fifo_drain(AVFifoBuffer* f, int size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_get_sample_fmt_name(AVSampleFormat sample_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVSampleFormat av_get_sample_fmt([MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVSampleFormat av_get_alt_sample_fmt(AVSampleFormat sample_fmt, int planar);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVSampleFormat av_get_packed_sample_fmt(AVSampleFormat sample_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVSampleFormat av_get_planar_sample_fmt(AVSampleFormat sample_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern sbyte* av_get_sample_fmt_string(IntPtr buf, int buf_size, AVSampleFormat sample_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_get_bytes_per_sample(AVSampleFormat sample_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_sample_fmt_is_planar(AVSampleFormat sample_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_samples_get_buffer_size(int* linesize, int nb_channels, int nb_samples, AVSampleFormat sample_fmt, int align);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_samples_fill_arrays(sbyte** audio_data, int* linesize, sbyte* buf, int nb_channels, int nb_samples, AVSampleFormat sample_fmt, int align);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_samples_alloc(sbyte** audio_data, int* linesize, int nb_channels, int nb_samples, AVSampleFormat sample_fmt, int align);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_samples_alloc_array_and_samples(sbyte*** audio_data, int* linesize, int nb_channels, int nb_samples, AVSampleFormat sample_fmt, int align);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_samples_copy(sbyte** dst, sbyte** src, int dst_offset, int src_offset, int nb_samples, int nb_channels, AVSampleFormat sample_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_samples_set_silence(sbyte** audio_data, int offset, int nb_samples, int nb_channels, AVSampleFormat sample_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_audio_fifo_free(AVAudioFifo* af);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVAudioFifo* av_audio_fifo_alloc(AVSampleFormat sample_fmt, int channels, int nb_samples);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_audio_fifo_realloc(AVAudioFifo* af, int nb_samples);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_audio_fifo_write(AVAudioFifo* af, void** data, int nb_samples);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_audio_fifo_peek(AVAudioFifo* af, void** data, int nb_samples);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_audio_fifo_read(AVAudioFifo* af, void** data, int nb_samples);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_audio_fifo_drain(AVAudioFifo* af, int nb_samples);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_audio_fifo_reset(AVAudioFifo* af);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_audio_fifo_size(AVAudioFifo* af);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_audio_fifo_space(AVAudioFifo* af);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong av_get_channel_layout([MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern void av_get_channel_layout_string(IntPtr buf, int buf_size, int nb_channels, ulong channel_layout);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_bprint_channel_layout(AVBPrint* bp, int nb_channels, ulong channel_layout);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_get_channel_layout_nb_channels(ulong channel_layout);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern long av_get_default_channel_layout(int nb_channels);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_get_channel_layout_channel_index(ulong channel_layout, ulong channel);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong av_channel_layout_extract_channel(ulong channel_layout, int index);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_get_channel_name(ulong channel);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_get_channel_description(ulong channel);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_get_standard_channel_layout(uint index, ulong* layout, sbyte** name);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_get_cpu_flags();

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern void av_force_cpu_flags(int flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern void av_set_cpu_flags_mask(int mask);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_parse_cpu_flags([MarshalAs(UnmanagedType.LPStr)] string s);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_parse_cpu_caps(uint* flags, [MarshalAs(UnmanagedType.LPStr)] string s);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_cpu_count();

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVBufferRef* av_buffer_alloc(int size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVBufferRef* av_buffer_allocz(int size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVBufferRef* av_buffer_create(sbyte* data, int size, IntPtr* free, void* opaque, int flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_buffer_default_free(void* opaque, sbyte* data);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVBufferRef* av_buffer_ref(AVBufferRef* buf);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_buffer_unref(AVBufferRef** buf);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_buffer_is_writable(AVBufferRef* buf);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_buffer_get_opaque(AVBufferRef* buf);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_buffer_get_ref_count(AVBufferRef* buf);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_buffer_make_writable(AVBufferRef** buf);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_buffer_realloc(AVBufferRef** buf, int size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVBufferPool* av_buffer_pool_init(int size, IntPtr* alloc);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_buffer_pool_uninit(AVBufferPool** pool);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVBufferRef* av_buffer_pool_get(AVBufferPool* pool);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVDictionaryEntry* av_dict_get(AVDictionary* m, [MarshalAs(UnmanagedType.LPStr)] string key, AVDictionaryEntry* prev, int flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_dict_count(AVDictionary* m);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_dict_set(AVDictionary** pm, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value, int flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_dict_set_int(AVDictionary** pm, [MarshalAs(UnmanagedType.LPStr)] string key, long value, int flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_dict_parse_string(AVDictionary** pm, [MarshalAs(UnmanagedType.LPStr)] string str, [MarshalAs(UnmanagedType.LPStr)] string key_val_sep, [MarshalAs(UnmanagedType.LPStr)] string pairs_sep, int flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_dict_copy(AVDictionary** dst, AVDictionary* src, int flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_dict_free(AVDictionary** m);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_dict_get_string(AVDictionary* m, sbyte** buffer, sbyte key_val_sep, sbyte pairs_sep);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern long av_frame_get_best_effort_timestamp(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_set_best_effort_timestamp(AVFrame* frame, long val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern long av_frame_get_pkt_duration(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_set_pkt_duration(AVFrame* frame, long val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern long av_frame_get_pkt_pos(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_set_pkt_pos(AVFrame* frame, long val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern long av_frame_get_channel_layout(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_set_channel_layout(AVFrame* frame, long val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_frame_get_channels(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_set_channels(AVFrame* frame, int val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_frame_get_sample_rate(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_set_sample_rate(AVFrame* frame, int val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVDictionary* av_frame_get_metadata(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_set_metadata(AVFrame* frame, AVDictionary* val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_frame_get_decode_error_flags(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_set_decode_error_flags(AVFrame* frame, int val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_frame_get_pkt_size(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_set_pkt_size(AVFrame* frame, int val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVDictionary** avpriv_frame_get_metadatap(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern sbyte* av_frame_get_qp_table(AVFrame* f, int* stride, int* type);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_frame_set_qp_table(AVFrame* f, AVBufferRef* buf, int stride, int type);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVColorSpace av_frame_get_colorspace(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_set_colorspace(AVFrame* frame, AVColorSpace val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVColorRange av_frame_get_color_range(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_set_color_range(AVFrame* frame, AVColorRange val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_get_colorspace_name(AVColorSpace val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFrame* av_frame_alloc();

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_free(AVFrame** frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_frame_ref(AVFrame* dst, AVFrame* src);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFrame* av_frame_clone(AVFrame* src);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_unref(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_move_ref(AVFrame* dst, AVFrame* src);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_frame_get_buffer(AVFrame* frame, int align);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_frame_is_writable(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_frame_make_writable(AVFrame* frame);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_frame_copy(AVFrame* dst, AVFrame* src);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_frame_copy_props(AVFrame* dst, AVFrame* src);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVBufferRef* av_frame_get_plane_buffer(AVFrame* frame, int plane);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFrameSideData* av_frame_new_side_data(AVFrame* frame, AVFrameSideDataType type, int size);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVFrameSideData* av_frame_get_side_data(AVFrame* frame, AVFrameSideDataType type);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_frame_remove_side_data(AVFrame* frame, AVFrameSideDataType type);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_frame_side_data_name(AVFrameSideDataType type);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_show2(void* obj, void* av_log_obj, int req_flags, int rej_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_opt_set_defaults(void* s);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_opt_set_defaults2(void* s, int mask, int flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_set_options_string(void* ctx, [MarshalAs(UnmanagedType.LPStr)] string opts, [MarshalAs(UnmanagedType.LPStr)] string key_val_sep, [MarshalAs(UnmanagedType.LPStr)] string pairs_sep);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_from_string(void* ctx, [MarshalAs(UnmanagedType.LPStr)] string opts, string[] shorthand, [MarshalAs(UnmanagedType.LPStr)] string key_val_sep, [MarshalAs(UnmanagedType.LPStr)] string pairs_sep);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_opt_free(void* obj);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_flag_is_set(void* obj, [MarshalAs(UnmanagedType.LPStr)] string field_name, [MarshalAs(UnmanagedType.LPStr)] string flag_name);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_dict(void* obj, AVDictionary** options);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_dict2(void* obj, AVDictionary** options, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_get_key_value(sbyte** ropts, [MarshalAs(UnmanagedType.LPStr)] string key_val_sep, [MarshalAs(UnmanagedType.LPStr)] string pairs_sep, uint flags, sbyte** rkey, sbyte** rval);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_eval_flags(void* obj, AVOption* o, [MarshalAs(UnmanagedType.LPStr)] string val, int* flags_out);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_eval_int(void* obj, AVOption* o, [MarshalAs(UnmanagedType.LPStr)] string val, int* int_out);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_eval_int64(void* obj, AVOption* o, [MarshalAs(UnmanagedType.LPStr)] string val, long* int64_out);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_eval_float(void* obj, AVOption* o, [MarshalAs(UnmanagedType.LPStr)] string val, float* float_out);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_eval_double(void* obj, AVOption* o, [MarshalAs(UnmanagedType.LPStr)] string val, double* double_out);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_eval_q(void* obj, AVOption* o, [MarshalAs(UnmanagedType.LPStr)] string val, AVRational* q_out);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVOption* av_opt_find(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string unit, int opt_flags, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVOption* av_opt_find2(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string unit, int opt_flags, int search_flags, void** target_obj);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVOption* av_opt_next(void* obj, AVOption* prev);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_opt_child_next(void* obj, void* prev);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVClass* av_opt_child_class_next(AVClass* parent, AVClass* prev);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string val, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_int(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, long val, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_double(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, double val, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_q(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, AVRational val, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_bin(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, sbyte* val, int size, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_image_size(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, int w, int h, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_pixel_fmt(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, AVPixelFormat fmt, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_sample_fmt(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, AVSampleFormat fmt, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_video_rate(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, AVRational val, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_channel_layout(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, long ch_layout, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_set_dict_val(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, AVDictionary* val, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_get(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, int search_flags, sbyte** out_val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_get_int(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, int search_flags, long* out_val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_get_double(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, int search_flags, double* out_val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_get_q(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, int search_flags, AVRational* out_val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_get_image_size(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, int search_flags, int* w_out, int* h_out);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_get_pixel_fmt(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, int search_flags, AVPixelFormat* out_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_get_sample_fmt(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, int search_flags, AVSampleFormat* out_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_get_video_rate(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, int search_flags, AVRational* out_val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_get_channel_layout(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, int search_flags, long* ch_layout);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_get_dict_val(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, int search_flags, AVDictionary** out_val);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* av_opt_ptr(AVClass* avclass, void* obj, [MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_opt_freep_ranges(AVOptionRanges** ranges);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_query_ranges(AVOptionRanges** param0, void* obj, [MarshalAs(UnmanagedType.LPStr)] string key, int flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_copy(void* dest, void* src);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_query_ranges_default(AVOptionRanges** param0, void* obj, [MarshalAs(UnmanagedType.LPStr)] string key, int flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_is_set_to_default(void* obj, AVOption* o);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_is_set_to_default_by_name(void* obj, [MarshalAs(UnmanagedType.LPStr)] string name, int search_flags);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_opt_serialize(void* obj, int opt_flags, int flags, sbyte** buffer, sbyte key_val_sep, sbyte pairs_sep);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_read_image_line(ushort* dst, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] sbyte*[] data, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] linesize, AVPixFmtDescriptor* desc, int x, int y, int c, int w, int read_pal_component);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_write_image_line(ushort* src, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] sbyte*[] data, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] linesize, AVPixFmtDescriptor* desc, int x, int y, int c, int w);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVPixelFormat av_get_pix_fmt([MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_get_pix_fmt_name(AVPixelFormat pix_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern sbyte* av_get_pix_fmt_string(IntPtr buf, int buf_size, AVPixelFormat pix_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_get_bits_per_pixel(AVPixFmtDescriptor* pixdesc);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_get_padded_bits_per_pixel(AVPixFmtDescriptor* pixdesc);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVPixFmtDescriptor* av_pix_fmt_desc_get(AVPixelFormat pix_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVPixFmtDescriptor* av_pix_fmt_desc_next(AVPixFmtDescriptor* prev);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVPixelFormat av_pix_fmt_desc_get_id(AVPixFmtDescriptor* desc);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_pix_fmt_get_chroma_sub_sample(AVPixelFormat pix_fmt, int* h_shift, int* v_shift);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_pix_fmt_count_planes(AVPixelFormat pix_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern AVPixelFormat av_pix_fmt_swap_endianness(AVPixelFormat pix_fmt);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_get_pix_fmt_loss(AVPixelFormat dst_pix_fmt, AVPixelFormat src_pix_fmt, int has_alpha);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVPixelFormat av_find_best_pix_fmt_of_2(AVPixelFormat dst_pix_fmt1, AVPixelFormat dst_pix_fmt2, AVPixelFormat src_pix_fmt, int has_alpha, int* loss_ptr);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_color_range_name(AVColorRange range);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_color_primaries_name(AVColorPrimaries primaries);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_color_transfer_name(AVColorTransferCharacteristic transfer);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_color_space_name(AVColorSpace space);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern string av_chroma_location_name(AVChromaLocation location);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_image_fill_max_pixsteps([MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] max_pixsteps, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] max_pixstep_comps, AVPixFmtDescriptor* pixdesc);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_image_get_linesize(AVPixelFormat pix_fmt, int width, int plane);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_image_fill_linesizes([MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] linesizes, AVPixelFormat pix_fmt, int width);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_image_fill_pointers([MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] sbyte*[] data, AVPixelFormat pix_fmt, int height, sbyte* ptr, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] linesizes);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_image_alloc([MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] sbyte*[] pointers, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] linesizes, int w, int h, AVPixelFormat pix_fmt, int align);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_image_copy_plane(sbyte* dst, int dst_linesize, sbyte* src, int src_linesize, int bytewidth, int height);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void av_image_copy([MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] sbyte*[] dst_data, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] dst_linesizes, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] sbyte*[] src_data, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] src_linesizes, AVPixelFormat pix_fmt, int width, int height);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_image_fill_arrays([MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] sbyte*[] dst_data, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] dst_linesize, sbyte* src, AVPixelFormat pix_fmt, int width, int height, int align);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_image_get_buffer_size(AVPixelFormat pix_fmt, int width, int height, int align);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_image_copy_to_buffer(sbyte* dst, int dst_size, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] sbyte*[] src_data, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] src_linesize, AVPixelFormat pix_fmt, int width, int height, int align);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int av_image_check_size(uint w, uint h, int log_offset, void* log_ctx);

		[DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
		public static extern int av_image_check_sar(uint w, uint h, AVRational sar);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVClass* swr_get_class();

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern SwrContext* swr_alloc();

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int swr_init(SwrContext* s);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int swr_is_initialized(SwrContext* s);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern SwrContext* swr_alloc_set_opts(SwrContext* s, long out_ch_layout, AVSampleFormat out_sample_fmt, int out_sample_rate, long in_ch_layout, AVSampleFormat in_sample_fmt, int in_sample_rate, int log_offset, void* log_ctx);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void swr_free(SwrContext** s);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void swr_close(SwrContext* s);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int swr_convert(SwrContext* s, sbyte** @out, int out_count, sbyte** @in, int in_count);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern long swr_next_pts(SwrContext* s, long pts);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int swr_set_compensation(SwrContext* s, int sample_delta, int compensation_distance);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int swr_set_channel_mapping(SwrContext* s, int* channel_map);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int swr_set_matrix(SwrContext* s, double* matrix, int stride);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int swr_drop_output(SwrContext* s, int count);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int swr_inject_silence(SwrContext* s, int count);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern long swr_get_delay(SwrContext* s, long @base);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int swr_get_out_samples(SwrContext* s, int in_samples);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint swresample_version();

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public static extern string swresample_configuration();

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public static extern string swresample_license();

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int swr_convert_frame(SwrContext* swr, AVFrame* output, AVFrame* input);

		[DllImport("swresample-2", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int swr_config_frame(SwrContext* swr, AVFrame* @out, AVFrame* @in);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint swscale_version();

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public static extern string swscale_configuration();

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public static extern string swscale_license();

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int* sws_getCoefficients(int colorspace);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public static extern int sws_isSupportedInput(AVPixelFormat pix_fmt);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public static extern int sws_isSupportedOutput(AVPixelFormat pix_fmt);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public static extern int sws_isSupportedEndiannessConversion(AVPixelFormat pix_fmt);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern SwsContext* sws_alloc_context();

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int sws_init_context(SwsContext* sws_context, SwsFilter* srcFilter, SwsFilter* dstFilter);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void sws_freeContext(SwsContext* swsContext);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern SwsContext* sws_getContext(int srcW, int srcH, AVPixelFormat srcFormat, int dstW, int dstH, AVPixelFormat dstFormat, int flags, SwsFilter* srcFilter, SwsFilter* dstFilter, double* param);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int sws_scale(SwsContext* c, sbyte** srcSlice, int* srcStride, int srcSliceY, int srcSliceH, sbyte** dst, int* dstStride);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int sws_setColorspaceDetails(SwsContext* c, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] inv_table, int srcRange, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] table, int dstRange, int brightness, int contrast, int saturation);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int sws_getColorspaceDetails(SwsContext* c, int** inv_table, int* srcRange, int** table, int* dstRange, int* brightness, int* contrast, int* saturation);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern SwsVector* sws_allocVec(int length);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern SwsVector* sws_getGaussianVec(double variance, double quality);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern SwsVector* sws_getConstVec(double c, int length);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern SwsVector* sws_getIdentityVec();

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void sws_scaleVec(SwsVector* a, double scalar);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void sws_normalizeVec(SwsVector* a, double height);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void sws_convVec(SwsVector* a, SwsVector* b);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void sws_addVec(SwsVector* a, SwsVector* b);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void sws_subVec(SwsVector* a, SwsVector* b);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void sws_shiftVec(SwsVector* a, int shift);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern SwsVector* sws_cloneVec(SwsVector* a);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void sws_printVec2(SwsVector* a, AVClass* log_ctx, int log_level);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void sws_freeVec(SwsVector* a);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern SwsFilter* sws_getDefaultFilter(float lumaGBlur, float chromaGBlur, float lumaSharpen, float chromaSharpen, float chromaHShift, float chromaVShift, int verbose);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void sws_freeFilter(SwsFilter* filter);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern SwsContext* sws_getCachedContext(SwsContext* context, int srcW, int srcH, AVPixelFormat srcFormat, int dstW, int dstH, AVPixelFormat dstFormat, int flags, SwsFilter* srcFilter, SwsFilter* dstFilter, double* param);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void sws_convertPalette8ToPacked32(sbyte* src, sbyte* dst, int num_pixels, sbyte* palette);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void sws_convertPalette8ToPacked24(sbyte* src, sbyte* dst, int num_pixels, sbyte* palette);

		[DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern AVClass* sws_get_class();
	}
}
