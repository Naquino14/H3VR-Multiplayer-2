// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockWeapon
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FlintlockWeapon : FVRFireArm
  {
    [Header("Flintlock")]
    public List<FlintlockFlashPan> FlashPans;
    private int m_curFlashpan;
    public FlintlockPseudoRamRod RamRod;
    [Header("Trigger")]
    public Transform Trigger;
    public Vector2 TriggerRots = new Vector2(0.0f, 5f);
    private float m_triggerFloat;
    private float m_lastTriggerFloat;
    [Header("Hammer")]
    public Transform Hammer;
    public FlintlockWeapon.HState HammerState;
    public Vector3 HammerRots = new Vector3(0.0f, 20f, 45f);
    private float m_curHammerRot;
    private float m_tarHammerRot;
    [Header("Flint")]
    public FlintlockWeapon.FlintState FState;
    private Vector3 m_flintUses = Vector3.one;
    private bool m_hasFlint = true;
    public MeshFilter FlintMesh;
    public MeshRenderer FlintRenderer;
    public List<Mesh> FlintMeshes;
    public FlintlockFlintScrew FlintlockScrew;
    public FlintlockFlintHolder FlintlockHolder;
    public ParticleSystem Sparks;
    [Header("Audio")]
    public AudioEvent AudEvent_HammerCock;
    public AudioEvent AudEvent_HammerHalfCock;
    public AudioEvent AudEvent_HammerHit_Clean;
    public AudioEvent AudEvent_HammerHit_Dull;
    public AudioEvent AudEvent_Spark;
    public AudioEvent AudEvent_FlintBreak;
    public AudioEvent AudEvent_FlintHolderScrew;
    public AudioEvent AudEvent_FlintHolderUnscrew;
    public AudioEvent AudEvent_FlintRemove;
    public AudioEvent AudEvent_FlintReplace;
    [Header("Destruction")]
    public List<GameObject> DisableOnDestroy;
    public List<GameObject> EnableOnDestroy;
    private bool m_isDestroyed;
    public List<GameObject> SpawnOnDestroy;
    public Transform SpawnOnDestroyPoint;
    public GameObject RamRodProj;
    private float FireRefire = 0.2f;

    public bool HasFlint() => this.m_hasFlint;

    protected override void Awake()
    {
      base.Awake();
      for (int index = 0; index < this.FlashPans.Count; ++index)
        this.FlashPans[index].SetWeapon(this);
      this.m_flintUses = new Vector3((float) Random.Range(8, 15), (float) Random.Range(5, 9), (float) Random.Range(4, 8));
    }

    public void Blowup()
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      for (int index = 0; index < this.DisableOnDestroy.Count; ++index)
        this.DisableOnDestroy[index].SetActive(false);
      for (int index = 0; index < this.EnableOnDestroy.Count; ++index)
        this.EnableOnDestroy[index].SetActive(true);
      for (int index = 0; index < this.SpawnOnDestroy.Count; ++index)
        Object.Instantiate<GameObject>(this.SpawnOnDestroy[index], this.SpawnOnDestroyPoint.position, this.SpawnOnDestroyPoint.rotation);
    }

    public void AddFlint(Vector3 uses)
    {
      this.m_hasFlint = true;
      this.m_flintUses = uses;
      this.FlintRenderer.enabled = true;
      if ((double) this.m_flintUses.x > 0.0)
        this.SetFlintState(FlintlockWeapon.FlintState.New);
      else if ((double) this.m_flintUses.y > 0.0)
        this.SetFlintState(FlintlockWeapon.FlintState.Used);
      else if ((double) this.m_flintUses.z > 0.0)
        this.SetFlintState(FlintlockWeapon.FlintState.Worn);
      else
        this.SetFlintState(FlintlockWeapon.FlintState.Broken);
    }

    public Vector3 RemoveFlint()
    {
      this.m_hasFlint = false;
      this.FlintRenderer.enabled = false;
      return this.m_flintUses;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      this.m_triggerFloat = !this.m_hasTriggeredUpSinceBegin || this.IsAltHeld ? 0.0f : hand.Input.TriggerFloat;
      if (this.m_hasTriggeredUpSinceBegin && !this.m_isDestroyed)
      {
        if (hand.IsInStreamlinedMode)
        {
          if (hand.Input.BYButtonDown)
            this.MoveToHalfCock();
          if (hand.Input.AXButtonDown)
            this.MoveToFullCock();
        }
        else if (hand.Input.TouchpadDown)
        {
          if (hand.Input.TouchpadWestPressed || hand.Input.TouchpadEastPressed)
            this.MoveToHalfCock();
          else if (hand.Input.TouchpadSouthPressed)
            this.MoveToFullCock();
        }
      }
      base.UpdateInteraction(hand);
    }

    private bool HitWithFlint()
    {
      if (!this.m_hasFlint)
        return false;
      switch (this.FState)
      {
        case FlintlockWeapon.FlintState.New:
          --this.m_flintUses.x;
          if ((double) this.m_flintUses.x <= 0.0)
          {
            this.SetFlintState(FlintlockWeapon.FlintState.Used);
            this.PlayAudioAsHandling(this.AudEvent_FlintBreak, this.FlintRenderer.transform.position);
          }
          return true;
        case FlintlockWeapon.FlintState.Used:
          --this.m_flintUses.y;
          if ((double) this.m_flintUses.y <= 0.0)
          {
            this.SetFlintState(FlintlockWeapon.FlintState.Worn);
            this.PlayAudioAsHandling(this.AudEvent_FlintBreak, this.FlintRenderer.transform.position);
          }
          return true;
        case FlintlockWeapon.FlintState.Worn:
          --this.m_flintUses.z;
          if ((double) this.m_flintUses.z <= 0.0)
          {
            this.SetFlintState(FlintlockWeapon.FlintState.Broken);
            this.PlayAudioAsHandling(this.AudEvent_FlintBreak, this.FlintRenderer.transform.position);
          }
          return true;
        case FlintlockWeapon.FlintState.Broken:
          return false;
        default:
          return false;
      }
    }

    private void SetFlintState(FlintlockWeapon.FlintState f)
    {
      this.FState = f;
      this.FlintMesh.mesh = this.FlintMeshes[(int) f];
    }

    public void Fire(float recoilMult = 1f)
    {
      if ((double) this.FireRefire < 0.100000001490116)
        return;
      this.FireRefire = 0.0f;
      if ((Object) this.m_hand != (Object) null)
      {
        this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
        if ((Object) this.AltGrip != (Object) null && (Object) this.AltGrip.m_hand != (Object) null)
          this.AltGrip.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
      }
      GM.CurrentSceneSettings.OnShotFired((FVRFireArm) this);
      if (this.IsSuppressed())
        GM.CurrentPlayerBody.VisibleEvent(0.1f);
      else
        GM.CurrentPlayerBody.VisibleEvent(2f);
      this.Recoil(this.IsTwoHandStabilized(), this.IsForegripStabilized(), this.IsShoulderStabilized(), VerticalRecoilMult: recoilMult);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.FireRefire < 0.200000002980232)
        this.FireRefire += Time.deltaTime;
      if ((double) this.m_triggerFloat != (double) this.m_lastTriggerFloat)
      {
        this.Trigger.localEulerAngles = new Vector3(Mathf.Lerp(this.TriggerRots.x, this.TriggerRots.y, this.m_triggerFloat), 0.0f, 0.0f);
        this.m_lastTriggerFloat = this.m_triggerFloat;
      }
      if (this.m_isDestroyed)
        return;
      if ((double) this.m_curHammerRot != (double) this.m_tarHammerRot)
      {
        float num = 7200f;
        if (this.HammerState == FlintlockWeapon.HState.Halfcock || this.HammerState == FlintlockWeapon.HState.Fullcock)
          num = 360f;
        this.m_curHammerRot = Mathf.MoveTowards(this.m_curHammerRot, this.m_tarHammerRot, Time.deltaTime * num);
        this.SetAnimatedComponent(this.Hammer, this.m_curHammerRot, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
      }
      if ((double) this.m_triggerFloat <= 0.699999988079071 || this.HammerState != FlintlockWeapon.HState.Fullcock)
        return;
      this.ReleaseHammer();
    }

    private void ReleaseHammer()
    {
      if (this.HammerState != FlintlockWeapon.HState.Fullcock)
        return;
      this.HammerState = FlintlockWeapon.HState.Uncocked;
      this.m_tarHammerRot = this.HammerRots.x;
      if (this.HitWithFlint())
      {
        this.PlayAudioAsHandling(this.AudEvent_HammerHit_Clean, this.Hammer.position);
        if (this.FlashPans[this.m_curFlashpan].FrizenState == FlintlockFlashPan.FState.Down)
        {
          this.PlayAudioAsHandling(this.AudEvent_Spark, this.Hammer.position);
          this.Sparks.Emit(15);
        }
        this.FlashPans[this.m_curFlashpan].HammerHit(this.FState, true);
      }
      else
      {
        this.FlashPans[this.m_curFlashpan].HammerHit(this.FState, false);
        this.PlayAudioAsHandling(this.AudEvent_HammerHit_Dull, this.Hammer.position);
      }
    }

    private void MoveToHalfCock()
    {
      if (this.HammerState != FlintlockWeapon.HState.Uncocked)
        return;
      this.HammerState = FlintlockWeapon.HState.Halfcock;
      this.m_tarHammerRot = this.HammerRots.y;
      this.PlayAudioAsHandling(this.AudEvent_HammerHalfCock, this.Hammer.position);
    }

    private void MoveToFullCock()
    {
      if (this.HammerState == FlintlockWeapon.HState.Fullcock)
        return;
      this.HammerState = FlintlockWeapon.HState.Fullcock;
      this.m_tarHammerRot = this.HammerRots.z;
      this.PlayAudioAsHandling(this.AudEvent_HammerCock, this.Hammer.position);
    }

    public enum HState
    {
      Uncocked,
      Halfcock,
      Fullcock,
    }

    public enum FlintState
    {
      New,
      Used,
      Worn,
      Broken,
    }
  }
}
