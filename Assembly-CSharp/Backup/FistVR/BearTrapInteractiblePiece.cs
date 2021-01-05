// Decompiled with JetBrains decompiler
// Type: FistVR.BearTrapInteractiblePiece
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class BearTrapInteractiblePiece : FVRInteractiveObject
  {
    public Transform BaseTransform;
    public Transform BladeTransform;
    public Transform Reference_Open;
    public Transform Reference_Closed;
    public AudioEvent AudEvent_SnapShut;
    public AudioEvent AudEvent_LockOpen;
    public BearTrapInteractiblePiece.BearTrapState State;
    public LayerMask LM_Damage;
    public Transform DamagePoint;
    public float DamageRadius;
    public Transform fakeDamPoint;
    public Transform fakeDamDir;
    private BearTrap m_trap;

    public void SetBearTrap(BearTrap trap) => this.m_trap = trap;

    public override bool IsInteractable() => this.State == BearTrapInteractiblePiece.BearTrapState.Closed;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.State = BearTrapInteractiblePiece.BearTrapState.InteractingWith;
      Vector3 vector3 = Vector3.ProjectOnPlane(hand.transform.position - this.BaseTransform.position, this.BladeTransform.forward);
      float num = Vector3.Angle(vector3, Vector3.up);
      if ((double) num < 24.0)
        this.BladeTransform.rotation = this.Reference_Closed.rotation;
      else if ((double) num > 60.0)
      {
        if ((Object) GM.ZMaster != (Object) null)
          GM.ZMaster.FlagM.AddToFlag("s_t", 1);
        this.LockOpen();
        this.m_trap.ForceOpen();
        this.ForceBreakInteraction();
      }
      else
        this.BladeTransform.rotation = Quaternion.LookRotation(this.Reference_Closed.forward, vector3);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      float num = Vector3.Angle(Vector3.ProjectOnPlane(hand.transform.position - this.BaseTransform.position, this.BladeTransform.forward), Vector3.up);
      if ((double) num < 24.0)
        this.Close();
      else if ((double) num > 60.0)
      {
        this.LockOpen();
        this.m_trap.ForceOpen();
      }
      else
        this.SnapShut();
      base.EndInteraction(hand);
    }

    public void Open()
    {
      this.BladeTransform.rotation = this.Reference_Open.rotation;
      this.State = BearTrapInteractiblePiece.BearTrapState.Open;
    }

    public void Close()
    {
      this.BladeTransform.rotation = this.Reference_Closed.rotation;
      this.State = BearTrapInteractiblePiece.BearTrapState.Closed;
    }

    public void LockOpen()
    {
      if (this.State == BearTrapInteractiblePiece.BearTrapState.Open)
        return;
      this.BladeTransform.rotation = this.Reference_Open.rotation;
      SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, this.AudEvent_LockOpen, this.transform.position, Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
      this.State = BearTrapInteractiblePiece.BearTrapState.Open;
    }

    public void SnapShut()
    {
      if (this.State == BearTrapInteractiblePiece.BearTrapState.Closed)
        return;
      this.BladeTransform.rotation = this.Reference_Closed.rotation;
      SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, this.AudEvent_SnapShut, this.transform.position, Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
      this.State = BearTrapInteractiblePiece.BearTrapState.Closed;
      Collider[] colliderArray = Physics.OverlapSphere(this.DamagePoint.position, this.DamageRadius, (int) this.LM_Damage, QueryTriggerInteraction.Ignore);
      if (colliderArray.Length <= 0)
        return;
      HashSet<IFVRDamageable> fvrDamageableSet = new HashSet<IFVRDamageable>();
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        if ((Object) colliderArray[index].attachedRigidbody != (Object) null)
        {
          IFVRDamageable component = colliderArray[index].attachedRigidbody.GetComponent<IFVRDamageable>();
          if (component != null && !fvrDamageableSet.Contains(component))
          {
            fvrDamageableSet.Add(component);
            component.Damage(new Damage()
            {
              Dam_Cutting = 15000f,
              Dam_Piercing = 15000f,
              Dam_TotalKinetic = 30000f,
              damageSize = 0.1f,
              Class = Damage.DamageClass.Environment,
              hitNormal = -this.fakeDamDir.forward,
              point = this.fakeDamPoint.position,
              Source_IFF = 0,
              strikeDir = this.fakeDamDir.forward
            });
          }
        }
      }
      fvrDamageableSet.Clear();
    }

    public bool CanSnapShut() => this.State == BearTrapInteractiblePiece.BearTrapState.Open;

    public enum BearTrapState
    {
      Open,
      Closed,
      InteractingWith,
    }
  }
}
