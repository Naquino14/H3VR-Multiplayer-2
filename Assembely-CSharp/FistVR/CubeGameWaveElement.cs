using System;

namespace FistVR
{
	[Serializable]
	public class CubeGameWaveElement
	{
		public CubeSpawnWall.SpawnWallType WallType;

		public CubeGameEnemyType Enemy;

		public int WallIndex;

		public int MinEnemies;

		public int MaxEnemies;

		public bool TrickleSpawn;
	}
}
