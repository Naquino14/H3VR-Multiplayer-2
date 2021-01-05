// Decompiled with JetBrains decompiler
// Type: FistVR.Minigun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Minigun : FVRFireArm
  {
    [Header("Minigun Config")]
    public Transform ForeEnd;
    private float m_foreRot;
    [Header("Bullet Config")]
    public GameObject Projectile;
    private int m_numBullets;
    public int m_maxBullets = 14;
    public bool UsesDisplay = true;
    public int m_firingFrameDelay = 1;
    public int m_tickToFire = 1;
    private bool m_isTriggerEngaged;
    private bool m_isMotorSpinningUp;
    private float m_motorRate;
    private float m_tarMotorRate;
    private float m_timeSinceLastShot = 1f;
    public FVRTailSoundClass TailClass = FVRTailSoundClass.FullPower;
    public Minigun.MotorState m_curState;
    public Minigun.MotorState m_lastState;
    [Header("Proxy Bullet Config")]
    public GameObject[] ProxyBullets;
    public MeshFilter[] ProxyMeshFilters;
    public Renderer[] ProxyRenderers;
    public FVRLoadedRound[] LoadedRounds;
    [Header("Audio Config")]
    public AudioSource MinigunMotorAudioSource;
    public AudioClip ClipMotorIdle;
    public AudioClip ClipFiring;
    public AudioClip ClipEmpty;
    public bool UsesEmptyClip;
    public ParticleSystem Sparks;
    public ParticleSystem Eject_Shells;
    public ParticleSystem Eject_Links;
    public bool PlaysShotSound = true;
    public Renderer MinigunFore;
    public bool ChangesColor = true;
    private float m_heat;
    private bool m_hasCycledUp;
    private int DestabilizedShots;

    protected override void Awake()
    {
      base.Awake();
      this.UpdateDisplay();
      this.RootRigidbody.maxAngularVelocity = 15f;
    }

    public override int GetTutorialState()
    {
      if ((Object) this.Magazine != (Object) null && !this.Magazine.HasARound())
        return 3;
      if ((Object) this.AltGrip != (Object) null)
        return 2;
      return (Object) this.Magazine == (Object) null ? 0 : 1;
    }

    private void UpdateBulletResevoir()
    {
      if (!((Object) this.Magazine != (Object) null) || this.m_numBullets >= this.m_maxBullets)
        return;
      this.LoadRoundFromMag();
    }

    private void LoadRoundFromMag()
    {
      MinigunBox magazine = (MinigunBox) this.Magazine;
      if (!magazine.HasAmmo())
        return;
      magazine.RemoveRound();
      this.LoadedRounds[this.m_numBullets].LR_Class = magazine.RoundClass;
      this.LoadedRounds[this.m_numBullets].LR_Mesh = AM.GetRoundMesh(magazine.RoundType, magazine.RoundClass);
      this.LoadedRounds[this.m_numBullets].LR_Material = AM.GetRoundMaterial(magazine.RoundType, magazine.RoundClass);
      this.LoadedRounds[this.m_numBullets].LR_ProjectilePrefab = magazine.ProjectilePrefab;
      ++this.m_numBullets;
      this.UpdateDisplay();
    }

    private void UpdateDisplay()
    {
      if (!this.UsesDisplay)
        return;
      for (int index = 0; index < this.ProxyBullets.Length; ++index)
      {
        if (this.m_numBullets > index)
        {
          if (!this.ProxyBullets[index].activeSelf)
            this.ProxyBullets[index].SetActive(true);
          this.ProxyMeshFilters[index].mesh = this.LoadedRounds[index].LR_Mesh;
          this.ProxyRenderers[index].material = this.LoadedRounds[index].LR_Material;
        }
        else if (this.ProxyBullets[index].activeSelf)
          this.ProxyBullets[index].SetActive(false);
      }
    }

    private void ConsumeRound()
    {
      if (this.m_numBullets > 0)
      {
        for (int index = 0; index < this.m_numBullets - 1; ++index)
        {
          this.LoadedRounds[index].LR_Class = this.LoadedRounds[index + 1].LR_Class;
          this.LoadedRounds[index].LR_Mesh = this.LoadedRounds[index + 1].LR_Mesh;
          this.LoadedRounds[index].LR_Material = this.LoadedRounds[index + 1].LR_Material;
          this.LoadedRounds[index].LR_ProjectilePrefab = this.LoadedRounds[index + 1].LR_ProjectilePrefab;
        }
        --this.m_numBullets;
      }
      this.UpdateDisplay();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.m_timeSinceLastShot < 1.0)
        this.m_timeSinceLastShot += Time.deltaTime;
      this.m_heat -= Time.deltaTime * 0.01f;
      this.m_heat = Mathf.Clamp(this.m_heat, 0.0f, 1f);
      if (this.ChangesColor)
        this.MinigunFore.material.SetFloat("_EmissionWeight", this.m_heat * this.m_heat);
      this.UpdateBulletResevoir();
      if (!this.IsHeld)
      {
        this.m_isTriggerEngaged = false;
        this.m_tarMotorRate = 0.0f;
      }
      else if (this.m_isTriggerEngaged)
        this.m_tarMotorRate = this.m_hand.Input.TriggerFloat;
      else if (this.m_hasCycledUp)
        this.m_tarMotorRate = 0.0f;
      if ((double) this.m_motorRate <= 0.00999999977648258)
        this.m_hasCycledUp = false;
      if ((double) this.m_motorRate > 0.850000023841858)
        this.m_hasCycledUp = true;
      this.m_motorRate = (double) this.m_tarMotorRate <= (double) this.m_motorRate ? Mathf.MoveTowards(this.m_motorRate, this.m_tarMotorRate, Time.deltaTime * 0.6f) : Mathf.MoveTowards(this.m_motorRate, this.m_tarMotorRate, Time.deltaTime);
      this.m_motorRate = Mathf.Clamp(this.m_motorRate, 0.0f, 1f);
      this.m_foreRot += Time.deltaTime * 1800f * this.m_motorRate;
      this.m_foreRot = Mathf.Repeat(this.m_foreRot, 360f);
      this.ForeEnd.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_foreRot);
      this.m_curState = (double) this.m_motorRate > 0.0 ? ((double) this.m_motorRate < 0.899999976158142 ? ((double) this.m_motorRate <= 0.300000011920929 ? (!this.m_isTriggerEngaged ? Minigun.MotorState.SpinningDown : Minigun.MotorState.SpinningUp) : (!this.m_isTriggerEngaged ? Minigun.MotorState.SpinningDown : Minigun.MotorState.Idling)) : (this.m_numBullets <= 0 ? Minigun.MotorState.Idling : Minigun.MotorState.Firing)) : Minigun.MotorState.Off;
      if (this.m_curState == Minigun.MotorState.SpinningUp && this.m_lastState != Minigun.MotorState.SpinningUp)
        this.PlayAudioEvent(FirearmAudioEventType.HandleUp);
      if (this.m_curState == Minigun.MotorState.SpinningDown && this.m_lastState != Minigun.MotorState.SpinningDown)
        this.PlayAudioEvent(FirearmAudioEventType.HandleDown);
      if (this.m_curState == Minigun.MotorState.Firing && (Object) this.MinigunMotorAudioSource.clip != (Object) this.ClipFiring)
        this.MinigunMotorAudioSource.clip = this.ClipFiring;
      if (this.m_curState == Minigun.MotorState.Idling && (Object) this.MinigunMotorAudioSource.clip != (Object) this.ClipMotorIdle)
        this.MinigunMotorAudioSource.clip = this.ClipMotorIdle;
      if ((this.m_curState == Minigun.MotorState.Firing || this.m_curState == Minigun.MotorState.Idling) && !this.MinigunMotorAudioSource.isPlaying)
        this.MinigunMotorAudioSource.Play();
      if (this.m_curState != Minigun.MotorState.Firing && this.m_curState != Minigun.MotorState.Idling && this.MinigunMotorAudioSource.isPlaying)
        this.MinigunMotorAudioSource.Stop();
      if (this.m_curState == Minigun.MotorState.Firing)
      {
        if (this.m_tickToFire <= 0)
        {
          this.m_tickToFire = this.m_firingFrameDelay;
          this.Fire();
        }
        else
          --this.m_tickToFire;
      }
      this.m_lastState = this.m_curState;
    }

    private void Fire()
    {
      if (this.m_numBullets <= 0)
        return;
      if ((Object) this.m_hand != (Object) null)
        this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
      if ((Object) this.AltGrip != (Object) null)
        this.AltGrip.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
      this.FireMuzzleSmoke();
      if (GM.CurrentSceneSettings.IsSceneLowLight)
        this.Sparks.Emit(8);
      else
        this.Sparks.Emit(2);
      this.Sparks.transform.position = this.GetMuzzle().position;
      this.Eject_Shells.Emit(1);
      this.Eject_Links.Emit(1);
      this.m_heat += 1f / 1000f;
      Vector3 position = this.GetMuzzle().position;
      GameObject gameObject = Object.Instantiate<GameObject>(this.LoadedRounds[0].LR_ProjectilePrefab, position, this.GetMuzzle().rotation);
      gameObject.GetComponent<BallisticProjectile>().Fire(gameObject.transform.forward, (FVRFireArm) this);
      if (this.PlaysShotSound)
      {
        if (this.IsSuppressed())
        {
          this.m_pool_shot.PlayClip(this.AudioClipSet.Shots_Suppressed, this.MuzzlePos.position);
          this.m_pool_tail.PlayClipPitchOverride(SM.GetTailSet(FVRTailSoundClass.SuppressedLarge, GM.CurrentPlayerBody.GetCurrentSoundEnvironment()), position, this.AudioClipSet.TailPitchMod_Main);
        }
        else
        {
          this.m_pool_shot.PlayClip(this.AudioClipSet.Shots_Main, this.MuzzlePos.position);
          this.m_pool_tail.PlayClipPitchOverride(SM.GetTailSet(this.TailClass, GM.CurrentPlayerBody.GetCurrentSoundEnvironment()), position, this.AudioClipSet.TailPitchMod_Main);
        }
      }
      this.ConsumeRound();
      if (this.IsHeld && (Object) this.AltGrip == (Object) null)
      {
        ++this.DestabilizedShots;
        if (this.DestabilizedShots > 5)
        {
          if ((Object) this.m_hand != (Object) null)
          {
            this.m_hand.EndInteractionIfHeld((FVRInteractiveObject) this);
            this.ForceBreakInteraction();
          }
          this.RootRigidbody.AddForceAtPosition((-this.transform.forward + this.transform.up + Random.onUnitSphere * 0.25f) * 30f, this.MuzzlePos.position, ForceMode.Impulse);
          this.RootRigidbody.AddRelativeTorque(Vector3.right * 20f, ForceMode.Impulse);
          this.DestabilizedShots = 0;
        }
        this.m_curState = Minigun.MotorState.SpinningDown;
      }
      else
        this.DestabilizedShots = 0;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.m_isTriggerEngaged = false;
      if (this.IsAltHeld || !this.m_hand.Input.TriggerPressed || !this.m_hasTriggeredUpSinceBegin)
        return;
      this.m_isTriggerEngaged = true;
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (this.m_curState == Minigun.MotorState.SpinningUp)
        this.RootRigidbody.angularVelocity += this.transform.forward * 2f * this.m_motorRate;
      if (this.m_curState != Minigun.MotorState.Firing)
        return;
      this.RootRigidbody.velocity += Random.onUnitSphere * 0.4f;
      this.RootRigidbody.angularVelocity += Random.onUnitSphere * 1.4f;
    }

    public enum MotorState
    {
      Off,
      SpinningUp,
      Firing,
      SpinningDown,
      Idling,
    }
  }
}
