// Decompiled with JetBrains decompiler
// Type: FistVR.FVRGrenade
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRGrenade : FVRPhysicalObject
  {
    public Transform FuseCylinder;
    private int m_fuseCylinderSetting = 2;
    private float m_fuseCurYRotation;
    private float m_fuseTarYRotation;
    public FVRGrenadePin Pin;
    private bool m_isPinPulled;
    public bool Uses2ndPin;
    public FVRGrenadePin Pin2;
    private bool m_isPinPulled2;
    public HingeJoint LeverJoint;
    private bool m_isLeverReleased;
    public GameObject FakeHandle;
    public GameObject RealHandle;
    private Dictionary<int, float> FuseTimings = new Dictionary<int, float>();
    private bool m_isFused;
    private float m_startFuseTime = 15f;
    private float m_fuseTime = 15f;
    public float DefaultFuse = 3.3f;
    private float m_fuse_tick;
    private float m_fuse_PitchStart = 1.8f;
    private float m_fuse_PitchEnd = 3.7f;
    private float m_fuse_StartRefire = 0.4f;
    private float m_fuse_EndRefire = 0.02f;
    public GameObject ExplosionFX;
    public GameObject ExplosionSoundFX;
    public AudioSource FuseAudio;
    public ParticleSystem FusePSystem;
    private Vector3 m_releasePoint;
    public bool FuseOnSpawn;
    public SmokeSolidEmitter SmokeEmitter;
    public AudioEvent AudioEvent_StrikerActivate;
    public AudioEvent AudioEvent_SafetyLeverRelease;
    public AudioEvent AudioEvent_Pinpull;
    private bool m_hasSploded;
    public int IFF;

    public override int GetTutorialState() => this.m_isPinPulled ? 1 : 0;

    public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
    {
      GameObject gameObject = base.DuplicateFromSpawnLock(hand);
      gameObject.GetComponent<FVRGrenade>().SetFuseSetting(this.m_fuseCylinderSetting);
      return gameObject;
    }

    public void SetFuseSetting(int i) => this.m_fuseCylinderSetting = i;

    private void IncreaseFuseSetting()
    {
      if (this.m_fuseCylinderSetting < 4)
        ++this.m_fuseCylinderSetting;
      else
        this.m_fuseCylinderSetting = 0;
      this.m_fuseTarYRotation = (float) ((double) this.m_fuseCylinderSetting * 24.0 - 48.0);
    }

    private void DecreaseFuseSetting()
    {
      if (this.m_fuseCylinderSetting > 0)
        --this.m_fuseCylinderSetting;
      this.m_fuseTarYRotation = (float) ((double) this.m_fuseCylinderSetting * 24.0 - 48.0);
    }

    public void PullPin()
    {
      this.m_isPinPulled = true;
      SM.PlayGenericSound(this.AudioEvent_Pinpull, this.transform.position);
      if (this.IsHeld || this.Uses2ndPin && !this.m_isPinPulled2)
        return;
      this.ReleaseLever();
    }

    public void PullPin2()
    {
      this.m_isPinPulled2 = true;
      SM.PlayGenericSound(this.AudioEvent_Pinpull, this.transform.position);
      if (this.IsHeld || !this.m_isPinPulled)
        return;
      this.ReleaseLever();
    }

    public void ReleaseLever()
    {
      if (this.m_isLeverReleased)
        return;
      SM.PlayGenericSound(this.AudioEvent_StrikerActivate, this.transform.position);
      SM.PlayGenericSound(this.AudioEvent_SafetyLeverRelease, this.transform.position);
      this.m_isLeverReleased = true;
      this.FakeHandle.SetActive(false);
      this.RealHandle.SetActive(true);
      JointSpring spring = this.LeverJoint.spring;
      spring.targetPosition = -135f;
      spring.spring = 4f;
      this.LeverJoint.spring = spring;
      this.m_isFused = true;
      if ((Object) this.FuseCylinder != (Object) null)
      {
        this.m_startFuseTime = this.FuseTimings[this.m_fuseCylinderSetting];
        this.m_fuseTime = this.FuseTimings[this.m_fuseCylinderSetting];
      }
      else
      {
        this.m_startFuseTime = this.DefaultFuse;
        this.m_fuseTime = this.DefaultFuse;
      }
      this.m_releasePoint = this.transform.position;
    }

    protected override void Awake()
    {
      base.Awake();
      this.FuseTimings.Add(0, 12f);
      this.FuseTimings.Add(1, 8f);
      this.FuseTimings.Add(2, 5f);
      this.FuseTimings.Add(3, 3f);
      this.FuseTimings.Add(4, 2f);
      if (!this.FuseOnSpawn)
        return;
      this.Invoke("PullPin", 0.1f);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((Object) this.FuseCylinder != (Object) null)
      {
        this.m_fuseCurYRotation = Mathf.Lerp(this.m_fuseCurYRotation, this.m_fuseTarYRotation, Time.deltaTime * 8f);
        this.FuseCylinder.localEulerAngles = new Vector3(0.0f, this.m_fuseCurYRotation, 0.0f);
      }
      if ((Object) this.LeverJoint != (Object) null && this.m_isLeverReleased && (double) Mathf.Abs(this.LeverJoint.angle) > 130.0)
      {
        this.LeverJoint.transform.SetParent((Transform) null);
        Object.Destroy((Object) this.LeverJoint);
      }
      if (!this.m_isFused)
        return;
      this.m_fuseTime -= Time.deltaTime;
      if ((Object) this.FuseCylinder != (Object) null)
      {
        float t = Mathf.Pow(Mathf.Clamp((float) (1.0 - (double) this.m_fuseTime / (double) this.m_startFuseTime), 0.0f, 1f), 2f);
        if ((double) this.m_fuse_tick <= 0.0)
        {
          this.m_fuse_tick = Mathf.Lerp(this.m_fuse_StartRefire, this.m_fuse_EndRefire, t);
          this.FuseAudio.pitch = Mathf.Lerp(this.m_fuse_PitchStart, this.m_fuse_PitchEnd, t);
          this.FuseAudio.PlayOneShot(this.FuseAudio.clip);
          this.FusePSystem.Emit(2);
        }
        else
          this.m_fuse_tick -= Time.deltaTime;
      }
      if ((double) this.m_fuseTime > 0.0500000007450581)
        return;
      if (!this.m_hasSploded)
      {
        this.m_hasSploded = true;
        if ((Object) this.ExplosionFX != (Object) null)
        {
          GameObject gameObject = Object.Instantiate<GameObject>(this.ExplosionFX, this.transform.position, Quaternion.identity);
          Explosion component1 = gameObject.GetComponent<Explosion>();
          if ((Object) component1 != (Object) null)
            component1.IFF = this.IFF;
          ExplosionSound component2 = gameObject.GetComponent<ExplosionSound>();
          if ((Object) component2 != (Object) null)
            component2.IFF = this.IFF;
        }
        if ((Object) this.ExplosionSoundFX != (Object) null)
        {
          GameObject gameObject = Object.Instantiate<GameObject>(this.ExplosionSoundFX, this.transform.position, Quaternion.identity);
          Explosion component1 = gameObject.GetComponent<Explosion>();
          if ((Object) component1 != (Object) null)
            component1.IFF = this.IFF;
          ExplosionSound component2 = gameObject.GetComponent<ExplosionSound>();
          if ((Object) component2 != (Object) null)
            component2.IFF = this.IFF;
        }
      }
      if ((Object) this.SmokeEmitter != (Object) null)
      {
        this.SmokeEmitter.Engaged = true;
      }
      else
      {
        if (this.IsHeld)
        {
          FVRViveHand hand = this.m_hand;
          this.m_hand.ForceSetInteractable((FVRInteractiveObject) null);
          this.EndInteraction(hand);
        }
        Object.Destroy((Object) this.gameObject);
      }
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.IFF = GM.CurrentPlayerBody.GetPlayerIFF();
      bool flag1 = false;
      if (hand.IsInStreamlinedMode && hand.Input.BYButtonDown)
        flag1 = true;
      else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown)
        flag1 = true;
      if ((Object) this.FuseCylinder != (Object) null && !this.m_isPinPulled && flag1)
        this.IncreaseFuseSetting();
      bool flag2 = false;
      if (this.Uses2ndPin)
      {
        if (this.m_isPinPulled && this.m_isPinPulled2 && !this.m_isLeverReleased)
          flag2 = true;
      }
      else if (this.m_isPinPulled && !this.m_isLeverReleased)
        flag2 = true;
      if (!flag2 || !flag1)
        return;
      this.ReleaseLever();
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      bool flag = false;
      if (this.Uses2ndPin)
      {
        if (this.m_isPinPulled && this.m_isPinPulled2 && !this.m_isLeverReleased)
          flag = true;
      }
      else if (this.m_isPinPulled && !this.m_isLeverReleased)
        flag = true;
      if (!flag)
        return;
      this.ReleaseLever();
    }
  }
}
