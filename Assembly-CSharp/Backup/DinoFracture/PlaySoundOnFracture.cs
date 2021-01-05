// Decompiled with JetBrains decompiler
// Type: DinoFracture.PlaySoundOnFracture
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using DinoFracture.Internal;
using UnityEngine;

namespace DinoFracture
{
  [RequireComponent(typeof (AudioSource))]
  public class PlaySoundOnFracture : MonoBehaviour
  {
    private void OnFracture(OnFractureEventArgs args)
    {
      if (!((Object) args.OriginalObject.gameObject == (Object) this.gameObject))
        return;
      GameObject gameObject = new GameObject("FractureTempAudioSource");
      AudioSource component = this.GetComponent<AudioSource>();
      AudioSource audioSource = gameObject.AddComponent<AudioSource>();
      audioSource.bypassEffects = component.bypassEffects;
      audioSource.bypassListenerEffects = component.bypassListenerEffects;
      audioSource.bypassReverbZones = component.bypassReverbZones;
      audioSource.clip = component.clip;
      audioSource.dopplerLevel = component.dopplerLevel;
      audioSource.ignoreListenerPause = component.ignoreListenerPause;
      audioSource.ignoreListenerVolume = component.ignoreListenerVolume;
      audioSource.loop = component.loop;
      audioSource.maxDistance = component.maxDistance;
      audioSource.mute = component.mute;
      audioSource.mute = component.mute;
      audioSource.minDistance = component.minDistance;
      audioSource.mute = component.mute;
      audioSource.panStereo = component.panStereo;
      audioSource.spatialBlend = component.spatialBlend;
      audioSource.pitch = component.pitch;
      audioSource.playOnAwake = component.playOnAwake;
      audioSource.priority = component.priority;
      audioSource.rolloffMode = component.rolloffMode;
      audioSource.spread = component.spread;
      audioSource.time = component.time;
      audioSource.timeSamples = component.timeSamples;
      audioSource.velocityUpdateMode = component.velocityUpdateMode;
      audioSource.volume = component.volume;
      gameObject.AddComponent<DestroyOnAudioFinish>();
    }
  }
}
