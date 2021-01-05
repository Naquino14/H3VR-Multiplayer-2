// Decompiled with JetBrains decompiler
// Type: FistVR.BallisticTestingAimer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BallisticTestingAimer : FVRPhysicalObject
  {
    [Header("Entity Spawner Panel Params")]
    public GameObject Indicator_Locked;
    public GameObject Indicator_Unlocked;
    public AudioEvent[] AudClips_DeviceBeeps;
    [Header("Laser")]
    public Transform SpawnLaserPoint;
    private RaycastHit m_hit;
    public LayerMask LM_SpawnRay;
    public Transform SpawnLaserCylinder;
    private bool LaserOn;
    private int m_projShape;
    private int m_projData;
    private int m_numProjectiles = 1;
    private float m_spread;
    public GameObject[] ProjectilePrefabs;
    public Transform Muzzle;
    [Header("NewAudioImplementation")]
    public FVRFirearmAudioSet AudioClipSet;
    protected SM.AudioSourcePool m_pool_shot;
    protected SM.AudioSourcePool m_pool_tail;
    protected SM.AudioSourcePool m_pool_mechanics;
    protected SM.AudioSourcePool m_pool_handling;
    private float m_refireTick = 0.1f;
    public FVRTailSoundClass TailClass;

    protected override void Awake()
    {
      base.Awake();
      this.m_pool_shot = SM.CreatePool(3, 3, FVRPooledAudioType.GunShot);
      if ((Object) this.AudioClipSet == (Object) null)
        Debug.Log((object) this.gameObject.name);
      this.m_pool_tail = SM.CreatePool(this.AudioClipSet.TailConcurrentLimit, this.AudioClipSet.TailConcurrentLimit, FVRPooledAudioType.GunTail);
      this.m_pool_handling = SM.CreatePool(3, 3, FVRPooledAudioType.GunHand);
      this.m_pool_mechanics = SM.CreatePool(3, 3, FVRPooledAudioType.GunMech);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.m_hand.Input.TouchpadDown && (double) this.m_hand.Input.TouchpadAxes.magnitude > 0.200000002980232)
      {
        if ((double) Vector2.Angle(this.m_hand.Input.TouchpadAxes, Vector2.down) < 45.0)
        {
          this.ToggleKinematicLocked();
          if (this.IsKinematicLocked)
            SM.PlayGenericSound(this.AudClips_DeviceBeeps[0], this.transform.position);
          else
            SM.PlayGenericSound(this.AudClips_DeviceBeeps[1], this.transform.position);
          this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
        }
        else if ((double) Vector2.Angle(this.m_hand.Input.TouchpadAxes, Vector2.right) < 45.0)
        {
          this.ToggleLaser();
          this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
        }
        else if ((double) Vector2.Angle(this.m_hand.Input.TouchpadAxes, Vector2.left) < 45.0)
        {
          this.ToggleLaser();
          this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
        }
      }
      if (!this.m_hand.Input.TriggerPressed || !this.m_hasTriggeredUpSinceBegin)
        return;
      this.m_refireTick -= Time.deltaTime;
      if ((double) this.m_refireTick > 0.0)
        return;
      this.m_refireTick = 0.2f;
      this.Fire();
    }

    public void SetRoundShape(int i) => this.m_projShape = i;

    public void SetRoundData(int i) => this.m_projData = i;

    public void SetNumProjectiles(int i) => this.m_numProjectiles = i;

    public void SetSpread(float f) => this.m_spread = f;

    public void Fire()
    {
      this.PlayAudioGunShot();
      for (int index = 0; index < this.m_numProjectiles; ++index)
      {
        GameObject projectilePrefab = this.ProjectilePrefabs[this.m_projData];
        if ((Object) projectilePrefab != (Object) null)
        {
          GameObject gameObject = Object.Instantiate<GameObject>(projectilePrefab, this.Muzzle.position, this.Muzzle.rotation);
          gameObject.transform.Rotate(new Vector3(Random.Range(-this.m_spread, this.m_spread), Random.Range(-this.m_spread, this.m_spread), 0.0f));
          BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
          component.ProjType = (BallisticProjectileType) this.m_projShape;
          component.Fire(component.MuzzleVelocityBase, gameObject.transform.forward, (FVRFireArm) null);
        }
      }
    }

    public void PlayAudioGunShot()
    {
      Vector3 position = this.transform.position;
      this.PlayAudioEvent(FirearmAudioEventType.Shots_Main);
      if (!this.AudioClipSet.UsesTail_Main)
        return;
      AudioEvent tailSet = SM.GetTailSet(this.TailClass, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      this.m_pool_tail.PlayClipPitchOverride(tailSet, position, this.AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.BoundsLaser();
      if (this.IsKinematicLocked)
      {
        if (this.Indicator_Unlocked.activeSelf)
          this.Indicator_Unlocked.SetActive(false);
        if (this.Indicator_Locked.activeSelf)
          return;
        this.Indicator_Locked.SetActive(true);
      }
      else
      {
        if (!this.Indicator_Unlocked.activeSelf)
          this.Indicator_Unlocked.SetActive(true);
        if (!this.Indicator_Locked.activeSelf)
          return;
        this.Indicator_Locked.SetActive(false);
      }
    }

    private void ToggleLaser()
    {
      if (this.LaserOn)
      {
        this.LaserOn = false;
        SM.PlayGenericSound(this.AudClips_DeviceBeeps[0], this.transform.position);
      }
      else
      {
        this.LaserOn = true;
        SM.PlayGenericSound(this.AudClips_DeviceBeeps[1], this.transform.position);
      }
    }

    private void BoundsLaser()
    {
      if (this.LaserOn)
      {
        if (!this.SpawnLaserCylinder.gameObject.activeSelf)
          this.SpawnLaserCylinder.gameObject.SetActive(true);
        if (Physics.Raycast(this.SpawnLaserPoint.position, this.SpawnLaserPoint.forward, out this.m_hit, 10f, (int) this.LM_SpawnRay, QueryTriggerInteraction.Ignore))
          this.SpawnLaserCylinder.localScale = new Vector3(0.005f, 0.005f, this.m_hit.distance);
        else
          this.SpawnLaserCylinder.localScale = new Vector3(0.005f, 0.005f, 0.005f);
      }
      else
      {
        if (!this.SpawnLaserCylinder.gameObject.activeSelf)
          return;
        this.SpawnLaserCylinder.gameObject.SetActive(false);
      }
    }

    public void PlayAudioEvent(FirearmAudioEventType eType)
    {
      Vector3 position = this.transform.position;
      switch (eType)
      {
        case FirearmAudioEventType.BoltSlideForward:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.BoltSlideForward, position);
          break;
        case FirearmAudioEventType.BoltSlideBack:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.BoltSlideBack, position);
          break;
        case FirearmAudioEventType.BoltSlideForwardHeld:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.BoltSlideForwardHeld, position);
          break;
        case FirearmAudioEventType.BoltSlideBackHeld:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.BoltSlideBackHeld, position);
          break;
        case FirearmAudioEventType.BoltSlideBackLocked:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.BoltSlideBackLocked, position);
          break;
        case FirearmAudioEventType.CatchOnSear:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.CatchOnSear, position);
          break;
        case FirearmAudioEventType.HammerHit:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.HammerHit, position);
          break;
        case FirearmAudioEventType.Prefire:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.Prefire, position);
          break;
        case FirearmAudioEventType.BoltRelease:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.BoltRelease, position);
          break;
        case FirearmAudioEventType.HandleGrab:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleGrab, position);
          break;
        case FirearmAudioEventType.HandleBack:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleBack, position);
          break;
        case FirearmAudioEventType.HandleForward:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleForward, position);
          break;
        case FirearmAudioEventType.HandleUp:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleUp, position);
          break;
        case FirearmAudioEventType.HandleDown:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleDown, position);
          break;
        case FirearmAudioEventType.Safety:
          this.m_pool_handling.PlayClip(this.AudioClipSet.Safety, position);
          break;
        case FirearmAudioEventType.FireSelector:
          this.m_pool_handling.PlayClip(this.AudioClipSet.FireSelector, position);
          break;
        case FirearmAudioEventType.TriggerReset:
          this.m_pool_handling.PlayClip(this.AudioClipSet.TriggerReset, position);
          break;
        case FirearmAudioEventType.BreachOpen:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BreachOpen, position);
          break;
        case FirearmAudioEventType.BreachClose:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BreachClose, position);
          break;
        case FirearmAudioEventType.MagazineIn:
          this.m_pool_handling.PlayClip(this.AudioClipSet.MagazineIn, position);
          break;
        case FirearmAudioEventType.MagazineOut:
          this.m_pool_handling.PlayClip(this.AudioClipSet.MagazineOut, position);
          break;
        case FirearmAudioEventType.TopCoverRelease:
          this.m_pool_handling.PlayClip(this.AudioClipSet.TopCoverRelease, position);
          break;
        case FirearmAudioEventType.TopCoverUp:
          this.m_pool_handling.PlayClip(this.AudioClipSet.TopCoverUp, position);
          break;
        case FirearmAudioEventType.TopCoverDown:
          this.m_pool_handling.PlayClip(this.AudioClipSet.TopCoverDown, position);
          break;
        case FirearmAudioEventType.MagazineInsertRound:
          this.m_pool_handling.PlayClip(this.AudioClipSet.MagazineInsertRound, position);
          break;
        case FirearmAudioEventType.MagazineEjectRound:
          this.m_pool_handling.PlayClip(this.AudioClipSet.MagazineEjectRound, position);
          break;
        case FirearmAudioEventType.StockOpen:
          this.m_pool_handling.PlayClip(this.AudioClipSet.StockOpen, position);
          break;
        case FirearmAudioEventType.StockClosed:
          this.m_pool_handling.PlayClip(this.AudioClipSet.StockClosed, position);
          break;
        case FirearmAudioEventType.BipodOpen:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BipodOpen, position);
          break;
        case FirearmAudioEventType.BipodClosed:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BipodClosed, position);
          break;
        case FirearmAudioEventType.HandleForwardEmpty:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleForwardEmpty, position);
          break;
        case FirearmAudioEventType.HandleBackEmpty:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleBackEmpty, position);
          break;
        case FirearmAudioEventType.ChamberManual:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.ChamberManual, position);
          break;
        case FirearmAudioEventType.BeltGrab:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BeltGrab, position);
          break;
        case FirearmAudioEventType.BeltRelease:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BeltRelease, position);
          break;
        case FirearmAudioEventType.BeltSeat:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BeltSeat, position);
          break;
        default:
          switch (eType - 100)
          {
            case FirearmAudioEventType.BoltSlideForward:
              this.m_pool_shot.PlayClip(this.AudioClipSet.Shots_Main, position);
              return;
            case FirearmAudioEventType.BoltSlideBack:
              this.m_pool_shot.PlayClip(this.AudioClipSet.Shots_Suppressed, position);
              return;
            case FirearmAudioEventType.BoltSlideForwardHeld:
              this.m_pool_shot.PlayClip(this.AudioClipSet.Shots_LowPressure, position);
              return;
            default:
              return;
          }
      }
    }
  }
}
