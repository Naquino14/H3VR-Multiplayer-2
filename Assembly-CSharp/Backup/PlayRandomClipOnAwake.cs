// Decompiled with JetBrains decompiler
// Type: PlayRandomClipOnAwake
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class PlayRandomClipOnAwake : MonoBehaviour
{
  public AudioSource source;
  public AudioClip[] clips;
  public Vector2 pitchRange;
  public Vector2 volumeRange;

  private void Awake()
  {
    this.source.pitch = Random.Range(this.pitchRange.x, this.pitchRange.y);
    this.source.PlayOneShot(this.clips[Random.Range(0, this.clips.Length)], Random.Range(this.volumeRange.x, this.volumeRange.y));
  }
}
