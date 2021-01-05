using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigMusicVolumeTester : MonoBehaviour
	{
		public ZosigGameManager M;

		public List<ZosigMusicVolume> VolumeList = new List<ZosigMusicVolume>();

		private ZosigMusicVolume m_curVolume;

		private int m_startingIndex;

		private void Start()
		{
			m_curVolume = VolumeList[0];
		}

		private void Update()
		{
			m_startingIndex++;
			if (m_startingIndex >= VolumeList.Count)
			{
				m_startingIndex = 0;
			}
			if (TestVolumeBool(VolumeList[m_startingIndex], GM.CurrentPlayerBody.Head.position))
			{
				m_curVolume = VolumeList[m_startingIndex];
				M.SetMusicTrack(m_curVolume.TrackName);
			}
		}

		public bool TestVolumeBool(ZosigMusicVolume z, Vector3 pos)
		{
			bool result = true;
			Vector3 vector = z.transform.InverseTransformPoint(pos);
			if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f || Mathf.Abs(vector.z) > 0.5f)
			{
				result = false;
			}
			return result;
		}
	}
}
