// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.SoundPlayOneshot
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class SoundPlayOneshot : MonoBehaviour
  {
    public AudioClip[] waveFiles;
    private AudioSource thisAudioSource;
    public float volMin;
    public float volMax;
    public float pitchMin;
    public float pitchMax;
    public bool playOnAwake;

    private void Awake()
    {
      this.thisAudioSource = this.GetComponent<AudioSource>();
      if (!this.playOnAwake)
        return;
      this.Play();
    }

    public void Play()
    {
      if (!((Object) this.thisAudioSource != (Object) null) || !this.thisAudioSource.isActiveAndEnabled || Util.IsNullOrEmpty<AudioClip>(this.waveFiles))
        return;
      this.thisAudioSource.volume = Random.Range(this.volMin, this.volMax);
      this.thisAudioSource.pitch = Random.Range(this.pitchMin, this.pitchMax);
      this.thisAudioSource.PlayOneShot(this.waveFiles[Random.Range(0, this.waveFiles.Length)]);
    }

    public void Pause()
    {
      if (!((Object) this.thisAudioSource != (Object) null))
        return;
      this.thisAudioSource.Pause();
    }

    public void UnPause()
    {
      if (!((Object) this.thisAudioSource != (Object) null))
        return;
      this.thisAudioSource.UnPause();
    }
  }
}
