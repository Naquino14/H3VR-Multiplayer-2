// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OmniSpawner : MonoBehaviour
  {
    protected OmniWaveEngagementRange m_range;
    public float ScoreMultiplier = 1f;
    public float TargetY = -10f;
    public float DeploySpeed = 1f;
    protected Vector3 m_startPos;
    protected Vector3 m_endPos;
    protected float m_activationLerp;
    protected bool m_isConfigured;
    protected bool m_isDoneSpawning;
    protected bool m_isReadyForWaveEnd;
    protected OmniSpawner.SpawnerState m_state;
    protected int points;
    public AudioSource AudSource;
    public AudioClip AudClip_Success;
    public AudioClip AudClip_Failure;
    public AudioClip AudClip_Spawn;

    public virtual void Configure(OmniSpawnDef Def, OmniWaveEngagementRange Range)
    {
      this.m_isConfigured = true;
      this.m_range = Range;
      this.m_startPos = this.transform.position;
      this.m_endPos = this.transform.position;
      this.m_endPos.y = this.TargetY;
    }

    public virtual void BeginSpawning()
    {
    }

    public virtual void EndSpawning()
    {
    }

    public virtual void Activate() => this.m_state = OmniSpawner.SpawnerState.Activating;

    public virtual int Deactivate()
    {
      this.m_state = OmniSpawner.SpawnerState.Deactivating;
      return this.points;
    }

    protected virtual void Activating()
    {
      if ((double) this.m_activationLerp < 1.0)
      {
        this.m_activationLerp += Time.deltaTime * this.DeploySpeed;
      }
      else
      {
        this.m_activationLerp = 1f;
        this.m_state = OmniSpawner.SpawnerState.Activated;
      }
      this.transform.position = Vector3.Lerp(this.m_startPos, this.m_endPos, this.m_activationLerp);
    }

    protected virtual void Deactivating()
    {
      if ((double) this.m_activationLerp > 0.0)
      {
        this.m_activationLerp -= Time.deltaTime * this.DeploySpeed;
      }
      else
      {
        this.m_activationLerp = 0.0f;
        this.m_state = OmniSpawner.SpawnerState.Deactivated;
        Object.Destroy((Object) this.gameObject);
      }
      this.transform.position = Vector3.Lerp(this.m_startPos, this.m_endPos, this.m_activationLerp);
    }

    public virtual bool IsConfigured() => this.m_isConfigured;

    public virtual bool IsDoneSpawning() => this.m_isDoneSpawning;

    public virtual bool IsReadyForWaveEnd() => this.m_isReadyForWaveEnd;

    public virtual OmniSpawner.SpawnerState GetState() => this.m_state;

    public void AddPoints(int p) => this.points += (int) (this.ScoreMultiplier * (float) p);

    public void PlaySpawnSound()
    {
      if (this.AudSource.isPlaying)
        return;
      this.AudSource.clip = this.AudClip_Spawn;
      this.AudSource.Play();
    }

    public void PlaySuccessSound()
    {
      this.AudSource.pitch = Random.Range(0.92f, 1.08f);
      this.AudSource.PlayOneShot(this.AudClip_Success, 1f);
    }

    public void PlayFailureSound()
    {
      this.AudSource.pitch = Random.Range(0.92f, 1.08f);
      this.AudSource.PlayOneShot(this.AudClip_Failure, 1f);
    }

    public OmniWaveEngagementRange GetEngagementRange() => this.m_range;

    protected float GetRange()
    {
      switch (this.m_range)
      {
        case OmniWaveEngagementRange.m5:
          return 5f;
        case OmniWaveEngagementRange.m10:
          return 10f;
        case OmniWaveEngagementRange.m15:
          return 15f;
        case OmniWaveEngagementRange.m20:
          return 20f;
        case OmniWaveEngagementRange.m25:
          return 25f;
        case OmniWaveEngagementRange.m50:
          return 50f;
        case OmniWaveEngagementRange.m100:
          return 100f;
        case OmniWaveEngagementRange.m150:
          return 150f;
        case OmniWaveEngagementRange.m200:
          return 200f;
        default:
          return 0.0f;
      }
    }

    public enum SpawnerState
    {
      Deactivated,
      Deactivating,
      Activated,
      Activating,
    }
  }
}
