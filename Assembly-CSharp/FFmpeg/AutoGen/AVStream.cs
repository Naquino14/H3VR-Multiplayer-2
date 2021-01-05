// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVStream
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FFmpeg.AutoGen
{
  public struct AVStream
  {
    public int index;
    public int id;
    public unsafe AVCodecContext* codec;
    public unsafe void* priv_data;
    public AVFrac pts;
    public AVRational time_base;
    public long start_time;
    public long duration;
    public long nb_frames;
    public int disposition;
    public AVDiscard discard;
    public AVRational sample_aspect_ratio;
    public unsafe AVDictionary* metadata;
    public AVRational avg_frame_rate;
    public AVPacket attached_pic;
    public unsafe AVPacketSideData* side_data;
    public int nb_side_data;
    public int event_flags;
    public unsafe FFmpeg.AutoGen.info* info;
    public int pts_wrap_bits;
    public long first_dts;
    public long cur_dts;
    public long last_IP_pts;
    public int last_IP_duration;
    public int probe_packets;
    public int codec_info_nb_frames;
    public AVStreamParseType need_parsing;
    public unsafe AVCodecParserContext* parser;
    public unsafe AVPacketList* last_in_packet_buffer;
    public AVProbeData probe_data;
    public unsafe fixed long pts_buffer[17];
    public unsafe AVIndexEntry* index_entries;
    public int nb_index_entries;
    public uint index_entries_allocated_size;
    public AVRational r_frame_rate;
    public int stream_identifier;
    public long interleaver_chunk_size;
    public long interleaver_chunk_duration;
    public int request_probe;
    public int skip_to_keyframe;
    public int skip_samples;
    public long start_skip_samples;
    public long first_discard_sample;
    public long last_discard_sample;
    public int nb_decoded_frames;
    public long mux_ts_offset;
    public long pts_wrap_reference;
    public int pts_wrap_behavior;
    public int update_initial_durations_done;
    public unsafe fixed long pts_reorder_error[17];
    public unsafe fixed sbyte pts_reorder_error_count[17];
    public long last_dts_for_order_check;
    public sbyte dts_ordered;
    public sbyte dts_misordered;
    public int inject_global_side_data;
    public unsafe sbyte* recommended_encoder_configuration;
    public AVRational display_aspect_ratio;
    public unsafe FFFrac* priv_pts;
    public unsafe AVStreamInternal* @internal;
  }
}
