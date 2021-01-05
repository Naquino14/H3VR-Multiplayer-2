using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

namespace FistVR
{
	public class ZosigEndingManager : MonoBehaviour
	{
		public enum RW_EndPhase
		{
			InitPlayer,
			RoarnartTeleportsIn,
			RoarnartSpeaks,
			Explodes1,
			RoarnartContinues,
			TeleportIn1,
			TeleportIn2,
			TeleportIn3,
			TeleportIn4,
			TeleportIn5,
			Bobert,
			Dustin,
			Josh,
			Alister,
			Ralph,
			FinalLine,
			TeleportOut1,
			TeleportOut2,
			TeleportOut3,
			TeleportOut4,
			TeleportOut5,
			ScreenOff,
			Unlocks,
			Credits0,
			Credits1,
			Credits2,
			Credits3,
			Credits4,
			FadeOut,
			LevelLoad
		}

		[Serializable]
		public class EndingCharacter
		{
			public ZosigSpawnManager SpawnManager;

			public ZosigNPCProfile Profile;

			public AudioSource AudSource;

			public Sosig S;

			public Transform SpawnInPoint;

			public SosigEnemyTemplate Template;

			public int Index;

			private bool m_isSpeaking;

			private float HeadJitterTick = 0.1f;

			public float BeginSpeaking()
			{
				m_isSpeaking = true;
				AudSource.Play();
				return AudSource.clip.length;
			}

			public void Spawn()
			{
				S = SpawnManager.SpawnNPCToPoint(Template, Index, SpawnInPoint);
			}

			public void Explode()
			{
				if (S != null)
				{
					S.ClearSosig();
				}
			}

			public void ClearSosig()
			{
				if (S != null)
				{
					S.DeSpawnSosig();
				}
			}

			public void Update(float t)
			{
				if (!AudSource.isPlaying)
				{
					m_isSpeaking = false;
				}
				if (!m_isSpeaking)
				{
					return;
				}
				if (HeadJitterTick > 0f)
				{
					HeadJitterTick -= Time.deltaTime;
					return;
				}
				HeadJitterTick = UnityEngine.Random.Range(Profile.SpeakJitterRange.x, Profile.SpeakJitterRange.y);
				if (S != null && S.Links[0] != null)
				{
					S.Links[0].R.AddForceAtPosition(UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(Profile.SpeakPowerRange.x, Profile.SpeakPowerRange.y), S.Links[0].transform.position + Vector3.up * 0.3f, ForceMode.Impulse);
				}
			}
		}

		public ZosigSpawnManager SpawnManager;

		public RW_EndPhase EndPhase;

		public Transform TeleportPlace;

		public List<AudioClip> Lines_Narrator;

		public List<AudioClip> Lines_Bobert;

		public List<AudioClip> Lines_Dustin;

		public List<AudioClip> Lines_Josh;

		public List<AudioClip> Lines_Alister;

		public List<AudioClip> Lines_Ralph;

		public AudioSource AudSource_Narrator_Reverbee;

		private bool m_isEnding;

		public Renderer FinalScreen;

		private bool m_isScreenTalking;

		private bool m_isScreenOn;

		public GameObject TeleportEffect;

		public AudioEvent AudEvent_Teleport;

		[Header("Credits")]
		public GameObject CreditsCanvas;

		public List<GameObject> CreditsPages;

		public GameObject UnlocksCanvas;

		public Text UnlockLabel;

		public EndingCharacter EndCh_Roarnart;

		public EndingCharacter EndCh_Bobert;

		public EndingCharacter EndCh_Dustin;

		public EndingCharacter EndCh_Josh;

		public EndingCharacter EndCh_Alister;

		public EndingCharacter EndCh_Ralph;

		public List<ItemSpawnerID> RW_Classic;

		public List<ItemSpawnerID> RW_Arcade;

		public List<ItemSpawnerID> RW_Hardcore;

		public List<ItemSpawnerID> RW_Ralph;

		public List<ItemSpawnerID> RW_Alister;

		public List<ItemSpawnerID> RW_Bobert;

		public List<ItemSpawnerID> RW_Dustin;

		public List<ItemSpawnerID> RW_Josh;

