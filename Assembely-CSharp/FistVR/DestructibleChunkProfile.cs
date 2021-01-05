using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New DestructibleChunkProfile", menuName = "Destruction/ChunkProfile", order = 0)]
	public class DestructibleChunkProfile : ScriptableObject
	{
		[Serializable]
		public class DGeoStage
		{
			public bool SpawnsOnEnterIndex;

			public GameObject SpawnOnEnterIndex;

			public List<Mesh> Meshes;

			public Mesh GetMesh(int index)
			{
				return Meshes[Mathf.Clamp(index, 0, Meshes.Count - 1)];
			}
		}

		[Header("Chunk Vars")]
		public string Name;

		public int MaxRandomIndex;

		public bool ScalesSpawns;

		public float TotalLife;

		public float DamageCutoff;

		public bool IsDestroyedOnZeroLife;

		public bool SpawnsOnDestruction;

		public GameObject SpawnOnDestruction;

		public bool UsesFinalMesh;

		public Mesh FinalMesh;

		public List<DGeoStage> DGeoStages;
	}
}
