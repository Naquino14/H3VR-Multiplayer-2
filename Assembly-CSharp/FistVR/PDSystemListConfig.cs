// Decompiled with JetBrains decompiler
// Type: FistVR.PDSystemListConfig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New PD SystemList Config", menuName = "PancakeDrone/SystemList Definition", order = 0)]
  public class PDSystemListConfig : ScriptableObject
  {
    public List<PDSystemListConfig.ComponentConfig> Components = new List<PDSystemListConfig.ComponentConfig>();

    [ContextMenu("Prime")]
    public void Prime()
    {
      this.Components.Clear();
      for (int index = 1; index < (int) byte.MaxValue; ++index)
      {
        if (Enum.IsDefined(typeof (PDComponentID), (object) index))
        {
          PDSystemListConfig.ComponentConfig componentConfig = new PDSystemListConfig.ComponentConfig();
          componentConfig.ID = (PDComponentID) index;
          string str = componentConfig.ID.ToString();
          if (str.Contains("PDL"))
          {
            componentConfig.SysType = PDSysType.PDL;
            componentConfig.Col = false;
            componentConfig.TTD = false;
          }
          else if (str.Contains("CDL"))
          {
            componentConfig.SysType = PDSysType.CDL;
            componentConfig.Pow = false;
            componentConfig.TTD = false;
            componentConfig.CIG = false;
          }
          else if (str.Contains("ISR") || str.Contains("ISC") || (str.Contains("ISS") || str.Contains("ARH")) || (str.Contains("ARL") || str.Contains("ART")))
          {
            componentConfig.SysType = PDSysType.STR;
            componentConfig.Pow = false;
            componentConfig.Col = false;
            componentConfig.TTD = false;
            componentConfig.CIG = false;
          }
          this.Components.Add(componentConfig);
        }
      }
    }

    [Serializable]
    public class ComponentConfig
    {
      public PDComponentID ID;
      public PDSysType SysType;
      public PDArmorType ArmType;
      public PDOmniModuleType OmnType;
      public bool Pow = true;
      public bool Col = true;
      public bool TTD = true;
      public bool CIG = true;
    }
  }
}
