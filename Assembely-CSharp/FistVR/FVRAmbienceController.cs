using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRAmbienceController : MonoBehaviour
	{
		public AudioSource AmbienceA;

		public AudioSource AmbienceB;

		private bool m_isTransitioning;

		private bool m_towardsA = true;

		private float targetVolumeA;

		private float targetVolumeB;

		public List<AudioClip> AmbienceClips = new List<AudioClip>();

		public List<float> AmbienceVolumes = new List<float>();

		public bool UsesAmbienceVolumes;

		public float MaxVolume = 0.3f;

		public List<FVRAmbienceZone> Zones = new List<FVRAmbienceZone>();

		public int PlayerAmbientIndex;

		private int m_startingIndex;

		public float TransitionSpeed = 2f;

		private void Start()
		{
			m_startingIndex = PlayerAmbientIndex;
			AmbienceA.clip = AmbienceClips[PlayerAmbientIndex];
			targetVolumeA = MaxVolume;
			AmbienceA.Play();
		}

		private void Update()
		{
			int ambientIndex = GetAmbientIndex(GM.CurrentPlayerBody.Head.position);
			if (ambientIndex != PlayerAmbientIndex)
			{
				TransitionToAmbientIndex(ambientIndex);
			}
			AmbienceA.volume = Mathf.MoveTowards(AmbienceA.volume, targetVolumeA, Time.deltaTime * TransitionSpeed);
			AmbienceB.volume = Mathf.MoveTowards(AmbienceB.volume, targetVolumeB, Time.deltaTime * TransitionSpeed);
			if (AmbienceA.volume <= 0f || AmbienceB.volume <= 0f)
			{
				m_isTransitioning = false;
			}
			if (AmbienceA.volume <= 0f && AmbienceA.isPlaying)
			{
				AmbienceA.Stop();
			}
			if (AmbienceB.volume <= 0f && AmbienceB.isPlaying)
			{
				AmbienceB.Stop();
			}
		}

		private void TransitionToAmbientIndex(int i)
		{
			if (m_isTransitioning)
			{
				return;
			}
			PlayerAmbientIndex = i;
			m_isTransitioning = true;
			if (m_towardsA)
			{
				m_towardsA = false;
			}
			else
			{
				m_towardsA = true;
			}
			if (m_towardsA)
			{
				targetVolumeA = MaxVolume;
				if (UsesAmbienceVolumes)
				{
					targetVolumeA = AmbienceVolumes[i];
				}
				targetVolumeB = 0f;
				AmbienceA.clip = AmbienceClips[i];
				AmbienceA.Play();
			}
			else
			{
				targetVolumeB = MaxVolume;
				if (UsesAmbienceVolumes)
				{
					targetVolumeB = AmbienceVolumes[i];
				}
				targetVolumeA = 0f;
				AmbienceB.clip = AmbienceClips[i];
				AmbienceB.Play();
			}
		}

		public bool TestVolumeBool(FVRAmbienceZone z, Vector3 pos)
		{
			bool result = true;
			Vector3 vector = z.t.InverseTransformPoint(pos);
			if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f || Mathf.Abs(vector.z) > 0.5f)
			{
				result = false;
			}
			return result;
		}

		public int GetAmbientIndex(Vector3 pos)
		{
			int num = 100;
			bool flag = false;
			for (int i = 0; i < Zones.Count; i++)
			{
				if (Zones[i].ZoneIndex < num)
				{
					if (TestVolumeBool(Zones[i], pos))
					{
						num = Zones[i].ZoneIndex;
						flag = true;
					}
					if (num <= 0)
					{
						break;
					}
				}
			}
			if (flag)
			{
				return num;
			}
			return m_startingIndex;
		}
	}
}
