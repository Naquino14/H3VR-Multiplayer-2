// Decompiled with JetBrains decompiler
// Type: FistVR.ItemSpawnerObjectDefinition
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Definition", menuName = "ItemSpawner/ObjectDefinition", order = 0)]
  public class ItemSpawnerObjectDefinition : ScriptableObject
  {
    public string DisplayName;
    public ItemSpawnerObjectDefinition.ItemSpawnerCategory Category;
    public GameObject Prefab;
    public Sprite Sprite;
    public GameObject[] AdditionalPrefabs;

    public enum ItemSpawnerCategory
    {
      None = -1, // 0xFFFFFFFF
      Melee = 0,
      Explosives = 1,
      Handguns = 2,
      Shotguns = 3,
      Submachineguns = 4,
      Rifles = 5,
      Heavyweapons = 6,
      Magazines = 7,
      Ammunition = 8,
      Attachments = 9,
      Misc = 10, // 0x0000000A
      Horseshoes = 11, // 0x0000000B
    }
  }
}
