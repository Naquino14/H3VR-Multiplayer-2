// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_Detpack
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MF2_Detpack : FVRPhysicalObject, IFVRDamageable
  {
    public AudioEvent AudEvent_Beep;
    public AudioEvent AudEvent_Boop;
    public AudioEvent AudEvent_Tick;
    public List<Transform> SpawnPoints;
    public List<GameObject> SpawnOnDestroy;
    public List<GameObject> NumberDisplay;
    private float m_fuseTime = 10f;
    private bool m_isFusing;
    private bool m_hasDetonated;
    private int lastnum = 10;

    private void Detonate()
    {
      if (this.m_hasDetonated)
        return;
      this.m_hasDetonated = true;
      for (int index = 0; index < this.SpawnPoints.Count; ++index)
      {
        Rigidbody component = Object.Instantiate<GameObject>(this.SpawnOnDestroy[index], this.SpawnPoints[index].position, this.SpawnPoints[index].rotation).GetComponent<Rigidbody>();
        if ((Object) component != (Object) null)
        {
          Vector3 onUnitSphere = Random.onUnitSphere;
          component.velocity = onUnitSphere * Random.Range(0.3f, 2f);
          component.angularVelocity = Random.onUnitSphere * Random.Range(1f, 7f);
        }
      }
      Object.Destroy((Object) this.gameObject);
    }

    public void Damage(FistVR.Damage d)
    {
      if (this.IsHeld || (Object) this.QuickbeltSlot != (Object) null)
        return;
      this.m_isFusing = true;
      this.m_fuseTime = Mathf.Min(this.m_fuseTime, Random.Range(0.1f, 0.2f));
    }

    public void InitiateCountDown(FVRViveHand hand)
    {
      if (this.IsHeld && (Object) hand == (Object) this.m_hand)
        return;
      if (!this.m_isFusing)
        SM.PlayGenericSound(this.AudEvent_Beep, this.transform.position);
      this.m_isFusing = true;
      this.m_fuseTime = Mathf.Min(this.m_fuseTime, 10f);
    }

    public void ResetCountDown(FVRViveHand hand)
    {
      if (this.IsHeld && (Object) hand == (Object) this.m_hand)
        return;
      if (this.m_isFusing)
        SM.PlayGenericSound(this.AudEvent_Boop, this.transform.position);
      this.m_isFusing = false;
      this.m_fuseTime = 10f;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.m_isFusing)
      {
        this.m_fuseTime -= Time.deltaTime;
        if ((double) this.m_fuseTime <= 0.0)
          this.Detonate();
      }
      this.UpdateDisplay();
    }

    private void UpdateDisplay()
    {
      int num1 = Mathf.Clamp(Mathf.FloorToInt(this.m_fuseTime), 0, 10);
      if (num1 == this.lastnum)
        return;
      float num2 = 1f + Mathf.Abs((float) num1 * 0.1f);
      SM.PlayCoreSoundOverrides(FVRPooledAudioType.Generic, this.AudEvent_Tick, this.transform.position, new Vector2(1f, 1f), new Vector2(num2, num2));
      for (int index = 0; index < this.NumberDisplay.Count; ++index)
      {
        if (index == num1)
          this.NumberDisplay[index].SetActive(true);
        else
          this.NumberDisplay[index].SetActive(false);
      }
      this.lastnum = num1;
    }
  }
}
