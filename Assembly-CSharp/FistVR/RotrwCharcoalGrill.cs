// Decompiled with JetBrains decompiler
// Type: FistVR.RotrwCharcoalGrill
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RotrwCharcoalGrill : MonoBehaviour
  {
    public RotrwCharcoalGrillHandle Lid;
    public RotrwCharcoalGrillGrate Grate;
    public Transform GrillGrateSpot;
    private float m_timeSincePickup = 1f;
    public AudioSource AudSource_Fire;
    private float m_currentVolume;
    private float m_targetVolume;
    private int m_numCharcoalActive;
    public LayerMask LM_Search;
    public Transform CheckPoint;
    public Transform CheckPointSausages;
    public float CheckRadius = 0.2f;
    private float m_charcoalCheckTick = 1f;
    public List<FVRObject> GrilledPowerups;
    public AudioEvent AudEvent_Success;
    public List<GameObject> Explosions;

    public bool CanPickupGrate() => (double) this.m_timeSincePickup >= 4.0;

    public void DemountGrate() => this.m_timeSincePickup = 0.0f;

    public void MountGrate() => this.m_timeSincePickup = 0.0f;

    protected void Update()
    {
      if ((double) this.m_timeSincePickup < 5.0)
        this.m_timeSincePickup += Time.deltaTime;
      this.m_targetVolume = Mathf.Clamp((float) this.m_numCharcoalActive * 0.08f, 0.0f, 0.4f);
      this.m_currentVolume = Mathf.MoveTowards(this.m_currentVolume, this.m_targetVolume, Time.deltaTime * 0.3f);
      if ((double) this.m_currentVolume > 0.0 && !this.AudSource_Fire.isPlaying)
        this.AudSource_Fire.Play();
      else if ((double) this.m_currentVolume <= 0.0 && this.AudSource_Fire.isPlaying)
        this.AudSource_Fire.Stop();
      if ((double) this.m_currentVolume > 0.0)
        this.AudSource_Fire.volume = this.m_currentVolume;
      if ((double) this.m_charcoalCheckTick > 0.0)
      {
        this.m_charcoalCheckTick -= Time.deltaTime;
      }
      else
      {
        this.m_charcoalCheckTick = Random.Range(1f, 2f);
        Collider[] colliderArray = Physics.OverlapSphere(this.CheckPoint.position, this.CheckRadius, (int) this.LM_Search, QueryTriggerInteraction.Collide);
        int num = 0;
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          RotrwCharcoal component = colliderArray[index].gameObject.GetComponent<RotrwCharcoal>();
          if ((Object) component != (Object) null && component.IsOnFire)
            ++num;
        }
        this.m_numCharcoalActive = num;
        if (!this.Lid.IsLidClosed() || this.m_numCharcoalActive <= 0)
          return;
        bool flag = false;
        List<RW_Powerup> rwPowerupList = new List<RW_Powerup>();
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if ((Object) colliderArray[index].attachedRigidbody != (Object) null)
          {
            RW_Powerup component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<RW_Powerup>();
            if ((Object) component != (Object) null && !rwPowerupList.Contains(component) && !component.Cooked)
            {
              rwPowerupList.Add(component);
              flag = true;
            }
          }
        }
        for (int index1 = rwPowerupList.Count - 1; index1 >= 0; --index1)
        {
          RW_Powerup rwPowerup = rwPowerupList[index1];
          int powerupType = (int) rwPowerup.PowerupType;
          PowerUpDuration powerupDuration = rwPowerup.PowerupDuration;
          PowerUpIntensity powerupIntensity = rwPowerup.PowerupIntensity;
          bool isPuke = rwPowerup.isPuke;
          bool isInverted = rwPowerup.isInverted;
          RotrwMeatCore.CoreType mcMadeWith = rwPowerup.GetMCMadeWith();
          Vector3 position = rwPowerupList[index1].transform.position;
          Quaternion rotation = rwPowerupList[index1].transform.rotation;
          Object.Destroy((Object) rwPowerupList[index1].gameObject);
          if (powerupType >= 0)
          {
            RW_Powerup component = Object.Instantiate<GameObject>(this.GrilledPowerups[powerupType].GetGameObject(), position, rotation).GetComponent<RW_Powerup>();
            component.SetParams(component.PowerupType, powerupIntensity, powerupDuration, isPuke, isInverted);
            component.SetMCMadeWith(mcMadeWith);
            this.SetDisplayFlags(component);
          }
          else
          {
            for (int index2 = 0; index2 < this.Explosions.Count; ++index2)
              Object.Instantiate<GameObject>(this.Explosions[index2], position, Random.rotation);
          }
        }
        if (!flag)
          return;
        SM.PlayGenericSound(this.AudEvent_Success, this.GrillGrateSpot.position);
      }
    }

    private void SetDisplayFlags(RW_Powerup p)
    {
      if ((Object) GM.ZMaster == (Object) null)
        return;
      string empty = string.Empty;
      string flag1 = p.isInverted ? "flagRecipeInverted" + ((int) p.PowerupType).ToString() : "flagRecipe" + ((int) p.PowerupType).ToString();
      string flag2 = "flagCoreName" + ((int) p.GetMCMadeWith()).ToString();
      string flag3 = "flagCoreIntensity" + ((int) p.GetMCMadeWith()).ToString();
      string flag4 = "flagCoreDuration" + ((int) p.GetMCMadeWith()).ToString();
      string flag5 = "flagCoreSpecial" + ((int) p.GetMCMadeWith()).ToString();
      GM.ZMaster.FlagM.SetFlag(flag1, 1);
      GM.ZMaster.FlagM.SetFlag(flag2, 1);
      GM.ZMaster.FlagM.SetFlag(flag5, 1);
      switch (p.PowerupType)
      {
        case PowerupType.Health:
          GM.ZMaster.FlagM.SetFlag(flag3, 1);
          break;
        case PowerupType.QuadDamage:
          GM.ZMaster.FlagM.SetFlag(flag3, 1);
          GM.ZMaster.FlagM.SetFlag(flag4, 1);
          break;
        case PowerupType.InfiniteAmmo:
          GM.ZMaster.FlagM.SetFlag(flag4, 1);
          break;
        case PowerupType.Invincibility:
          GM.ZMaster.FlagM.SetFlag(flag3, 1);
          GM.ZMaster.FlagM.SetFlag(flag4, 1);
          break;
        case PowerupType.Ghosted:
          GM.ZMaster.FlagM.SetFlag(flag4, 1);
          break;
        case PowerupType.FarOutMeat:
          GM.ZMaster.FlagM.SetFlag(flag4, 1);
          break;
        case PowerupType.MuscleMeat:
          GM.ZMaster.FlagM.SetFlag(flag3, 1);
          GM.ZMaster.FlagM.SetFlag(flag4, 1);
          break;
        case PowerupType.SnakeEye:
          GM.ZMaster.FlagM.SetFlag(flag4, 1);
          break;
        case PowerupType.Blort:
          GM.ZMaster.FlagM.SetFlag(flag4, 1);
          break;
        case PowerupType.Regen:
          GM.ZMaster.FlagM.SetFlag(flag3, 1);
          GM.ZMaster.FlagM.SetFlag(flag4, 1);
          break;
        case PowerupType.Cyclops:
          GM.ZMaster.FlagM.SetFlag(flag4, 1);
          break;
      }
    }
  }
}
