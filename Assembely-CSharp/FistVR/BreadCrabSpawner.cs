using UnityEngine;

namespace FistVR
{
	public class BreadCrabSpawner : SosigWearable, IFVRDamageable
	{
		public float m_lifeLeft = 2500f;

		private bool m_isDead;

		public FVRObject BreadCrab;

		public Transform BreadCrabSpawnPoint;

		private bool m_hasSpawned;

		public override void Damage(Damage d)
		{
			m_lifeLeft -= d.Dam_TotalKinetic;
			if (m_lifeLeft <= 0f && !m_isDead)
			{
				m_isDead = true;
				S.DestroyLink(S.Links[0], d.Class);
			}
			base.Damage(d);
		}

		private void Update()
		{
			SpawnCheck();
		}

		private void SpawnCheck()
		{
			if (!m_isDead && !m_hasSpawned && !(S == null) && S.BodyState == Sosig.SosigBodyState.Dead && S.Links[0] != null)
			{
				m_hasSpawned = true;
				L.DeRegisterWearable(this);
				Object.Instantiate(BreadCrab.GetGameObject(), BreadCrabSpawnPoint.position, BreadCrabSpawnPoint.rotation);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
