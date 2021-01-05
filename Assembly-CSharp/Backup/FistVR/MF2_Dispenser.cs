// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_Dispenser
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MF2_Dispenser : MonoBehaviour, IFVRDamageable
  {
    public AIEntity E;
    public List<MF2_DispenserButton> Buttons_Front;
    public List<MF2_DispenserButton> Buttons_Rear;
    public List<Transform> EnergyArrows;
    public Transform Bay_Front;
    public Transform Bay_Rear;
    public Transform BayPoint_Front;
    public Transform BayPoint_Rear;
    public Transform BaySpawnPoint_Front;
    public Transform BaySpawnPoint_Rear;
    private float m_bayRotFront;
    private float m_bayRotRear;
    public FVRObject TurretTippyToy;
    public GameObject TurretMagazine;
    private bool m_isHealingActive;
    private bool m_isReloadingActive;
    private bool m_isDamageBuffActive;
    private float m_energy;
    private float m_tickTilVendCheck = 1f;
    public Transform SpinnyBit;
    private float m_spinnerRot;
    public List<Transform> ShotgunPoints;
    public FVRObject ShotgunToSpawn;
    [Header("Audio Functionality")]
    public AudioEvent AudEvent_ButtonPress_Engage;
    public AudioEvent AudEvent_ButtonPress_Reset;
    public AudioEvent AudEvent_GenerateMetal;
    public AudioEvent AudEvent_ReloadPulse;
    public AudioEvent AudEvent_HealingEngage;
    public AudioEvent AudEvent_HealingPulse;
    public AudioEvent AudEvent_Fail;
    [Header("Healing Functionality")]
    public LayerMask HealDetect;
    public float HealRange = 10f;
    private float m_HealTick = 1f;
    [Header("FX")]
    public ParticleSystem PSystem_Health;
    public ParticleSystem PSystem_Ammo;
    [Header("Damage")]
    public float DamageRemaining = 10000f;
    public Transform SpawnOnDeathPoint;
    public List<GameObject> SpawnOnDestroy;
    public List<Transform> ShardPoints;
    public List<GameObject> Shards;
    private bool m_hasInvokedDestroy;
    private bool m_isDestroyed;
    private float hitRefire = 1f;

    private void Start()
    {
      this.ResetOn(false);
      this.m_HealTick = Random.Range(0.5f, 1f);
      if (this.E.IFFCode != 1 || GM.CurrentPlayerBody.GetPlayerIFF() != 0 && GM.CurrentPlayerBody.GetPlayerIFF() != -3)
        return;
      this.E.IFFCode = GM.CurrentPlayerBody.GetPlayerIFF();
    }

    public void Damage(FistVR.Damage d)
    {
      if (this.m_hasInvokedDestroy || this.m_isDestroyed || d.Source_IFF == this.E.IFFCode)
        return;
      this.DamageRemaining -= d.Dam_TotalKinetic;
      if ((double) this.DamageRemaining >= 0.0)
        return;
      this.m_hasInvokedDestroy = true;
      this.Invoke("DestroyMe", Random.Range(0.1f, 0.2f));
    }

    private void DestroyMe()
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      for (int index = 0; index < this.Shards.Count; ++index)
        Object.Instantiate<GameObject>(this.Shards[index], this.ShardPoints[index].position, this.ShardPoints[index].rotation);
      for (int index = 0; index < this.SpawnOnDestroy.Count; ++index)
        Object.Instantiate<GameObject>(this.SpawnOnDestroy[index], this.SpawnOnDeathPoint.position, this.SpawnOnDeathPoint.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    public void ButtonPressed(
      MF2_DispenserButton.DispenserButtonType t,
      MF2_DispenserButton.DispenserSide s)
    {
      Transform transform = this.BaySpawnPoint_Front;
      if (s == MF2_DispenserButton.DispenserSide.Rear)
        transform = this.BaySpawnPoint_Rear;
      switch (t)
      {
        case MF2_DispenserButton.DispenserButtonType.Heal:
          if (this.m_isHealingActive)
            break;
          this.HealOn();
          break;
        case MF2_DispenserButton.DispenserButtonType.Reload:
          if (this.m_isReloadingActive)
            break;
          this.ReloadOn();
          break;
        case MF2_DispenserButton.DispenserButtonType.Reset:
          if (!this.m_isHealingActive && !this.m_isReloadingActive)
            break;
          this.ResetOn(true);
          break;
        case MF2_DispenserButton.DispenserButtonType.Turret:
          if ((double) this.m_energy >= 90.0)
          {
            this.m_energy -= 90f;
            Object.Instantiate<GameObject>(this.TurretTippyToy.GetGameObject(), transform.position, transform.rotation);
            SM.PlayGenericSound(this.AudEvent_ReloadPulse, this.transform.position);
            break;
          }
          SM.PlayGenericSound(this.AudEvent_Fail, this.transform.position);
          break;
        case MF2_DispenserButton.DispenserButtonType.Magazine:
          if ((double) this.m_energy >= 20.0)
          {
            this.m_energy -= 20f;
            Object.Instantiate<GameObject>(this.TurretMagazine, transform.position, transform.rotation);
            SM.PlayGenericSound(this.AudEvent_ReloadPulse, this.transform.position);
            break;
          }
          SM.PlayGenericSound(this.AudEvent_Fail, this.transform.position);
          break;
      }
    }

    private void Update()
    {
      float num1 = 0.0f;
      if (this.m_isHealingActive)
        ++num1;
      if (this.m_isReloadingActive)
        num1 += 2f;
      if (this.m_isHealingActive && this.m_isReloadingActive)
        num1 += 3f;
      this.m_energy -= num1 * Time.deltaTime;
      if ((double) this.m_energy <= 0.0 && (this.m_isHealingActive || this.m_isReloadingActive))
        this.ResetOn(true);
      float num2 = 0.0f;
      if (this.m_isHealingActive)
        num2 += 360f;
      if (this.m_isReloadingActive)
        num2 += 720f;
      if (this.m_isHealingActive && this.m_isReloadingActive)
        num2 += 720f;
      this.m_spinnerRot += Time.deltaTime * num2;
      this.m_spinnerRot = Mathf.Repeat(this.m_spinnerRot, 360f);
      this.SpinnyBit.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_spinnerRot);
      this.m_HealTick -= Time.deltaTime;
      if ((double) this.m_HealTick <= 0.0)
      {
        this.m_HealTick = 1f;
        if (this.m_isHealingActive || this.m_isReloadingActive)
          this.HealPulse();
      }
      if (this.E.IFFCode == GM.CurrentPlayerBody.GetPlayerIFF())
      {
        float max1 = Mathf.Min(Vector3.Distance(this.BayPoint_Front.position, GM.CurrentPlayerBody.LeftHand.position), Vector3.Distance(this.BayPoint_Front.position, GM.CurrentPlayerBody.RightHand.position));
        float max2 = Mathf.Min(Vector3.Distance(this.BayPoint_Rear.position, GM.CurrentPlayerBody.LeftHand.position), Vector3.Distance(this.BayPoint_Rear.position, GM.CurrentPlayerBody.RightHand.position));
        float num3 = 0.0f;
        float x = 0.0f;
        if ((double) max1 <= 0.200000002980232)
          num3 = Mathf.Lerp(90f, 0.0f, Mathf.Clamp(max1 - 0.15f, 0.0f, max1) * 20f);
        if ((double) max2 <= 0.200000002980232)
          x = Mathf.Lerp(-90f, 0.0f, Mathf.Clamp(max2 - 0.15f, 0.0f, max2) * 20f);
        if ((double) this.m_bayRotFront != (double) num3)
        {
          this.m_bayRotFront = num3;
          this.Bay_Front.localEulerAngles = new Vector3(this.m_bayRotFront, 0.0f, 0.0f);
        }
        if ((double) this.m_bayRotRear != (double) x)
        {
          this.m_bayRotRear = x;
          this.Bay_Rear.localEulerAngles = new Vector3(x, 0.0f, 0.0f);
        }
      }
      this.m_energy = Mathf.Clamp(this.m_energy, 0.0f, 100f);
      float z = Mathf.Lerp(-90f, 90f, this.m_energy * 0.01f);
      for (int index = 0; index < this.EnergyArrows.Count; ++index)
        this.EnergyArrows[index].localEulerAngles = new Vector3(0.0f, 0.0f, z);
      if ((double) this.hitRefire <= 0.0)
        return;
      this.hitRefire -= Time.deltaTime;
    }

    public void HitCharge()
    {
      if ((double) this.hitRefire > 0.0)
        return;
      this.m_energy += 8f;
      this.m_energy = Mathf.Clamp(this.m_energy, 0.0f, 100f);
      this.hitRefire = 0.5f;
    }

    private void HealPulse()
    {
      bool flag1 = false;
      bool flag2 = false;
      Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.HealRange, (int) this.HealDetect, QueryTriggerInteraction.Collide);
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
        {
          SosigLink component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<SosigLink>();
          if ((Object) component != (Object) null && component.S.E.IFFCode == this.E.IFFCode)
          {
            if (this.m_isHealingActive)
            {
              component.S.BuffHealing_Engage(1f, 20f);
              flag1 = true;
            }
            if (this.m_isReloadingActive)
            {
              component.S.ActivatePower(PowerupType.InfiniteAmmo, PowerUpIntensity.High, PowerUpDuration.Blip, false, false);
              flag2 = true;
            }
          }
        }
      }
      if (this.E.IFFCode == GM.CurrentPlayerBody.GetPlayerIFF() && (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) <= (double) this.HealRange)
      {
        if (this.m_isHealingActive)
        {
          flag1 = true;
          GM.CurrentPlayerBody.ActivatePower(PowerupType.Regen, PowerUpIntensity.Low, PowerUpDuration.Blip, false, false);
        }
        if (this.m_isReloadingActive)
        {
          flag2 = true;
          GM.CurrentPlayerBody.ActivatePower(PowerupType.InfiniteAmmo, PowerUpIntensity.Low, PowerUpDuration.Blip, false, false);
        }
      }
      if (flag1)
      {
        SM.PlayGenericSound(this.AudEvent_HealingPulse, this.transform.position);
        this.PSystem_Health.Emit(10);
      }
      if (!flag2)
        return;
      SM.PlayGenericSound(this.AudEvent_ReloadPulse, this.transform.position);
      this.PSystem_Ammo.Emit(10);
    }

    private void HealOn()
    {
      if ((double) this.m_energy <= 0.0)
        return;
      this.m_isHealingActive = true;
      SM.PlayGenericSound(this.AudEvent_HealingEngage, this.transform.position);
      this.Buttons_Front[0].transform.localPosition = new Vector3(0.0f, 0.0f, -0.035f);
      this.Buttons_Rear[0].transform.localPosition = new Vector3(0.0f, 0.0f, 0.035f);
      this.Buttons_Front[1].transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
      this.Buttons_Rear[1].transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
      SM.PlayGenericSound(this.AudEvent_ButtonPress_Reset, this.transform.position);
    }

    private void ReloadOn()
    {
      if ((double) this.m_energy <= 0.0)
        return;
      this.m_isReloadingActive = true;
      SM.PlayGenericSound(this.AudEvent_GenerateMetal, this.transform.position);
      this.Buttons_Front[2].transform.localPosition = new Vector3(0.0f, 0.0f, -0.035f);
      this.Buttons_Rear[2].transform.localPosition = new Vector3(0.0f, 0.0f, 0.035f);
      this.Buttons_Front[1].transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
      this.Buttons_Rear[1].transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
      SM.PlayGenericSound(this.AudEvent_ButtonPress_Engage, this.transform.position);
    }

    private void ResetOn(bool makeSound)
    {
      if (makeSound)
        SM.PlayGenericSound(this.AudEvent_ButtonPress_Reset, this.transform.position);
      this.m_isHealingActive = false;
      this.m_isReloadingActive = false;
      this.Buttons_Front[0].transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
      this.Buttons_Rear[0].transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
      this.Buttons_Front[1].transform.localPosition = new Vector3(0.0f, 0.0f, -0.035f);
      this.Buttons_Rear[1].transform.localPosition = new Vector3(0.0f, 0.0f, 0.035f);
      this.Buttons_Front[2].transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
      this.Buttons_Rear[2].transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }
  }
}