		public List<ItemSpawnerID> RW_Nar;

		private int b_clip;

		private int d_clip;

		private int j_clip;

		private int a_clip;

		private int r_clip;

		private int n_clip = 2;

		private float m_phaseTimer;

		private float m_screenIntensity = 1f;

		private float m_screenIntensityTick = 0.1f;

		private void Start()
		{
		}

		public void InitEnding()
		{
			m_isEnding = true;
			PickClips();
			InitPhase(RW_EndPhase.InitPlayer);
		}

		private void PickClips()
		{
			int flagValue = GM.ZMaster.FlagM.GetFlagValue("s_t");
			int flagValue2 = GM.ZMaster.FlagM.GetFlagValue("s_c");
			int flagValue3 = GM.ZMaster.FlagM.GetFlagValue("s_l");
			int flagValue4 = GM.ZMaster.FlagM.GetFlagValue("s_m");
			int flagValue5 = GM.ZMaster.FlagM.GetFlagValue("s_g");
			if (flagValue > 50 && GM.ZMaster.FlagM.GetFlagValue("quest06_state") >= 3)
			{
				b_clip = 0;
			}
			else if (flagValue < 10 || GM.ZMaster.FlagM.GetFlagValue("quest06_state") < 2)
			{
				b_clip = 2;
			}
			else
			{
				b_clip = 1;
			}
			if (flagValue2 > 20 && GM.ZMaster.FlagM.GetFlagValue("quest03_state") >= 3)
			{
				d_clip = 0;
			}
			else if (flagValue2 < 5 || GM.ZMaster.FlagM.GetFlagValue("quest03_state") < 2)
			{
				d_clip = 2;
			}
			else
			{
				d_clip = 1;
			}
			if (flagValue3 > 20 && GM.ZMaster.FlagM.GetFlagValue("quest17_state") >= 3)
			{
				j_clip = 0;
			}
			else if (flagValue3 < 8 || GM.ZMaster.FlagM.GetFlagValue("quest17_state") < 2)
			{
				j_clip = 2;
			}
			else
			{
				j_clip = 1;
			}
			if (flagValue4 > 200 && GM.ZMaster.FlagM.GetFlagValue("quest10_state") >= 2)
			{
				a_clip = 0;
			}
			else if (flagValue4 < 50 || GM.ZMaster.FlagM.GetFlagValue("quest10_state") < 2)
			{
				a_clip = 2;
			}
			else
			{
				a_clip = 1;
			}
			if (flagValue5 > 200 && GM.ZMaster.FlagM.GetFlagValue("quest18_state") >= 3)
			{
				r_clip = 0;
			}
			else if (flagValue5 < 50 || GM.ZMaster.FlagM.GetFlagValue("quest18_state") < 2)
			{
				r_clip = 2;
			}
			else
			{
				r_clip = 1;
			}
			if (b_clip == 2 || d_clip == 2 || j_clip == 2 || a_clip == 2 || r_clip == 2)
			{
				n_clip = 3;
			}
			if (GM.ZMaster.FlagM.GetFlagValue("g_c") > 0)
			{
				n_clip = 4;
			}
		}

