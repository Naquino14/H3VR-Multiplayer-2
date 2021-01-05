// Decompiled with JetBrains decompiler
// Type: FistVR.FVRPooledAudioSource
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Audio;

namespace FistVR
{
  public class FVRPooledAudioSource : MonoBehaviour
  {
    public AudioSource Source;
    public AudioLowPassFilter LowPassFilter;
    private bool m_hasLowPass;
    private AudioMixerGroup m_originalMixer;
    public AnimationCurve OcclusionFactorCurve;
    public AnimationCurve OcclusionVolumeCurve;
    public LayerMask OcclusionLM;
    private bool m_hasFollowTrans;
    private Transform m_followTrans;
    private bool m_isBeingDestroyed;

    public void Awake()
    {
      this.m_originalMixer = this.Source.outputAudioMixerGroup;
      if (!((Object) this.LowPassFilter != (Object) null))
        return;
      this.m_hasLowPass = true;
    }

    public void Play(
      AudioEvent audioEvent,
      Vector3 pos,
      Vector2 pitch,
      Vector2 volume,
      AudioMixerGroup mixerOverride = null)
    {
      if (this.Source.isPlaying)
        this.Source.Stop();
      this.m_hasFollowTrans = false;
      this.m_followTrans = (Transform) null;
      this.transform.position = pos;
      this.Source.clip = audioEvent.Clips[Random.Range(0, audioEvent.Clips.Count)];
      this.Source.volume = Random.Range(volume.x, volume.y);
      this.Source.pitch = Random.Range(pitch.x, pitch.y);
      if ((Object) mixerOverride != (Object) null)
        this.Source.outputAudioMixerGroup = mixerOverride;
      else if ((Object) this.Source.outputAudioMixerGroup != (Object) this.m_originalMixer)
        this.Source.outputAudioMixerGroup = this.m_originalMixer;
      if (this.m_hasLowPass)
        this.LowPassFilter.cutoffFrequency = this.GetLowPassOcclusionValue(this.transform.position, GM.CurrentPlayerBody.Head.position);
      if (!this.gameObject.activeSelf)
        this.gameObject.SetActive(true);
      this.Source.enabled = true;
      if (this.m_isBeingDestroyed)
        return;
      this.Source.Play();
    }

    public void Tick(float t)
    {
      if (!this.m_hasFollowTrans)
        return;
      if ((Object) this.m_followTrans == (Object) null)
        this.m_hasFollowTrans = false;
      else
        this.transform.position = this.m_followTrans.position;
    }

    public void FollowThisTransform(Transform t)
    {
      this.m_followTrans = t;
      if (!((Object) t != (Object) null))
        return;
      this.m_hasFollowTrans = true;
    }

    public void SetLowPassFreq(float freq)
    {
      if (!this.m_hasLowPass)
        return;
      this.LowPassFilter.cutoffFrequency = freq;
    }

    private float GetLowPassOcclusionValue(Vector3 start, Vector3 end)
    {
      if (!Physics.Linecast(start, end, (int) this.OcclusionLM, QueryTriggerInteraction.Ignore))
        return 22000f;
      float time = Vector3.Distance(start, end);
      this.Source.volume *= this.OcclusionVolumeCurve.Evaluate(time);
      return this.OcclusionFactorCurve.Evaluate(time);
    }

    private void OnDestroy() => this.m_isBeingDestroyed = true;
  }
}
