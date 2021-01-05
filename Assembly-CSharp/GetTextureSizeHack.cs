// Decompiled with JetBrains decompiler
// Type: GetTextureSizeHack
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class GetTextureSizeHack : MonoBehaviour
{
  public int Width;
  public int Height;

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    this.Width = source.width;
    this.Height = source.height;
  }
}