		private void InitPhase(RW_EndPhase p)
		{
			EndPhase = p;
			m_phaseTimer = 0f;
			switch (EndPhase)
			{
			case RW_EndPhase.InitPlayer:
			{
				Vector3 forward = GM.CurrentMovementManager.Head.forward;
				forward.y = 0f;
				GM.CurrentMovementManager.TeleportToPoint(TeleportPlace.position, isAbsolute: false, GM.CurrentPlayerBody.transform.forward);
				if (GM.CurrentMovementManager.Hands[0].CurrentInteractable != null)
				{
					GM.CurrentMovementManager.Hands[0].CurrentInteractable.ForceBreakInteraction();
				}
				if (GM.CurrentMovementManager.Hands[1].CurrentInteractable != null)
				{
					GM.CurrentMovementManager.Hands[1].CurrentInteractable.ForceBreakInteraction();
				}
				GM.CurrentMovementManager.CleanupFlagsForModeSwitch();
				GM.CurrentMovementManager.Hands[0].gameObject.SetActive(value: false);
				GM.CurrentMovementManager.Hands[1].gameObject.SetActive(value: false);
				GM.CurrentPlayerBody.HealthBar.SetActive(value: false);
				m_phaseTimer = 2f;
				break;
			}
			case RW_EndPhase.RoarnartTeleportsIn:
				EndCh_Roarnart.Spawn();
				SM.PlayGenericSound(AudEvent_Teleport, EndCh_Roarnart.SpawnInPoint.position);
				UnityEngine.Object.Instantiate(TeleportEffect, EndCh_Roarnart.SpawnInPoint.position, EndCh_Roarnart.SpawnInPoint.rotation);
				m_phaseTimer = 1f;
				break;
			case RW_EndPhase.RoarnartSpeaks:
				GM.ZMaster.SetMusic_Speaking();
				EndCh_Roarnart.AudSource.clip = Lines_Narrator[0];
				m_phaseTimer = EndCh_Roarnart.BeginSpeaking();
				break;
			case RW_EndPhase.Explodes1:
				EndCh_Roarnart.Explode();
				m_phaseTimer = 0.7f;
				break;
			case RW_EndPhase.RoarnartContinues:
				m_isScreenOn = true;
				m_isScreenTalking = true;
				AudSource_Narrator_Reverbee.clip = Lines_Narrator[1];
				AudSource_Narrator_Reverbee.Play();
				m_phaseTimer = AudSource_Narrator_Reverbee.clip.length + 0.5f;
				break;
			case RW_EndPhase.TeleportIn1:
				GM.ZMaster.SetMusic_Gameplay();
				EndCh_Roarnart.ClearSosig();
				EndCh_Bobert.Spawn();
				SM.PlayGenericSound(AudEvent_Teleport, EndCh_Bobert.SpawnInPoint.position);
				UnityEngine.Object.Instantiate(TeleportEffect, EndCh_Bobert.SpawnInPoint.position, EndCh_Bobert.SpawnInPoint.rotation);
				m_phaseTimer = 1f;
				break;
			case RW_EndPhase.TeleportIn2:
				EndCh_Dustin.Spawn();
				SM.PlayGenericSound(AudEvent_Teleport, EndCh_Dustin.SpawnInPoint.position);
				UnityEngine.Object.Instantiate(TeleportEffect, EndCh_Dustin.SpawnInPoint.position, EndCh_Dustin.SpawnInPoint.rotation);
				m_phaseTimer = 1f;
				break;
			case RW_EndPhase.TeleportIn3:
				EndCh_Josh.Spawn();
				SM.PlayGenericSound(AudEvent_Teleport, EndCh_Josh.SpawnInPoint.position);
				UnityEngine.Object.Instantiate(TeleportEffect, EndCh_Josh.SpawnInPoint.position, EndCh_Josh.SpawnInPoint.rotation);
				m_phaseTimer = 1f;
				break;
			case RW_EndPhase.TeleportIn4:
				EndCh_Alister.Spawn();
				SM.PlayGenericSound(AudEvent_Teleport, EndCh_Alister.SpawnInPoint.position);
				UnityEngine.Object.Instantiate(TeleportEffect, EndCh_Alister.SpawnInPoint.position, EndCh_Alister.SpawnInPoint.rotation);
				m_phaseTimer = 1f;
				break;
			case RW_EndPhase.TeleportIn5:
				EndCh_Ralph.Spawn();
				SM.PlayGenericSound(AudEvent_Teleport, EndCh_Ralph.SpawnInPoint.position);
				UnityEngine.Object.Instantiate(TeleportEffect, EndCh_Ralph.SpawnInPoint.position, EndCh_Ralph.SpawnInPoint.rotation);
				m_phaseTimer = 2f;
				break;
			case RW_EndPhase.Bobert:
				GM.ZMaster.SetMusic_Speaking();
				EndCh_Bobert.AudSource.clip = Lines_Bobert[b_clip];
				m_phaseTimer = EndCh_Bobert.BeginSpeaking() + 0.5f;
				break;
			case RW_EndPhase.Dustin:
				EndCh_Dustin.AudSource.clip = Lines_Dustin[d_clip];
				m_phaseTimer = EndCh_Dustin.BeginSpeaking() + 0.5f;
				break;
			case RW_EndPhase.Josh:
				EndCh_Josh.AudSource.clip = Lines_Josh[j_clip];
				m_phaseTimer = EndCh_Josh.BeginSpeaking() + 0.5f;
				break;
			case RW_EndPhase.Alister:
				EndCh_Alister.AudSource.clip = Lines_Alister[a_clip];
				m_phaseTimer = EndCh_Alister.BeginSpeaking() + 0.5f;
				break;
			case RW_EndPhase.Ralph:
				EndCh_Ralph.AudSource.clip = Lines_Ralph[r_clip];
				m_phaseTimer = EndCh_Ralph.BeginSpeaking() + 0.5f;
				break;
			case RW_EndPhase.FinalLine:
				m_isScreenOn = true;
				m_isScreenTalking = true;
				AudSource_Narrator_Reverbee.clip = Lines_Narrator[n_clip];
				AudSource_Narrator_Reverbee.Play();
				m_phaseTimer = AudSource_Narrator_Reverbee.clip.length + 3f;
				break;
			case RW_EndPhase.TeleportOut1:
				GM.ZMaster.SetMusic_Gameplay();
				SM.PlayGenericSound(AudEvent_Teleport, EndCh_Bobert.SpawnInPoint.position);
				UnityEngine.Object.Instantiate(TeleportEffect, EndCh_Bobert.SpawnInPoint.position, EndCh_Bobert.SpawnInPoint.rotation);
				EndCh_Bobert.ClearSosig();
				m_phaseTimer = 1f;
				break;
			case RW_EndPhase.TeleportOut2:
				SM.PlayGenericSound(AudEvent_Teleport, EndCh_Dustin.SpawnInPoint.position);
				UnityEngine.Object.Instantiate(TeleportEffect, EndCh_Dustin.SpawnInPoint.position, EndCh_Dustin.SpawnInPoint.rotation);
				EndCh_Dustin.ClearSosig();
				m_phaseTimer = 1f;
				break;
			case RW_EndPhase.TeleportOut3:
				SM.PlayGenericSound(AudEvent_Teleport, EndCh_Josh.SpawnInPoint.position);
				UnityEngine.Object.Instantiate(TeleportEffect, EndCh_Josh.SpawnInPoint.position, EndCh_Josh.SpawnInPoint.rotation);
				EndCh_Josh.ClearSosig();
				m_phaseTimer = 1f;
				break;
			case RW_EndPhase.TeleportOut4:
				SM.PlayGenericSound(AudEvent_Teleport, EndCh_Alister.SpawnInPoint.position);
				UnityEngine.Object.Instantiate(TeleportEffect, EndCh_Alister.SpawnInPoint.position, EndCh_Alister.SpawnInPoint.rotation);
				EndCh_Alister.ClearSosig();
				m_phaseTimer = 1f;
				break;
			case RW_EndPhase.TeleportOut5:
				SM.PlayGenericSound(AudEvent_Teleport, EndCh_Ralph.SpawnInPoint.position);
				UnityEngine.Object.Instantiate(TeleportEffect, EndCh_Ralph.SpawnInPoint.position, EndCh_Ralph.SpawnInPoint.rotation);
				EndCh_Ralph.ClearSosig();
				m_phaseTimer = 1f;
				break;
			case RW_EndPhase.ScreenOff:
			{
				Color value = new Color(0f, 0f, 0f, 1f);
				FinalScreen.material.SetColor("_EmissionColor", value);
				m_phaseTimer = 1f;
				break;
			}
			case RW_EndPhase.Unlocks:
				DisplayUnlocks(b: true);
				m_phaseTimer = 15f;
				break;
			case RW_EndPhase.Credits0:
				DisplayUnlocks(b: false);
				DisplayCredits(0);
				m_phaseTimer = 15f;
				break;
			case RW_EndPhase.Credits1:
				DisplayCredits(1);
				m_phaseTimer = 15f;
				break;
			case RW_EndPhase.Credits2:
				DisplayCredits(2);
				m_phaseTimer = 15f;
				break;
			case RW_EndPhase.Credits3:
				DisplayCredits(3);
				m_phaseTimer = 15f;
				break;
			case RW_EndPhase.Credits4:
				DisplayCredits(4);
				m_phaseTimer = 15f;
				break;
			case RW_EndPhase.FadeOut:
				SteamVR_Fade.Start(Color.black, 2.5f);
				m_phaseTimer = 3f;
				break;
			case RW_EndPhase.LevelLoad:
				GM.CurrentMovementManager.Hands[0].gameObject.SetActive(value: true);
				GM.CurrentMovementManager.Hands[1].gameObject.SetActive(value: true);
				SteamVR_LoadLevel.Begin("RotWienersStagingScene");
				break;
			}
		}

