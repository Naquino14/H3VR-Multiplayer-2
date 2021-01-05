using System.Collections.Generic;
using RUST.Steamworks;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class HG_GameManager : MonoBehaviour
	{
		public enum State
		{
			InMenu,
			InGame
		}

		[Header("Object Connections")]
		public HG_UIManager UIM;

		public SMEME smeme;

		public GameObject ItemSpawner;

		public GameObject TrashCan;

		private bool m_usingSteamworks;

		[Header("State")]
		public State m_state;

		[Header("Mode Managers")]
		public List<HG_ModeManager> ModeManagers;

		public List<HG_ModeProfile> ModeProfiles;

		private HG_ModeManager m_curModeManager;

		private HG_ModeManager.HG_Mode m_selectedMode;

		[Header("Zones")]
		public List<HG_Zone> Zones;

		[Header("Scoring System")]
		public HighScoreManager HSM;

		private string m_curSequenceID = string.Empty;

		private string m_name = string.Empty;

		private CSteamID m_ID;

		private bool m_hasCurrentScore;

		private int m_currentScore;

		private int m_currentRank;

		private bool m_hasPreviousScore;

		private int m_previousScore;

		private bool m_hasRankInCurrentSequence;

		private int m_rankInCurrentSequence;

		private bool m_hasScoresTop;

		private bool m_hasScoresPlayer;

		private bool m_doRequestScoresTop;

		private bool m_doRequestScoresPlayer;

		private List<HighScoreManager.HighScore> m_scoresTop;

		private List<HighScoreManager.HighScore> m_scoresPlayer;

		public Text MusicToggle;

		public AudioSource AudSource_BattleMusic;

		private float m_musicVolume;

		private bool m_isMusicPlaying;

		private bool m_isPlaying;

		public GameObject Wieners_Arena1;

		public GameObject Wieners_Arena2;

		public List<GameObject> Arena1Pieces;

		public List<GameObject> Arena2Pieces;

		public List<GameObject> Arena3Pieces;

		public GronchJobManager JobManager;

		public bool IsBattleMusicOn = true;

		private bool m_getCase;

		public ItemSpawnerID CaseID;

		private void Start()
		{
			InitScoring();
			if (SteamManager.Initialized)
			{
				m_usingSteamworks = true;
			}
		}

		public void PlayerDied()
		{
			if (m_isPlaying && m_curModeManager != null)
			{
				m_curModeManager.HandlePlayerDeath();
			}
			else
			{
				JobManager.HandlePlayerDeath();
			}
		}

		public void ToggleMusic()
		{
			IsBattleMusicOn = !IsBattleMusicOn;
			if (IsBattleMusicOn)
			{
				MusicToggle.text = "Sweet Battle Music: On";
			}
			else
			{
				MusicToggle.text = "Sweet Battle Music: off";
			}
		}

		public void SelectGameMode(int i)
		{
			HG_ModeManager.HG_Mode hG_Mode = (m_selectedMode = (HG_ModeManager.HG_Mode)i);
			switch (m_selectedMode)
			{
			case HG_ModeManager.HG_Mode.TargetRelay_Sprint:
				m_curModeManager = ModeManagers[0];
				break;
			case HG_ModeManager.HG_Mode.TargetRelay_Jog:
				m_curModeManager = ModeManagers[0];
				break;
			case HG_ModeManager.HG_Mode.TargetRelay_Marathon:
				m_curModeManager = ModeManagers[0];
				break;
			case HG_ModeManager.HG_Mode.AssaultNPepper_Skirmish:
				m_curModeManager = ModeManagers[1];
				break;
			case HG_ModeManager.HG_Mode.AssaultNPepper_Brawl:
				m_curModeManager = ModeManagers[1];
				break;
			case HG_ModeManager.HG_Mode.AssaultNPepper_Maelstrom:
				m_curModeManager = ModeManagers[1];
				break;
			case HG_ModeManager.HG_Mode.MeatNMetal_Neophyte:
				m_curModeManager = ModeManagers[2];
				break;
			case HG_ModeManager.HG_Mode.MeatNMetal_Warrior:
				m_curModeManager = ModeManagers[2];
				break;
			case HG_ModeManager.HG_Mode.MeatNMetal_Veteran:
				m_curModeManager = ModeManagers[2];
				break;
			case HG_ModeManager.HG_Mode.BattlePetite_Open:
				m_curModeManager = ModeManagers[3];
				break;
			case HG_ModeManager.HG_Mode.BattlePetite_Sosiggun:
				m_curModeManager = ModeManagers[3];
				break;
			case HG_ModeManager.HG_Mode.BattlePetite_Melee:
				m_curModeManager = ModeManagers[3];
				break;
			case HG_ModeManager.HG_Mode.KingOfTheGrill_Invasion:
				m_curModeManager = ModeManagers[4];
				break;
			case HG_ModeManager.HG_Mode.KingOfTheGrill_Resurrection:
				m_curModeManager = ModeManagers[4];
				break;
			case HG_ModeManager.HG_Mode.KingOfTheGrill_Anachronism:
				m_curModeManager = ModeManagers[4];
				break;
			case HG_ModeManager.HG_Mode.MeatleGear_Open:
				m_curModeManager = ModeManagers[5];
				break;
			case HG_ModeManager.HG_Mode.MeatleGear_ScavengingSnake:
				m_curModeManager = ModeManagers[5];
				break;
			case HG_ModeManager.HG_Mode.MeatleGear_ThirdSnake:
				m_curModeManager = ModeManagers[5];
				break;
			}
			UIM.SetSelectedModeText(ModeProfiles[i]);
			SwitchToModeID("Meatmas2018" + hG_Mode);
		}

		public void SetArenaVisibility(int ar)
		{
			switch (ar)
			{
			case 0:
			{
				for (int l = 0; l < Arena1Pieces.Count; l++)
				{
					Arena1Pieces[l].SetActive(value: true);
				}
				for (int m = 0; m < Arena2Pieces.Count; m++)
				{
					Arena2Pieces[m].SetActive(value: true);
				}
				for (int n = 0; n < Arena3Pieces.Count; n++)
				{
					Arena3Pieces[n].SetActive(value: true);
				}
				break;
			}
			case 1:
			{
				Wieners_Arena1.SetActive(value: true);
				Wieners_Arena2.SetActive(value: false);
				for (int num = 0; num < Arena1Pieces.Count; num++)
				{
					Arena1Pieces[num].SetActive(value: true);
				}
				for (int num2 = 0; num2 < Arena2Pieces.Count; num2++)
				{
					Arena2Pieces[num2].SetActive(value: false);
				}
				for (int num3 = 0; num3 < Arena3Pieces.Count; num3++)
				{
					Arena3Pieces[num3].SetActive(value: false);
				}
				break;
			}
			case 2:
			{
				Wieners_Arena1.SetActive(value: false);
				Wieners_Arena2.SetActive(value: true);
				for (int num4 = 0; num4 < Arena1Pieces.Count; num4++)
				{
					Arena1Pieces[num4].SetActive(value: false);
				}
				for (int num5 = 0; num5 < Arena2Pieces.Count; num5++)
				{
					Arena2Pieces[num5].SetActive(value: true);
				}
				for (int num6 = 0; num6 < Arena3Pieces.Count; num6++)
				{
					Arena3Pieces[num6].SetActive(value: false);
				}
				break;
			}
			case 3:
			{
				Wieners_Arena1.SetActive(value: false);
				Wieners_Arena2.SetActive(value: false);
				for (int i = 0; i < Arena1Pieces.Count; i++)
				{
					Arena1Pieces[i].SetActive(value: false);
				}
				for (int j = 0; j < Arena2Pieces.Count; j++)
				{
					Arena2Pieces[j].SetActive(value: false);
				}
				for (int k = 0; k < Arena3Pieces.Count; k++)
				{
					Arena3Pieces[k].SetActive(value: true);
				}
				break;
			}
			}
		}

		public void BeginGame()
		{
			GM.CurrentPlayerBody.ResetHealth();
			if (m_selectedMode != 0)
			{
				m_isPlaying = true;
				if (IsBattleMusicOn)
				{
					StartMusic();
				}
				switch (m_selectedMode)
				{
				case HG_ModeManager.HG_Mode.TargetRelay_Sprint:
					SetArenaVisibility(1);
					ModeManagers[0].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.TargetRelay_Jog:
					SetArenaVisibility(1);
					ModeManagers[0].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.TargetRelay_Marathon:
					SetArenaVisibility(1);
					ModeManagers[0].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.AssaultNPepper_Skirmish:
					SetArenaVisibility(1);
					ModeManagers[1].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.AssaultNPepper_Brawl:
					SetArenaVisibility(1);
					ModeManagers[1].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.AssaultNPepper_Maelstrom:
					SetArenaVisibility(1);
					ModeManagers[1].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.MeatNMetal_Neophyte:
					SetArenaVisibility(1);
					ModeManagers[2].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.MeatNMetal_Warrior:
					SetArenaVisibility(1);
					ModeManagers[2].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.MeatNMetal_Veteran:
					SetArenaVisibility(1);
					ModeManagers[2].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.BattlePetite_Open:
					SetArenaVisibility(2);
					ModeManagers[3].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.BattlePetite_Sosiggun:
					SetArenaVisibility(2);
					ModeManagers[3].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.BattlePetite_Melee:
					SetArenaVisibility(2);
					ModeManagers[3].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.KingOfTheGrill_Invasion:
					SetArenaVisibility(2);
					ModeManagers[4].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.KingOfTheGrill_Resurrection:
					SetArenaVisibility(2);
					ModeManagers[4].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.KingOfTheGrill_Anachronism:
					SetArenaVisibility(2);
					ModeManagers[4].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.MeatleGear_Open:
					SetArenaVisibility(3);
					ModeManagers[5].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.MeatleGear_ScavengingSnake:
					SetArenaVisibility(3);
					ModeManagers[5].InitMode(m_selectedMode);
					break;
				case HG_ModeManager.HG_Mode.MeatleGear_ThirdSnake:
					SetArenaVisibility(3);
					ModeManagers[5].InitMode(m_selectedMode);
					break;
				}
			}
		}

		public void StartMusic()
		{
			m_musicVolume = 0f;
			m_isMusicPlaying = true;
		}

		public void FadeOutMusic()
		{
			m_isMusicPlaying = false;
		}

		private void MusicUpdate()
		{
			if (!IsBattleMusicOn)
			{
				return;
			}
			if (m_isMusicPlaying)
			{
				if (!AudSource_BattleMusic.isPlaying)
				{
					AudSource_BattleMusic.Play();
				}
				if (m_musicVolume < 0.2f)
				{
					m_musicVolume += Time.deltaTime * 0.2f;
					AudSource_BattleMusic.volume = m_musicVolume;
				}
				else
				{
					m_musicVolume = 0.2f;
					AudSource_BattleMusic.volume = m_musicVolume;
				}
			}
			else if (m_musicVolume > 0f)
			{
				m_musicVolume -= Time.deltaTime * 0.2f;
				AudSource_BattleMusic.volume = m_musicVolume;
			}
			else
			{
				m_musicVolume = 0f;
				AudSource_BattleMusic.volume = 0f;
				if (AudSource_BattleMusic.isPlaying)
				{
					AudSource_BattleMusic.Stop();
				}
			}
		}

		public void IsNoLongerPlaying()
		{
			m_isPlaying = false;
		}

		public void Case()
		{
			m_getCase = true;
		}

		public void EndModeScore()
		{
			if (m_getCase)
			{
				m_getCase = false;
				GM.MMFlags.AddHat(CaseID.ItemID);
				smeme.UpdateInventory();
				smeme.DrawInventory();
				GM.MMFlags.SaveToFile();
			}
			SetArenaVisibility(0);
			Wieners_Arena1.SetActive(value: true);
			Wieners_Arena2.SetActive(value: true);
			m_isPlaying = false;
			SubmitScoreAndGoToBoard();
		}

		private void InitScoring()
		{
			if (SteamManager.Initialized)
			{
				m_name = SteamFriends.GetPersonaName();
				m_ID = SteamUser.GetSteamID();
			}
		}

		private void Update()
		{
			UpdateHighScoreCallbacks();
			MusicUpdate();
		}

		public void SwitchToModeID(string id)
		{
			m_curSequenceID = id;
			m_hasPreviousScore = false;
			m_previousScore = 0;
			m_hasRankInCurrentSequence = false;
			m_rankInCurrentSequence = 0;
			UIM.RedrawHighScoreDisplay(m_curSequenceID);
			RequestHighScoreChart();
		}

		private void ProcessHighScore(int score)
		{
			m_hasCurrentScore = true;
			m_currentScore = score;
			HSM.UpdateScore(m_curSequenceID, score, HandleProcessHighScore);
		}

		public void HandleProcessHighScore(int prevRank, int newRank)
		{
			m_currentRank = newRank;
			m_doRequestScoresTop = true;
		}

		private void RequestHighScoreChart()
		{
			UIM.ClearGlobalHighScoreDisplay();
			Debug.Log("Requesting High Score Chart");
			m_hasScoresTop = false;
			m_hasScoresPlayer = false;
			m_scoresTop = null;
			m_scoresPlayer = null;
			m_doRequestScoresTop = true;
		}

		public void HandleHighScoreChartTop(List<HighScoreManager.HighScore> scores)
		{
			Debug.Log("HighScore Top Chart Received");
			m_hasScoresTop = true;
			m_scoresTop = scores;
			m_doRequestScoresPlayer = true;
		}

		public void HandleHighScoreChartPlayer(List<HighScoreManager.HighScore> scores)
		{
			Debug.Log("HighScore Player Chart Received");
			m_hasScoresPlayer = true;
			m_scoresPlayer = scores;
			RedrawHighScoreChart();
		}

		private void UpdateHighScoreCallbacks()
		{
			if (m_doRequestScoresTop)
			{
				m_doRequestScoresTop = false;
				HSM.GetLeaderboards(m_curSequenceID, 1, 6, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, HandleHighScoreChartTop);
			}
			if (m_doRequestScoresPlayer)
			{
				m_doRequestScoresPlayer = false;
				HSM.GetLeaderboards(m_curSequenceID, -1, 1, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, HandleHighScoreChartPlayer);
			}
		}

		private void RedrawHighScoreChart()
		{
			Debug.Log("m_hasScoresTop:" + m_hasScoresTop + " m_hasScoresPlayer:" + m_hasScoresPlayer);
			if (!m_hasScoresTop || !m_hasScoresPlayer)
			{
				return;
			}
			List<HighScoreManager.HighScore> list = new List<HighScoreManager.HighScore>();
			for (int i = 0; i < m_scoresTop.Count; i++)
			{
				list.Add(m_scoresTop[i]);
			}
			if (m_scoresPlayer.Count > 0)
			{
				int rank = m_scoresPlayer[0].rank;
				int num = 0;
				int num2 = 0;
				for (int j = 0; j < m_scoresPlayer.Count; j++)
				{
					if (m_scoresPlayer[j].steamID == m_ID)
					{
						num = j;
						num2 = m_scoresPlayer[j].rank;
						if (!m_hasCurrentScore)
						{
							m_currentScore = m_scoresPlayer[j].score;
							m_currentRank = m_scoresPlayer[j].rank;
							m_hasCurrentScore = true;
						}
						m_hasPreviousScore = true;
						m_hasRankInCurrentSequence = true;
						m_previousScore = m_scoresPlayer[j].score;
						m_rankInCurrentSequence = m_scoresPlayer[j].rank;
					}
				}
				if (num2 > 6)
				{
					int num3 = 0;
					for (int k = 6 - m_scoresPlayer.Count; k < 6; k++)
					{
						list[k] = m_scoresPlayer[num3];
						num3++;
					}
				}
			}
			UIM.SetGlobalHighScoreDisplay(list);
		}

		private void SubmitScoreAndGoToBoard()
		{
			int score = m_curModeManager.GetScore();
			GM.Omni.OmniFlags.AddScore(m_curSequenceID, m_curModeManager.GetScore());
			GM.Omni.SaveToFile();
			if (m_usingSteamworks)
			{
				ProcessHighScore(score);
			}
			UIM.RedrawHighScoreDisplay(m_curSequenceID);
			UIM.SetScoringReadouts(m_curModeManager.GetScoringReadOuts());
		}
	}
}
