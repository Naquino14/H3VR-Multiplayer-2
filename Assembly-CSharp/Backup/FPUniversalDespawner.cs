// Decompiled with JetBrains decompiler
// Type: FPUniversalDespawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class FPUniversalDespawner : MonoBehaviour, IFastPoolItem
{
  [SerializeField]
  private int targetPoolID;
  [SerializeField]
  private bool despawnDelayed;
  [SerializeField]
  private float delay;
  [SerializeField]
  private bool despawnOnParticlesDead;
  [SerializeField]
  private bool resetParticleSystem;
  [SerializeField]
  private bool despawnOnAudioSourceStop;
  private bool needCheck;
  private AudioSource aSource;
  private ParticleSystem pSystem;

  public int TargetPoolID
  {
    get => this.targetPoolID;
    set => this.targetPoolID = value;
  }

  public bool DespawnDelayed => this.despawnDelayed;

  public float Delay => this.delay;

  public bool DespawnOnParticlesDead => this.despawnOnParticlesDead;

  public bool ResetParticleSystem => this.resetParticleSystem;

  public bool DespawnOnAudioSourceStop => this.despawnOnAudioSourceStop;

  private void Start()
  {
    if (this.despawnDelayed)
      this.StartCoroutine(this.Despawn(this.delay));
    if (this.despawnOnAudioSourceStop)
    {
      this.aSource = this.GetComponentInChildren<AudioSource>();
      this.needCheck = true;
    }
    if (this.despawnOnParticlesDead)
    {
      this.pSystem = this.GetComponentInChildren<ParticleSystem>();
      this.needCheck = true;
    }
    if (!this.needCheck)
      return;
    this.StartCoroutine(this.CheckAlive());
  }

  public void OnFastInstantiate()
  {
    if (this.despawnDelayed)
      this.StartCoroutine(this.Despawn(this.delay));
    if (this.needCheck)
      this.StartCoroutine(this.CheckAlive());
    if (!this.despawnOnParticlesDead || !((Object) this.pSystem != (Object) null) || !this.resetParticleSystem)
      return;
    this.pSystem.Play(true);
  }

  public void OnFastDestroy()
  {
    this.StopAllCoroutines();
    if (!this.despawnOnParticlesDead || !((Object) this.pSystem != (Object) null) || !this.resetParticleSystem)
      return;
    this.pSystem.Clear(true);
  }

  [DebuggerHidden]
  private IEnumerator Despawn(float despawn_delay) => (IEnumerator) new FPUniversalDespawner.\u003CDespawn\u003Ec__Iterator0()
  {
    despawn_delay = despawn_delay,
    \u0024this = this
  };

  [DebuggerHidden]
  private IEnumerator CheckAlive() => (IEnumerator) new FPUniversalDespawner.\u003CCheckAlive\u003Ec__Iterator1()
  {
    \u0024this = this
  };
}
