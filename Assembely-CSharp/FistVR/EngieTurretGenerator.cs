using UnityEngine;

namespace FistVR
{
	public class EngieTurretGenerator : SosigWearable
	{
		[Header("Turret Spawning")]
		public FVRObject TurretObj;

		private AutoMeater m_spawnedTurret;

		private float m_timeToAllowTurretSpawn;

		public LayerMask LM_PlaceBlocker;

		public override void Start()
		{
			m_timeToAllowTurretSpawn = Random.Range(5f, 20f);
		}

		private void Update()
		{
			if (L != null)
			{
				if (m_timeToAllowTurretSpawn > 0f)
				{
					m_timeToAllowTurretSpawn -= Time.deltaTime;
				}
				else if (S.CurrentOrder == Sosig.SosigOrder.Skirmish)
				{
					m_timeToAllowTurretSpawn = Random.Range(1f, 3f);
					PlaceCheck();
				}
			}
		}

		private void PlaceCheck()
		{
			if (!(m_spawnedTurret != null))
			{
				Vector3 position = S.transform.position + S.transform.forward + S.transform.up;
				if (!Physics.CheckSphere(position, 0.3f, LM_PlaceBlocker))
				{
					GameObject gameObject = Object.Instantiate(TurretObj.GetGameObject(), position, Quaternion.LookRotation(S.transform.forward, S.transform.up));
					m_spawnedTurret = gameObject.GetComponent<AutoMeater>();
				}
			}
		}
	}
}
