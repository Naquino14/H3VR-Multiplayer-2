using UnityEngine;

namespace FistVR
{
	public class MG_MeatMachine : MonoBehaviour
	{
		public MG_StartingRoom room;

		public ParticleSystem PSystem_Sparks;

		public GameObject[] SpawnOnMeat;

		public Transform SpawnPoint;

		private int m_meatFedIn;

		private bool m_hasPlayedFinal;

		public Transform[] Rollers;

		private float m_roll;

		private void Update()
		{
			m_roll += Time.deltaTime * 1720f;
			m_roll = Mathf.Repeat(m_roll, 360f);
			Rollers[0].localEulerAngles = new Vector3(0f, 0f, 0f - m_roll);
			Rollers[1].localEulerAngles = new Vector3(0f, 0f, m_roll);
		}

		public void FedMeatIn(int id)
		{
			GM.MGMaster.Narrator.PlayMeatFeedIn(id);
			m_meatFedIn++;
			if (m_meatFedIn == 1)
			{
				GM.MGMaster.MeatRoom2.OpenDoors(playSound: true);
				GM.MGMaster.SpawnBadGuySet1();
			}
			if (m_meatFedIn == 2)
			{
				GM.MGMaster.MeatRoom3.OpenDoors(playSound: true);
				GM.MGMaster.SpawnBadGuySet2();
			}
			if (m_meatFedIn >= 3)
			{
				GM.MGMaster.IsCountingDown = false;
				PlayFinalWon();
			}
			PSystem_Sparks.Emit(50);
			FXM.InitiateMuzzleFlash(PSystem_Sparks.transform.position, PSystem_Sparks.transform.forward, 5f, Color.white, 0.7f);
			for (int i = 0; i < SpawnOnMeat.Length; i++)
			{
				Object.Instantiate(SpawnOnMeat[i], SpawnPoint.position, SpawnPoint.rotation);
			}
		}

		public void FedObjIn()
		{
			FXM.InitiateMuzzleFlash(PSystem_Sparks.transform.position, PSystem_Sparks.transform.forward, 5f, Color.white, 0.7f);
			PSystem_Sparks.Emit(150);
		}

		public void PlayFinalWon()
		{
			if (!m_hasPlayedFinal)
			{
				m_hasPlayedFinal = true;
				Invoke("DelayedPlay", 13f);
			}
			room.CloseDoors();
		}

		private void DelayedPlay()
		{
			if (GM.Options.MeatGrinderFlags.HasPlayerEverWon)
			{
				GM.MGMaster.Narrator.PlayWonAgain(GM.Options.MeatGrinderFlags.SuccessEventVoiceIndex);
				if (GM.Options.MeatGrinderFlags.SuccessEventVoiceIndex <= 2)
				{
					Invoke("LoadMainMenu", 30f);
				}
				else
				{
					Invoke("LoadMainMenu", 8f);
				}
				GM.Options.MeatGrinderFlags.SuccessEventVoiceIndex++;
				GM.Options.MeatGrinderFlags.SuccessEventVoiceIndex = Mathf.Clamp(GM.Options.MeatGrinderFlags.SuccessEventVoiceIndex, 0, GM.Options.MeatGrinderFlags.MaxSuccessEventVoiceIndex - 1);
			}
			else
			{
				GM.MGMaster.Narrator.PlayWonFirstTime();
				GM.Options.MeatGrinderFlags.HasPlayerEverWon = true;
				GM.Options.SaveToFile();
				Invoke("LoadMainMenu", 65f);
			}
		}

		private void LoadMainMenu()
		{
			SteamVR_LoadLevel.Begin("MainMenu3");
		}
	}
}
