// Decompiled with JetBrains decompiler
// Type: FistVR.PowerUp_Cloud
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class PowerUp_Cloud : MonoBehaviour
  {
    public LayerMask LM_Collide;
    public PowerupType PType;
    public PowerUpIntensity PIntensity;
    public PowerUpDuration PDuration;
    private bool PIsPuke;
    public bool PIsInverted;
    public float CloudRadius = 5f;
    private float m_tickTilCheck = 0.02f;
    private float timeTilGone = 1f;
    private bool m_hasChecked;

    public void SetParams(
      PowerupType t,
      PowerUpIntensity i,
      PowerUpDuration d,
      bool puke,
      bool inverted)
    {
      this.PType = t;
      this.PIntensity = i;
      this.PDuration = d;
      this.PIsPuke = puke;
      this.PIsInverted = inverted;
    }

    private void Update()
    {
      this.m_tickTilCheck -= Time.deltaTime;
      if ((double) this.m_tickTilCheck <= 0.0)
        this.Check();
      this.timeTilGone -= Time.deltaTime;
      if ((double) this.timeTilGone > 0.0)
        return;
      Object.Destroy((Object) this.gameObject);
    }

    private void Check()
    {
      if (this.m_hasChecked)
        return;
      this.m_hasChecked = true;
      Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.CloudRadius, (int) this.LM_Collide, QueryTriggerInteraction.Ignore);
      HashSet<Sosig> sosigSet = new HashSet<Sosig>();
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
        {
          SosigLink component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<SosigLink>();
          if ((Object) component != (Object) null)
            sosigSet.Add(component.S);
        }
      }
      foreach (Sosig sosig in sosigSet)
        sosig.ActivatePower(this.PType, this.PIntensity, this.PDuration, this.PIsPuke, this.PIsInverted);
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) >= (double) this.CloudRadius)
        return;
      GM.CurrentPlayerBody.ActivatePower(this.PType, this.PIntensity, this.PDuration, this.PIsPuke, this.PIsInverted);
    }
  }
}
