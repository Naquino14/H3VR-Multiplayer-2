using UnityEngine;

namespace FistVR
{
	public class OmniSpawner_Grid : OmniSpawner
	{
		private OmniSpawnDef_Grid m_def;

		public GameObject[] GridPrefabs;

		private bool m_canPresent;

		private bool m_hasGrid;

		private OmniGrid m_curGrid;

		public override void Configure(OmniSpawnDef Def, OmniWaveEngagementRange Range)
		{
			base.Configure(Def, Range);
			m_def = Def as OmniSpawnDef_Grid;
		}

		public override void BeginSpawning()
		{
			base.BeginSpawning();
			m_canPresent = true;
		}

		public override void EndSpawning()
		{
			base.EndSpawning();
			m_canPresent = false;
		}

		public override void Activate()
		{
			base.Activate();
		}

		public override int Deactivate()
		{
			m_curGrid.DespawnGrid();
			return base.Deactivate();
		}

		private void Update()
		{
			UpdateMe();
		}

		private void UpdateMe()
		{
			if (m_isConfigured)
			{
				switch (m_state)
				{
				case SpawnerState.Activated:
					SpawningLoop();
					break;
				case SpawnerState.Activating:
					Activating();
					break;
				case SpawnerState.Deactivating:
					Deactivating();
					break;
				}
			}
		}

		private void SpawningLoop()
		{
			if (m_canPresent && !m_hasGrid)
			{
				m_hasGrid = true;
				Vector3 position = base.transform.position;
				Vector3 endPos = new Vector3(0f, 1.25f, GetRange());
				GameObject gameObject = Object.Instantiate(GridPrefabs[(int)m_def.Size], position, Quaternion.identity);
				m_curGrid = gameObject.GetComponent<OmniGrid>();
				m_curGrid.Init(this, position, endPos, Quaternion.identity, Quaternion.identity, m_def.Configuration, m_def.Instructions);
				m_isDoneSpawning = true;
			}
		}

		public void GridIsFinished()
		{
			m_isReadyForWaveEnd = true;
			Deactivate();
		}
	}
}
