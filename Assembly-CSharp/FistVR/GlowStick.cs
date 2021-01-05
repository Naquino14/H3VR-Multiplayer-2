// Decompiled with JetBrains decompiler
// Type: FistVR.GlowStick
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class GlowStick : MonoBehaviour
  {
    public GlowStickColor[] Colors;
    public AlloyAreaLight Light;
    public Renderer Rend;

    private void Awake()
    {
      int index = Random.Range(0, this.Colors.Length);
      this.Light.Color = this.Colors[index].LightColor;
      this.Rend.material = this.Colors[index].Mat;
    }
  }
}
