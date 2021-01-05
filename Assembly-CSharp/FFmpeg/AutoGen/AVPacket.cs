// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVPacket
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FFmpeg.AutoGen
{
  public struct AVPacket
  {
    public unsafe AVBufferRef* buf;
    public long pts;
    public long dts;
    public unsafe sbyte* data;
    public int size;
    public int stream_index;
    public int flags;
    public unsafe AVPacketSideData* side_data;
    public int side_data_elems;
    public long duration;
    public long pos;
    public long convergence_duration;
  }
}
