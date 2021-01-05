using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class AIEntity : MonoBehaviour
	{
		public delegate void AIEventReceive(AIEvent e);

		public delegate void AIReceiveSuppression(Vector3 pos, Vector3 dir, int iffcode, float intensity, float range);

		public int IFFCode = -1;

		public bool ReceivesEvent_Damage;

		public float ManagerCheckFrequency = 0.2f;

		private float m_checkTick;

		[Header("Core Connections")]
		public Transform FacingTransform;

		private bool m_hasFacingTransform;

		public Transform GroundTransform;

		private bool m_hasGroundTransform;

		public List<AIEntityIFFBeacon> Beacons;

		public bool UsesFakedPosition;

		public Vector3 FakePos;

		[Header("Visual System")]
		public bool ReceivesEvent_Visual;

		public float MaximumSightRange = 100f;

		public float MaximumSightFOV = 45f;

		public AnimationCurve SightDistanceByFOVMultiplier;

		public bool IsVisualCheckOmni;

		public Transform SensoryFrame;

		public LayerMask LM_VisualOcclusionCheck;

		[Header("Sonic System")]
		public bool ReceivesEvent_Sonic;

		public float SonicThreshold;

		public float MaxHearingDistance = 300f;

		[Header("Reported Values")]
		public float DangerMultiplier = 1f;

		public float VisibilityMultiplier = 1f;

		public float MaxDistanceVisibleFrom = 300f;

		public event AIEventReceive AIEventReceiveEvent;

		public event AIReceiveSuppression AIReceiveSuppressionEvent;

		public void Start()
		{
			if (SensoryFrame == null)
			{
				SensoryFrame = base.transform;
			}
			if (FacingTransform != null)
			{
				m_hasFacingTransform = true;
			}
			if (GroundTransform != null)
			{
				m_hasGroundTransform = true;
			}
			m_checkTick = Random.Range(0f, ManagerCheckFrequency);
			if (GM.CurrentAIManager != null)
			{
				GM.CurrentAIManager.RegisterAIEntity(this);
			}
		}

		public void OnDestroy()
		{
			if (GM.CurrentAIManager != null)
			{
				GM.CurrentAIManager.DeRegisterAIEntity(this);
			}
		}

		public bool ReadyForManagerCheck()
		{
			if (m_checkTick <= 0f)
			{
				return true;
			}
			return false;
		}

		public Vector3 GetPos()
		{
			if (UsesFakedPosition)
			{
				return FakePos;
			}
			return base.transform.position;
		}

		public Vector3 GetGroundPos()
		{
			if (m_hasGroundTransform)
			{
				return GroundTransform.position;
			}
			return base.transform.position;
		}

		public Vector3 GetThreatFacing()
		{
			if (m_hasFacingTransform)
			{
				return FacingTransform.forward;
			}
			return base.transform.forward;
		}

		public void Tick(float t)
		{
			if (m_checkTick >= 0f)
			{
				m_checkTick -= t;
			}
			if (Beacons.Count > 0)
			{
				for (int i = 0; i < Beacons.Count; i++)
				{
					if (Beacons[i] != null && Beacons[i].IFF != IFFCode)
					{
						Beacons[i].IFF = IFFCode;
					}
				}
			}
			if (ReceivesEvent_Sonic)
			{
				SonicThreshold = Mathf.MoveTowards(SonicThreshold, 0f, t * GM.CurrentAIManager.SonicThresholdDecayCurve.Evaluate(SonicThreshold));
			}
		}

		public void ResetTick()
		{
			m_checkTick = ManagerCheckFrequency;
		}

		public void ProcessLoudness(float loudness)
		{
			if (loudness >= SonicThreshold)
			{
				SonicThreshold = Random.Range(loudness * 1.1f, loudness * 1.25f);
			}
			SonicThreshold = Mathf.Clamp(SonicThreshold, 0f, 200f);
		}

		public void OnAIEventReceive(AIEvent e)
		{
			if (this.AIEventReceiveEvent != null)
			{
				this.AIEventReceiveEvent(e);
			}
		}

		public void OnAIReceiveSuppression(Vector3 pos, Vector3 dir, int iffcode, float intensity, float range)
		{
			if (this.AIReceiveSuppressionEvent != null)
			{
				this.AIReceiveSuppressionEvent(pos, dir, iffcode, intensity, range);
			}
		}
	}
}
