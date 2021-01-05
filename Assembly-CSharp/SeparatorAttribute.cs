// Decompiled with JetBrains decompiler
// Type: SeparatorAttribute
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class SeparatorAttribute : PropertyAttribute
{
  public readonly string Title;
  public readonly bool WithOffset;

  public SeparatorAttribute() => this.Title = string.Empty;

  public SeparatorAttribute(string title, bool withOffset = false)
  {
    this.Title = title;
    this.WithOffset = withOffset;
  }
}
