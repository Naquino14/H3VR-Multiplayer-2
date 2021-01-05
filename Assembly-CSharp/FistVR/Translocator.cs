// Decompiled with JetBrains decompiler
// Type: FistVR.Translocator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Translocator : MonoBehaviour
  {
    public string FlagWhenPowered;
    public int FlagValueWhenPowered;
    public List<Translocator.RessemblerCoreSlot> Slots;
    public AudioSource AudSource_TeleportHum;
    public GameObject GlowStuff;
    public bool RequiresSlots = true;
    public Translocator EndPoint;
    public Transform ExitPoint;
    public AudioEvent AudEvent_Insert;
    public AudioEvent AudEvent_Arrive;
    private ZosigFlagManager M;
    public AudioSource AudSource_ChargeUp;
    public GameObject TeleportArriveEffect;
    private float m_cycleUpEnergy;
    private float m_recycleTick;
    private bool m_isPoweredUp;

    public void Init(ZosigFlagManager m)
    {
      this.M = m;
      for (int index = 0; index < this.Slots.Count; ++index)
        this.Slots[index].InitFromFlagM(m, this);
      if (m.GetFlagValue(this.FlagWhenPowered) >= this.FlagValueWhenPowered)
        this.InitPowerState(true);
      else
        this.InitPowerState(false);
    }

    private void InitPowerState(bool b)
    {
      if (b)
      {
        for (int index = 0; index < this.Slots.Count; ++index)
          this.Slots[index].SetPoweredState(true, false);
      }
      this.UpdatePowerState();
    }

    public bool InsertCoreToSlot(int i)
    {
      if (i >= this.Slots.Count)
        return false;
      SM.PlayGenericSound(this.AudEvent_Insert, this.Slots[i].CoreTrigger.transform.position);
      this.Slots[i].SetPoweredState(true, true);
      return true;
    }

    public bool IsPowered()
    {
      if (!this.RequiresSlots)
        return true;
      bool flag = true;
      for (int index = 0; index < this.Slots.Count; ++index)
      {
        if (!this.Slots[index].IsPowered)
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    private void Update() => this.UpdatePlayerStuff();

    private void UpdatePlayerStuff()
    {
      if (!this.m_isPoweredUp)
        return;
      if ((double) this.m_recycleTick > 0.0)
      {
        this.m_recycleTick -= Time.deltaTime;
      }
      else
      {
        Vector3 position1 = GM.CurrentPlayerBody.Head.position;
        position1.y = 0.0f;
        Vector3 position2 = this.transform.position;
        position2.y = 0.0f;
        float num1 = Vector3.Distance(position1, position2);
        float num2 = Mathf.Abs(GM.CurrentPlayerBody.transform.position.y - this.transform.position.y);
        if ((double) num1 < 0.600000023841858 && (double) num2 < 0.6 && (UnityEngine.Object) this.EndPoint != (UnityEngine.Object) null)
        {
          this.m_cycleUpEnergy += Time.deltaTime;
          if (!this.AudSource_ChargeUp.isPlaying)
            this.AudSource_ChargeUp.Play();
          if ((double) this.m_cycleUpEnergy <= 1.0)
            return;
          this.TeleportToEndPoint(this.EndPoint);
        }
        else
        {
          this.m_cycleUpEnergy = 0.0f;
          if (!this.AudSource_ChargeUp.isPlaying)
            return;
          this.AudSource_ChargeUp.Stop();
        }
      }
    }

    private void TeleportToEndPoint(Translocator e)
    {
      this.m_cycleUpEnergy = 0.0f;
      this.m_recycleTick = 2f;
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(e.ExitPoint.position, true);
      e.PlayArriveSound();
      if ((double) UnityEngine.Random.Range(0.0f, 1f) >= 0.0199999995529652)
        ;
    }

    public void PlayArriveSound()
    {
      SM.PlayGenericSound(this.AudEvent_Arrive, this.transform.position);
      UnityEngine.Object.Instantiate<GameObject>(this.TeleportArriveEffect, this.transform.position, this.transform.rotation);
    }

    private void UpdatePowerState()
    {
      this.m_isPoweredUp = this.IsPowered();
      if (this.m_isPoweredUp)
      {
        this.M.SetFlagMaxBlend(this.FlagWhenPowered, this.FlagValueWhenPowered);
        this.GlowStuff.SetActive(true);
        if (this.AudSource_TeleportHum.isPlaying)
          return;
        this.AudSource_TeleportHum.Play();
      }
      else
      {
        this.GlowStuff.SetActive(false);
        if (!this.AudSource_TeleportHum.isPlaying)
          return;
        this.AudSource_TeleportHum.Stop();
      }
    }

    [Serializable]
    public class RessemblerCoreSlot
    {
      public Translocator T;
      public bool IsPowered;
      public GameObject CoreTrigger;
      public string FlagForBeingEnabled;
      public Renderer PoweredRenderer;
      private ZosigFlagManager M;

      public void InitFromFlagM(ZosigFlagManager m, Translocator t)
      {
        this.T = t;
        this.M = m;
        if (m.GetFlagValue(this.FlagForBeingEnabled) > 0)
          this.SetPoweredState(true, false);
        else
          this.SetPoweredState(false, false);
      }

      public void SetPoweredState(bool b, bool SetTStateAfter)
      {
        this.IsPowered = b;
        if (b)
        {
          this.PoweredRenderer.enabled = true;
          this.CoreTrigger.SetActive(false);
          this.M.SetFlagMaxBlend(this.FlagForBeingEnabled, 1);
        }
        else
        {
          this.PoweredRenderer.enabled = false;
          this.CoreTrigger.SetActive(true);
        }
        if (!SetTStateAfter)
          return;
        this.T.UpdatePowerState();
      }
    }
  }
}
