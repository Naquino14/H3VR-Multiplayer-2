// Decompiled with JetBrains decompiler
// Type: FistVR.PDElectricalSystem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class PDElectricalSystem : MonoBehaviour
  {
    public PD PD;
    public List<PDElectricalSystem.PowerSource> PowerSources;

    public void Init()
    {
      for (int index = 0; index < this.PowerSources.Count; ++index)
      {
        this.PowerSources[index].ESys = this;
        this.PowerSources[index].Init();
      }
    }

    public void Tick(float t)
    {
      for (int index = 0; index < this.PowerSources.Count; ++index)
        this.PowerSources[index].Tick(t);
      this.PD.GetSys(3).MaxPowerAvailable = 1f;
      this.PD.GetSys(5).MaxPowerAvailable = 0.2f;
      this.PD.GetSys(6).MaxPowerAvailable = 0.2f;
      this.PD.GetSys(5).PState = PDSys.PowerState.EmergencyPower;
      this.PD.GetSys(6).PState = PDSys.PowerState.EmergencyPower;
    }

    [Serializable]
    public class PowerSource
    {
      public PDElectricalSystem ESys;
      public PDComponentID ID;
      private PDSys.PowerState BaseState;

      public void Init() => this.ESys.PD.GetSys(5).PState = this.BaseState;

      public void Tick(float t)
      {
      }
    }
  }
}
