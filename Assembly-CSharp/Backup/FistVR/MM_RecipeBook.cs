// Decompiled with JetBrains decompiler
// Type: FistVR.MM_RecipeBook
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class MM_RecipeBook : MonoBehaviour
  {
    public List<MM_RecipeBook.ImageList> ResourceLists;
    private float UpdateTick = 1f;

    private void Start()
    {
    }

    private void Update()
    {
      this.UpdateTick -= Time.deltaTime;
      if ((double) this.UpdateTick > 0.0)
        return;
      this.UpdateTick = UnityEngine.Random.Range(0.3f, 1f);
      for (int index1 = 0; index1 < this.ResourceLists.Count; ++index1)
      {
        if (!this.ResourceLists[index1].ImagesActive && GM.MMFlags.MMMisKnown[index1])
        {
          this.ResourceLists[index1].ImagesActive = true;
          for (int index2 = 0; index2 < this.ResourceLists[index1].RevealThese.Count; ++index2)
            this.ResourceLists[index1].RevealThese[index2].gameObject.SetActive(true);
        }
      }
    }

    [Serializable]
    public class ImageList
    {
      public bool ImagesActive;
      public List<Image> RevealThese;
    }
  }
}
