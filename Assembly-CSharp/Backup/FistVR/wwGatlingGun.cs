// Decompiled with JetBrains decompiler
// Type: FistVR.wwGatlingGun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class wwGatlingGun : MonoBehaviour
  {
    public Transform GunBarrels;
    public Transform MuzzlePos;
    private float m_curRot;
    private float m_rotTilShot = 36f;
    public int AmmoType;
    public FVRFirearmAudioSet AudioClipSet;
    public FVRTailSoundClass TailClass = FVRTailSoundClass.FullPower;
    protected SM.AudioSourcePool m_pool_shot;
    protected SM.AudioSourcePool m_pool_tail;
    protected SM.AudioSourcePool m_pool_mechanics;
    public wwGatlingGun.MuzzleFireType[] MuzzleFX;

    public void Awake()
    {
      this.m_pool_shot = SM.CreatePool(3, 3, FVRPooledAudioType.GunShot);
      this.m_pool_tail = SM.CreatePool(2, 2, FVRPooledAudioType.GunTail);
      this.m_pool_mechanics = SM.CreatePool(3, 3, FVRPooledAudioType.GunMech);
    }

    public void CrankGun(float crank)
    {
      float num = Mathf.Clamp(crank * 0.4f, 0.0f, 2f);
      this.m_curRot += num;
      if ((double) this.m_curRot > 180.0)
        this.m_curRot -= 360f;
      this.GunBarrels.transform.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_curRot);
      this.m_rotTilShot -= num;
      if ((double) this.m_rotTilShot > 0.0)
        return;
      this.FireShot();
      this.m_rotTilShot = 36f;
    }

    private void FireShot()
    {
      wwGatlingGun.MuzzleFireType muzzleFireType = this.MuzzleFX[this.AmmoType];
      for (int index = 0; index < muzzleFireType.MuzzleFires.Length; ++index)
        muzzleFireType.MuzzleFires[index].Emit(muzzleFireType.MuzzleFireAmounts[index]);
      this.m_pool_shot.PlayClip(this.AudioClipSet.Shots_Main, this.MuzzlePos.position);
      this.m_pool_mechanics.PlayClip(this.AudioClipSet.HammerHit, this.MuzzlePos.position);
      this.m_pool_tail.PlayClipPitchOverride(SM.GetTailSet(this.TailClass, GM.CurrentPlayerBody.GetCurrentSoundEnvironment()), this.MuzzlePos.position, this.AudioClipSet.TailPitchMod_Main);
      UnityEngine.Object.Instantiate<GameObject>(muzzleFireType.ProjectilePrefab, this.MuzzlePos.position, this.MuzzlePos.rotation).GetComponent<BallisticProjectile>().Fire(this.MuzzlePos.forward, (FVRFireArm) null);
    }

    public void Update()
    {
    }

    [Serializable]
    public class MuzzleFireType
    {
      public Vector2 ShotVolume = new Vector2(0.9f, 1f);
      public ParticleSystem[] MuzzleFires;
      public int[] MuzzleFireAmounts;
      public GameObject ProjectilePrefab;
    }
  }
}
