// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVCodecParserContext
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FFmpeg.AutoGen
{
  public struct AVCodecParserContext
  {
    public unsafe void* priv_data;
    public unsafe AVCodecParser* parser;
    public long frame_offset;
    public long cur_offset;
    public long next_frame_offset;
    public int pict_type;
    public int repeat_pict;
    public long pts;
    public long dts;
    public long last_pts;
    public long last_dts;
    public int fetch_timestamp;
    public int cur_frame_start_index;
    public unsafe fixed long cur_frame_offset[4];
    public unsafe fixed long cur_frame_pts[4];
    public unsafe fixed long cur_frame_dts[4];
    public int flags;
    public long offset;
    public unsafe fixed long cur_frame_end[4];
    public int key_frame;
    public long convergence_duration;
    public int dts_sync_point;
    public int dts_ref_dts_delta;
    public int pts_dts_delta;
    public unsafe fixed long cur_frame_pos[4];
    public long pos;
    public long last_pos;
    public int duration;
    public AVFieldOrder field_order;
    public AVPictureStructure picture_structure;
    public int output_picture_number;
    public int width;
    public int height;
    public int coded_width;
    public int coded_height;
    public int format;
  }
}
