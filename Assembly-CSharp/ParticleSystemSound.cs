// Decompiled with JetBrains decompiler
// Type: ParticleSystemSound
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ParticleSystemSound : MonoBehaviour
{
  public AudioClip[] _shootSound;
  public float _shootPitchMax = 1.25f;
  public float _shootPitchMin = 0.75f;
  public float _shootVolumeMax = 0.75f;
  public float _shootVolumeMin = 0.25f;
  public AudioClip[] _explosionSound;
  public float _explosionPitchMax = 1.25f;
  public float _explosionPitchMin = 0.75f;
  public float _explosionVolumeMax = 0.75f;
  public float _explosionVolumeMin = 0.25f;
  public AudioClip[] _crackleSound;
  public float _crackleDelay = 0.25f;
  public int _crackleMultiplier = 3;
  public float _cracklePitchMax = 1.25f;
  public float _cracklePitchMin = 0.75f;
  public float _crackleVolumeMax = 0.75f;
  public float _crackleVolumeMin = 0.25f;

  public void LateUpdate()
  {
    ParticleSystem.Particle[] particles1 = new ParticleSystem.Particle[this.GetComponent<ParticleSystem>().particleCount];
    int particles2 = this.GetComponent<ParticleSystem>().GetParticles(particles1);
    for (int index1 = 0; index1 < particles2; ++index1)
    {
      if (this._explosionSound.Length > 0 && (double) particles1[index1].remainingLifetime < (double) Time.deltaTime)
      {
        SoundController.instance.Play(this._explosionSound[Random.Range(0, this._explosionSound.Length)], Random.Range(this._explosionVolumeMax, this._explosionVolumeMin), Random.Range(this._explosionPitchMin, this._explosionPitchMax), particles1[index1].position);
        if (this._crackleSound.Length > 0)
        {
          for (int index2 = 0; index2 < this._crackleMultiplier; ++index2)
            this.StartCoroutine(this.Crackle(particles1[index1].position, this._crackleDelay + (float) index2 * 0.1f));
        }
      }
      if (this._shootSound.Length > 0 && (double) particles1[index1].remainingLifetime >= (double) particles1[index1].startLifetime - (double) Time.deltaTime)
        SoundController.instance.Play(this._shootSound[Random.Range(0, this._shootSound.Length)], Random.Range(this._shootVolumeMax, this._shootVolumeMin), Random.Range(this._shootPitchMin, this._shootPitchMax), particles1[index1].position);
    }
  }

  [DebuggerHidden]
  public IEnumerator Crackle(Vector3 pos, float delay) => (IEnumerator) new ParticleSystemSound.\u003CCrackle\u003Ec__Iterator0()
  {
    delay = delay,
    pos = pos,
    \u0024this = this
  };
}
