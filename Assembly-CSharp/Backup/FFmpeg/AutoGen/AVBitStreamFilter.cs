// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVBitStreamFilter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FFmpeg.AutoGen
{
  public struct AVBitStreamFilter
  {
    public unsafe sbyte* name;
    public int priv_data_size;
    public IntPtr filter;
    public IntPtr close;
    public unsafe AVBitStreamFilter* next;
  }
}
