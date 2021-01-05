// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVProgram
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FFmpeg.AutoGen
{
  public struct AVProgram
  {
    public int id;
    public int flags;
    public AVDiscard discard;
    public unsafe uint* stream_index;
    public uint nb_stream_indexes;
    public unsafe AVDictionary* metadata;
    public int program_num;
    public int pmt_pid;
    public int pcr_pid;
    public long start_time;
    public long end_time;
    public long pts_wrap_reference;
    public int pts_wrap_behavior;
  }
}
