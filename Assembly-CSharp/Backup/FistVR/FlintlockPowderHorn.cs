// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockPowderHorn
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlintlockPowderHorn : FVRPhysicalObject, IFVRDamageable
  {
    public AudioEvent AudEvent_Tap;
    public AudioEvent AudEvent_Cap;
    public Transform PowderSpawnPoint;
    public GameObject PowderPrefab;
    public Transform Cap;
    public Transform CapPoint_On;
    public Transform CapPoint_Off;
    public FlintlockPowderHornCap MyCap;
    private bool m_isCapped = true;
    public Transform PourForward;
    public GameObject Explode;
    public bool UsesTriggerHoldDrop;
    private float timeSinceSpawn;
    private bool m_isDestroyed;

    public bool IsCapped => this.m_isCapped;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.m_hasTriggeredUpSinceBegin || (double) Vector3.Angle(this.PourForward.forward, Vector3.up) <= 60.0 || this.m_isCapped)
        return;
      if (hand.Input.TriggerDown)
      {
        this.timeSinceSpawn = 0.0f;
        this.TapOut(true);
      }
      else
      {
        if (!this.UsesTriggerHoldDrop || !hand.Input.TriggerPressed || ((double) this.timeSinceSpawn <= 0.0399999991059303 || this.MyCap.IsFull()))
          return;
        this.timeSinceSpawn = 0.0f;
        this.TapOut(false);
      }
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.timeSinceSpawn < 1.0)
        this.timeSinceSpawn += Time.deltaTime;
      if (this.m_isCapped || (double) Vector3.Angle(this.PourForward.forward, Vector3.up) <= 120.0 || (double) this.timeSinceSpawn <= 0.0399999991059303)
        return;
      this.timeSinceSpawn = 0.0f;
      Object.Instantiate<GameObject>(this.PowderPrefab, this.PowderSpawnPoint.position, Random.rotation);
    }

    public void ToggleCap()
    {
      this.m_isCapped = !this.m_isCapped;
      SM.PlayGenericSound(this.AudEvent_Cap, this.transform.position);
      if (this.m_isCapped)
      {
        this.Cap.position = this.CapPoint_On.position;
        this.Cap.rotation = this.CapPoint_On.rotation;
      }
      else
      {
        this.Cap.position = this.CapPoint_Off.position;
        this.Cap.rotation = this.CapPoint_Off.rotation;
      }
    }

    public void Damage(FistVR.Damage d)
    {
      if ((double) d.Dam_Thermal <= 1.0 || this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      Object.Instantiate<GameObject>(this.Explode, this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    private void TapOut(bool sound)
    {
      if (this.m_isCapped)
        return;
      if (sound)
        SM.PlayGenericSound(this.AudEvent_Tap, this.transform.position);
      Object.Instantiate<GameObject>(this.PowderPrefab, this.PowderSpawnPoint.position, Random.rotation);
    }
  }
}
