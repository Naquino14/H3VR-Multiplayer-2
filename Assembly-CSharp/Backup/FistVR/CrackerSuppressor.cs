// Decompiled with JetBrains decompiler
// Type: FistVR.CrackerSuppressor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class CrackerSuppressor : Suppressor
  {
    [Header("Cracker")]
    public GameObject Unsploded;
    public GameObject Sploded;
    public GameObject Effect;
    private bool m_isSploded;

    public override void ShotEffect()
    {
      base.ShotEffect();
      if (this.m_isSploded)
        return;
      this.m_isSploded = true;
      this.Unsploded.SetActive(false);
      this.Sploded.SetActive(true);
      this.Effect.SetActive(true);
    }
  }
}
