// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigEnemyTemplate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New EnemyTemplate", menuName = "Zosig/EnemyTemplate", order = 0)]
  public class ZosigEnemyTemplate : ScriptableObject
  {
    public List<GameObject> SosigPrefabs;
    public List<SosigConfigTemplate> ConfigTemplates;
    public List<SosigWearableConfig> WearableTemplates;
    public List<FVRObject> WeaponOptions;
    public List<FVRObject> WeaponOptions_Secondary;
  }
}
