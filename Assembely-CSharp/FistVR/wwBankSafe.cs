using UnityEngine;

namespace FistVR
{
	public class wwBankSafe : MonoBehaviour
	{
		public wwTargetPuzzle Puzzle;

		public GameObject[] ContentsObjects;

		public Transform[] ContentsPoses;

		public int SafeState;

		public float[] DoorStateRotations;

		public Transform Door;

		public GameObject SafeOpenTrigger;

		public GameObject SafeParticles;

		public ParticleSystem PSystem;

		public AudioEvent PayoutSoundEvent;

		public AudioEvent UnlockEvent;

		public void SetState(int stateIndex, bool playSound, bool stateEvent)
		{
			SafeState = stateIndex;
			if (playSound)
			{
				SM.PlayGenericSound(UnlockEvent, base.transform.position);
			}
			Door.localEulerAngles = new Vector3(0f, DoorStateRotations[SafeState], 0f);
			if (SafeState == 1)
			{
				SafeOpenTrigger.SetActive(value: true);
				SafeParticles.SetActive(value: true);
				if (stateEvent)
				{
					SM.PlayGenericSound(UnlockEvent, base.transform.position);
					Puzzle.Manager.RegisterPuzzleSafeStateChange(Puzzle.PuzzleIndex, 1);
				}
			}
			else
			{
				SafeOpenTrigger.SetActive(value: false);
				SafeParticles.SetActive(value: false);
			}
			if (SafeState == 2 && stateEvent)
			{
				Invoke("PayoutBurst", 0.2f);
				Invoke("PayoutBurst", 1.2f);
				Invoke("PayoutBurst", 2.2f);
				for (int i = 0; i < ContentsObjects.Length; i++)
				{
					Object.Instantiate(ContentsObjects[i], ContentsPoses[i].position, ContentsPoses[i].rotation);
				}
				Puzzle.Manager.RegisterPuzzleSafeStateChange(Puzzle.PuzzleIndex, 2);
			}
		}

		public void PayoutBurst()
		{
			SM.PlayGenericSound(PayoutSoundEvent, base.transform.position);
			PSystem.Emit(100);
		}
	}
}
