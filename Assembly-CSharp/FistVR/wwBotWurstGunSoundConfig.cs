// Decompiled with JetBrains decompiler
// Type: FistVR.wwBotWurstGunSoundConfig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "BotWurstGunSoundConfig", menuName = "Agents/Wurstwurld/BotSoundConfig", order = 0)]
  public class wwBotWurstGunSoundConfig : ScriptableObject
  {
    public List<wwBotWurstGunSoundConfig.BotGunShotSet> ShotSets;
    public AudioEvent EjectionBack;
    public AudioEvent EjectionForward;
    public AudioEvent GoingToReload;
    public AudioEvent Reloading;
    public AudioEvent RecoveringFromReload;
    public AutoMeaterFirearmSoundProfile MigrateFrom;

    [ContextMenu("MigrateBotSetToThis")]
    public void MigrateBotSetToThis()
    {
      for (int index1 = 0; index1 < this.MigrateFrom.ShotSets.Count; ++index1)
      {
        AutoMeaterFirearmSoundProfile.GunShotSet shotSet = this.MigrateFrom.ShotSets[index1];
        this.ShotSets[index1].SampleName = shotSet.SampleName;
        for (int index2 = 0; index2 < shotSet.EnvironmentsUsed.Count; ++index2)
          this.ShotSets[index1].EnvironmentsUsed.Add(shotSet.EnvironmentsUsed[index2]);
        this.ShotSets[index1].ShotSet_Near = shotSet.ShotSet_Near;
        this.ShotSets[index1].ShotSet_Far = shotSet.ShotSet_Far;
        this.ShotSets[index1].ShotSet_Distant = shotSet.ShotSet_Distant;
      }
    }

    [Serializable]
    public class BotGunShotSet
    {
      public string SampleName;
      public List<FVRSoundEnvironment> EnvironmentsUsed = new List<FVRSoundEnvironment>();
      public AudioEvent ShotSet_Near;
      public AudioEvent ShotSet_Far;
      public AudioEvent ShotSet_Distant;
    }
  }
}
