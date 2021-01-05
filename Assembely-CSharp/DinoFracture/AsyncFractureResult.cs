using UnityEngine;

namespace DinoFracture
{
	public sealed class AsyncFractureResult
	{
		public bool IsComplete
		{
			get;
			private set;
		}

		public GameObject PiecesRoot
		{
			get;
			private set;
		}

		public Bounds EntireMeshBounds
		{
			get;
			private set;
		}

		internal bool StopRequested
		{
			get;
			private set;
		}

		internal void SetResult(GameObject rootGO, Bounds bounds)
		{
			if (IsComplete)
			{
				Debug.LogWarning("DinoFracture: Setting AsyncFractureResult's results twice.");
				return;
			}
			PiecesRoot = rootGO;
			EntireMeshBounds = bounds;
			IsComplete = true;
		}

		public void StopFracture()
		{
			StopRequested = true;
		}
	}
}
