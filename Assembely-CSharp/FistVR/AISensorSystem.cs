using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class AISensorSystem : MonoBehaviour
	{
		public class SensorPing : IComparable<SensorPing>
		{
			public Transform Tr;

			public Rigidbody Rb;

			public float threat;

			public Vector3 LastKnownPosition = Vector3.zero;

			public float TimeLastSeen;

			public int CompareTo(SensorPing other)
			{
				if (other == null)
				{
					return 1;
				}
				if (threat > other.threat)
				{
					return -1;
				}
				if (threat < other.threat)
				{
					return 1;
				}
				return 0;
			}
		}

		public string IFFvalue;

		public SensorPing PriorityTarget;

		public SensorPing LastPriorityTarget;

		private HashSet<Transform> m_knownTargetsHash = new HashSet<Transform>();

		private Dictionary<Transform, SensorPing> m_knownTargetsDic = new Dictionary<Transform, SensorPing>();

		private HashSet<SensorPing> m_detectedHash = new HashSet<SensorPing>();

		private List<SensorPing> m_detectedTargets = new List<SensorPing>();

		private float m_timeTilDisregard = 6f;

		public int SensorTestCount = 1;

		public AISensor[] Sensors;

		private int m_current_vision_tests;

		private int m_current_sensor;

		private AudioSource m_aud;

		public AudioClip AudClip_NewSensorContact;

		public AudioClip AudClip_ContactLost;

		private float soundCooldown = 0.6f;

		public float soundCooldownReset = 1.5f;

		private void Awake()
		{
			m_aud = GetComponent<AudioSource>();
		}

		public void Regard(Transform t)
		{
			if (m_knownTargetsHash.Add(t))
			{
				SensorPing sensorPing = new SensorPing();
				sensorPing.Tr = t;
				if (t.gameObject.GetComponent<Rigidbody>() != null)
				{
					sensorPing.Rb = t.gameObject.GetComponent<Rigidbody>();
				}
				m_knownTargetsDic.Add(t, sensorPing);
			}
			if (m_detectedHash.Add(m_knownTargetsDic[t]))
			{
				m_detectedTargets.Add(m_knownTargetsDic[t]);
			}
			m_knownTargetsDic[t].LastKnownPosition = t.position;
			m_knownTargetsDic[t].TimeLastSeen = Time.time;
			m_knownTargetsDic[t].threat = 0f - Vector3.Distance(base.transform.position, t.position);
		}

		public void UpdateSensorSystem()
		{
			if (soundCooldown > 0f)
			{
				soundCooldown -= Time.deltaTime;
			}
			if (m_detectedTargets.Count > 0)
			{
				for (int num = m_detectedTargets.Count - 1; num >= 0; num--)
				{
					if (m_detectedTargets[num] == null || Time.time - m_detectedTargets[num].TimeLastSeen > m_timeTilDisregard)
					{
						m_detectedHash.Remove(m_detectedTargets[num]);
						m_detectedTargets.RemoveAt(num);
					}
				}
			}
			else
			{
				SetPriorityTarget(null);
			}
			if (m_detectedTargets.Count > 0)
			{
				m_detectedTargets.Sort();
				SetPriorityTarget(m_detectedTargets[0]);
			}
			while (m_current_sensor < Sensors.Length && m_current_vision_tests < SensorTestCount)
			{
				if (Sensors[m_current_sensor] != null && Sensors[m_current_sensor].SensorLoop())
				{
					m_current_vision_tests++;
				}
				m_current_sensor++;
			}
			if (m_current_sensor >= Sensors.Length)
			{
				m_current_sensor = 0;
			}
			m_current_vision_tests = 0;
			for (int i = 0; i < m_detectedTargets.Count; i++)
			{
				Debug.DrawLine(base.transform.position, m_detectedTargets[i].LastKnownPosition, Color.red);
			}
		}

		private void SetPriorityTarget(SensorPing ping)
		{
			if (PriorityTarget != null && ping == null)
			{
				if (soundCooldown <= 0f)
				{
					soundCooldown = soundCooldownReset;
					m_aud.PlayOneShot(AudClip_ContactLost, 1f);
				}
			}
			else if (ping != PriorityTarget && soundCooldown <= 0f)
			{
				soundCooldown = soundCooldownReset;
				m_aud.PlayOneShot(AudClip_NewSensorContact, 1f);
			}
			PriorityTarget = ping;
			if (ping != null)
			{
				LastPriorityTarget = ping;
			}
		}
	}
}
