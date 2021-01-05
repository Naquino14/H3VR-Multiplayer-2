// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVOption
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FFmpeg.AutoGen
{
  public struct AVOption
  {
    public unsafe sbyte* name;
    public unsafe sbyte* help;
    public int offset;
    public AVOptionType type;
    public default_val default_val;
    public double min;
    public double max;
    public int flags;
    public unsafe sbyte* unit;
  }
}
