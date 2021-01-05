using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MF_Zone : MonoBehaviour
	{
		public MF_ZoneCategory Cat;

		public bool DrawGiz;

		public Transform Point_CapturePointPlinth;

		public List<MF_ZonePoint> TargetPoints_Assault;

		public List<MF_ZonePoint> TargetPoints_Support;

		public List<MF_ZonePoint> TargetPoints_Sniping;

		private MF_CapturePoint m_capturePoint;

		public MF_CapturePoint GetCapturePoint()
		{
			return m_capturePoint;
		}

		public MF_CapturePoint SpawnCapturePoint(GameObject prefab)
		{
			GameObject gameObject = Object.Instantiate(prefab, Point_CapturePointPlinth.position, Point_CapturePointPlinth.rotation);
			m_capturePoint = gameObject.GetComponent<MF_CapturePoint>();
			m_capturePoint.SetZone(this);
			return m_capturePoint;
		}

		public Transform GetTargetPointByClass(MF_Class c)
		{
			switch (c)
			{
			case MF_Class.Scout:
			case MF_Class.Spy:
			case MF_Class.Engineer:
			case MF_Class.Pyro:
				return TargetPoints_Assault[Random.Range(0, TargetPoints_Assault.Count)].transform;
			case MF_Class.Medic:
			case MF_Class.Demoman:
			case MF_Class.Soldier:
			case MF_Class.Heavy:
				return TargetPoints_Support[Random.Range(0, TargetPoints_Support.Count)].transform;
			case MF_Class.Sniper:
				return TargetPoints_Sniping[Random.Range(0, TargetPoints_Sniping.Count)].transform;
			default:
				return TargetPoints_Assault[Random.Range(0, TargetPoints_Assault.Count)].transform;
			}
		}

		private void OnDrawGizmos()
		{
			if (DrawGiz)
			{
				Gizmos.color = new Color(1f, 1f, 1f, 1f);
				Gizmos.DrawSphere(base.transform.position, 0.25f);
				Gizmos.DrawWireSphere(base.transform.position, 0.25f);
			}
		}
	}
}
