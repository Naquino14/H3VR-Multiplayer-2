// Decompiled with JetBrains decompiler
// Type: FistVR.HealthPickUp
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class HealthPickUp : MonoBehaviour
  {
    public GameObject Spawn;
    public GameObject Root;
    public bool m_isPartialHeal;
    public float m_partialHealAmount = 0.1f;
    private bool m_hasDone;

    private void Update()
    {
      if ((double) Mathf.Min(Vector3.Distance(GM.CurrentPlayerBody.LeftHand.position, this.transform.position), Vector3.Distance(GM.CurrentPlayerBody.RightHand.position, this.transform.position)) >= 0.25)
        return;
      this.Boom();
    }

    private void Boom()
    {
      if (this.m_hasDone)
        return;
      this.m_hasDone = true;
      if ((double) GM.GetPlayerHealth() >= 1.0)
        return;
      if (this.m_isPartialHeal)
        GM.CurrentPlayerBody.HealPercent(this.m_partialHealAmount);
      else
        GM.CurrentPlayerBody.ResetHealth();
      Object.Instantiate<GameObject>(this.Spawn, this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.Root);
    }
  }
}
