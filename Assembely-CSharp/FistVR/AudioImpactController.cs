using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class AudioImpactController : MonoBehaviour
	{
		[Serializable]
		public class AltImpactType
		{
			public ImpactType Type;

			public List<Collider> Cols;
		}

		[Header("Impact Stuff")]
		public ImpactType ImpactType = ImpactType.Generic;

		public bool SoundOnRB = true;

		public bool SoundOnNonRb = true;

		private float m_tickTilCollisionSound = 0.1f;

		public float DelayLengthMult = 1f;

		public float DistLimit = 25f;

		public FVRPooledAudioType PoolToUse = FVRPooledAudioType.Impacts;

		public float HitThreshold_High = 2f;

		public float HitThreshold_Medium = 1f;

		public float HitThreshold_Ignore = 0.25f;

		private bool m_hasPlayedAudioThisFrame;

		public List<Rigidbody> IgnoreRBs = new List<Rigidbody>();

		[Header("Sonic Event Stuff")]
		public bool CausesSonicEventOnSoundPlay;

		public float BaseLoudness = 1f;

		public float LoudnessVelocityMult = 1f;

		public float MaxLoudness = 10f;

		private int m_iFFForSonicEvent = -3;

		[Header("Alternate Type Stuff")]
		public bool UsesAltTypes;

		public List<AltImpactType> Alts;

		public void SetIFF(int i)
		{
			m_iFFForSonicEvent = i;
		}

		private void Update()
		{
			if (m_tickTilCollisionSound > 0f)
			{
				m_tickTilCollisionSound -= Time.deltaTime;
			}
			m_hasPlayedAudioThisFrame = false;
		}

		private void OnCollisionEnter(Collision col)
		{
			if (m_tickTilCollisionSound <= 0f)
			{
				ProcessCollision(col);
			}
		}

		public void SetCollisionsTickDown(float f)
		{
			m_tickTilCollisionSound = f;
		}

		public void SetCollisionsTickDownMax(float f)
		{
			m_tickTilCollisionSound = Mathf.Max(m_tickTilCollisionSound, f);
		}

		private void ProcessCollision(Collision col)
		{
			if (m_hasPlayedAudioThisFrame)
			{
				return;
			}
			bool flag = false;
			if (col.collider.attachedRigidbody != null)
			{
				flag = true;
			}
			bool flag2 = false;
			if (flag && SoundOnRB)
			{
				flag2 = true;
			}
			if (!flag && SoundOnNonRb)
			{
				flag2 = true;
			}
			if (!flag2 || (flag && IgnoreRBs.Contains(col.collider.attachedRigidbody)))
			{
				return;
			}
			m_hasPlayedAudioThisFrame = true;
			float magnitude = col.relativeVelocity.magnitude;
			if (magnitude < HitThreshold_Ignore)
			{
				return;
			}
			AudioImpactIntensity impactIntensity = AudioImpactIntensity.Light;
			if (magnitude > HitThreshold_High)
			{
				impactIntensity = AudioImpactIntensity.Hard;
			}
			else if (magnitude > HitThreshold_Medium)
			{
				impactIntensity = AudioImpactIntensity.Medium;
			}
			MatSoundType matSoundType = MatSoundType.HardSurface;
			PMat component = col.collider.transform.GetComponent<PMat>();
			if (component == null && flag)
			{
				component = col.collider.attachedRigidbody.transform.GetComponent<PMat>();
			}
			if (component != null && component.MatDef != null)
			{
				matSoundType = component.MatDef.SoundType;
			}
			ImpactType impacttype = ImpactType;
			if (UsesAltTypes)
			{
				Collider thisCollider = col.contacts[0].thisCollider;
				bool flag3 = false;
				for (int i = 0; i < Alts.Count; i++)
				{
					if (flag3)
					{
						break;
					}
					for (int j = 0; j < Alts[i].Cols.Count; j++)
					{
						if (Alts[i].Cols[j] == thisCollider)
						{
							impacttype = Alts[i].Type;
							flag3 = true;
						}
						if (flag3)
						{
							break;
						}
					}
				}
			}
			if (magnitude > HitThreshold_Medium)
			{
				m_tickTilCollisionSound = SM.PlayImpactSound(impacttype, matSoundType, impactIntensity, base.transform.position, PoolToUse, DistLimit) * DelayLengthMult;
			}
			else
			{
				m_tickTilCollisionSound = SM.PlayImpactSound(impacttype, matSoundType, impactIntensity, base.transform.position, PoolToUse, DistLimit, magnitude / HitThreshold_Medium, 1f) * DelayLengthMult;
			}
			if (CausesSonicEventOnSoundPlay && m_tickTilCollisionSound >= 0f)
			{
				float num = Mathf.Clamp(BaseLoudness + magnitude * LoudnessVelocityMult, 0f, MaxLoudness) * SM.GetImpactSoundVolumeMultFromMaterial(matSoundType);
				GM.CurrentSceneSettings.OnPerceiveableSound(num, Mathf.Clamp(num, 0f, GM.CurrentSceneSettings.MaxImpactSoundEventDistance), base.transform.position, m_iFFForSonicEvent);
			}
			if (m_tickTilCollisionSound < 0f)
			{
				m_tickTilCollisionSound = 0f;
			}
		}
	}
}
