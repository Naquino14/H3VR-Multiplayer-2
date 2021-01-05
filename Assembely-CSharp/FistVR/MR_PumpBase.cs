using UnityEngine;

namespace FistVR
{
	public class MR_PumpBase : MonoBehaviour, IMG_HandlePumpable
	{
		public MR_SpikePuzzle Puzzle;

		public Transform Spike;

		public float Extension = 0.731f;

		public bool IsGoof;

		public Transform SpawnPos;

		public GameObject GoofPrefab;

		private bool hasGoofed;

		private void Update()
		{
			if (Spike != null)
			{
				Extension = Spike.transform.localPosition.z;
			}
		}

		public void Pump(float delta)
		{
			if (IsGoof)
			{
				if (!hasGoofed)
				{
					hasGoofed = true;
					Object.Instantiate(GoofPrefab, SpawnPos.position, SpawnPos.rotation);
					GM.MGMaster.Narrator.PlayJumpScare();
				}
			}
			else
			{
				Extension += delta * 0.01f;
				Extension = Mathf.Clamp(Extension, -0.42f, 0.9f);
				Spike.transform.localPosition = new Vector3(0f, 0f, Extension);
				Puzzle.UpdatePuzzle();
			}
		}
	}
}
