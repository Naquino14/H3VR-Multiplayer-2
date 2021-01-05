// Decompiled with JetBrains decompiler
// Type: FistVR.IPSCTarget
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class IPSCTarget : MonoBehaviour, IFVRDamageable
  {
    public Texture2D MaskTexture;
    private bool HasBeenShot;
    public GameObject[] HitZones;
    public BreachingTargetManager Manager;
    public Transform XYGridOrigin;
    public bool IsAutoResetting;
    private float m_resetTick;
    public float RefireRate = 0.25f;
    public AudioEvent HitSound;

    private void Update()
    {
      if (!this.IsAutoResetting)
        return;
      if ((double) this.m_resetTick > 0.0)
      {
        this.m_resetTick -= Time.deltaTime;
      }
      else
      {
        if (!this.HasBeenShot)
          return;
        this.ResetState();
      }
    }

    public void Damage(FistVR.Damage dam)
    {
      if (dam.Class != FistVR.Damage.DamageClass.Projectile || this.HasBeenShot)
        return;
      Vector3 vector3 = this.XYGridOrigin.InverseTransformPoint(dam.point);
      vector3.z = 0.0f;
      vector3.x = Mathf.Clamp(vector3.x, 0.0f, 1f);
      vector3.y = Mathf.Clamp(vector3.y, 0.0f, 1f);
      Color pixel = this.MaskTexture.GetPixel(Mathf.RoundToInt((float) this.MaskTexture.width * vector3.x), Mathf.RoundToInt((float) this.MaskTexture.width * vector3.y));
      if ((double) pixel.r > 0.5 && (double) pixel.g < 0.5)
      {
        this.HasBeenShot = true;
        this.RegisterHit(0);
        this.m_resetTick = this.RefireRate;
      }
      else if ((double) pixel.r > 0.5 && (double) pixel.g > 0.5)
      {
        this.HasBeenShot = true;
        this.RegisterHit(1);
        this.m_resetTick = this.RefireRate;
      }
      else if ((double) pixel.g > 0.5 && (double) pixel.r < 0.5)
      {
        this.HasBeenShot = true;
        this.RegisterHit(2);
        this.m_resetTick = this.RefireRate;
      }
      else
      {
        if ((double) pixel.b <= 0.5)
          return;
        this.HasBeenShot = true;
        this.RegisterHit(3);
        this.m_resetTick = this.RefireRate;
      }
    }

    private void ResetState()
    {
      this.HasBeenShot = false;
      Debug.Log((object) "resetting");
      for (int index = 0; index < this.HitZones.Length; ++index)
      {
        if ((Object) this.HitZones[index] != (Object) null && this.HitZones[index].activeSelf)
          this.HitZones[index].SetActive(false);
      }
    }

    private void RegisterHit(int i)
    {
      this.HitZones[i].SetActive(true);
      this.Invoke("PlaySound", 0.15f);
      if (!((Object) this.Manager != (Object) null))
        return;
      switch (i)
      {
        case 0:
          this.Manager.RegisterScore(5);
          break;
        case 1:
          this.Manager.RegisterScore(4);
          break;
        case 2:
          this.Manager.RegisterScore(3);
          break;
        case 3:
          this.Manager.RegisterScore(1);
          break;
      }
    }

    private void PlaySound()
    {
      float num = Mathf.Lerp(0.4f, 0.2f, Vector3.Distance(GM.CurrentPlayerRoot.position, this.transform.position) / 100f);
      this.HitSound.VolumeRange = new Vector2(num, num);
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.HitSound, this.transform.position);
    }
  }
}
