// Decompiled with JetBrains decompiler
// Type: FistVR.PDSys
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class PDSys
  {
    public PDComponentID ID;
    public PDSysType SysType;
    public PDArmorType ArmType;
    public PDOmniModuleType OmnType;
    public bool NeedsPower = true;
    public bool NeedsCoolant = true;
    public bool TakesDamFromThermal = true;
    public bool CanIgnite = true;
    public PDSys.PowerState PState;
    public PDSys.CoolantState CState;
    public PDSys.ThermalState TState;
    public PDSys.DamageState DState;
    protected float m_integrity = 100f;
    protected float m_coolantPressure = 100f;
    protected float m_heat;
    public float MaxPowerAvailable = 1f;
    public PD PD;
    [SerializeField]
    protected List<int> m_powerParents = new List<int>();
    [SerializeField]
    protected List<int> m_powerChildren = new List<int>();
    [SerializeField]
    protected List<int> m_coolantParents = new List<int>();
    [SerializeField]
    protected List<int> m_coolantChildren = new List<int>();
    protected PDInterface m_interface;
    protected bool m_hasInterface;

    public float GetIntegrity() => this.m_integrity;

    public float GetHeat() => this.m_heat;

    public void RemoveHeat(float h) => this.m_heat -= h;

    public void AddHeat(float h) => this.m_heat += h;

    public void ConnectInterface(PDInterface Interface)
    {
      this.m_interface = Interface;
      if (!((UnityEngine.Object) this.m_interface != (UnityEngine.Object) null))
        return;
      this.m_hasInterface = true;
    }

    public void AddPowerChild(PDComponentID child)
    {
      if (this.m_powerChildren.Contains((int) child))
        return;
      this.m_powerChildren.Add((int) child);
    }

    public void AddPowerParent(PDComponentID parent)
    {
      if (this.m_powerParents.Contains((int) parent))
        return;
      this.m_powerParents.Add((int) parent);
    }

    public void AddCoolantChild(PDComponentID child)
    {
      if (this.m_coolantChildren.Contains((int) child))
        return;
      this.m_coolantChildren.Add((int) child);
    }

    public void AddCoolantParent(PDComponentID parent)
    {
      if (this.m_coolantParents.Contains((int) parent))
        return;
      this.m_coolantParents.Add((int) parent);
    }

    public void UpdatePowerState()
    {
      this.PState = PDSys.PowerState.Unpowered;
      float b = 0.0f;
      for (int index = 0; index < this.m_powerParents.Count; ++index)
      {
        this.PState = (PDSys.PowerState) Mathf.Min((int) this.PState, (int) this.PD.GetSys(this.m_powerParents[index]).PState);
        b = Mathf.Max((float) (int) this.PD.GetSys(this.m_powerParents[index]).MaxPowerAvailable, b);
      }
      this.MaxPowerAvailable = b;
    }

    public void UpdateCoolantState(float t)
    {
      this.CState = PDSys.CoolantState.Unpressurized;
      float target = 0.0f;
      if (this.ID == PDComponentID.SYS_CCS)
      {
        this.CState = PDSys.CoolantState.Pressurized;
        target = this.PD.TempCoolantRate;
        this.m_heat -= this.m_heat * 2f * t;
      }
      for (int index = 0; index < this.m_coolantParents.Count; ++index)
      {
        this.CState = (PDSys.CoolantState) Mathf.Min((int) this.CState, (int) this.PD.GetSys(this.m_coolantParents[index]).CState);
        target += (float) (int) this.PD.GetSys(this.m_coolantParents[index]).m_coolantPressure;
      }
      if (this.m_coolantParents.Count > 0)
        target /= (float) this.m_coolantParents.Count;
      this.m_coolantPressure = Mathf.MoveTowards(this.m_coolantPressure, target, t * 10f);
      if (this.m_coolantChildren.Count <= 0)
        return;
      for (int index = 0; index < this.m_coolantChildren.Count; ++index)
      {
        float h = this.m_coolantPressure / 100f * this.PD.GetSys(this.m_coolantChildren[index]).GetHeat() * t;
        this.PD.GetSys(this.m_coolantChildren[index]).RemoveHeat(h);
        this.m_heat += h;
      }
      this.m_heat = Mathf.Clamp(this.m_heat, 0.0f, 200f);
    }

    public void Print(string s) => this.m_interface.PrintEventMessage(this, s);

    public string GetIdentifier() => this.ID.ToString();

    public enum PowerState
    {
      Powered,
      EmergencyPower,
      Unpowered,
    }

    public enum CoolantState
    {
      Pressurized,
      Unpressurized,
    }

    public enum ThermalState
    {
      Cooled,
      Uncooled,
    }

    public enum DamageState
    {
      Undamaged,
      Compromised,
      Nonfunctional,
    }
  }
}
