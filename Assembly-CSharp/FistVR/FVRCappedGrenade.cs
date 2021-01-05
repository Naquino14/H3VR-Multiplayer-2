// Decompiled with JetBrains decompiler
// Type: FistVR.FVRCappedGrenade
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRCappedGrenade : FVRMeleeWeapon
  {
    [Header("Grenade Stuff")]
    public GameObject Cap_Primary_Prefab;
    public bool IsPrimaryCapRemoved;
    public bool UsesSecondaryCap;
    public GameObject Cap_Secondary_Prefab;
    public bool IsSecondaryCapRemoved;
    public FVRCappedGrenade.CappedGrenadeFuseType FuseType;
    public float FuseTime = 4f;
    private float m_fuseTimeLeft = 4f;
    private bool m_IsFuseActive;
    public AudioEvent AudEvent_CapRemovePrimary;
    public AudioEvent AudEvent_CapRemoveSecondary;
    [Header("Explosion Stuff")]
    public GameObject[] SpawnOnDestruction;
    private bool m_hasExploded;
    public bool HasPopOutShell;
    public List<Transform> ShellPieces;
    public List<Vector3> ShellPoses;

    protected override void Start()
    {
      base.Start();
      this.m_fuseTimeLeft = this.FuseTime;
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (!this.m_IsFuseActive || this.FuseType != FVRCappedGrenade.CappedGrenadeFuseType.Timed)
        return;
      this.m_fuseTimeLeft -= Time.deltaTime;
      if ((double) this.m_fuseTimeLeft > 0.0)
        return;
      this.Explode();
    }

    public void CapRemoved(bool isPrimary, FVRViveHand hand, FVRCappedGrenadeCap cap)
    {
      if (isPrimary)
      {
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_CapRemovePrimary, this.transform.position);
        this.IsPrimaryCapRemoved = true;
      }
      else
      {
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_CapRemoveSecondary, this.transform.position);
        this.IsSecondaryCapRemoved = true;
      }
      if (this.UsesSecondaryCap && this.IsPrimaryCapRemoved && this.IsSecondaryCapRemoved || !this.UsesSecondaryCap && this.IsPrimaryCapRemoved)
        this.m_IsFuseActive = true;
      FVRPhysicalObject component = Object.Instantiate<GameObject>(!isPrimary ? this.Cap_Secondary_Prefab : this.Cap_Primary_Prefab, cap.transform.position, cap.transform.rotation).GetComponent<FVRPhysicalObject>();
      cap.EndInteraction(hand);
      hand.ForceSetInteractable((FVRInteractiveObject) component);
      component.BeginInteraction(hand);
      Object.Destroy((Object) cap.gameObject);
      if (!this.HasPopOutShell)
        return;
      for (int index = 0; index < this.ShellPieces.Count; ++index)
        this.ShellPieces[index].localPosition = this.ShellPoses[index];
    }

    public override void OnCollisionEnter(Collision col)
    {
      base.OnCollisionEnter(col);
      if (!this.m_IsFuseActive || this.FuseType != FVRCappedGrenade.CappedGrenadeFuseType.Impact || ((double) col.relativeVelocity.magnitude <= 1.5 || this.IsHeld))
        return;
      this.Explode();
    }

    private void Explode()
    {
      if (this.m_hasExploded)
        return;
      this.m_hasExploded = true;
      for (int index = 0; index < this.SpawnOnDestruction.Length; ++index)
        Object.Instantiate<GameObject>(this.SpawnOnDestruction[index], this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    public enum CappedGrenadeFuseType
    {
      Timed,
      Impact,
    }
  }
}
