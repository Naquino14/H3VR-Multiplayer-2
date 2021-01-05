using UnityEngine;

namespace FistVR
{
	public class wwRewardChest : MonoBehaviour
	{
		public wwParkManager Manager;

		private int m_state;

		public GameObject ReadyToOpenParticles;

		public GameObject ReadyToOpenTrigger;

		public GameObject KeyPrefab;

		public Transform KeyPosition;

		public Transform Cap;

		public GameObject BlackOut;

		public GameObject Lock;

		private float XRot_Closed;

		private float XRot_Open = -70f;

		public int Index;

		public AudioEvent ChestUnlockedEvent;

		public AudioEvent ChestOpenEvent;

		public ItemSpawnerID[] RewardUnlocks;

		public GameObject[] RewardPrefabs;

		public Transform[] RewardPoints;

		public void Awake()
		{
		}

		public int GetState()
		{
			return m_state;
		}

		public void UnlockChest()
		{
			SM.PlayGenericSound(ChestUnlockedEvent, base.transform.position);
			SetState(1, stateEvent: true);
			Manager.RegisterRewardChestStateChange(Index, 1);
		}

		public void OpenChest()
		{
			SetState(2, stateEvent: true);
			Manager.RegisterRewardChestStateChange(Index, 2);
			GameObject gameObject = Object.Instantiate(KeyPrefab, KeyPosition.position, KeyPosition.rotation);
			wwKey component = gameObject.GetComponent<wwKey>();
			component.KeyIndex = Index;
			component.State = 1;
			component.Manager = Manager;
			SM.PlayGenericSound(ChestOpenEvent, base.transform.position);
			for (int i = 0; i < RewardPrefabs.Length; i++)
			{
				Object.Instantiate(RewardPrefabs[i], RewardPoints[i].position, RewardPoints[i].rotation);
			}
			if (RewardUnlocks.Length > 0)
			{
				for (int j = 0; j < RewardUnlocks.Length; j++)
				{
					GM.Rewards.RewardUnlocks.UnlockReward(RewardUnlocks[j]);
				}
				GM.Rewards.SaveToFile();
			}
			Manager.RegisterKeyStateChange(Index, 1);
		}

		public void SetState(int newState, bool stateEvent)
		{
			m_state = newState;
			switch (newState)
			{
			case 0:
				ReadyToOpenParticles.SetActive(value: false);
				ReadyToOpenTrigger.SetActive(value: false);
				BlackOut.SetActive(value: true);
				Lock.SetActive(value: true);
				Cap.localEulerAngles = new Vector3(XRot_Closed, 0f, 0f);
				break;
			case 1:
				ReadyToOpenParticles.SetActive(value: true);
				ReadyToOpenTrigger.SetActive(value: true);
				BlackOut.SetActive(value: true);
				Lock.SetActive(value: false);
				Cap.localEulerAngles = new Vector3(XRot_Closed, 0f, 0f);
				break;
			case 2:
				ReadyToOpenParticles.SetActive(value: false);
				ReadyToOpenTrigger.SetActive(value: false);
				BlackOut.SetActive(value: false);
				Lock.SetActive(value: false);
				Cap.localEulerAngles = new Vector3(XRot_Open, 0f, 0f);
				break;
			}
		}
	}
}
