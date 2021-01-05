// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.LinearAudioPitch
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class LinearAudioPitch : MonoBehaviour
  {
    public LinearMapping linearMapping;
    public AnimationCurve pitchCurve;
    public float minPitch;
    public float maxPitch;
    public bool applyContinuously = true;
    private AudioSource audioSource;

    private void Awake()
    {
      if ((Object) this.audioSource == (Object) null)
        this.audioSource = this.GetComponent<AudioSource>();
      if (!((Object) this.linearMapping == (Object) null))
        return;
      this.linearMapping = this.GetComponent<LinearMapping>();
    }

    private void Update()
    {
      if (!this.applyContinuously)
        return;
      this.Apply();
    }

    private void Apply() => this.audioSource.pitch = Mathf.Lerp(this.minPitch, this.maxPitch, this.pitchCurve.Evaluate(this.linearMapping.value));
  }
}
