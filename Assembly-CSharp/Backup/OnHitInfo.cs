// Decompiled with JetBrains decompiler
// Type: OnHitInfo
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public struct OnHitInfo
{
  public Vector2 uv;
  public int score;

  public OnHitInfo(Vector2 uv, int score)
  {
    this.uv = uv;
    this.score = score;
  }
}
