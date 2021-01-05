using UnityEngine;

namespace FistVR
{
	public class ZosigNpcSpawnPoint : MonoBehaviour
	{
		public int NPCIndex = -1;

		public bool HasSpawned;

		public SosigEnemyTemplate Template;

		public bool NeedsFlag;

		public string FlagToSpawn;

		public int FlagValueOrHigherToSpawn;
	}
}
