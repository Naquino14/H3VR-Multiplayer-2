// Decompiled with JetBrains decompiler
// Type: FistVR.FVRSoundTailsDirectory
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Prefab Dir", menuName = "AudioPooling/TailsDef", order = 0)]
  public class FVRSoundTailsDirectory : ScriptableObject
  {
    public FVRTailSoundClass SoundClass;
    public List<FVRSoundTailsDirectory.TailSoundSet> SoundSets;

    [ContextMenu("Migrate")]
    public void Migrate()
    {
      for (int index = 0; index < this.SoundSets.Count; ++index)
      {
        if (this.SoundSets[index].Environment == FVRSoundEnvironment.InsideMedium)
          this.SoundSets[index].AudioEvent.Clips = this.SoundSets[2].AudioEvent.Clips;
        else if (this.SoundSets[index].Environment == FVRSoundEnvironment.OutsideEnclosedNarrow)
          this.SoundSets[index].AudioEvent.Clips = this.SoundSets[5].AudioEvent.Clips;
        else if (this.SoundSets[index].Environment == FVRSoundEnvironment.ShootingRange)
          this.SoundSets[index].AudioEvent.Clips = this.SoundSets[1].AudioEvent.Clips;
      }
    }

    [Serializable]
    public class TailSoundSet
    {
      public FVRSoundEnvironment Environment;
      public AudioEvent AudioEvent;
    }
  }
}
