using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New TNH_TakeChallenge", menuName = "TNH/TNH_TakeChallenge", order = 0)]
	public class TNH_TakeChallenge : ScriptableObject
	{
		public TNH_TurretType TurretType;

		public int NumTurrets;

		[SearchableEnum]
		public SosigEnemyID GID;

		public int NumGuards;

		public int IFFUsed = 1;
	}
}
