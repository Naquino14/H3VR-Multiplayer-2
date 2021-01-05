using UnityEngine;

namespace FistVR
{
	public class MG_LaserMine : MonoBehaviour, IFVRDamageable
	{
		private bool m_isExploded;

		public GameObject[] Spawns;

		public Transform SpawnPoint;

		public GameObject LaserBeam;

		public AnimationCurve[] ActivityCurves;

		private int m_curveIndex;

		private bool m_isActive = true;

		private float m_cycleTime = 3f;

		private float m_cycleTick;

		public void Awake()
		{
			m_curveIndex = Random.Range(0, ActivityCurves.Length - 1);
			m_cycleTime = Random.Range(5f, 8f);
			m_cycleTick = Random.Range(0f, 3f);
		}

		public void Update()
		{
			m_cycleTick += Time.deltaTime;
			if (m_cycleTick > m_cycleTime)
			{
				m_cycleTick -= m_cycleTime;
			}
			float num = ActivityCurves[m_curveIndex].Evaluate(m_cycleTick / m_cycleTime);
			if (num >= 0.5f)
			{
				if (!LaserBeam.activeSelf)
				{
					LaserBeam.SetActive(value: true);
				}
			}
			else if (LaserBeam.activeSelf)
			{
				LaserBeam.SetActive(value: false);
			}
		}

		public void Explode()
		{
			if (!m_isExploded)
			{
				m_isExploded = true;
				for (int i = 0; i < Spawns.Length; i++)
				{
					Object.Instantiate(Spawns[i], SpawnPoint.position, SpawnPoint.rotation);
				}
				Object.Destroy(base.gameObject);
			}
		}

		public void Damage(Damage d)
		{
			Explode();
		}
	}
}
