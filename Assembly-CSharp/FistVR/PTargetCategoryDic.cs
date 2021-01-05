// Decompiled with JetBrains decompiler
// Type: FistVR.PTargetCategoryDic
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New TNH_CharacterDatabase", menuName = "PaperTarget/CategorySet", order = 0)]
  public class PTargetCategoryDic : ScriptableObject
  {
    public List<PTargetCategoryDic.PTCat> Cats;

    [ContextMenu("SetIcons")]
    public void SetIcons()
    {
      for (int index1 = 0; index1 < this.Cats.Count; ++index1)
      {
        this.Cats[index1].TargetIcons.Clear();
        for (int index2 = 0; index2 < this.Cats[index1].Targets.Count; ++index2)
          this.Cats[index1].TargetIcons.Add(this.Cats[index1].Targets[index2].GetGameObject().GetComponent<PTargetReferenceHolder>().Profile.displayIcon);
      }
    }

    [Serializable]
    public class PTCat
    {
      public string Name;
      public Sprite CatImage;
      public List<Sprite> TargetIcons;
      public List<FVRObject> Targets;
    }
  }
}
