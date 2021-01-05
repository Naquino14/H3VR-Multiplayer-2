// Decompiled with JetBrains decompiler
// Type: FillTextureExample
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class FillTextureExample : MonoBehaviour
{
  public Texture2D myTex;
  public Color32[] colors;

  private void Update()
  {
    int length = this.myTex.width * this.myTex.height;
    if (length != this.colors.Length)
      this.colors = new Color32[length];
    this.myTex.SetPixels32(this.colors);
    this.myTex.Apply();
  }
}
