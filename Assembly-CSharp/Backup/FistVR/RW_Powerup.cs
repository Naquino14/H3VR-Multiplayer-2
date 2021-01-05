// Decompiled with JetBrains decompiler
// Type: FistVR.RW_Powerup
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RW_Powerup : FVRPhysicalObject
  {
    [Header("Powerup Stuff")]
    public PowerupType PowerupType;
    public PowerUpIntensity PowerupIntensity;
    public PowerUpDuration PowerupDuration;
    public bool isPuke;
    public bool isInverted;
    private bool hasTriggered;
    public bool Cooked = true;
    public GameObject Payload;
    public AudioEvent AudEvent_DIng;
    public GameObject NameDisplay_Normal;
    public GameObject NameDisplay_Inverted;
    public List<GameObject> Symbols_Duration;
    public List<GameObject> Symbols_Intensity;
    public GameObject Mold;
    private RotrwMeatCore.CoreType m_madeWithMeatCore;

    public void SetMCMadeWith(RotrwMeatCore.CoreType c) => this.m_madeWithMeatCore = c;

    public RotrwMeatCore.CoreType GetMCMadeWith() => this.m_madeWithMeatCore;

    protected override void Awake()
    {
      base.Awake();
      this.UpdateSymbols();
    }

    public void SetParams(
      PowerupType t,
      PowerUpIntensity intensity,
      PowerUpDuration d,
      bool p,
      bool inv)
    {
      this.PowerupType = t;
      this.PowerupIntensity = intensity;
      this.PowerupDuration = d;
      this.isPuke = p;
      this.isInverted = inv;
      if (inv)
      {
        if ((Object) this.NameDisplay_Inverted != (Object) null)
          this.NameDisplay_Inverted.SetActive(true);
        if ((Object) this.NameDisplay_Normal != (Object) null)
          this.NameDisplay_Normal.SetActive(false);
      }
      else
      {
        if ((Object) this.NameDisplay_Inverted != (Object) null)
          this.NameDisplay_Inverted.SetActive(false);
        if ((Object) this.NameDisplay_Normal != (Object) null)
          this.NameDisplay_Normal.SetActive(true);
      }
      if (this.isPuke && (Object) this.Mold != (Object) null)
        this.Mold.SetActive(true);
      this.UpdateSymbols();
    }

    private void UpdateSymbols()
    {
      bool flag1 = false;
      bool flag2 = false;
      switch (this.PowerupType)
      {
        case PowerupType.Health:
          flag1 = true;
          break;
        case PowerupType.QuadDamage:
          flag1 = true;
          flag2 = true;
          break;
        case PowerupType.InfiniteAmmo:
          flag2 = true;
          break;
        case PowerupType.Invincibility:
          flag1 = true;
          flag2 = true;
          break;
        case PowerupType.Ghosted:
          flag2 = true;
          break;
        case PowerupType.FarOutMeat:
          flag2 = true;
          break;
        case PowerupType.MuscleMeat:
          flag1 = true;
          flag2 = true;
          break;
        case PowerupType.SnakeEye:
          flag2 = true;
          break;
        case PowerupType.Blort:
          flag2 = true;
          break;
        case PowerupType.Regen:
          flag1 = true;
          flag2 = true;
          break;
        case PowerupType.Cyclops:
          flag2 = true;
          break;
      }
      for (int index = 0; index < this.Symbols_Duration.Count; ++index)
      {
        if (this.PowerupDuration == (PowerUpDuration) index && flag2)
        {
          if ((Object) this.Symbols_Duration[index] != (Object) null)
            this.Symbols_Duration[index].SetActive(true);
        }
        else if ((Object) this.Symbols_Duration[index] != (Object) null)
          this.Symbols_Duration[index].SetActive(false);
      }
      for (int index = 0; index < this.Symbols_Intensity.Count; ++index)
      {
        if (this.PowerupIntensity == (PowerUpIntensity) index && flag1)
        {
          if ((Object) this.Symbols_Intensity[index] != (Object) null)
            this.Symbols_Intensity[index].SetActive(true);
        }
        else if ((Object) this.Symbols_Intensity[index] != (Object) null)
          this.Symbols_Intensity[index].SetActive(false);
      }
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.transform.position + Vector3.up * -0.1f) >= 0.150000005960464 || !this.Cooked)
        return;
      this.PowerUp(hand);
    }

    public void Detonate()
    {
      if ((Object) this.Payload != (Object) null)
        Object.Instantiate<GameObject>(this.Payload, this.transform.position, this.transform.rotation).GetComponent<PowerUp_Cloud>().SetParams(this.PowerupType, this.PowerupIntensity, this.PowerupDuration, this.isPuke, this.isInverted);
      Object.Destroy((Object) this.GameObject);
    }

    private void PowerUp(FVRViveHand hand)
    {
      if (this.hasTriggered)
        return;
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_DIng, this.transform.position);
      GM.CurrentSceneSettings.OnPowerupUse(this.PowerupType);
      FVRViveHand fvrViveHand = hand;
      this.hasTriggered = true;
      this.EndInteraction(hand);
      fvrViveHand.ForceSetInteractable((FVRInteractiveObject) null);
      GM.CurrentPlayerBody.ActivatePower(this.PowerupType, this.PowerupIntensity, this.PowerupDuration, this.isPuke, this.isInverted);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
