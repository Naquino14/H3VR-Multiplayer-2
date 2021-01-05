// Decompiled with JetBrains decompiler
// Type: FistVR.MeatPackerator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class MeatPackerator : MonoBehaviour
  {
    private bool m_hasMeatCore;
    private bool m_hasFirstHerb;
    private bool m_hasSecondHerb;
    private int m_meatcoreType;
    private int m_firstHerb;
    private int m_secondHerb;
    public List<Image> Indicators;
    public List<Sprite> Sprites_Herbs;
    public List<Sprite> Sprites_Cores;
    public ParticleSystem PFX_GrindInsert;
    public AudioEvent AudEvent_Insert;
    public AudioEvent AudEvent_Fail;
    public AudioEvent AudEvent_Success;
    public FVRObject UncookedLink;
    public Transform SpawnPoint;

    private void OnTriggerEnter(Collider col) => this.TestCollider(col);

    private void TestCollider(Collider col)
    {
      if ((Object) col.attachedRigidbody == (Object) null)
        return;
      bool flag = false;
      RotrwHerb component1 = col.attachedRigidbody.gameObject.GetComponent<RotrwHerb>();
      if ((Object) component1 != (Object) null)
      {
        if (this.m_hasFirstHerb && this.m_hasSecondHerb)
          this.EjectIngredient((FVRPhysicalObject) component1);
        else if (!this.m_hasFirstHerb)
          this.HerbInserted(component1, true);
        else if (!this.m_hasSecondHerb)
          this.HerbInserted(component1, false);
        flag = true;
      }
      RotrwMeatCore component2 = col.attachedRigidbody.gameObject.GetComponent<RotrwMeatCore>();
      if ((Object) component2 != (Object) null)
      {
        if (this.m_hasMeatCore)
        {
          this.EjectIngredient((FVRPhysicalObject) component2);
        }
        else
        {
          this.GrindEffect();
          Object.Destroy((Object) component2.gameObject);
          this.MeatCoreInserted(component2);
        }
        flag = true;
      }
      if (flag)
        return;
      if ((Object) col.attachedRigidbody.GetComponent<FVRPhysicalObject>() != (Object) null)
      {
        FVRPhysicalObject component3 = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
        if (component3.IsHeld)
        {
          FVRViveHand hand = component3.m_hand;
          component3.EndInteraction(hand);
          hand.ForceSetInteractable((FVRInteractiveObject) null);
        }
      }
      col.attachedRigidbody.velocity = Vector3.up * 4f + Random.onUnitSphere * 1.5f;
    }

    private void GrindEffect()
    {
      this.PFX_GrindInsert.Emit(20);
      SM.PlayGenericSound(this.AudEvent_Insert, this.transform.position);
    }

    private void EjectIngredient(FVRPhysicalObject obj)
    {
      if (obj.IsHeld)
      {
        FVRViveHand hand = obj.m_hand;
        obj.EndInteraction(hand);
        hand.ForceSetInteractable((FVRInteractiveObject) null);
      }
      obj.RootRigidbody.velocity = Vector3.up * 4f + Random.onUnitSphere * 1.5f;
    }

    private void MeatCoreInserted(RotrwMeatCore mc)
    {
      this.m_meatcoreType = (int) mc.Type;
      this.m_hasMeatCore = true;
      this.UpdateIndicators();
    }

    private void HerbInserted(RotrwHerb h, bool isFirstHerb)
    {
      if (isFirstHerb)
      {
        this.m_firstHerb = (int) h.Type;
        this.m_hasFirstHerb = true;
      }
      else
      {
        this.m_secondHerb = (int) h.Type;
        this.m_hasSecondHerb = true;
      }
      this.GrindEffect();
      Object.Destroy((Object) h.gameObject);
      this.UpdateIndicators();
    }

    private void UpdateIndicators()
    {
      if (this.m_hasFirstHerb)
      {
        this.Indicators[0].gameObject.SetActive(true);
        this.Indicators[0].sprite = this.Sprites_Herbs[this.m_firstHerb];
      }
      else
        this.Indicators[0].gameObject.SetActive(false);
      if (this.m_hasSecondHerb)
      {
        this.Indicators[1].gameObject.SetActive(true);
        this.Indicators[1].sprite = this.Sprites_Herbs[this.m_secondHerb];
      }
      else
        this.Indicators[1].gameObject.SetActive(false);
      if (this.m_hasMeatCore)
      {
        this.Indicators[2].gameObject.SetActive(true);
        this.Indicators[2].sprite = this.Sprites_Cores[this.m_meatcoreType];
      }
      else
        this.Indicators[2].gameObject.SetActive(false);
    }

    private void MachineExplosion()
    {
    }

    public void Grind(int derp)
    {
      if (!this.m_hasMeatCore || !this.m_hasFirstHerb || !this.m_hasSecondHerb)
      {
        SM.PlayGenericSound(this.AudEvent_Fail, this.transform.position);
      }
      else
      {
        SM.PlayGenericSound(this.AudEvent_Success, this.transform.position);
        int num1 = Mathf.Min(this.m_firstHerb, this.m_secondHerb);
        int num2 = Mathf.Max(this.m_firstHerb, this.m_secondHerb);
        int num3 = 0;
        switch (num1)
        {
          case 0:
            switch (num2)
            {
              case 0:
                num3 = 0;
                break;
              case 1:
                num3 = 5;
                break;
              case 2:
                num3 = 6;
                break;
              case 3:
                num3 = 7;
                break;
              case 4:
                num3 = -2;
                break;
            }
            break;
          case 1:
            switch (num2)
            {
              case 1:
                num3 = 3;
                break;
              case 2:
                num3 = 8;
                break;
              case 3:
                num3 = 9;
                break;
              case 4:
                num3 = 10;
                break;
            }
            break;
          case 2:
            switch (num2)
            {
              case 2:
                num3 = 2;
                break;
              case 3:
                num3 = 11;
                break;
              case 4:
                num3 = 12;
                break;
            }
            break;
          case 3:
            switch (num2)
            {
              case 3:
                num3 = 4;
                break;
              case 4:
                num3 = -2;
                break;
            }
            break;
          case 4:
            if (num2 == 4)
            {
              num3 = 1;
              break;
            }
            break;
        }
        this.m_hasFirstHerb = false;
        this.m_hasMeatCore = false;
        this.m_hasSecondHerb = false;
        this.UpdateIndicators();
        RW_Powerup component = Object.Instantiate<GameObject>(this.UncookedLink.GetGameObject(), this.SpawnPoint.position, this.SpawnPoint.rotation).GetComponent<RW_Powerup>();
        if ((Object) GM.ZMaster != (Object) null)
          GM.ZMaster.FlagM.AddToFlag("s_c", 1);
        component.Cooked = false;
        component.PowerupType = (PowerupType) num3;
        component.SetMCMadeWith((RotrwMeatCore.CoreType) this.m_meatcoreType);
        switch (this.m_meatcoreType)
        {
          case 0:
            component.PowerupDuration = PowerUpDuration.VeryShort;
            component.PowerupIntensity = PowerUpIntensity.Low;
            break;
          case 1:
            component.PowerupDuration = PowerUpDuration.VeryShort;
            component.PowerupIntensity = PowerUpIntensity.High;
            component.isPuke = true;
            break;
          case 2:
            component.PowerupDuration = PowerUpDuration.Short;
            component.PowerupIntensity = PowerUpIntensity.Low;
            component.isInverted = true;
            break;
          case 3:
            component.PowerupDuration = PowerUpDuration.Blip;
            component.PowerupIntensity = PowerUpIntensity.High;
            break;
          case 4:
            component.PowerupDuration = PowerUpDuration.Full;
            component.PowerupIntensity = PowerUpIntensity.High;
            break;
          case 5:
            component.PowerupDuration = PowerUpDuration.Short;
            component.PowerupIntensity = PowerUpIntensity.Medium;
            break;
          case 6:
            component.PowerupDuration = PowerUpDuration.SuperLong;
            component.PowerupIntensity = PowerUpIntensity.Low;
            break;
          case 7:
            component.PowerupDuration = PowerUpDuration.Full;
            component.PowerupIntensity = PowerUpIntensity.High;
            component.isInverted = true;
            break;
        }
      }
    }
  }
}
