// Decompiled with JetBrains decompiler
// Type: FistVR.MM_MusicManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MM_MusicManager : MonoBehaviour
  {
    public AudioSource Music;
    public AudioSource AmbientMusic;
    private float tarVolume = 0.25f;
    private float tarVolumeAmbient = 0.1f;
    private bool m_isMusicEnabled = true;

    private void Start()
    {
      this.Music.volume = 0.0f;
      GM.CurrentSceneSettings.PlayerDeathEvent += new FVRSceneSettings.PlayerDeath(this.PlayerDied);
      this.AmbientMusic.volume = 0.0f;
    }

    public void PlayMusic()
    {
      this.Music.volume = 0.01f;
      this.tarVolume = 0.25f;
      this.tarVolumeAmbient = 0.0f;
      if (this.Music.isPlaying)
        return;
      this.Music.Play();
    }

    public void FadeOutMusic()
    {
      this.AmbientMusic.volume = 0.01f;
      this.tarVolume = 0.0f;
      this.tarVolumeAmbient = 0.1f;
      if (this.AmbientMusic.isPlaying)
        return;
      this.AmbientMusic.Play();
    }

    private void Update()
    {
      if (this.Music.isPlaying)
      {
        this.Music.volume = Mathf.MoveTowards(this.Music.volume, this.tarVolume, 0.2f * Time.deltaTime);
        if ((double) this.Music.volume <= 0.0)
          this.Music.Stop();
      }
      if (!this.AmbientMusic.isPlaying)
        return;
      this.AmbientMusic.volume = Mathf.MoveTowards(this.AmbientMusic.volume, this.tarVolumeAmbient, 0.2f * Time.deltaTime);
      if ((double) this.AmbientMusic.volume > 0.0)
        return;
      this.AmbientMusic.Stop();
    }

    public bool IsMusicEnabled() => this.m_isMusicEnabled;

    public void DisableMusic()
    {
      if (!this.m_isMusicEnabled)
        return;
      this.m_isMusicEnabled = false;
      this.Music.enabled = false;
      this.AmbientMusic.enabled = false;
    }

    public void EnableMusic()
    {
      if (this.m_isMusicEnabled)
        return;
      this.m_isMusicEnabled = true;
      this.Music.enabled = true;
      this.AmbientMusic.enabled = true;
      this.FadeOutMusic();
    }

    private void OnDestroy() => GM.CurrentSceneSettings.PlayerDeathEvent -= new FVRSceneSettings.PlayerDeath(this.PlayerDied);

    private void PlayerDied(bool killedSelf) => this.FadeOutMusic();
  }
}
