using UnityEngine;

namespace FistVR
{
	public class DestructibleChunk : MonoBehaviour, IFVRDamageable
	{
		public DestructibleChunkProfile Profile;

		public MeshFilter MeshFilter;

		public MeshCollider MeshCollider;

		private int m_chunkIndex;

		private bool m_scalesSpawns;

		private int m_currentDestructionRenderer;

		private int m_maxDestructionRenderer;

		private float m_currentLife;

		private float m_startingLife;

		private float m_damageCutoff;

		private bool m_isDestroyed;

		private void Awake()
		{
			Configure();
		}

		private void Configure()
		{
			m_currentLife = Profile.TotalLife;
			m_startingLife = Profile.TotalLife;
			m_damageCutoff = Profile.DamageCutoff;
			m_chunkIndex = Random.Range(0, Profile.MaxRandomIndex + 1);
			m_currentDestructionRenderer = 0;
			m_maxDestructionRenderer = Profile.DGeoStages.Count - 1;
			m_scalesSpawns = Profile.ScalesSpawns;
		}

		public void Damage(Damage d)
		{
			float value = d.Dam_TotalKinetic - Mathf.Abs(m_damageCutoff);
			value = Mathf.Clamp(value, 0f, d.Dam_TotalKinetic);
			m_currentLife -= value;
			UpdateDestruction();
		}

		private void UpdateDestruction()
		{
			if (m_isDestroyed)
			{
				return;
			}
			bool flag = false;
			if (m_currentLife <= 0f)
			{
				m_isDestroyed = true;
				if (Profile.IsDestroyedOnZeroLife)
				{
					flag = true;
				}
			}
			if (m_isDestroyed && Profile.SpawnsOnDestruction)
			{
				GameObject gameObject = Object.Instantiate(Profile.SpawnOnDestruction, base.transform.position, base.transform.rotation);
				if (m_scalesSpawns)
				{
					float num = gameObject.transform.localScale.x * base.transform.localScale.x;
					gameObject.transform.localScale = new Vector3(num, num, num);
				}
			}
			if (flag)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			float num2 = 1f - m_currentLife / m_startingLife;
			num2 *= (float)m_maxDestructionRenderer;
			int value = Mathf.RoundToInt(num2);
			value = Mathf.Clamp(value, 0, m_maxDestructionRenderer);
			if (m_currentDestructionRenderer != value)
			{
				m_currentDestructionRenderer = value;
				MeshFilter.mesh = Profile.DGeoStages[m_currentDestructionRenderer].GetMesh(m_chunkIndex);
				MeshCollider.sharedMesh = Profile.DGeoStages[m_currentDestructionRenderer].GetMesh(m_chunkIndex);
				if (Profile.DGeoStages[m_currentDestructionRenderer].SpawnsOnEnterIndex)
				{
					GameObject gameObject2 = Object.Instantiate(Profile.DGeoStages[m_currentDestructionRenderer].SpawnOnEnterIndex, base.transform.position, base.transform.rotation);
					if (m_scalesSpawns)
					{
						float num3 = gameObject2.transform.localScale.x * base.transform.localScale.x;
						gameObject2.transform.localScale = new Vector3(num3, num3, num3);
					}
				}
			}
			if (m_isDestroyed && Profile.UsesFinalMesh)
			{
				MeshFilter.mesh = Profile.FinalMesh;
				MeshCollider.sharedMesh = Profile.FinalMesh;
			}
		}
	}
}
