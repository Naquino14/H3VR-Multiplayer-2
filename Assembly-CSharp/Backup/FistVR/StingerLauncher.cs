// Decompiled with JetBrains decompiler
// Type: FistVR.StingerLauncher
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class StingerLauncher : FVRFireArm
  {
    [Header("Stinger Launcher Config")]
    public StingerLauncherFore Fore;
    public GameObject MissilePrefab;
    private bool m_hasMissile = true;
    public GameObject MissileDisplay;
    private bool m_isTargetttingEngaged;
    private bool m_hasTargetLock;
    private bool m_isCameraUnlocked;
    public Transform TargettingDirection;
    public float MaxAngleToTarget = 3f;
    public LayerMask TargettingLM;
    private AIEntity m_lockingEntity;
    private AIEntity m_targetEntity;
    private float m_lockTime;
    public AudioSource AudSource_TargetSound;
    public AudioClip AudClip_Targetting;
    public AudioClip AudClip_TargetLock;
    public AudioEvent AudEvent_Chirp;
    public Transform Trigger;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.IsAltHeld && hand.Input.TriggerDown && (this.m_hasTriggeredUpSinceBegin && this.m_hasTargetLock) && this.m_isCameraUnlocked)
        this.Fire();
      if (this.IsAltHeld || !this.m_hasTriggeredUpSinceBegin)
        return;
      this.SetAnimatedComponent(this.Trigger, hand.Input.TriggerFloat * 20f, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
    }

    private void ToggleTargettingEnabled()
    {
      if (!this.m_isTargetttingEngaged)
      {
        this.m_isTargetttingEngaged = true;
      }
      else
      {
        this.m_isTargetttingEngaged = false;
        this.m_hasTargetLock = false;
      }
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.m_hasMissile || !this.IsHeld || ((Object) this.Magazine == (Object) null || !this.Magazine.HasFuel()))
      {
        this.m_isTargetttingEngaged = false;
        this.m_hasTargetLock = false;
        this.m_isCameraUnlocked = false;
        this.m_targetEntity = (AIEntity) null;
        this.m_lockingEntity = (AIEntity) null;
        this.m_lockTime = 0.0f;
      }
      else if (this.IsHeld)
      {
        bool flag1 = false;
        bool flag2 = false;
        if (this.m_hand.IsInStreamlinedMode)
        {
          if (this.m_hand.Input.BYButtonDown)
            flag1 = true;
          if (this.Fore.IsHeld && this.Fore.m_hand.Input.BYButtonPressed)
            flag2 = true;
        }
        else
        {
          if (this.m_hand.Input.TouchpadDown)
            flag1 = true;
          if (this.Fore.IsHeld && this.Fore.m_hand.Input.TouchpadPressed)
            flag2 = true;
        }
        if (flag1 && this.m_hasTriggeredUpSinceBegin)
        {
          this.PlayAudioEvent(FirearmAudioEventType.FireSelector);
          this.ToggleTargettingEnabled();
        }
        if (this.m_hasTargetLock && (Object) this.m_targetEntity != (Object) null && (double) Vector3.Angle(this.m_targetEntity.GetPos() - this.TargettingDirection.position, this.TargettingDirection.forward) > (double) this.MaxAngleToTarget)
        {
          this.m_hasTargetLock = false;
          this.m_targetEntity = (AIEntity) null;
          this.m_lockingEntity = (AIEntity) null;
          this.m_lockTime = 0.0f;
        }
        if (!this.m_hasTargetLock && this.m_isTargetttingEngaged)
        {
          Collider[] colliderArray1 = Physics.OverlapCapsule(this.TargettingDirection.position, this.TargettingDirection.position + this.TargettingDirection.forward * 2000f, 3f, (int) this.TargettingLM, QueryTriggerInteraction.Collide);
          float num1 = 2400f;
          Collider collider = (Collider) null;
          bool flag3 = false;
          Vector3 position1 = this.TargettingDirection.position;
          for (int index = 0; index < colliderArray1.Length; ++index)
          {
            float num2 = Vector3.Distance(colliderArray1[index].transform.position, position1);
            if ((double) num2 > 20.0 && (double) num2 < (double) num1 && (Object) colliderArray1[index].GetComponent<AIEntity>() != (Object) null)
            {
              flag3 = true;
              collider = colliderArray1[index];
              num1 = num2;
            }
          }
          if (!flag3)
          {
            Collider[] colliderArray2 = Physics.OverlapCapsule(this.TargettingDirection.position + this.TargettingDirection.forward * 500f, this.TargettingDirection.position + this.TargettingDirection.forward * 2000f, 20f, (int) this.TargettingLM, QueryTriggerInteraction.Collide);
            float num2 = 2400f;
            collider = (Collider) null;
            flag3 = false;
            Vector3 position2 = this.TargettingDirection.position;
            for (int index = 0; index < colliderArray2.Length; ++index)
            {
              float num3 = Vector3.Distance(colliderArray2[index].transform.position, position2);
              if ((double) num3 > 20.0 && (double) num3 < (double) num2 && (Object) colliderArray2[index].GetComponent<AIEntity>() != (Object) null)
              {
                flag3 = true;
                collider = colliderArray2[index];
                num2 = num3;
              }
            }
          }
          if (flag3)
            this.m_lockingEntity = collider.GetComponent<AIEntity>();
        }
        if ((Object) this.m_lockingEntity != (Object) null && (Object) this.m_targetEntity == (Object) null)
        {
          this.m_lockTime += Time.deltaTime;
          if ((double) this.m_lockTime >= 3.5)
          {
            this.m_targetEntity = this.m_lockingEntity;
            this.m_hasTargetLock = true;
            SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Chirp, this.AudSource_TargetSound.transform.position);
          }
        }
        if (this.m_hasTargetLock && flag2)
        {
          this.m_isCameraUnlocked = true;
          this.MaxAngleToTarget = 30f;
        }
        else
        {
          this.m_isCameraUnlocked = false;
          this.MaxAngleToTarget = 3f;
        }
      }
      if (!this.IsHeld || !this.m_hasMissile)
      {
        if (this.AudSource_TargetSound.isPlaying)
          this.AudSource_TargetSound.Stop();
      }
      else if (this.m_hasTargetLock)
      {
        this.AudSource_TargetSound.clip = this.AudClip_TargetLock;
        if (!this.AudSource_TargetSound.isPlaying)
          this.AudSource_TargetSound.Play();
      }
      else if (this.m_isTargetttingEngaged)
      {
        this.AudSource_TargetSound.clip = this.AudClip_Targetting;
        if (!this.AudSource_TargetSound.isPlaying)
          this.AudSource_TargetSound.Play();
      }
      else if (this.AudSource_TargetSound.isPlaying)
        this.AudSource_TargetSound.Stop();
      if (!((Object) this.Magazine != (Object) null) || !this.Magazine.HasFuel())
        return;
      this.Magazine.DrainFuel(Time.deltaTime);
    }

    public void Fire()
    {
      if (!this.m_hasMissile)
        return;
      Object.Instantiate<GameObject>(this.MissilePrefab, this.GetMuzzle().position, this.GetMuzzle().rotation).GetComponent<StingerMissile>().Fire(this.m_targetEntity);
      this.FireMuzzleSmoke();
      this.PlayAudioGunShot(true, FVRTailSoundClass.Launcher, FVRTailSoundClass.SuppressedLarge, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      if ((Object) this.m_hand != (Object) null)
      {
        this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
        if ((Object) this.AltGrip != (Object) null && (Object) this.AltGrip.m_hand != (Object) null)
          this.AltGrip.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
      }
      GM.CurrentSceneSettings.OnShotFired((FVRFireArm) this);
      GM.CurrentPlayerBody.VisibleEvent(4f);
      this.m_hasMissile = false;
      this.MissileDisplay.SetActive(false);
      this.m_isTargetttingEngaged = false;
      this.m_hasTargetLock = false;
      this.m_isCameraUnlocked = false;
      this.m_targetEntity = (AIEntity) null;
      this.m_lockingEntity = (AIEntity) null;
      this.m_lockTime = 0.0f;
    }
  }
}
