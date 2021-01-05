using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class wwTargetManager : MonoBehaviour
	{
		[Serializable]
		public class TargetToRespawn
		{
			public FVRObject ObjectWrapper;

			public float TimeToRespawn;

			public Vector3 Pos;

			public Quaternion Rot;

			public float Scale;

			public bool DoesReScale;

			public Sprite Sprite;
		}

		public wwParkManager Manager;

		private List<TargetToRespawn> TargetsToRespawn = new List<TargetToRespawn>();

		private List<string> LastThreeTargetsStruck = new List<string>();

		private List<Sprite> LastThreeSprites = new List<Sprite>();

		public bool UsesWWExtendedSystems = true;

		public eSlab ESlab;

		public wwTargetPuzzle[] Puzzles;

		public GameObject[] CompletionCheckMarks;

		public AudioEvent TargSequenceCompletedEvent;

		private float CheckPuzzleComplettionTick = 0.1f;

		private void Awake()
		{
			wwTarget[] array = UnityEngine.Object.FindObjectsOfType<wwTarget>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetManager(this);
			}
			LastThreeTargetsStruck.Add(string.Empty);
			LastThreeTargetsStruck.Add(string.Empty);
			LastThreeTargetsStruck.Add(string.Empty);
			LastThreeTargetsStruck.Add(string.Empty);
			LastThreeSprites.Add(null);
			LastThreeSprites.Add(null);
			LastThreeSprites.Add(null);
			LastThreeSprites.Add(null);
		}

		public void ConfigurePuzzleStates(int[] PuzzleStates, int[] SafeStates)
		{
			for (int i = 0; i < Puzzles.Length; i++)
			{
				Puzzles[i].SetState(PuzzleStates[i], SafeStates[i]);
			}
			UpdateCheckMarks(PuzzleStates);
		}

		public void UpdateCheckMarks(int[] states)
		{
			for (int i = 0; i < states.Length; i++)
			{
				if (states[i] == 0 || states[i] == 1)
				{
					CompletionCheckMarks[i].SetActive(value: false);
				}
				else
				{
					CompletionCheckMarks[i].SetActive(value: true);
				}
			}
		}

		private void Update()
		{
			if (TargetsToRespawn.Count > 0)
			{
				for (int num = TargetsToRespawn.Count - 1; num >= 0; num--)
				{
					if (TargetsToRespawn[num].TimeToRespawn > 0f)
					{
						TargetsToRespawn[num].TimeToRespawn -= Time.deltaTime;
					}
					else
					{
						RespawnTarget(TargetsToRespawn[num]);
						TargetsToRespawn.RemoveAt(num);
					}
				}
			}
			if (!UsesWWExtendedSystems || Manager.RewardChests[0].GetState() >= 1)
			{
				return;
			}
			if (CheckPuzzleComplettionTick > 0f)
			{
				CheckPuzzleComplettionTick -= Time.deltaTime;
				return;
			}
			CheckPuzzleComplettionTick = 1f;
			bool flag = true;
			for (int i = 0; i < Puzzles.Length; i++)
			{
				if (Puzzles[i].PuzzleState != 2)
				{
					flag = false;
				}
			}
			if (flag)
			{
				Manager.UnlockChest(0);
			}
		}

		public void PrimeForRespawn(wwTarget t, Vector3 pos, Quaternion rot, float Scale, bool reScale)
		{
			if (t is wwTargetShatterable)
			{
				TargetToRespawn targetToRespawn = new TargetToRespawn();
				targetToRespawn.ObjectWrapper = (t as wwTargetShatterable).ObjectWrapper;
				targetToRespawn.Pos = pos;
				targetToRespawn.Rot = rot;
				targetToRespawn.Scale = Scale;
				targetToRespawn.DoesReScale = reScale;
				targetToRespawn.TimeToRespawn = t.RespawnTime;
				targetToRespawn.Sprite = t.TargetSprite;
				TargetsToRespawn.Add(targetToRespawn);
			}
		}

		public void StruckEvent(wwTarget t)
		{
			LastThreeTargetsStruck.Add(t.Ident);
			LastThreeSprites.Add(t.TargetSprite);
			LastThreeTargetsStruck.RemoveAt(0);
			LastThreeSprites.RemoveAt(0);
			if (UsesWWExtendedSystems)
			{
				ESlab.UpdateSprites(LastThreeSprites[0], LastThreeSprites[1], LastThreeSprites[2], LastThreeSprites[3]);
				TestPuzzles();
			}
		}

		private void TestPuzzles()
		{
			string text = string.Empty;
			for (int i = 0; i < LastThreeTargetsStruck.Count; i++)
			{
				text += LastThreeTargetsStruck[i];
			}
			for (int j = 0; j < Puzzles.Length; j++)
			{
				if (!(Puzzles[j] == null))
				{
					Puzzles[j].TestSequence(text);
				}
			}
		}

		private void RespawnTarget(TargetToRespawn t)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(t.ObjectWrapper.GetGameObject(), t.Pos, t.Rot);
			gameObject.transform.position = t.Pos;
			gameObject.transform.rotation = t.Rot;
			if (t.DoesReScale)
			{
				gameObject.transform.localScale = new Vector3(t.Scale, t.Scale, t.Scale);
			}
			gameObject.GetComponent<wwTarget>().SetupAfterSpawn(this, t.Pos, t.Rot, t.Scale, t.DoesReScale);
		}

		public void RegisterPuzzleStateChange(int puzzle, int newState)
		{
			if (newState == 2)
			{
				CompletionCheckMarks[puzzle].SetActive(value: true);
				SM.PlayGenericSound(TargSequenceCompletedEvent, GM.CurrentPlayerBody.transform.position);
			}
			Manager.RegisterTargetPuzzleStateChange(puzzle, newState);
		}

		public void RegisterPuzzleSafeStateChange(int puzzle, int newState)
		{
			Manager.RegisterTargetPuzzleSafeStateChange(puzzle, newState);
		}
	}
}
