using System.Collections.Generic;
using RUST.Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class OmniSequencer : MonoBehaviour
	{
		public enum State
		{
			ReadyToStart = 1,
			InWarmUp,
			InWave,
			InCleanup,
			SequenceCompleted
		}

		public enum OmniMenuCanvas
		{
			Root,
			List,
			Details,
			HighScore
		}

		[Header("Object Connections")]
		public GameObject ItemSpawner;

		public GameObject TrashCan;

		public OmniScoreManager ScoreManager;

		private bool m_usingSteamworks;

		public OmniSequenceLibrary Library;

		[Header("Audio")]
		public AudioSource AudSource;

		public AudioClip AudClip_SequenceStart;

		public AudioClip AudClip_SequenceEnd;

		public AudioClip AudClip_WaveStart;

		public AudioClip AudClip_WaveEnd;

		private bool m_hasPlayedStartingSound;

		private int m_waveIndex;

		private float m_timeLeftInWarmup;

		private float m_timeLeftInWave;

		private bool m_waveUsesQuickdraw;

		private bool m_waveUsesReflex;

		private float m_scoremult_Points = 1f;

		private float m_scoremult_Time = 1f;

		private float m_scoremult_Range = 1f;

		private int m_score;

		private bool m_isEarlyAbort;

		private bool m_isLatestScoreARankIncrease;

		private bool m_shouldDisplayLatestCurrencyEarned;

		private string m_currencyGainMessage = string.Empty;

		private string m_youAreNowRankMessage = string.Empty;

		[Header("State")]
		public State m_state = State.ReadyToStart;

		private List<OmniSpawner> m_spawners = new List<OmniSpawner>();

		public GameObject Menu;

		public GameObject GameMenu;

		[Header("UI Canvas Connections")]
		public GameObject Canvas_Menu_Root;

		public GameObject Canvas_Menu_SequenceList;

		public GameObject Canvas_Menu_SequenceDetails;

		public GameObject Canvas_Menu_HighScore;

		public GameObject Canvas_InWarmUp;

		public GameObject Canvas_InWave;

		public GameObject Canvas_EndOfSequence;

		public GameObject Canvas_Abort;

		[Header("Game UI In Warmup")]
		public Text WarmUp_Time;

		public Text WarmUp_Wave;

		public Text WarmUp_Instruction;

		public Text InWave_Time;

		public Text InWave_Wave;

		[Header("Game UI Sequence End")]
		public Text FinalDisplay;

		public Text FinalScore;

		public Keyboard FinalKeyboard;

		public Text LocalPlayerNameDisplay;

		[Header("UI Root")]
		public Sprite[] ThemeSprites;

		[Header("UI Sequence List")]
		public Text UIL_ThemeName;

		public Image UIL_ThemeImage;

		public Text[] UIL_SequenceList;

		public Text UIL_ThemeDetails;

		public Image[] UIL_SequenceAwardList;

		public GameObject UIL_ArrowNext;

		public GameObject UIL_ArrowPrev;

		public Sprite[] UIL_TrophySprites;

		[Header("UI Sequence Details")]
		public Text UID_SequenceName;

		public Text UID_SequenceTheme;

		public Text UID_SequenceDetails;

		public Text UID_SequenceDifficulty;

		public Text UID_SequenceFirearmType;

		public Text UID_SequenceAmmoMode;

		public Text UID_SequenceAllowedEquipmentList;

		public Text UID_SequenceWaveCount;

		public GameObject UID_BeginSequenceButton;

		public Image UID_SequenceImage;

		public GameObject IllegalEquipmentLabel;

		public GameObject BeginSequenceButton;

		private bool m_isIllegalEquipmentHeld;

		private float m_equipmentCheckTick = 0.5f;

		[Header("UI Sequence Highscores")]
		public Text UIH_SequenceName;

		public Text UIH_NewRank;

		public Text UIH_CurrencyMessage;

		public Text[] UIH_Scores_Global;

		public Text[] UIH_Scores_Local;

		public Image[] UIH_WeinerTrophies;

		public Color UIH_WeinerTrophy_Dark;

		public Color UIH_WeinerTrophy_Light;

		public ParticleSystem UIH_PacketRain;

		public AudioSource UIH_PacketRainAudio;

		public AudioClip UIH_PacketDingClip;

		private float m_packetRainEmissionRate;

		private float m_packetRainTimeTilShutOff;

		private bool m_isPacketRaining;

		private OmniSequencerSequenceDefinition SequenceDef;

		private OmniMenuCanvas m_curMenuCanvas;

		private OmniSequencerSequenceDefinition.OmniSequenceTheme m_curTheme = OmniSequencerSequenceDefinition.OmniSequenceTheme.CasualPlinking;

		private int m_sequencePage;

		private int m_maxSequencePage;

		private void Awake()
		{
			m_state = State.ReadyToStart;
			Menu.SetActive(value: true);
			GameMenu.SetActive(value: false);
			if (SteamManager.Initialized)
			{
				m_usingSteamworks = true;
			}
		}

		private void Update()
		{
			UpdateSequencer();
			UpdatePacketRain();
		}

		private void UpdateSequencer()
		{
			switch (m_state)
			{
			case State.ReadyToStart:
				UpdateReadyToStart();
				break;
			case State.InWarmUp:
				UpdateInWarmUp();
				break;
			case State.InWave:
				UpdateInWave();
				break;
			case State.InCleanup:
				UpdateInCleanup();
				break;
			case State.SequenceCompleted:
				UpdateSequenceCompleted();
				break;
			}
		}

		private void UpdateReadyToStart()
		{
			EquipmentCheckLoop();
		}

		private void UpdateInWarmUp()
		{
			if (m_waveIndex == 0 && !m_hasPlayedStartingSound && m_timeLeftInWarmup < 3.2f)
			{
				m_hasPlayedStartingSound = true;
				AudSource.clip = AudClip_SequenceStart;
				AudSource.Play();
			}
			bool flag = false;
			if (m_waveUsesQuickdraw)
			{
				for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
				{
					if (GM.CurrentMovementManager.Hands[i].CurrentInteractable != null && GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArm)
					{
						flag = true;
						if (m_timeLeftInWarmup < 2f)
						{
							m_timeLeftInWarmup = 2f;
						}
					}
				}
			}
			if (m_timeLeftInWarmup > 0f)
			{
				if (!flag)
				{
					m_timeLeftInWarmup -= Time.deltaTime;
				}
			}
			else
			{
				m_timeLeftInWarmup = 0f;
				for (int j = 0; j < m_spawners.Count; j++)
				{
					m_spawners[j].BeginSpawning();
				}
				m_state = State.InWave;
				AudSource.clip = AudClip_WaveStart;
				AudSource.Play();
				Canvas_InWarmUp.SetActive(value: false);
				Canvas_InWave.SetActive(value: true);
				Canvas_EndOfSequence.SetActive(value: false);
			}
			if (flag)
			{
				if (!WarmUp_Instruction.gameObject.activeSelf)
				{
					WarmUp_Instruction.gameObject.SetActive(value: true);
				}
				WarmUp_Instruction.text = "Holster Your Firearms!";
				if (WarmUp_Time.gameObject.activeSelf)
				{
					WarmUp_Time.gameObject.SetActive(value: false);
				}
			}
			else
			{
				if (WarmUp_Instruction.gameObject.activeSelf)
				{
					WarmUp_Instruction.gameObject.SetActive(value: false);
				}
				if (!WarmUp_Time.gameObject.activeSelf)
				{
					WarmUp_Time.gameObject.SetActive(value: true);
				}
				if (m_waveUsesReflex)
				{
					WarmUp_Time.text = "Get Ready!";
				}
				else
				{
					m_timeLeftInWarmup = Mathf.Clamp(m_timeLeftInWarmup, 0f, m_timeLeftInWarmup);
					WarmUp_Time.text = FloatToTime(m_timeLeftInWarmup, "#0:00.00");
				}
			}
			WarmUp_Wave.text = "Warmup For Wave " + (m_waveIndex + 1);
		}

		private void UpdateInWave()
		{
			bool flag = true;
			for (int i = 0; i < m_spawners.Count; i++)
			{
				if (!m_spawners[i].IsReadyForWaveEnd())
				{
					flag = false;
				}
			}
			if (m_timeLeftInWave <= 0f || flag)
			{
				float timeLeftInWave = m_timeLeftInWave;
				m_timeLeftInWave = 0f;
				for (int j = 0; j < m_spawners.Count; j++)
				{
					m_spawners[j].EndSpawning();
					int num = 0;
					num += (int)((float)m_spawners[j].Deactivate() * m_scoremult_Points);
					num += (int)(m_scoremult_Range * RangeToScoreMultiplier(m_spawners[j].GetEngagementRange()) * (float)num);
					m_score += num;
				}
				if (timeLeftInWave > 0f)
				{
					m_score += (int)(timeLeftInWave * 100f * m_scoremult_Time);
				}
				AudSource.clip = AudClip_WaveEnd;
				AudSource.Play();
				m_state = State.InCleanup;
			}
			else
			{
				m_timeLeftInWave -= Time.deltaTime;
			}
			m_timeLeftInWave = Mathf.Clamp(m_timeLeftInWave, 0f, m_timeLeftInWave);
			InWave_Time.text = FloatToTime(m_timeLeftInWave, "#0:00.00");
			InWave_Wave.text = "Wave " + (m_waveIndex + 1);
		}

		private void UpdateInCleanup()
		{
			bool flag = true;
			for (int i = 0; i < m_spawners.Count; i++)
			{
				if (m_spawners[i] != null && m_spawners[i].GetState() != 0)
				{
					flag = false;
				}
			}
			if (flag)
			{
				CleanupWave();
				if (m_waveIndex < SequenceDef.Waves.Count - 1)
				{
					m_waveIndex++;
					InitiateWave();
					return;
				}
				AudSource.clip = AudClip_SequenceEnd;
				AudSource.Play();
				m_state = State.SequenceCompleted;
				SequenceCompletedScreen();
			}
		}

		private void UpdateSequenceCompleted()
		{
			Canvas_InWarmUp.SetActive(value: false);
			Canvas_InWave.SetActive(value: false);
			Canvas_EndOfSequence.SetActive(value: true);
			Canvas_Abort.SetActive(value: false);
		}

		public bool InitiateSelectedSequence()
		{
			if (SequenceDef == null || m_state != State.ReadyToStart)
			{
				return false;
			}
			BeginSequence();
			return true;
		}

		public void BeginSequence()
		{
			if (m_state != State.ReadyToStart)
			{
				Debug.LogError("Attempted to begin sequence when not in ReadyToStart state. This should not be possible.");
				return;
			}
			UpdateIllegalEquipmentCheck();
			if (!m_isIllegalEquipmentHeld)
			{
				DisableIllegalEquipment();
				Menu.SetActive(value: false);
				GameMenu.SetActive(value: true);
				ItemSpawner.SetActive(value: false);
				TrashCan.SetActive(value: false);
				Canvas_Abort.SetActive(value: true);
				m_hasPlayedStartingSound = false;
				m_waveIndex = 0;
				InitiateWave();
			}
		}

		public void AbortSequence()
		{
			m_timeLeftInWarmup = 0f;
			m_timeLeftInWave = 0f;
			m_waveIndex = SequenceDef.Waves.Count - 1;
			m_state = State.InCleanup;
			for (int i = 0; i < m_spawners.Count; i++)
			{
				m_spawners[i].EndSpawning();
				m_spawners[i].Deactivate();
			}
			m_isEarlyAbort = true;
		}

		private void InitiateWave()
		{
			Debug.Log("Initiating Wave:" + m_waveIndex);
			if (m_waveIndex >= SequenceDef.Waves.Count)
			{
				Debug.LogError("Attempted to Initiate Out of Bound Wave. Cleanup phase should have caught this and passed us to sequence completed instead.");
				return;
			}
			OmniSequencerWaveDefinition omniSequencerWaveDefinition = SequenceDef.Waves[m_waveIndex];
			for (int i = 0; i < omniSequencerWaveDefinition.Spawners.Count; i++)
			{
				OmniSpawnDef def = omniSequencerWaveDefinition.Spawners[i].Def;
				Vector3 position = new Vector3(0f, -100f, GetRange(omniSequencerWaveDefinition.Spawners[i].Range));
				Quaternion identity = Quaternion.identity;
				GameObject gameObject = Object.Instantiate(def.SpawnerPrefab, position, identity);
				OmniSpawner component = gameObject.GetComponent<OmniSpawner>();
				m_spawners.Add(component);
				component.Configure(def, omniSequencerWaveDefinition.Spawners[i].Range);
				component.Activate();
			}
			m_state = State.InWarmUp;
			m_timeLeftInWarmup = omniSequencerWaveDefinition.TimeForWarmup + Random.Range(0f, omniSequencerWaveDefinition.TimeForWarmupRandomAddition);
			m_timeLeftInWave = omniSequencerWaveDefinition.TimeForWave;
			m_waveUsesQuickdraw = omniSequencerWaveDefinition.UsesQuickDraw;
			m_waveUsesReflex = omniSequencerWaveDefinition.UsesReflex;
			m_scoremult_Points = omniSequencerWaveDefinition.ScoreMultiplier_Points;
			m_scoremult_Range = omniSequencerWaveDefinition.ScoreMultiplier_Range;
			m_scoremult_Time = omniSequencerWaveDefinition.ScoreMultiplier_Time;
			Canvas_InWarmUp.SetActive(value: true);
			Canvas_InWave.SetActive(value: false);
			Canvas_EndOfSequence.SetActive(value: false);
		}

		private void CleanupWave()
		{
			for (int num = m_spawners.Count - 1; num >= 0; num--)
			{
				Object.Destroy(m_spawners[num]);
			}
			m_spawners.Clear();
		}

		private void SequenceCompletedScreen()
		{
			EnableAllEquipment();
			ItemSpawner.SetActive(value: true);
			TrashCan.SetActive(value: true);
			Canvas_Abort.SetActive(value: false);
			if (!m_isEarlyAbort)
			{
				FinalScore.text = m_score.ToString();
				FinalDisplay.text = SequenceDef.SequenceName + " Completed";
				LocalPlayerNameDisplay.text = GM.Omni.OmniFlags.StoredPlayerName;
				FinalKeyboard.SetActiveText(LocalPlayerNameDisplay);
			}
			else
			{
				m_isEarlyAbort = false;
				m_score = 0;
				GameMenu.SetActive(value: false);
				Menu.SetActive(value: true);
				SetMenuCanvas(OmniMenuCanvas.Details);
			}
		}

		protected float GetRange(OmniWaveEngagementRange r)
		{
			return r switch
			{
				OmniWaveEngagementRange.m5 => 5f, 
				OmniWaveEngagementRange.m10 => 10f, 
				OmniWaveEngagementRange.m15 => 15f, 
				OmniWaveEngagementRange.m20 => 20f, 
				OmniWaveEngagementRange.m25 => 25f, 
				OmniWaveEngagementRange.m50 => 50f, 
				OmniWaveEngagementRange.m100 => 100f, 
				OmniWaveEngagementRange.m150 => 150f, 
				OmniWaveEngagementRange.m200 => 200f, 
				_ => 0f, 
			};
		}

		private void SetMenuCanvas(OmniMenuCanvas canvas)
		{
			m_curMenuCanvas = canvas;
			switch (canvas)
			{
			case OmniMenuCanvas.Root:
				if (!Canvas_Menu_Root.activeSelf)
				{
					Canvas_Menu_Root.SetActive(value: true);
				}
				if (Canvas_Menu_SequenceList.activeSelf)
				{
					Canvas_Menu_SequenceList.SetActive(value: false);
				}
				if (Canvas_Menu_SequenceDetails.activeSelf)
				{
					Canvas_Menu_SequenceDetails.SetActive(value: false);
				}
				if (Canvas_Menu_HighScore.activeSelf)
				{
					Canvas_Menu_HighScore.SetActive(value: false);
				}
				RedrawMenu_Root();
				break;
			case OmniMenuCanvas.List:
				if (Canvas_Menu_Root.activeSelf)
				{
					Canvas_Menu_Root.SetActive(value: false);
				}
				if (!Canvas_Menu_SequenceList.activeSelf)
				{
					Canvas_Menu_SequenceList.SetActive(value: true);
				}
				if (Canvas_Menu_SequenceDetails.activeSelf)
				{
					Canvas_Menu_SequenceDetails.SetActive(value: false);
				}
				if (Canvas_Menu_HighScore.activeSelf)
				{
					Canvas_Menu_HighScore.SetActive(value: false);
				}
				RedrawMenu_List();
				break;
			case OmniMenuCanvas.Details:
				if (Canvas_Menu_Root.activeSelf)
				{
					Canvas_Menu_Root.SetActive(value: false);
				}
				if (Canvas_Menu_SequenceList.activeSelf)
				{
					Canvas_Menu_SequenceList.SetActive(value: false);
				}
				if (!Canvas_Menu_SequenceDetails.activeSelf)
				{
					Canvas_Menu_SequenceDetails.SetActive(value: true);
				}
				if (Canvas_Menu_HighScore.activeSelf)
				{
					Canvas_Menu_HighScore.SetActive(value: false);
				}
				RedrawMenu_Details();
				m_state = State.ReadyToStart;
				break;
			case OmniMenuCanvas.HighScore:
				if (Canvas_Menu_Root.activeSelf)
				{
					Canvas_Menu_Root.SetActive(value: false);
				}
				if (Canvas_Menu_SequenceList.activeSelf)
				{
					Canvas_Menu_SequenceList.SetActive(value: false);
				}
				if (Canvas_Menu_SequenceDetails.activeSelf)
				{
					Canvas_Menu_SequenceDetails.SetActive(value: false);
				}
				if (!Canvas_Menu_HighScore.activeSelf)
				{
					Canvas_Menu_HighScore.SetActive(value: true);
				}
				RedrawMenu_HighScore();
				break;
			}
		}

		private void RedrawMenu_Root()
		{
		}

		private void RedrawMenu_List()
		{
			UIL_ThemeName.text = m_curTheme.ToString();
			UIL_ThemeImage.sprite = Library.Themes[(int)m_curTheme].Sprite;
			UIL_ThemeDetails.text = Library.Themes[(int)m_curTheme].ThemeDetails;
			UIL_ArrowPrev.SetActive(m_sequencePage > 0);
			UIL_ArrowNext.SetActive(m_sequencePage < m_maxSequencePage);
			int num = Library.Themes[(int)m_curTheme].SequenceList.Length;
			int num2 = num;
			num = Mathf.Min(num, 8);
			if (num <= 0)
			{
				for (int i = 1; i < UIL_SequenceList.Length; i++)
				{
					UIL_SequenceList[i].gameObject.SetActive(value: false);
				}
				UIL_SequenceList[0].gameObject.SetActive(value: true);
				UIL_SequenceList[0].text = "Sequences Coming Soon";
				return;
			}
			for (int j = 0; j < 8; j++)
			{
				UIL_SequenceList[j].gameObject.SetActive(value: true);
				int num3 = m_sequencePage * 8 + j;
				if (num3 < num2)
				{
					UIL_SequenceList[j].text = num3 + 1 + ". " + Library.Themes[(int)m_curTheme].SequenceList[num3].SequenceName;
					int rank = GM.Omni.OmniFlags.GetRank(Library.Themes[(int)m_curTheme].SequenceList[num3].SequenceID);
					if (rank == 3)
					{
						UIL_SequenceAwardList[j].gameObject.SetActive(value: false);
						continue;
					}
					UIL_SequenceAwardList[j].gameObject.SetActive(value: true);
					UIL_SequenceAwardList[j].sprite = UIL_TrophySprites[rank];
				}
				else
				{
					UIL_SequenceList[j].gameObject.SetActive(value: false);
					UIL_SequenceAwardList[j].gameObject.SetActive(value: false);
				}
			}
		}

		private void RedrawMenu_Details()
		{
			UID_SequenceTheme.text = SequenceDef.Theme.ToString();
			UID_SequenceName.text = SequenceDef.SequenceName;
			UID_SequenceDetails.text = SequenceDef.Description;
			UID_SequenceDifficulty.text = "Difficulty: " + SequenceDef.Difficulty;
			UID_SequenceFirearmType.text = "Firearm Mode: " + SequenceDef.FirearmMode;
			UID_SequenceAmmoMode.text = "Ammo Mode: " + SequenceDef.AmmoMode;
			UID_SequenceWaveCount.text = "Waves: " + SequenceDef.Waves.Count;
			UID_SequenceImage.sprite = Library.Themes[(int)m_curTheme].Sprite;
			if (SequenceDef.FirearmMode == OmniSequencerSequenceDefinition.OmniSequenceFirearmMode.Open)
			{
				UID_SequenceAllowedEquipmentList.gameObject.SetActive(value: false);
			}
			else
			{
				UID_SequenceAllowedEquipmentList.gameObject.SetActive(value: true);
				string text = "Equipment Types Allowed:\n";
				for (int i = 0; i < SequenceDef.AllowedCats.Count; i++)
				{
					if (i > 0)
					{
						text += ", ";
					}
					string text2 = ItemSpawnerID.SubCatNames[(int)SequenceDef.AllowedCats[i]];
					text += text2;
				}
				UID_SequenceAllowedEquipmentList.text = text;
			}
			BeginSequenceButton.SetActive(value: true);
		}

		public void ClearGlobalHighScoreDisplay()
		{
			for (int i = 0; i < 6; i++)
			{
				UIH_Scores_Global[i].gameObject.SetActive(value: false);
			}
		}

		public void SetGlobalHighScoreDisplay(List<HighScoreManager.HighScore> scores)
		{
			for (int i = 0; i < 6; i++)
			{
				if (scores.Count > i)
				{
					UIH_Scores_Global[i].gameObject.SetActive(value: true);
					UIH_Scores_Global[i].text = scores[i].rank.ToString() + ". " + scores[i].score + " - " + scores[i].name;
				}
				else
				{
					UIH_Scores_Global[i].gameObject.SetActive(value: false);
				}
			}
		}

		private void RedrawMenu_HighScore()
		{
			UIH_SequenceName.text = "HighScores for " + SequenceDef.SequenceName;
			List<OmniScore> scoreList = GM.Omni.OmniFlags.GetScoreList(SequenceDef.SequenceID);
			for (int i = 0; i < scoreList.Count; i++)
			{
				UIH_Scores_Local[i].gameObject.SetActive(value: true);
				UIH_Scores_Local[i].text = (i + 1).ToString() + ": " + scoreList[i].Score + " - " + scoreList[i].Name;
			}
			for (int j = scoreList.Count; j < UIH_Scores_Local.Length; j++)
			{
				UIH_Scores_Local[j].gameObject.SetActive(value: false);
			}
			if (m_isLatestScoreARankIncrease)
			{
				m_isLatestScoreARankIncrease = false;
				UIH_NewRank.gameObject.SetActive(value: true);
				UIH_NewRank.text = m_youAreNowRankMessage;
			}
			else
			{
				UIH_NewRank.gameObject.SetActive(value: false);
			}
			if (m_shouldDisplayLatestCurrencyEarned)
			{
				m_shouldDisplayLatestCurrencyEarned = false;
				UIH_CurrencyMessage.gameObject.SetActive(value: true);
				UIH_CurrencyMessage.text = m_currencyGainMessage;
			}
			else
			{
				UIH_CurrencyMessage.gameObject.SetActive(value: false);
			}
			int rank = GM.Omni.OmniFlags.GetRank(SequenceDef.SequenceID);
			for (int k = 0; k < UIH_WeinerTrophies.Length; k++)
			{
				UIH_WeinerTrophies[k].color = UIH_WeinerTrophy_Dark;
			}
			if (rank < 3 && rank >= 0)
			{
				UIH_WeinerTrophies[rank].color = UIH_WeinerTrophy_Light;
			}
		}

		public void SubmitScoreAndGoToBoard()
		{
			int rank = GM.Omni.OmniFlags.GetRank(SequenceDef.SequenceID);
			GM.Omni.OmniFlags.StoredPlayerName = LocalPlayerNameDisplay.text;
			GM.Omni.OmniFlags.AddScore(SequenceDef, m_score);
			GM.Omni.SaveToFile();
			int rank2 = GM.Omni.OmniFlags.GetRank(SequenceDef.SequenceID);
			int rankForScore = SequenceDef.GetRankForScore(m_score);
			int num = SequenceDef.CurrencyForRanks[rankForScore];
			num = (int)Random.Range((float)num * 0.8f, (float)num * 1.1f);
			if (rank2 < rank)
			{
				m_isLatestScoreARankIncrease = true;
			}
			int num2 = 0;
			if (m_isLatestScoreARankIncrease)
			{
				int num3 = Mathf.Abs(rank2 - rank);
				num2 = SequenceDef.CurrencyForRanks[4] * num3;
			}
			GM.Omni.OmniUnlocks.GainCurrency(num + num2);
			InitiatePacketRain(num + num2);
			GM.Omni.SaveUnlocksToFile();
			if (m_isLatestScoreARankIncrease)
			{
				m_currencyGainMessage = "You Earned\n" + num + " + " + num2 + "\nS.A.U.C.E.\nPackets!";
			}
			else
			{
				m_currencyGainMessage = "You Earned\n" + num + "\nS.A.U.C.E.\nPackets!";
			}
			if (m_isLatestScoreARankIncrease)
			{
				switch (rank2)
				{
				case 0:
					m_youAreNowRankMessage = "You Are Now Ranked\nGold Weenie!";
					break;
				case 1:
					m_youAreNowRankMessage = "You Are Now Ranked\nSilver Weenie!";
					break;
				case 2:
					m_youAreNowRankMessage = "You Are Now Ranked\nBronze Weenie!";
					break;
				case 3:
					m_youAreNowRankMessage = string.Empty;
					break;
				}
			}
			m_shouldDisplayLatestCurrencyEarned = true;
			if (m_usingSteamworks)
			{
				ScoreManager.ProcessHighScore(m_score);
			}
			m_score = 0;
			GameMenu.SetActive(value: false);
			Menu.SetActive(value: true);
			SetMenuCanvas(OmniMenuCanvas.HighScore);
		}

		public void BackToRoot()
		{
			SetAmmoMode(OmniSequencerSequenceDefinition.OmniSequenceAmmoMode.Infinite);
			SetMenuCanvas(OmniMenuCanvas.Root);
		}

		public void BackToSequenceList()
		{
			SetAmmoMode(OmniSequencerSequenceDefinition.OmniSequenceAmmoMode.Infinite);
			SequenceDef = null;
			UpdateIllegalEquipmentCheck();
			SetMenuCanvas(OmniMenuCanvas.List);
		}

		public void BackToSequenceDetails()
		{
			UpdateIllegalEquipmentCheck();
			SetMenuCanvas(OmniMenuCanvas.Details);
		}

		public void SelectTheme(int i)
		{
			m_curTheme = (OmniSequencerSequenceDefinition.OmniSequenceTheme)i;
			m_sequencePage = 0;
			m_maxSequencePage = Mathf.FloorToInt((Library.Themes[(int)m_curTheme].SequenceList.Length - 1) / 8);
			SetAmmoMode(OmniSequencerSequenceDefinition.OmniSequenceAmmoMode.Infinite);
			SetMenuCanvas(OmniMenuCanvas.List);
		}

		public void SelectSequence(int i)
		{
			int num = m_sequencePage * 8 + i;
			if (num < Library.Themes[(int)m_curTheme].SequenceList.Length)
			{
				SequenceDef = Library.Themes[(int)m_curTheme].SequenceList[num];
				if (m_usingSteamworks)
				{
					ScoreManager.SwitchToSequenceID(SequenceDef.SequenceID);
				}
				SetAmmoMode(SequenceDef.AmmoMode);
				SetMenuCanvas(OmniMenuCanvas.Details);
				UpdateIllegalEquipmentCheck();
			}
		}

		public void ViewHighScorePage()
		{
			SetMenuCanvas(OmniMenuCanvas.HighScore);
		}

		public void NextSequencePage()
		{
			if (m_sequencePage < m_maxSequencePage)
			{
				m_sequencePage++;
				RedrawMenu_List();
			}
		}

		public void PrevSequencePage()
		{
			if (m_sequencePage > 0)
			{
				m_sequencePage--;
				RedrawMenu_List();
			}
		}

		private void SetAmmoMode(OmniSequencerSequenceDefinition.OmniSequenceAmmoMode mode)
		{
			switch (mode)
			{
			case OmniSequencerSequenceDefinition.OmniSequenceAmmoMode.Infinite:
				GM.CurrentSceneSettings.IsAmmoInfinite = true;
				GM.CurrentSceneSettings.IsSpawnLockingEnabled = true;
				break;
			case OmniSequencerSequenceDefinition.OmniSequenceAmmoMode.Spawnlockable:
				GM.CurrentSceneSettings.IsAmmoInfinite = false;
				GM.CurrentSceneSettings.IsSpawnLockingEnabled = true;
				break;
			case OmniSequencerSequenceDefinition.OmniSequenceAmmoMode.Fixed:
				GM.CurrentSceneSettings.IsAmmoInfinite = false;
				GM.CurrentSceneSettings.IsSpawnLockingEnabled = false;
				break;
			}
		}

		private void InitiatePacketRain(int amount)
		{
			float num = Mathf.Clamp((float)amount * 0.25f, 50f, 400f);
			float packetRainTimeTilShutOff = Mathf.Clamp((float)amount / num, 0.1f, 4f);
			m_packetRainEmissionRate = num;
			m_packetRainTimeTilShutOff = packetRainTimeTilShutOff;
			m_isPacketRaining = true;
			ParticleSystem.EmissionModule emission = UIH_PacketRain.emission;
			emission.rateOverTime = Random.Range(m_packetRainEmissionRate, m_packetRainEmissionRate);
			UIH_PacketRainAudio.PlayOneShot(UIH_PacketDingClip, 1f);
			UIH_PacketRainAudio.Play();
		}

		private void UpdatePacketRain()
		{
			if (m_isPacketRaining)
			{
				m_packetRainTimeTilShutOff -= Time.deltaTime;
				if (m_packetRainTimeTilShutOff <= 0f)
				{
					m_isPacketRaining = false;
					ParticleSystem.EmissionModule emission = UIH_PacketRain.emission;
					emission.rateOverTime = 0f;
					UIH_PacketRainAudio.Stop();
				}
			}
		}

		private void EquipmentCheckLoop()
		{
			if (SequenceDef == null || SequenceDef.FirearmMode == OmniSequencerSequenceDefinition.OmniSequenceFirearmMode.Open)
			{
				if (IllegalEquipmentLabel.activeSelf)
				{
					IllegalEquipmentLabel.SetActive(value: false);
				}
				if (!UID_BeginSequenceButton.activeSelf)
				{
					UID_BeginSequenceButton.SetActive(value: true);
				}
			}
			else if (m_isIllegalEquipmentHeld)
			{
				if (!IllegalEquipmentLabel.activeSelf)
				{
					IllegalEquipmentLabel.SetActive(value: true);
				}
				if (UID_BeginSequenceButton.activeSelf)
				{
					UID_BeginSequenceButton.SetActive(value: false);
				}
			}
			else
			{
				if (IllegalEquipmentLabel.activeSelf)
				{
					IllegalEquipmentLabel.SetActive(value: false);
				}
				if (!UID_BeginSequenceButton.activeSelf)
				{
					UID_BeginSequenceButton.SetActive(value: true);
				}
			}
			if (m_curMenuCanvas == OmniMenuCanvas.Details)
			{
				if (m_equipmentCheckTick > 0f)
				{
					m_equipmentCheckTick -= Time.deltaTime;
					return;
				}
				m_equipmentCheckTick = 0.5f;
				UpdateIllegalEquipmentCheck();
			}
		}

		private bool IsEquipmentIllegal(ItemSpawnerID id)
		{
			if (SequenceDef == null || id == null)
			{
				return false;
			}
			if (id.Category == ItemSpawnerID.EItemCategory.Magazine || id.SubCategory == ItemSpawnerID.ESubCategory.None || id.SubCategory == ItemSpawnerID.ESubCategory.RailAdapter || id.SubCategory == ItemSpawnerID.ESubCategory.Suppressor)
			{
				return false;
			}
			return !SequenceDef.AllowedCats.Contains(id.SubCategory);
		}

		public void UpdateIllegalEquipmentCheck()
		{
			if (SequenceDef == null || SequenceDef.FirearmMode == OmniSequencerSequenceDefinition.OmniSequenceFirearmMode.Open)
			{
				m_isIllegalEquipmentHeld = false;
				return;
			}
			FVRPhysicalObject[] array = Object.FindObjectsOfType<FVRPhysicalObject>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IDSpawnedFrom == null || (!array[i].IsHeld && !(array[i].QuickbeltSlot != null)))
				{
					continue;
				}
				if (IsEquipmentIllegal(array[i].IDSpawnedFrom))
				{
					m_isIllegalEquipmentHeld = true;
					return;
				}
				if (array[i].Attachments.Count <= 0)
				{
					continue;
				}
				for (int j = 0; j < array[i].Attachments.Count; j++)
				{
					if (array[i].Attachments[j].IDSpawnedFrom != null && IsEquipmentIllegal(array[i].Attachments[j].IDSpawnedFrom))
					{
						m_isIllegalEquipmentHeld = true;
						return;
					}
				}
			}
			m_isIllegalEquipmentHeld = false;
		}

		public void DisableIllegalEquipment()
		{
			if (SequenceDef.FirearmMode != OmniSequencerSequenceDefinition.OmniSequenceFirearmMode.Category)
			{
				return;
			}
			FVRPhysicalObject[] array = Object.FindObjectsOfType<FVRPhysicalObject>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IDSpawnedFrom != null && IsEquipmentIllegal(array[i].IDSpawnedFrom))
				{
					array[i].IsPickUpLocked = true;
				}
				else
				{
					if (array[i].Attachments.Count <= 0)
					{
						continue;
					}
					for (int j = 0; j < array[i].Attachments.Count; j++)
					{
						if (array[i].Attachments[j].IDSpawnedFrom != null && array[i].Attachments[j].IDSpawnedFrom != null && IsEquipmentIllegal(array[i].Attachments[j].IDSpawnedFrom))
						{
							array[i].Attachments[j].IsPickUpLocked = true;
							array[i].IsPickUpLocked = true;
							break;
						}
					}
				}
			}
		}

		public void EnableAllEquipment()
		{
			FVRPhysicalObject[] array = Object.FindObjectsOfType<FVRPhysicalObject>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].IsPickUpLocked = false;
			}
		}

		public float RangeToScoreMultiplier(OmniWaveEngagementRange range)
		{
			return range switch
			{
				OmniWaveEngagementRange.m5 => 0f, 
				OmniWaveEngagementRange.m10 => 0.1f, 
				OmniWaveEngagementRange.m15 => 0.25f, 
				OmniWaveEngagementRange.m20 => 0.5f, 
				OmniWaveEngagementRange.m25 => 1f, 
				OmniWaveEngagementRange.m50 => 2f, 
				OmniWaveEngagementRange.m100 => 3f, 
				OmniWaveEngagementRange.m150 => 4f, 
				OmniWaveEngagementRange.m200 => 5f, 
				_ => 1f, 
			};
		}

		public string FloatToTime(float toConvert, string format)
		{
			return format switch
			{
				"00.0" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"#0.0" => $"{Mathf.Floor(toConvert) % 60f:#0}:{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"00.00" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"00.000" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#00.000" => $"{Mathf.Floor(toConvert) % 60f:#00}:{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#0:00" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}", 
				"#00:00" => $"{Mathf.Floor(toConvert / 60f):#00}:{Mathf.Floor(toConvert) % 60f:00}", 
				"0:00.0" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"#0:00.0" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"0:00.00" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"#0:00.00" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"0:00.000" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#0:00.000" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				_ => "error", 
			};
		}
	}
}
