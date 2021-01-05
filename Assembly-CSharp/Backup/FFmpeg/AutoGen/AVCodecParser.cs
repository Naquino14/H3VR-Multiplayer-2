// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVCodecParser
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
