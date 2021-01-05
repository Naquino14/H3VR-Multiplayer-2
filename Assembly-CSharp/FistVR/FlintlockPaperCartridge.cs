// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockPaperCartridge
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FlintlockPaperCartridge : FVRPhysicalObject, IFVRDamageable
  {
    public List<Renderer> Rends;
    public FlintlockPaperCartridge.CartridgeState CState;
    public int numPowderChunksLeft = 20;
    public AudioEvent AudEvent_Bite;
    public AudioEvent AudEvent_Tap;
    public Transform PowderSpawnPoint;
    public GameObject PowderPrefab;
    public GameObject BitPart;
    public AudioEvent AudEvent_Spit;
    public GameObject Splode;
    private bool m_isDestroyed;
    private float m_tickDownToSpit = 0.2f;
    private bool m_tickingDownToSpit;
    private float timeSinceSpawn = 1f;

    private void SetRenderer(FlintlockPaperCartridge.CartridgeState s)
    {
      for (int index = 0; index < this.Rends.Count; ++index)
        this.Rends[index].enabled = s == (FlintlockPaperCartridge.CartridgeState) index;
    }

    protected override void Awake()
    {
      base.Awake();
      this.SetRenderer(this.CState);
    }

    public void Damage(FistVR.Damage d)
    {
      if (this.numPowderChunksLeft <= 0 || this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      Object.Instantiate<GameObject>(this.Splode, this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.m_hasTriggeredUpSinceBegin && (double) Vector3.Angle(-this.transform.forward, Vector3.up) > 60.0 && (hand.Input.TriggerDown && this.CState != FlintlockPaperCartridge.CartridgeState.Whole))
        this.TapOut();
      if (this.CState != FlintlockPaperCartridge.CartridgeState.Whole || (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f) >= 0.150000005960464)
        return;
      SM.PlayGenericSound(this.AudEvent_Bite, this.transform.position);
      this.CState = FlintlockPaperCartridge.CartridgeState.BitOpen;
      this.SetRenderer(this.CState);
      this.m_tickingDownToSpit = true;
      this.m_tickDownToSpit = Random.Range(0.3f, 0.6f);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.timeSinceSpawn < 1.0)
        this.timeSinceSpawn += Time.deltaTime;
      if (this.m_tickingDownToSpit)
      {
        this.m_tickDownToSpit -= Time.deltaTime;
        if ((double) this.m_tickDownToSpit <= 0.0)
        {
          this.m_tickingDownToSpit = false;
          Vector3 vector3 = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f;
          SM.PlayGenericSound(this.AudEvent_Spit, vector3);
          Rigidbody component = Object.Instantiate<GameObject>(this.BitPart, vector3, Random.rotation).GetComponent<Rigidbody>();
          component.velocity = GM.CurrentPlayerBody.Head.forward * Random.Range(2f, 4f) + Random.onUnitSphere;
          component.angularVelocity = Random.onUnitSphere * Random.Range(1f, 5f);
        }
      }
      if (this.CState != FlintlockPaperCartridge.CartridgeState.BitOpen || (double) Vector3.Angle(-this.transform.forward, Vector3.up) <= 120.0 || (this.numPowderChunksLeft <= 0 || (double) this.timeSinceSpawn <= 0.0399999991059303))
        return;
      --this.numPowderChunksLeft;
      this.timeSinceSpawn = 0.0f;
      Object.Instantiate<GameObject>(this.PowderPrefab, this.PowderSpawnPoint.position, Random.rotation);
    }

    private void TapOut()
    {
      SM.PlayGenericSound(this.AudEvent_Tap, this.transform.position);
      if (this.numPowderChunksLeft <= 0)
        return;
      --this.numPowderChunksLeft;
      this.timeSinceSpawn = 0.0f;
      Object.Instantiate<GameObject>(this.PowderPrefab, this.PowderSpawnPoint.position, Random.rotation);
    }

    public enum CartridgeState
    {
      Whole,
      BitOpen,
      Jammed,
    }
  }
}
