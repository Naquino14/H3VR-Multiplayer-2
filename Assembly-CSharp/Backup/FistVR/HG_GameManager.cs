// Decompiled with JetBrains decompiler
// Type: FistVR.HG_GameManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using RUST.Steamworks;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class HG_GameManager : MonoBehaviour
  {
    [Header("Object Connections")]
    public HG_UIManager UIM;
    public SMEME smeme;
    public GameObject ItemSpawner;
    public GameObject TrashCan;
    private bool m_usingSteamworks;
    [Header("State")]
    public HG_GameManager.State m_state;
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
      this.InitScoring();
      if (!SteamManager.Initialized)
        return;
      this.m_usingSteamworks = true;
    }

    public void PlayerDied()
    {
      if (this.m_isPlaying && (UnityEngine.Object) this.m_curModeManager != (UnityEngine.Object) null)
        this.m_curModeManager.HandlePlayerDeath();
      else
        this.JobManager.HandlePlayerDeath();
    }

    public void ToggleMusic()
    {
      this.IsBattleMusicOn = !this.IsBattleMusicOn;
      if (this.IsBattleMusicOn)
        this.MusicToggle.text = "Sweet Battle Music: On";
      else
        this.MusicToggle.text = "Sweet Battle Music: off";
    }

    public void SelectGameMode(int i)
    {
      HG_ModeManager.HG_Mode hgMode = (HG_ModeManager.HG_Mode) i;
      this.m_selectedMode = hgMode;
      switch (this.m_selectedMode)
      {
        case HG_ModeManager.HG_Mode.TargetRelay_Sprint:
          this.m_curModeManager = this.ModeManagers[0];
          break;
        case HG_ModeManager.HG_Mode.TargetRelay_Jog:
          this.m_curModeManager = this.ModeManagers[0];
          break;
        case HG_ModeManager.HG_Mode.TargetRelay_Marathon:
          this.m_curModeManager = this.ModeManagers[0];
          break;
        case HG_ModeManager.HG_Mode.AssaultNPepper_Skirmish:
          this.m_curModeManager = this.ModeManagers[1];
          break;
        case HG_ModeManager.HG_Mode.AssaultNPepper_Brawl:
          this.m_curModeManager = this.ModeManagers[1];
          break;
        case HG_ModeManager.HG_Mode.AssaultNPepper_Maelstrom:
          this.m_curModeManager = this.ModeManagers[1];
          break;
        case HG_ModeManager.HG_Mode.MeatNMetal_Neophyte:
          this.m_curModeManager = this.ModeManagers[2];
          break;
        case HG_ModeManager.HG_Mode.MeatNMetal_Warrior:
          this.m_curModeManager = this.ModeManagers[2];
          break;
        case HG_ModeManager.HG_Mode.MeatNMetal_Veteran:
          this.m_curModeManager = this.ModeManagers[2];
          break;
        case HG_ModeManager.HG_Mode.BattlePetite_Open:
          this.m_curModeManager = this.ModeManagers[3];
          break;
        case HG_ModeManager.HG_Mode.BattlePetite_Sosiggun:
          this.m_curModeManager = this.ModeManagers[3];
          break;
        case HG_ModeManager.HG_Mode.BattlePetite_Melee:
          this.m_curModeManager = this.ModeManagers[3];
          break;
        case HG_ModeManager.HG_Mode.KingOfTheGrill_Invasion:
          this.m_curModeManager = this.ModeManagers[4];
          break;
        case HG_ModeManager.HG_Mode.KingOfTheGrill_Resurrection:
          this.m_curModeManager = this.ModeManagers[4];
          break;
        case HG_ModeManager.HG_Mode.KingOfTheGrill_Anachronism:
          this.m_curModeManager = this.ModeManagers[4];
          break;
        case HG_ModeManager.HG_Mode.MeatleGear_Open:
          this.m_curModeManager = this.ModeManagers[5];
          break;
        case HG_ModeManager.HG_Mode.MeatleGear_ScavengingSnake:
          this.m_curModeManager = this.ModeManagers[5];
          break;
        case HG_ModeManager.HG_Mode.MeatleGear_ThirdSnake:
          this.m_curModeManager = this.ModeManagers[5];
          break;
      }
      this.UIM.SetSelectedModeText(this.ModeProfiles[i]);
      this.SwitchToModeID("Meatmas2018" + hgMode.ToString());
    }

    public void SetArenaVisibility(int ar)
    {
      switch (ar)
      {
        case 0:
          for (int index = 0; index < this.Arena1Pieces.Count; ++index)
            this.Arena1Pieces[index].SetActive(true);
          for (int index = 0; index < this.Arena2Pieces.Count; ++index)
            this.Arena2Pieces[index].SetActive(true);
          for (int index = 0; index < this.Arena3Pieces.Count; ++index)
            this.Arena3Pieces[index].SetActive(true);
          break;
        case 1:
          this.Wieners_Arena1.SetActive(true);
          this.Wieners_Arena2.SetActive(false);
          for (int index = 0; index < this.Arena1Pieces.Count; ++index)
            this.Arena1Pieces[index].SetActive(true);
          for (int index = 0; index < this.Arena2Pieces.Count; ++index)
            this.Arena2Pieces[index].SetActive(false);
          for (int index = 0; index < this.Arena3Pieces.Count; ++index)
            this.Arena3Pieces[index].SetActive(false);
          break;
        case 2:
          this.Wieners_Arena1.SetActive(false);
          this.Wieners_Arena2.SetActive(true);
          for (int index = 0; index < this.Arena1Pieces.Count; ++index)
            this.Arena1Pieces[index].SetActive(false);
          for (int index = 0; index < this.Arena2Pieces.Count; ++index)
            this.Arena2Pieces[index].SetActive(true);
          for (int index = 0; index < this.Arena3Pieces.Count; ++index)
            this.Arena3Pieces[index].SetActive(false);
          break;
        case 3:
          this.Wieners_Arena1.SetActive(false);
          this.Wieners_Arena2.SetActive(false);
          for (int index = 0; index < this.Arena1Pieces.Count; ++index)
            this.Arena1Pieces[index].SetActive(false);
          for (int index = 0; index < this.Arena2Pieces.Count; ++index)
            this.Arena2Pieces[index].SetActive(false);
          for (int index = 0; index < this.Arena3Pieces.Count; ++index)
            this.Arena3Pieces[index].SetActive(true);
          break;
      }
    }

    public void BeginGame()
    {
      GM.CurrentPlayerBody.ResetHealth();
      if (this.m_selectedMode == HG_ModeManager.HG_Mode.None)
        return;
      this.m_isPlaying = true;
      if (this.IsBattleMusicOn)
        this.StartMusic();
      switch (this.m_selectedMode)
      {
        case HG_ModeManager.HG_Mode.TargetRelay_Sprint:
          this.SetArenaVisibility(1);
          this.ModeManagers[0].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.TargetRelay_Jog:
          this.SetArenaVisibility(1);
          this.ModeManagers[0].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.TargetRelay_Marathon:
          this.SetArenaVisibility(1);
          this.ModeManagers[0].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.AssaultNPepper_Skirmish:
          this.SetArenaVisibility(1);
          this.ModeManagers[1].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.AssaultNPepper_Brawl:
          this.SetArenaVisibility(1);
          this.ModeManagers[1].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.AssaultNPepper_Maelstrom:
          this.SetArenaVisibility(1);
          this.ModeManagers[1].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.MeatNMetal_Neophyte:
          this.SetArenaVisibility(1);
          this.ModeManagers[2].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.MeatNMetal_Warrior:
          this.SetArenaVisibility(1);
          this.ModeManagers[2].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.MeatNMetal_Veteran:
          this.SetArenaVisibility(1);
          this.ModeManagers[2].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.BattlePetite_Open:
          this.SetArenaVisibility(2);
          this.ModeManagers[3].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.BattlePetite_Sosiggun:
          this.SetArenaVisibility(2);
          this.ModeManagers[3].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.BattlePetite_Melee:
          this.SetArenaVisibility(2);
          this.ModeManagers[3].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.KingOfTheGrill_Invasion:
          this.SetArenaVisibility(2);
          this.ModeManagers[4].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.KingOfTheGrill_Resurrection:
          this.SetArenaVisibility(2);
          this.ModeManagers[4].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.KingOfTheGrill_Anachronism:
          this.SetArenaVisibility(2);
          this.ModeManagers[4].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.MeatleGear_Open:
          this.SetArenaVisibility(3);
          this.ModeManagers[5].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.MeatleGear_ScavengingSnake:
          this.SetArenaVisibility(3);
          this.ModeManagers[5].InitMode(this.m_selectedMode);
          break;
        case HG_ModeManager.HG_Mode.MeatleGear_ThirdSnake:
          this.SetArenaVisibility(3);
          this.ModeManagers[5].InitMode(this.m_selectedMode);
          break;
      }
    }

    public void StartMusic()
    {
      this.m_musicVolume = 0.0f;
      this.m_isMusicPlaying = true;
    }

    public void FadeOutMusic() => this.m_isMusicPlaying = false;

    private void MusicUpdate()
    {
      if (!this.IsBattleMusicOn)
        return;
      if (this.m_isMusicPlaying)
      {
        if (!this.AudSource_BattleMusic.isPlaying)
          this.AudSource_BattleMusic.Play();
        if ((double) this.m_musicVolume < 0.200000002980232)
        {
          this.m_musicVolume += Time.deltaTime * 0.2f;
          this.AudSource_BattleMusic.volume = this.m_musicVolume;
        }
        else
        {
          this.m_musicVolume = 0.2f;
          this.AudSource_BattleMusic.volume = this.m_musicVolume;
        }
      }
      else if ((double) this.m_musicVolume > 0.0)
      {
        this.m_musicVolume -= Time.deltaTime * 0.2f;
        this.AudSource_BattleMusic.volume = this.m_musicVolume;
      }
      else
      {
        this.m_musicVolume = 0.0f;
        this.AudSource_BattleMusic.volume = 0.0f;
        if (!this.AudSource_BattleMusic.isPlaying)
          return;
        this.AudSource_BattleMusic.Stop();
      }
    }

    public void IsNoLongerPlaying() => this.m_isPlaying = false;

    public void Case() => this.m_getCase = true;

    public void EndModeScore()
    {
      if (this.m_getCase)
      {
        this.m_getCase = false;
        GM.MMFlags.AddHat(this.CaseID.ItemID);
        this.smeme.UpdateInventory();
        this.smeme.DrawInventory();
        GM.MMFlags.SaveToFile();
      }
      this.SetArenaVisibility(0);
      this.Wieners_Arena1.SetActive(true);
      this.Wieners_Arena2.SetActive(true);
      this.m_isPlaying = false;
      this.SubmitScoreAndGoToBoard();
    }

    private void InitScoring()
    {
      if (!SteamManager.Initialized)
        return;
      this.m_name = SteamFriends.GetPersonaName();
      this.m_ID = SteamUser.GetSteamID();
    }

    private void Update()
    {
      this.UpdateHighScoreCallbacks();
      this.MusicUpdate();
    }

    public void SwitchToModeID(string id)
    {
      this.m_curSequenceID = id;
      this.m_hasPreviousScore = false;
      this.m_previousScore = 0;
      this.m_hasRankInCurrentSequence = false;
      this.m_rankInCurrentSequence = 0;
      this.UIM.RedrawHighScoreDisplay(this.m_curSequenceID);
      this.RequestHighScoreChart();
    }

    private void ProcessHighScore(int score)
    {
      this.m_hasCurrentScore = true;
      this.m_currentScore = score;
      this.HSM.UpdateScore(this.m_curSequenceID, score, new Action<int, int>(this.HandleProcessHighScore));
    }

    public void HandleProcessHighScore(int prevRank, int newRank)
    {
      this.m_currentRank = newRank;
      this.m_doRequestScoresTop = true;
    }

    private void RequestHighScoreChart()
    {
      this.UIM.ClearGlobalHighScoreDisplay();
      Debug.Log((object) "Requesting High Score Chart");
      this.m_hasScoresTop = false;
      this.m_hasScoresPlayer = false;
      this.m_scoresTop = (List<HighScoreManager.HighScore>) null;
      this.m_scoresPlayer = (List<HighScoreManager.HighScore>) null;
      this.m_doRequestScoresTop = true;
    }

    public void HandleHighScoreChartTop(List<HighScoreManager.HighScore> scores)
    {
      Debug.Log((object) "HighScore Top Chart Received");
      this.m_hasScoresTop = true;
      this.m_scoresTop = scores;
      this.m_doRequestScoresPlayer = true;
    }

    public void HandleHighScoreChartPlayer(List<HighScoreManager.HighScore> scores)
    {
      Debug.Log((object) "HighScore Player Chart Received");
      this.m_hasScoresPlayer = true;
      this.m_scoresPlayer = scores;
      this.RedrawHighScoreChart();
    }

    private void UpdateHighScoreCallbacks()
    {
      if (this.m_doRequestScoresTop)
      {
        this.m_doRequestScoresTop = false;
        this.HSM.GetLeaderboards(this.m_curSequenceID, 1, 6, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, new Action<List<HighScoreManager.HighScore>>(this.HandleHighScoreChartTop));
      }
      if (!this.m_doRequestScoresPlayer)
        return;
      this.m_doRequestScoresPlayer = false;
      this.HSM.GetLeaderboards(this.m_curSequenceID, -1, 1, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, new Action<List<HighScoreManager.HighScore>>(this.HandleHighScoreChartPlayer));
    }

    private void RedrawHighScoreChart()
    {
      Debug.Log((object) ("m_hasScoresTop:" + (object) this.m_hasScoresTop + " m_hasScoresPlayer:" + (object) this.m_hasScoresPlayer));
      if (!this.m_hasScoresTop || !this.m_hasScoresPlayer)
        return;
      List<HighScoreManager.HighScore> scores = new List<HighScoreManager.HighScore>();
      for (int index = 0; index < this.m_scoresTop.Count; ++index)
        scores.Add(this.m_scoresTop[index]);
      if (this.m_scoresPlayer.Count > 0)
      {
        int rank = this.m_scoresPlayer[0].rank;
        int num1 = 0;
        int num2 = 0;
        for (int index = 0; index < this.m_scoresPlayer.Count; ++index)
        {
          if (this.m_scoresPlayer[index].steamID == this.m_ID)
          {
            num1 = index;
            num2 = this.m_scoresPlayer[index].rank;
            if (!this.m_hasCurrentScore)
            {
              this.m_currentScore = this.m_scoresPlayer[index].score;
              this.m_currentRank = this.m_scoresPlayer[index].rank;
              this.m_hasCurrentScore = true;
            }
            this.m_hasPreviousScore = true;
            this.m_hasRankInCurrentSequence = true;
            this.m_previousScore = this.m_scoresPlayer[index].score;
            this.m_rankInCurrentSequence = this.m_scoresPlayer[index].rank;
          }
        }
        if (num2 > 6)
        {
          int index1 = 0;
          for (int index2 = 6 - this.m_scoresPlayer.Count; index2 < 6; ++index2)
          {
            scores[index2] = this.m_scoresPlayer[index1];
            ++index1;
          }
        }
      }
      this.UIM.SetGlobalHighScoreDisplay(scores);
    }

    private void SubmitScoreAndGoToBoard()
    {
      int score = this.m_curModeManager.GetScore();
      GM.Omni.OmniFlags.AddScore(this.m_curSequenceID, this.m_curModeManager.GetScore());
      GM.Omni.SaveToFile();
      if (this.m_usingSteamworks)
        this.ProcessHighScore(score);
      this.UIM.RedrawHighScoreDisplay(this.m_curSequenceID);
      this.UIM.SetScoringReadouts(this.m_curModeManager.GetScoringReadOuts());
    }

    public enum State
    {
      InMenu,
      InGame,
    }
  }
}
