// Decompiled with JetBrains decompiler
// Type: SoundController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class SoundController : MonoBehaviour
{
  public AudioClip[] _audioClips;
  public int _audioChannels = 10;
  public float _masterVol = 0.5f;
  public float _soundVol = 1f;
  public float _musicVol = 1f;
  public bool _linearRollOff;
  public AudioSource[] channels;
  public int channel;
  public AudioSource[] _musicChannels;
  public int _musicChannel;
  private float _currentMusicVol;
  private float _fadeTo;
  public static SoundController instance;

  public void OnApplicationQuit() => SoundController.instance = (SoundController) null;

  public void Start()
  {
    if ((Object) SoundController.instance != (Object) null)
    {
      Object.Destroy((Object) this.gameObject);
    }
    else
    {
      SoundController.instance = this;
      Object.DontDestroyOnLoad((Object) this.gameObject);
    }
    this.AddChannels();
    Object.DontDestroyOnLoad((Object) this.transform.gameObject);
  }

  public void StopMusic(bool fade) => this.PlayMusic((AudioClip) null, 0.0f, 1f, fade);

  public void FadeUpMusic()
  {
    if ((double) this._musicChannels[this._musicChannel].volume < (double) this._fadeTo)
      this._musicChannels[this._musicChannel].volume += 1f / 400f;
    else
      this.CancelInvoke(nameof (FadeUpMusic));
  }

  public void FadeDownMusic()
  {
    int index = 0;
    if (this._musicChannel == 0)
      index = 1;
    if ((double) this._musicChannels[index].volume > 0.0)
    {
      this._musicChannels[index].volume -= 1f / 400f;
    }
    else
    {
      this._musicChannels[index].Stop();
      this.CancelInvoke(nameof (FadeDownMusic));
    }
  }

  public void UpdateMusicVolume()
  {
    for (int index = 0; index < 2; ++index)
      this._musicChannels[index].volume = this._currentMusicVol * this._masterVol * this._musicVol;
  }

  public void AddChannels()
  {
    this.channels = new AudioSource[this._audioChannels];
    this._musicChannels = new AudioSource[2];
    if (this.channels.Length <= this._audioChannels)
    {
      for (int index = 0; index < this._audioChannels; ++index)
      {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<AudioSource>();
        gameObject.name = "AudioChannel " + (object) index;
        gameObject.transform.parent = this.transform;
        this.channels[index] = gameObject.GetComponent<AudioSource>();
        if (this._linearRollOff)
          this.channels[index].rolloffMode = AudioRolloffMode.Linear;
      }
    }
    for (int index = 0; index < 2; ++index)
    {
      GameObject gameObject = new GameObject();
      gameObject.AddComponent<AudioSource>();
      gameObject.name = "MusicChannel " + (object) index;
      gameObject.transform.parent = this.transform;
      this._musicChannels[index] = gameObject.GetComponent<AudioSource>();
      this._musicChannels[index].loop = true;
      this._musicChannels[index].volume = 0.0f;
      if (this._linearRollOff)
        this._musicChannels[index].rolloffMode = AudioRolloffMode.Linear;
    }
  }

  public void PlayMusic(AudioClip clip, float volume, float pitch, bool fade)
  {
    if (!fade)
      this._musicChannels[this._musicChannel].volume = 0.0f;
    this._musicChannel = this._musicChannel != 0 ? 0 : 1;
    this._currentMusicVol = volume;
    this._musicChannels[this._musicChannel].clip = clip;
    if (fade)
    {
      this._fadeTo = volume * this._masterVol * this._musicVol;
      this.InvokeRepeating("FadeUpMusic", 0.01f, 0.01f);
      this.InvokeRepeating("FadeDownMusic", 0.01f, 0.01f);
    }
    else
      this._musicChannels[this._musicChannel].volume = volume * this._masterVol * this._musicVol;
    this._musicChannels[this._musicChannel].GetComponent<AudioSource>().pitch = pitch;
    this._musicChannels[this._musicChannel].GetComponent<AudioSource>().Play();
  }

  public void Play(int audioClipIndex, float volume, float pitch)
  {
    if (this.channel < this.channels.Length - 1)
      ++this.channel;
    else
      this.channel = 0;
    if (audioClipIndex >= this._audioClips.Length)
      return;
    this.channels[this.channel].clip = this._audioClips[audioClipIndex];
    this.channels[this.channel].GetComponent<AudioSource>().volume = volume * this._masterVol * this._soundVol;
    this.channels[this.channel].GetComponent<AudioSource>().pitch = pitch;
    this.channels[this.channel].GetComponent<AudioSource>().Play();
  }

  public void Play(AudioClip clip, float volume, float pitch, Vector3 position)
  {
    if (this.channel < this.channels.Length - 1)
      ++this.channel;
    else
      this.channel = 0;
    this.channels[this.channel].clip = clip;
    this.channels[this.channel].GetComponent<AudioSource>().volume = volume * this._masterVol * this._soundVol;
    this.channels[this.channel].GetComponent<AudioSource>().pitch = pitch;
    this.channels[this.channel].transform.position = position;
    this.channels[this.channel].GetComponent<AudioSource>().Play();
  }

  public void Play(AudioClip clip, float volume, float pitch)
  {
    if (this.channel < this.channels.Length - 1)
      ++this.channel;
    else
      this.channel = 0;
    this.channels[this.channel].clip = clip;
    this.channels[this.channel].GetComponent<AudioSource>().volume = volume * this._masterVol * this._soundVol;
    this.channels[this.channel].GetComponent<AudioSource>().pitch = pitch;
    this.channels[this.channel].GetComponent<AudioSource>().Play();
  }

  public void StopAll()
  {
    for (int index = 0; index < this.channels.Length; ++index)
      this.channels[index].Stop();
  }
}
