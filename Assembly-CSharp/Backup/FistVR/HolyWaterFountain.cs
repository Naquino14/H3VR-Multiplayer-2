// Decompiled with JetBrains decompiler
// Type: FistVR.HolyWaterFountain
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class HolyWaterFountain : MonoBehaviour
  {
    public GameObject Water;
    private float m_waterCheckTick = 1f;

    private void Update()
    {
      if ((double) this.m_waterCheckTick > 0.0)
      {
        this.m_waterCheckTick -= Time.deltaTime;
      }
      else
      {
        this.m_waterCheckTick = 1f;
        float num = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position);
        if ((double) num < 50.0)
          this.Water.SetActive(true);
        else
          this.Water.SetActive(false);
        if ((double) num >= 0.5)
          return;
        GM.CurrentPlayerBody.ResetHealth();
      }
    }
  }
}
