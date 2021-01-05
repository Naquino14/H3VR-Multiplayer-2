// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockFlashPan
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FlintlockFlashPan : MonoBehaviour, IFVRDamageable
  {
    private FlintlockWeapon m_weapon;
    [Header("Frizen")]
    public Transform Frizen;
    public Vector2 FrizenRots = new Vector2(0.0f, 45f);
    public FlintlockFlashPan.FState FrizenState = FlintlockFlashPan.FState.Up;
    [Header("VFX")]
    public GameObject GrainPrefab;
    public Transform GrainSpawnPoint;
    public List<Renderer> GrainPileGeo;
    public ParticleSystem Flash_Fire;
    public ParticleSystem Flash_Smoke;
    public ParticleSystem Shot_Fire;
    public ParticleSystem Shot_Smoke;
    [Header("Audio")]
    public AudioEvent AudEvent_FrizenUp;
    public AudioEvent AudEvent_FrizenDown;
    public AudioEvent AudEvent_FlashpanIgnite;
    public AudioEvent AudEvent_PowderLandOnFlashpan;
    [Header("Barrels")]
    public List<FlintlockBarrel> Barrels;
    private float numGrainsPowderOn;
    private bool m_isIgnited;
    private float timeSinceSpawn;

    public FlintlockWeapon GetWeapon() => this.m_weapon;

    public void SetWeapon(FlintlockWeapon w)
    {
      this.m_weapon = w;
      for (int index = 0; index < this.Barrels.Count; ++index)
      {
        this.Barrels[index].SetWeapon(w);
        this.Barrels[index].SetPan(this);
      }
    }

    public void FlashBlast(int x, int y)
    {
      this.Shot_Fire.Emit(x);
      this.Shot_Smoke.Emit(y);
    }

    private void Update()
    {
      if (this.m_isIgnited)
      {
        this.numGrainsPowderOn -= Time.deltaTime * 20f;
        this.numGrainsPowderOn = Mathf.Clamp(this.numGrainsPowderOn, 0.0f, this.numGrainsPowderOn);
        this.SetGrainPileGeo(Mathf.CeilToInt(this.numGrainsPowderOn));
        this.Flash_Fire.Emit(1);
        this.Flash_Smoke.Emit(2);
        if ((double) this.numGrainsPowderOn <= 0.0)
        {
          this.SetGrainPileGeo(-1);
          this.m_isIgnited = false;
          this.FireBarrels();
        }
      }
      if ((double) this.timeSinceSpawn < 1.0)
        this.timeSinceSpawn += Time.deltaTime;
      if (this.FrizenState == FlintlockFlashPan.FState.Up && (double) Vector3.Angle(this.m_weapon.transform.up, Vector3.up) > 85.0 && ((double) this.timeSinceSpawn > 0.0399999991059303 && (double) this.numGrainsPowderOn > 0.0))
      {
        --this.numGrainsPowderOn;
        this.numGrainsPowderOn = Mathf.Clamp(this.numGrainsPowderOn, 0.0f, this.numGrainsPowderOn);
        if ((double) this.numGrainsPowderOn <= 0.0)
          this.SetGrainPileGeo(-1);
        else
          this.SetGrainPileGeo(Mathf.CeilToInt(this.numGrainsPowderOn));
        this.timeSinceSpawn = 0.0f;
        Object.Instantiate<GameObject>(this.GrainPrefab, this.GrainSpawnPoint.position, Random.rotation);
      }
      if (!this.m_weapon.IsHeld)
        ;
    }

    private void FireBarrels()
    {
      for (int index = 0; index < this.Barrels.Count; ++index)
        this.Barrels[index].Ignite();
    }

    public void HammerHit(FlintlockWeapon.FlintState f, bool Flint)
    {
      if (this.FrizenState == FlintlockFlashPan.FState.Down && Flint)
        this.Ignite();
      this.SetFrizenUp();
    }

    public void Ignite()
    {
      if ((double) this.numGrainsPowderOn <= 0.0)
        return;
      this.m_isIgnited = true;
      this.m_weapon.PlayAudioAsHandling(this.AudEvent_FlashpanIgnite, this.Frizen.position);
    }

    private void SetGrainPileGeo(int g)
    {
      for (int index = 0; index < this.GrainPileGeo.Count; ++index)
        this.GrainPileGeo[index].enabled = index == g;
    }

    public void ToggleFrizenState()
    {
      if (this.FrizenState == FlintlockFlashPan.FState.Down)
        this.SetFrizenUp();
      else
        this.SetFrizenDown();
    }

    private void SetFrizenUp()
    {
      if (this.FrizenState == FlintlockFlashPan.FState.Up)
        return;
      this.FrizenState = FlintlockFlashPan.FState.Up;
      this.m_weapon.SetAnimatedComponent(this.Frizen, this.FrizenRots.y, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
      this.m_weapon.PlayAudioAsHandling(this.AudEvent_FrizenUp, this.Frizen.position);
    }

    private void SetFrizenDown()
    {
      if (this.FrizenState == FlintlockFlashPan.FState.Down || this.m_weapon.HammerState == FlintlockWeapon.HState.Uncocked)
        return;
      this.FrizenState = FlintlockFlashPan.FState.Down;
      this.m_weapon.SetAnimatedComponent(this.Frizen, this.FrizenRots.x, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
      this.m_weapon.PlayAudioAsHandling(this.AudEvent_FrizenDown, this.Frizen.position);
    }

    private bool IsPanFull() => (double) this.numGrainsPowderOn > 4.0;

    public float GetPanContents() => this.numGrainsPowderOn;

    public void ClearPan()
    {
      this.numGrainsPowderOn = 0.0f;
      this.SetGrainPileGeo(-1);
    }

    private void AddGrain()
    {
      ++this.numGrainsPowderOn;
      this.SetGrainPileGeo(Mathf.CeilToInt(this.numGrainsPowderOn));
      this.m_weapon.PlayAudioAsHandling(this.AudEvent_PowderLandOnFlashpan, this.Frizen.position);
    }

    public void OnTriggerEnter(Collider other)
    {
      if (this.FrizenState == FlintlockFlashPan.FState.Down || this.IsPanFull() || ((Object) other.attachedRigidbody == (Object) null || (double) Vector3.Angle(this.transform.up, Vector3.up) > 70.0) || !other.attachedRigidbody.gameObject.CompareTag("flintlock_powdergrain"))
        return;
      this.AddGrain();
      Object.Destroy((Object) other.gameObject);
    }

    public void Damage(FistVR.Damage d)
    {
      if (this.FrizenState != FlintlockFlashPan.FState.Up || (double) d.Dam_Thermal <= 0.0 && d.Class != FistVR.Damage.DamageClass.Projectile)
        return;
      this.Ignite();
    }

    public enum FState
    {
      Down,
      Up,
    }
  }
}
