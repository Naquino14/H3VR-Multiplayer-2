// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVFrame
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FFmpeg.AutoGen
{
  public struct AVFrame
  {
    public unsafe sbyte* data0;
    public unsafe sbyte* data1;
    public unsafe sbyte* data2;
    public unsafe sbyte* data3;
    public unsafe sbyte* data4;
    public unsafe sbyte* data5;
    public unsafe sbyte* data6;
    public unsafe sbyte* data7;
    public unsafe fixed int linesize[8];
    public unsafe sbyte** extended_data;
    public int width;
    public int height;
    public int nb_samples;
    public int format;
    public int key_frame;
    public AVPictureType pict_type;
    public AVRational sample_aspect_ratio;
    public long pts;
    public long pkt_pts;
    public long pkt_dts;
    public int coded_picture_number;
    public int display_picture_number;
    public int quality;
    public unsafe void* opaque;
    public unsafe fixed ulong error[8];
    public int repeat_pict;
    public int interlaced_frame;
    public int top_field_first;
    public int palette_has_changed;
    public long reordered_opaque;
    public int sample_rate;
    public ulong channel_layout;
    public unsafe AVBufferRef* buf0;
    public unsafe AVBufferRef* buf1;
    public unsafe AVBufferRef* buf2;
    public unsafe AVBufferRef* buf3;
    public unsafe AVBufferRef* buf4;
    public unsafe AVBufferRef* buf5;
    public unsafe AVBufferRef* buf6;
    public unsafe AVBufferRef* buf7;
    public unsafe AVBufferRef** extended_buf;
    public int nb_extended_buf;
    public unsafe AVFrameSideData** side_data;
    public int nb_side_data;
    public int flags;
    public AVColorRange color_range;
    public AVColorPrimaries color_primaries;
    public AVColorTransferCharacteristic color_trc;
    public AVColorSpace colorspace;
    public AVChromaLocation chroma_location;
    public long best_effort_timestamp;
    public long pkt_pos;
    public long pkt_duration;
    public unsafe AVDictionary* metadata;
    public int decode_error_flags;
    public int channels;
    public int pkt_size;
    public unsafe sbyte* qscale_table;
    public int qstride;
    public int qscale_type;
    public unsafe AVBufferRef* qp_table_buf;
  }
}
