// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigZoneBoom
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ZosigZoneBoom : MonoBehaviour
  {
    private float m_tick = 0.1f;

    private void Start()
    {
    }

    private void OnTriggerStay(Collider col)
    {
      if (!((Object) col.GetComponent<FVRPlayerHitbox>() != (Object) null))
        return;
      this.m_tick -= Time.deltaTime;
      if ((double) this.m_tick > 0.0)
        return;
      this.m_tick = Random.Range(0.05f, 0.2f);
    }
  }
}
