using UnityEngine;

namespace FistVR
{
	public class FVROpticLaser : MonoBehaviour
	{
		public Transform SightLine;

		public LayerMask CastLayer;

		public Transform Laser;

		public Transform LaserDot;

		private bool m_isOn;

		private RaycastHit m_hit;

		public bool IsOn => m_isOn;

		private void Awake()
		{
			Laser.gameObject.SetActive(value: false);
			LaserDot.gameObject.SetActive(value: false);
		}

		public void TurnLaserOn()
		{
			m_isOn = true;
		}

		public void TurnLaserOff()
		{
			m_isOn = false;
		}

		private void Update()
		{
			if (!m_isOn)
			{
				Laser.gameObject.SetActive(value: false);
				LaserDot.gameObject.SetActive(value: false);
				return;
			}
			Laser.gameObject.SetActive(value: true);
			LaserDot.gameObject.SetActive(value: true);
			Vector3 vector = ((!Physics.Raycast(SightLine.position, SightLine.forward, out m_hit, 100f, CastLayer)) ? (SightLine.position + SightLine.forward * 100f) : m_hit.point);
			Laser.LookAt(vector, Vector3.up);
			if (Physics.Raycast(Laser.position, Laser.forward, out m_hit, 100f, CastLayer))
			{
				Laser.localScale = new Vector3(1f, 1f, m_hit.distance * 0.7692308f);
				LaserDot.position = m_hit.point;
			}
			else
			{
				Laser.localScale = new Vector3(1f, 1f, 100f);
				LaserDot.position = vector;
			}
		}
	}
}
