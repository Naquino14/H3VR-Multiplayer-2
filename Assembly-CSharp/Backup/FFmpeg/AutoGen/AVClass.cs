// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVClass
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FFmpeg.AutoGen
{
  public struct AVClass
  {
    public unsafe sbyte* class_name;
    public IntPtr item_name;
    public unsafe AVOption* option;
    public int version;
    public int log_level_offset_offset;
    public int parent_log_context_offset;
    public IntPtr child_next;
    public IntPtr child_class_next;
    public AVClassCategory category;
    public IntPtr get_category;
    public IntPtr query_ranges;
  }
}
