using UnityEngine;

namespace FistVR
{
	public class OmniSpawner : MonoBehaviour
	{
		public enum SpawnerState
		{
			Deactivated,
			Deactivating,
			Activated,
			Activating
		}

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

		protected SpawnerState m_state;

		protected int points;

		public AudioSource AudSource;

		public AudioClip AudClip_Success;

		public AudioClip AudClip_Failure;

		public AudioClip AudClip_Spawn;

		public virtual void Configure(OmniSpawnDef Def, OmniWaveEngagementRange Range)
		{
			m_isConfigured = true;
			m_range = Range;
			m_startPos = base.transform.position;
			m_endPos = base.transform.position;
			m_endPos.y = TargetY;
		}

		public virtual void BeginSpawning()
		{
		}

		public virtual void EndSpawning()
		{
		}

		public virtual void Activate()
		{
			m_state = SpawnerState.Activating;
		}

		public virtual int Deactivate()
		{
			m_state = SpawnerState.Deactivating;
			return points;
		}

		protected virtual void Activating()
		{
			if (m_activationLerp < 1f)
			{
				m_activationLerp += Time.deltaTime * DeploySpeed;
			}
			else
			{
				m_activationLerp = 1f;
				m_state = SpawnerState.Activated;
			}
			base.transform.position = Vector3.Lerp(m_startPos, m_endPos, m_activationLerp);
		}

		protected virtual void Deactivating()
		{
			if (m_activationLerp > 0f)
			{
				m_activationLerp -= Time.deltaTime * DeploySpeed;
			}
			else
			{
				m_activationLerp = 0f;
				m_state = SpawnerState.Deactivated;
				Object.Destroy(base.gameObject);
			}
			base.transform.position = Vector3.Lerp(m_startPos, m_endPos, m_activationLerp);
		}

		public virtual bool IsConfigured()
		{
			return m_isConfigured;
		}

		public virtual bool IsDoneSpawning()
		{
			return m_isDoneSpawning;
		}

		public virtual bool IsReadyForWaveEnd()
		{
			return m_isReadyForWaveEnd;
		}

		public virtual SpawnerState GetState()
		{
			return m_state;
		}

		public void AddPoints(int p)
		{
			float num = ScoreMultiplier * (float)p;
			points += (int)num;
		}

		public void PlaySpawnSound()
		{
			if (!AudSource.isPlaying)
			{
				AudSource.clip = AudClip_Spawn;
				AudSource.Play();
			}
		}

		public void PlaySuccessSound()
		{
			AudSource.pitch = Random.Range(0.92f, 1.08f);
			AudSource.PlayOneShot(AudClip_Success, 1f);
		}

		public void PlayFailureSound()
		{
			AudSource.pitch = Random.Range(0.92f, 1.08f);
			AudSource.PlayOneShot(AudClip_Failure, 1f);
		}

		public OmniWaveEngagementRange GetEngagementRange()
		{
			return m_range;
		}

		protected float GetRange()
		{
			return m_range switch
			{
				OmniWaveEngagementRange.m5 => 5f, 
				OmniWaveEngagementRange.m10 => 10f, 
				OmniWaveEngagementRange.m15 => 15f, 
				OmniWaveEngagementRange.m20 => 20f, 
				OmniWaveEngagementRange.m25 => 25f, 
				OmniWaveEngagementRange.m50 => 50f, 
				OmniWaveEngagementRange.m100 => 100f, 
				OmniWaveEngagementRange.m150 => 150f, 
				OmniWaveEngagementRange.m200 => 200f, 
				_ => 0f, 
			};
		}
	}
}
