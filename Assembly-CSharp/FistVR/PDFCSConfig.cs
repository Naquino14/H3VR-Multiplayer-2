// Decompiled with JetBrains decompiler
// Type: FistVR.PDFCSConfig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New PD FCS Config", menuName = "PancakeDrone/FCS Definition", order = 0)]
  public class PDFCSConfig : ScriptableObject
  {
    public List<PDFCSConfig.ThrusterConfig> Configs;

    [Serializable]
    public class ThrusterConfig
    {
      [SearchableEnum]
      public PDComponentID Sys;
      public PDFCS.Thruster.Facing TDir;
      public bool IsAng;
      public bool IsGAB;
      public PDThrusterConfig TConfig;
    }
  }
}
