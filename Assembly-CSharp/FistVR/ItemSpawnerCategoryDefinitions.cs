// Decompiled with JetBrains decompiler
// Type: FistVR.ItemSpawnerCategoryDefinitions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Definition", menuName = "ItemSpawner/CategoryDefinition", order = 0)]
  public class ItemSpawnerCategoryDefinitions : ScriptableObject
  {
    public ItemSpawnerCategoryDefinitions.Category[] Categories;

    [Serializable]
    public class Category
    {
      public ItemSpawnerID.EItemCategory Cat;
      public string DisplayName;
      public Sprite Sprite;
      public ItemSpawnerCategoryDefinitions.SubCategory[] Subcats;
      [Header("DisplayToggle")]
      public bool DoesDisplay_Sandbox = true;
      public bool DoesDisplay_Unlocks = true;
    }

    [Serializable]
    public class SubCategory
    {
      public ItemSpawnerID.ESubCategory Subcat;
      public string DisplayName;
      public Sprite Sprite;
      [Header("DisplayToggle")]
      public bool DoesDisplay_Sandbox = true;
      public bool DoesDisplay_Unlocks = true;
    }
  }
}
