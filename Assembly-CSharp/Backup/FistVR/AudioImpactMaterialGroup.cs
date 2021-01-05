// Decompiled with JetBrains decompiler
// Type: FistVR.AudioImpactMaterialGroup
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New AudioImpactMaterialGroup", menuName = "AudioPooling/AudioImpactMaterialGroup", order = 0)]
  public class AudioImpactMaterialGroup : ScriptableObject
  {
    public List<AudioClip> Clips_Hard;
    public List<AudioClip> Clips_Medium;
    public List<AudioClip> Clips_Light;
    public Vector3 Volumes = new Vector3(1f, 1f, 1f);
    public List<AudioClip> Clips_Temp;

    [ContextMenu("SortMe")]
    public void SortMe()
    {
      for (int index = 0; index < this.Clips_Temp.Count; ++index)
      {
        if (this.Clips_Temp[index].name.Contains("Light"))
          this.Clips_Light.Add(this.Clips_Temp[index]);
        else if (this.Clips_Temp[index].name.Contains("Medium"))
          this.Clips_Medium.Add(this.Clips_Temp[index]);
        else if (this.Clips_Temp[index].name.Contains("Hard"))
          this.Clips_Hard.Add(this.Clips_Temp[index]);
      }
      this.Clips_Temp.Clear();
    }
  }
}
