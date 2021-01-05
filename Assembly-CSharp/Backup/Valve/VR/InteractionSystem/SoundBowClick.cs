// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.SoundBowClick
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class SoundBowClick : MonoBehaviour
  {
    public AudioClip bowClick;
    public AnimationCurve pitchTensionCurve;
    public float minPitch;
    public float maxPitch;
    private AudioSource thisAudioSource;

    private void Awake() => this.thisAudioSource = this.GetComponent<AudioSource>();

    public void PlayBowTensionClicks(float normalizedTension)
    {
      this.thisAudioSource.pitch = (this.maxPitch - this.minPitch) * this.pitchTensionCurve.Evaluate(normalizedTension) + this.minPitch;
      this.thisAudioSource.PlayOneShot(this.bowClick);
    }
  }
}
