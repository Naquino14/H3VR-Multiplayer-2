// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_EnemyTemplate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New EnemyTemplate", menuName = "TNH/TNH_EnemyTemplate", order = 0)]
  public class TNH_EnemyTemplate : ScriptableObject
  {
    public TNH_EnemyType Type;
    public List<FVRObject> SosigPrefabs;
    public SosigConfigTemplate ConfigTemplate_Standard;
    public SosigConfigTemplate ConfigTemplate_Easy;
    public List<SosigOutfitConfig> OutfitConfig;
    public List<FVRObject> WeaponOptions;
    public List<FVRObject> WeaponOptions_Secondary;
    public float SecondaryChance;
  }
}
