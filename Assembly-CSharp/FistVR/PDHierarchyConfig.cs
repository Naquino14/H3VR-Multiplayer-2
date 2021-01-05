// Decompiled with JetBrains decompiler
// Type: FistVR.PDHierarchyConfig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New PD Hierarchy Config", menuName = "PancakeDrone/Hierarchy Definition", order = 0)]
  public class PDHierarchyConfig : ScriptableObject
  {
    public List<PDHierarchyConfig.PDChildren> IDs = new List<PDHierarchyConfig.PDChildren>();

    [ContextMenu("Prime")]
    public void Prime()
    {
      this.IDs.Clear();
      for (int index = 1; index < (int) byte.MaxValue; ++index)
      {
        if (Enum.IsDefined(typeof (PDComponentID), (object) index))
        {
          PDHierarchyConfig.PDChildren pdChildren = new PDHierarchyConfig.PDChildren();
          pdChildren.ID = (PDComponentID) index;
          pdChildren.SetName(((PDComponentID) index).ToString());
          this.IDs.Add(pdChildren);
        }
      }
    }

    [Serializable]
    public class PDChildren
    {
      public string name;
      public PDComponentID ID;
      [SearchableEnum]
      public List<PDComponentID> Children;

      public void SetName(string s) => this.name = s;
    }
  }
}
