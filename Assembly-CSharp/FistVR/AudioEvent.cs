// Decompiled with JetBrains decompiler
// Type: FistVR.AudioEvent
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class AudioEvent
  {
    public List<AudioClip> Clips;
    public Vector2 VolumeRange = new Vector2(1f, 1f);
    public Vector2 PitchRange = new Vector2(1f, 1f);
    public Vector2 ClipLengthRange = new Vector2(1f, 1f);

    public AudioEvent() => this.Clips = new List<AudioClip>();

    public void SetLengthRange()
    {
      float length1 = this.Clips[0].length;
      float length2 = this.Clips[0].length;
      for (int index = 1; index < this.Clips.Count; ++index)
      {
        if ((double) this.Clips[index].length < (double) length1)
          length1 = this.Clips[index].length;
        if ((double) this.Clips[index].length > (double) length2)
          length2 = this.Clips[index].length;
      }
      this.ClipLengthRange = new Vector2(length1, length2);
    }
  }
}
