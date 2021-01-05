// Decompiled with JetBrains decompiler
// Type: FistVR.TAH_WaveDefinition
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New WaveDefinition", menuName = "TakeAndHold/TAHWaveDefinition", order = 0)]
  public class TAH_WaveDefinition : ScriptableObject
  {
    public int NumSpawnPointsToUse = 1;
    public float WarmUpToSpawnTime = 3f;
    public float TimeForWave = 10f;
    public float SpawnCooldownTime = 1f;
    public int NumBots = 3;
    public int WaveIntensity = 1;
    public List<TAH_BotSpawnProfile> BotSpawnProfiles;
  }
}