		private void DisplayCredits(int page)
		{
			if (page < 0)
			{
				CreditsCanvas.SetActive(value: false);
				return;
			}
			CreditsCanvas.SetActive(value: true);
			for (int i = 0; i < CreditsPages.Count; i++)
			{
				if (i == page)
				{
					CreditsPages[i].SetActive(value: true);
				}
				else
				{
					CreditsPages[i].SetActive(value: false);
				}
			}
		}

		private void DisplayUnlocks(bool b)
		{
			string text = "New Unlocks Earned:";
			if (GM.ZMaster.FlagM.GetFlagValue("flag_Difficulty") == 0)
			{
				GM.ROTRWSaves.HBC = true;
				for (int i = 0; i < RW_Classic.Count; i++)
				{
					if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(RW_Classic[i]))
					{
						GM.Rewards.RewardUnlocks.UnlockReward(RW_Classic[i]);
						text = text + "\nUnlocked: " + RW_Classic[i].DisplayName + "!";
					}
				}
			}
			else if (GM.ZMaster.FlagM.GetFlagValue("flag_Difficulty") == 1)
			{
				GM.ROTRWSaves.HBA = true;
				for (int j = 0; j < RW_Arcade.Count; j++)
				{
					if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(RW_Arcade[j]))
					{
						GM.Rewards.RewardUnlocks.UnlockReward(RW_Arcade[j]);
						text = text + "\nUnlocked: " + RW_Arcade[j].DisplayName + "!";
					}
				}
			}
			else if (GM.ZMaster.FlagM.GetFlagValue("flag_Difficulty") == 2)
			{
				GM.ROTRWSaves.HBH = true;
				for (int k = 0; k < RW_Hardcore.Count; k++)
				{
					if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(RW_Hardcore[k]))
					{
						GM.Rewards.RewardUnlocks.UnlockReward(RW_Hardcore[k]);
						text = text + "\nUnlocked: " + RW_Hardcore[k].DisplayName + "!";
					}
				}
			}
			if (r_clip == 0)
			{
				for (int l = 0; l < RW_Ralph.Count; l++)
				{
					if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(RW_Ralph[l]))
					{
						GM.Rewards.RewardUnlocks.UnlockReward(RW_Ralph[l]);
						text = text + "\nUnlocked: " + RW_Ralph[l].DisplayName + "!";
					}
				}
			}
			if (a_clip == 0)
			{
				for (int m = 0; m < RW_Alister.Count; m++)
				{
					if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(RW_Alister[m]))
					{
						GM.Rewards.RewardUnlocks.UnlockReward(RW_Alister[m]);
						text = text + "\nUnlocked: " + RW_Alister[m].DisplayName + "!";
					}
				}
			}
			if (b_clip == 0)
			{
				for (int n = 0; n < RW_Bobert.Count; n++)
				{
					if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(RW_Bobert[n]))
					{
						GM.Rewards.RewardUnlocks.UnlockReward(RW_Bobert[n]);
						text = text + "\nUnlocked: " + RW_Bobert[n].DisplayName + "!";
					}
				}
			}
			if (d_clip == 0)
			{
				for (int num = 0; num < RW_Dustin.Count; num++)
				{
					if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(RW_Dustin[num]))
					{
						GM.Rewards.RewardUnlocks.UnlockReward(RW_Dustin[num]);
						text = text + "\nUnlocked: " + RW_Dustin[num].DisplayName + "!";
					}
				}
			}
			if (j_clip == 0)
			{
				for (int num2 = 0; num2 < RW_Josh.Count; num2++)
				{
					if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(RW_Josh[num2]))
					{
						GM.Rewards.RewardUnlocks.UnlockReward(RW_Josh[num2]);
						text = text + "\nUnlocked: " + RW_Josh[num2].DisplayName + "!";
					}
				}
			}
			if (n_clip == 4)
			{
				for (int num3 = 0; num3 < RW_Nar.Count; num3++)
				{
					if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(RW_Nar[num3]))
					{
						GM.Rewards.RewardUnlocks.UnlockReward(RW_Nar[num3]);
						text = text + "\nUnlocked: " + RW_Nar[num3].DisplayName + "!";
					}
				}
			}
			UnlockLabel.text = text;
			if (b)
			{
				UnlocksCanvas.SetActive(value: true);
				if (GM.ZMaster.FlagM.GetFlagValue("flag_Difficulty") == 0)
				{
					GM.ROTRWSaves.HBC = true;
				}
				else if (GM.ZMaster.FlagM.GetFlagValue("flag_Difficulty") == 1)
				{
					GM.ROTRWSaves.HBA = true;
				}
				else if (GM.ZMaster.FlagM.GetFlagValue("flag_Difficulty") == 2)
				{
					GM.ROTRWSaves.HBH = true;
				}
			}
			else
			{
				UnlocksCanvas.SetActive(value: false);
			}
			GM.ZMaster.FlagM.Save();
			GM.ROTRWSaves.SaveToFile();
			GM.Rewards.SaveToFile();
		}

		private void Update()
		{
			if (!m_isEnding)
			{
				return;
			}
			switch (EndPhase)
			{
			case RW_EndPhase.RoarnartSpeaks:
				EndCh_Roarnart.Update(Time.deltaTime);
				break;
			case RW_EndPhase.RoarnartContinues:
			{
				if (!AudSource_Narrator_Reverbee.isPlaying)
				{
					m_isScreenOn = true;
					m_isScreenTalking = false;
					m_screenIntensity = 1f;
				}
				else
				{
					m_screenIntensityTick -= Time.deltaTime;
					if (m_screenIntensityTick <= 0f)
					{
						m_screenIntensityTick = UnityEngine.Random.Range(0.05f, 0.2f);
						m_screenIntensity = UnityEngine.Random.Range(1f, 5f);
					}
				}
				Color value2 = new Color(m_screenIntensity, m_screenIntensity, m_screenIntensity, 1f);
				FinalScreen.material.SetColor("_EmissionColor", value2);
				break;
			}
			case RW_EndPhase.Bobert:
				EndCh_Bobert.Update(Time.deltaTime);
				break;
			case RW_EndPhase.Dustin:
				EndCh_Dustin.Update(Time.deltaTime);
				break;
			case RW_EndPhase.Josh:
				EndCh_Josh.Update(Time.deltaTime);
				break;
			case RW_EndPhase.Alister:
				EndCh_Alister.Update(Time.deltaTime);
				break;
			case RW_EndPhase.Ralph:
				EndCh_Ralph.Update(Time.deltaTime);
				break;
			case RW_EndPhase.FinalLine:
			{
				if (!AudSource_Narrator_Reverbee.isPlaying)
				{
					m_isScreenOn = true;
					m_isScreenTalking = false;
					m_screenIntensity = 1f;
				}
				else
				{
					m_screenIntensityTick -= Time.deltaTime;
					if (m_screenIntensityTick <= 0f)
					{
						m_screenIntensityTick = UnityEngine.Random.Range(0.03f, 0.08f);
						m_screenIntensity = UnityEngine.Random.Range(1f, 5f);
					}
				}
				Color value = new Color(m_screenIntensity, m_screenIntensity, m_screenIntensity, 1f);
				FinalScreen.material.SetColor("_EmissionColor", value);
				break;
			}
			}
			m_phaseTimer -= Time.deltaTime;
			if (m_phaseTimer <= 0f && EndPhase != RW_EndPhase.LevelLoad)
			{
				InitPhase(EndPhase + 1);
			}
		}
	}
}
