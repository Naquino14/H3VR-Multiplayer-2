// Decompiled with JetBrains decompiler
// Type: FistVR.ExplosionSound
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ExplosionSound : MonoBehaviour
  {
    public AudioEvent AudioEvent_Explosion_Near;
    public AudioEvent AudioEvent_Explosion_Far;
    public FVRTailSoundClass NearTailClass = FVRTailSoundClass.Explosion;
    public FVRTailSoundClass DistantTailClass;
    public Vector2 TailPitchRangeNear = new Vector2(1f, 1f);
    public Vector2 TailPitchRangeFar = new Vector2(1f, 1f);
    public Vector2 TailPitchRangeDistant = new Vector2(1f, 1f);
    public bool FireOnStart;
    private bool m_waitingToExplode;
    private bool m_hasExploded;
    private float dist;
    private float delay;
    public float Loudness = 150f;
    public int IFF;

    private void Start()
    {
      if (!this.FireOnStart)
        return;
      this.Invoke("Explode", 0.05f);
    }

    public void CancelSound() => this.CancelInvoke("Explode");

    private void Explode()
    {
      this.dist = Vector3.Distance(GM.CurrentPlayerRoot.position, this.transform.position);
      this.delay = this.dist / 343f;
      this.ExplodeSound();
    }

    private void Update()
    {
    }

    public void ExplodeSound()
    {
      bool flag = SM.DoesReverbSystemExist();
      FVRReverbEnvironment reverbEnvironment = (FVRReverbEnvironment) null;
      FVRSoundEnvironment soundEnvironment;
      if (flag)
      {
        reverbEnvironment = SM.GetReverbEnvironment(this.transform.position);
        soundEnvironment = reverbEnvironment.Environment;
      }
      else
        soundEnvironment = SM.GetSoundEnvironment(this.transform.position);
      if ((double) this.dist < 20.0)
      {
        SM.PlayCoreSoundDelayed(FVRPooledAudioType.Explosion, this.AudioEvent_Explosion_Near, this.transform.position, this.delay);
        SM.AudioSourcePool corePool = SM.GetCorePool(FVRPooledAudioType.ExplosionTail);
        AudioEvent tailSet = SM.GetTailSet(this.NearTailClass, soundEnvironment);
        FVRPooledAudioSource pooledAudioSource = corePool.PlayClipVolumePitchOverride(tailSet, this.transform.position, 1f * tailSet.VolumeRange, this.TailPitchRangeNear);
        if (flag && (Object) reverbEnvironment == (Object) SM.GetPlayerReverbEnvironment() || flag)
          pooledAudioSource.SetLowPassFreq(22000f);
      }
      else if ((double) this.dist < 100.0)
      {
        SM.PlayCoreSoundDelayed(FVRPooledAudioType.Explosion, this.AudioEvent_Explosion_Far, this.transform.position, this.delay);
        SM.AudioSourcePool corePool = SM.GetCorePool(FVRPooledAudioType.ExplosionTail);
        AudioEvent tailSet = SM.GetTailSet(this.NearTailClass, soundEnvironment);
        float num = Mathf.Lerp(1f, 0.6f, (float) (((double) this.dist - 20.0) / 80.0));
        corePool.PlayDelayedClip(this.delay, tailSet, this.transform.position, num * tailSet.VolumeRange, this.TailPitchRangeFar);
      }
      else
      {
        SM.AudioSourcePool corePool = SM.GetCorePool(FVRPooledAudioType.ExplosionTail);
        AudioEvent tailSet = SM.GetTailSet(this.DistantTailClass, soundEnvironment);
        corePool.PlayDelayedClip(this.delay, tailSet, this.transform.position, tailSet.VolumeRange, this.TailPitchRangeDistant);
      }
      Vector3 position = this.transform.position;
      GM.CurrentSceneSettings.OnPerceiveableSound(this.Loudness, this.Loudness * SM.GetSoundTravelDistanceMultByEnvironment(soundEnvironment), position, this.IFF);
      GM.CurrentSceneSettings.OnSuppressingEvent(position, Vector3.up, this.IFF, Mathf.Clamp(this.Loudness * 0.01f, 1f, 2f), Mathf.Clamp(this.Loudness * 0.1f, 5f, 20f));
    }
  }
}
