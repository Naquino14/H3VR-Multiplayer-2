// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSequencer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using RUST.Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class OmniSequencer : MonoBehaviour
  {
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
    public OmniSequencer.State m_state = OmniSequencer.State.ReadyToStart;
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
    private OmniSequencer.OmniMenuCanvas m_curMenuCanvas;
    private OmniSequencerSequenceDefinition.OmniSequenceTheme m_curTheme = OmniSequencerSequenceDefinition.OmniSequenceTheme.CasualPlinking;
    private int m_sequencePage;
    private int m_maxSequencePage;

    private void Awake()
    {
      this.m_state = OmniSequencer.State.ReadyToStart;
      this.Menu.SetActive(true);
      this.GameMenu.SetActive(false);
      if (!SteamManager.Initialized)
        return;
      this.m_usingSteamworks = true;
    }

    private void Update()
    {
      this.UpdateSequencer();
      this.UpdatePacketRain();
    }

    private void UpdateSequencer()
    {
      switch (this.m_state)
      {
        case OmniSequencer.State.ReadyToStart:
          this.UpdateReadyToStart();
          break;
        case OmniSequencer.State.InWarmUp:
          this.UpdateInWarmUp();
          break;
        case OmniSequencer.State.InWave:
          this.UpdateInWave();
          break;
        case OmniSequencer.State.InCleanup:
          this.UpdateInCleanup();
          break;
        case OmniSequencer.State.SequenceCompleted:
          this.UpdateSequenceCompleted();
          break;
      }
    }

    private void UpdateReadyToStart() => this.EquipmentCheckLoop();

    private void UpdateInWarmUp()
    {
      if (this.m_waveIndex == 0 && !this.m_hasPlayedStartingSound && (double) this.m_timeLeftInWarmup < 3.20000004768372)
      {
        this.m_hasPlayedStartingSound = true;
        this.AudSource.clip = this.AudClip_SequenceStart;
        this.AudSource.Play();
      }
      bool flag = false;
      if (this.m_waveUsesQuickdraw)
      {
        for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
        {
          if ((Object) GM.CurrentMovementManager.Hands[index].CurrentInteractable != (Object) null && GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArm)
          {
            flag = true;
            if ((double) this.m_timeLeftInWarmup < 2.0)
              this.m_timeLeftInWarmup = 2f;
          }
        }
      }
      if ((double) this.m_timeLeftInWarmup > 0.0)
      {
        if (!flag)
          this.m_timeLeftInWarmup -= Time.deltaTime;
      }
      else
      {
        this.m_timeLeftInWarmup = 0.0f;
        for (int index = 0; index < this.m_spawners.Count; ++index)
          this.m_spawners[index].BeginSpawning();
        this.m_state = OmniSequencer.State.InWave;
        this.AudSource.clip = this.AudClip_WaveStart;
        this.AudSource.Play();
        this.Canvas_InWarmUp.SetActive(false);
        this.Canvas_InWave.SetActive(true);
        this.Canvas_EndOfSequence.SetActive(false);
      }
      if (flag)
      {
        if (!this.WarmUp_Instruction.gameObject.activeSelf)
          this.WarmUp_Instruction.gameObject.SetActive(true);
        this.WarmUp_Instruction.text = "Holster Your Firearms!";
        if (this.WarmUp_Time.gameObject.activeSelf)
          this.WarmUp_Time.gameObject.SetActive(false);
      }
      else
      {
        if (this.WarmUp_Instruction.gameObject.activeSelf)
          this.WarmUp_Instruction.gameObject.SetActive(false);
        if (!this.WarmUp_Time.gameObject.activeSelf)
          this.WarmUp_Time.gameObject.SetActive(true);
        if (this.m_waveUsesReflex)
        {
          this.WarmUp_Time.text = "Get Ready!";
        }
        else
        {
          this.m_timeLeftInWarmup = Mathf.Clamp(this.m_timeLeftInWarmup, 0.0f, this.m_timeLeftInWarmup);
          this.WarmUp_Time.text = this.FloatToTime(this.m_timeLeftInWarmup, "#0:00.00");
        }
      }
      this.WarmUp_Wave.text = "Warmup For Wave " + (this.m_waveIndex + 1).ToString();
    }

    private void UpdateInWave()
    {
      bool flag = true;
      for (int index = 0; index < this.m_spawners.Count; ++index)
      {
        if (!this.m_spawners[index].IsReadyForWaveEnd())
          flag = false;
      }
      if ((double) this.m_timeLeftInWave <= 0.0 || flag)
      {
        float timeLeftInWave = this.m_timeLeftInWave;
        this.m_timeLeftInWave = 0.0f;
        for (int index = 0; index < this.m_spawners.Count; ++index)
        {
          this.m_spawners[index].EndSpawning();
          int num = 0 + (int) ((double) this.m_spawners[index].Deactivate() * (double) this.m_scoremult_Points);
          this.m_score += num + (int) ((double) this.m_scoremult_Range * (double) this.RangeToScoreMultiplier(this.m_spawners[index].GetEngagementRange()) * (double) num);
        }
        if ((double) timeLeftInWave > 0.0)
          this.m_score += (int) ((double) timeLeftInWave * 100.0 * (double) this.m_scoremult_Time);
        this.AudSource.clip = this.AudClip_WaveEnd;
        this.AudSource.Play();
        this.m_state = OmniSequencer.State.InCleanup;
      }
      else
        this.m_timeLeftInWave -= Time.deltaTime;
      this.m_timeLeftInWave = Mathf.Clamp(this.m_timeLeftInWave, 0.0f, this.m_timeLeftInWave);
      this.InWave_Time.text = this.FloatToTime(this.m_timeLeftInWave, "#0:00.00");
      this.InWave_Wave.text = "Wave " + (this.m_waveIndex + 1).ToString();
    }

    private void UpdateInCleanup()
    {
      bool flag = true;
      for (int index = 0; index < this.m_spawners.Count; ++index)
      {
        if ((Object) this.m_spawners[index] != (Object) null && this.m_spawners[index].GetState() != OmniSpawner.SpawnerState.Deactivated)
          flag = false;
      }
      if (!flag)
        return;
      this.CleanupWave();
      if (this.m_waveIndex < this.SequenceDef.Waves.Count - 1)
      {
        ++this.m_waveIndex;
        this.InitiateWave();
      }
      else
      {
        this.AudSource.clip = this.AudClip_SequenceEnd;
        this.AudSource.Play();
        this.m_state = OmniSequencer.State.SequenceCompleted;
        this.SequenceCompletedScreen();
      }
    }

    private void UpdateSequenceCompleted()
    {
      this.Canvas_InWarmUp.SetActive(false);
      this.Canvas_InWave.SetActive(false);
      this.Canvas_EndOfSequence.SetActive(true);
      this.Canvas_Abort.SetActive(false);
    }

    public bool InitiateSelectedSequence()
    {
      if ((Object) this.SequenceDef == (Object) null || this.m_state != OmniSequencer.State.ReadyToStart)
        return false;
      this.BeginSequence();
      return true;
    }

    public void BeginSequence()
    {
      if (this.m_state != OmniSequencer.State.ReadyToStart)
      {
        Debug.LogError((object) "Attempted to begin sequence when not in ReadyToStart state. This should not be possible.");
      }
      else
      {
        this.UpdateIllegalEquipmentCheck();
        if (this.m_isIllegalEquipmentHeld)
          return;
        this.DisableIllegalEquipment();
        this.Menu.SetActive(false);
        this.GameMenu.SetActive(true);
        this.ItemSpawner.SetActive(false);
        this.TrashCan.SetActive(false);
        this.Canvas_Abort.SetActive(true);
        this.m_hasPlayedStartingSound = false;
        this.m_waveIndex = 0;
        this.InitiateWave();
      }
    }

    public void AbortSequence()
    {
      this.m_timeLeftInWarmup = 0.0f;
      this.m_timeLeftInWave = 0.0f;
      this.m_waveIndex = this.SequenceDef.Waves.Count - 1;
      this.m_state = OmniSequencer.State.InCleanup;
      for (int index = 0; index < this.m_spawners.Count; ++index)
      {
        this.m_spawners[index].EndSpawning();
        this.m_spawners[index].Deactivate();
      }
      this.m_isEarlyAbort = true;
    }

    private void InitiateWave()
    {
      Debug.Log((object) ("Initiating Wave:" + (object) this.m_waveIndex));
      if (this.m_waveIndex >= this.SequenceDef.Waves.Count)
      {
        Debug.LogError((object) "Attempted to Initiate Out of Bound Wave. Cleanup phase should have caught this and passed us to sequence completed instead.");
      }
      else
      {
        OmniSequencerWaveDefinition wave = this.SequenceDef.Waves[this.m_waveIndex];
        for (int index = 0; index < wave.Spawners.Count; ++index)
        {
          OmniSpawnDef def = wave.Spawners[index].Def;
          Vector3 position = new Vector3(0.0f, -100f, this.GetRange(wave.Spawners[index].Range));
          Quaternion identity = Quaternion.identity;
          OmniSpawner component = Object.Instantiate<GameObject>(def.SpawnerPrefab, position, identity).GetComponent<OmniSpawner>();
          this.m_spawners.Add(component);
          component.Configure(def, wave.Spawners[index].Range);
          component.Activate();
        }
        this.m_state = OmniSequencer.State.InWarmUp;
        this.m_timeLeftInWarmup = wave.TimeForWarmup + Random.Range(0.0f, wave.TimeForWarmupRandomAddition);
        this.m_timeLeftInWave = wave.TimeForWave;
        this.m_waveUsesQuickdraw = wave.UsesQuickDraw;
        this.m_waveUsesReflex = wave.UsesReflex;
        this.m_scoremult_Points = wave.ScoreMultiplier_Points;
        this.m_scoremult_Range = wave.ScoreMultiplier_Range;
        this.m_scoremult_Time = wave.ScoreMultiplier_Time;
        this.Canvas_InWarmUp.SetActive(true);
        this.Canvas_InWave.SetActive(false);
        this.Canvas_EndOfSequence.SetActive(false);
      }
    }

    private void CleanupWave()
    {
      for (int index = this.m_spawners.Count - 1; index >= 0; --index)
        Object.Destroy((Object) this.m_spawners[index]);
      this.m_spawners.Clear();
    }

    private void SequenceCompletedScreen()
    {
      this.EnableAllEquipment();
      this.ItemSpawner.SetActive(true);
      this.TrashCan.SetActive(true);
      this.Canvas_Abort.SetActive(false);
      if (!this.m_isEarlyAbort)
      {
        this.FinalScore.text = this.m_score.ToString();
        this.FinalDisplay.text = this.SequenceDef.SequenceName + " Completed";
        this.LocalPlayerNameDisplay.text = GM.Omni.OmniFlags.StoredPlayerName;
        this.FinalKeyboard.SetActiveText(this.LocalPlayerNameDisplay);
      }
      else
      {
        this.m_isEarlyAbort = false;
        this.m_score = 0;
        this.GameMenu.SetActive(false);
        this.Menu.SetActive(true);
        this.SetMenuCanvas(OmniSequencer.OmniMenuCanvas.Details);
      }
    }

    protected float GetRange(OmniWaveEngagementRange r)
    {
      switch (r)
      {
        case OmniWaveEngagementRange.m5:
          return 5f;
        case OmniWaveEngagementRange.m10:
          return 10f;
        case OmniWaveEngagementRange.m15:
          return 15f;
        case OmniWaveEngagementRange.m20:
          return 20f;
        case OmniWaveEngagementRange.m25:
          return 25f;
        case OmniWaveEngagementRange.m50:
          return 50f;
        case OmniWaveEngagementRange.m100:
          return 100f;
        case OmniWaveEngagementRange.m150:
          return 150f;
        case OmniWaveEngagementRange.m200:
          return 200f;
        default:
          return 0.0f;
      }
    }

    private void SetMenuCanvas(OmniSequencer.OmniMenuCanvas canvas)
    {
      this.m_curMenuCanvas = canvas;
      switch (canvas)
      {
        case OmniSequencer.OmniMenuCanvas.Root:
          if (!this.Canvas_Menu_Root.activeSelf)
            this.Canvas_Menu_Root.SetActive(true);
          if (this.Canvas_Menu_SequenceList.activeSelf)
            this.Canvas_Menu_SequenceList.SetActive(false);
          if (this.Canvas_Menu_SequenceDetails.activeSelf)
            this.Canvas_Menu_SequenceDetails.SetActive(false);
          if (this.Canvas_Menu_HighScore.activeSelf)
            this.Canvas_Menu_HighScore.SetActive(false);
          this.RedrawMenu_Root();
          break;
        case OmniSequencer.OmniMenuCanvas.List:
          if (this.Canvas_Menu_Root.activeSelf)
            this.Canvas_Menu_Root.SetActive(false);
          if (!this.Canvas_Menu_SequenceList.activeSelf)
            this.Canvas_Menu_SequenceList.SetActive(true);
          if (this.Canvas_Menu_SequenceDetails.activeSelf)
            this.Canvas_Menu_SequenceDetails.SetActive(false);
          if (this.Canvas_Menu_HighScore.activeSelf)
            this.Canvas_Menu_HighScore.SetActive(false);
          this.RedrawMenu_List();
          break;
        case OmniSequencer.OmniMenuCanvas.Details:
          if (this.Canvas_Menu_Root.activeSelf)
            this.Canvas_Menu_Root.SetActive(false);
          if (this.Canvas_Menu_SequenceList.activeSelf)
            this.Canvas_Menu_SequenceList.SetActive(false);
          if (!this.Canvas_Menu_SequenceDetails.activeSelf)
            this.Canvas_Menu_SequenceDetails.SetActive(true);
          if (this.Canvas_Menu_HighScore.activeSelf)
            this.Canvas_Menu_HighScore.SetActive(false);
          this.RedrawMenu_Details();
          this.m_state = OmniSequencer.State.ReadyToStart;
          break;
        case OmniSequencer.OmniMenuCanvas.HighScore:
          if (this.Canvas_Menu_Root.activeSelf)
            this.Canvas_Menu_Root.SetActive(false);
          if (this.Canvas_Menu_SequenceList.activeSelf)
            this.Canvas_Menu_SequenceList.SetActive(false);
          if (this.Canvas_Menu_SequenceDetails.activeSelf)
            this.Canvas_Menu_SequenceDetails.SetActive(false);
          if (!this.Canvas_Menu_HighScore.activeSelf)
            this.Canvas_Menu_HighScore.SetActive(true);
          this.RedrawMenu_HighScore();
          break;
      }
    }

    private void RedrawMenu_Root()
    {
    }

    private void RedrawMenu_List()
    {
      this.UIL_ThemeName.text = this.m_curTheme.ToString();
      this.UIL_ThemeImage.sprite = this.Library.Themes[(int) this.m_curTheme].Sprite;
      this.UIL_ThemeDetails.text = this.Library.Themes[(int) this.m_curTheme].ThemeDetails;
      this.UIL_ArrowPrev.SetActive(this.m_sequencePage > 0);
      this.UIL_ArrowNext.SetActive(this.m_sequencePage < this.m_maxSequencePage);
      int length = this.Library.Themes[(int) this.m_curTheme].SequenceList.Length;
      int num = length;
      if (Mathf.Min(length, 8) <= 0)
      {
        for (int index = 1; index < this.UIL_SequenceList.Length; ++index)
          this.UIL_SequenceList[index].gameObject.SetActive(false);
        this.UIL_SequenceList[0].gameObject.SetActive(true);
        this.UIL_SequenceList[0].text = "Sequences Coming Soon";
      }
      else
      {
        for (int index1 = 0; index1 < 8; ++index1)
        {
          this.UIL_SequenceList[index1].gameObject.SetActive(true);
          int index2 = this.m_sequencePage * 8 + index1;
          if (index2 < num)
          {
            this.UIL_SequenceList[index1].text = (index2 + 1).ToString() + ". " + this.Library.Themes[(int) this.m_curTheme].SequenceList[index2].SequenceName;
            int rank = GM.Omni.OmniFlags.GetRank(this.Library.Themes[(int) this.m_curTheme].SequenceList[index2].SequenceID);
            if (rank == 3)
            {
              this.UIL_SequenceAwardList[index1].gameObject.SetActive(false);
            }
            else
            {
              this.UIL_SequenceAwardList[index1].gameObject.SetActive(true);
              this.UIL_SequenceAwardList[index1].sprite = this.UIL_TrophySprites[rank];
            }
          }
          else
          {
            this.UIL_SequenceList[index1].gameObject.SetActive(false);
            this.UIL_SequenceAwardList[index1].gameObject.SetActive(false);
          }
        }
      }
    }

    private void RedrawMenu_Details()
    {
      this.UID_SequenceTheme.text = this.SequenceDef.Theme.ToString();
      this.UID_SequenceName.text = this.SequenceDef.SequenceName;
      this.UID_SequenceDetails.text = this.SequenceDef.Description;
      this.UID_SequenceDifficulty.text = "Difficulty: " + this.SequenceDef.Difficulty.ToString();
      this.UID_SequenceFirearmType.text = "Firearm Mode: " + this.SequenceDef.FirearmMode.ToString();
      this.UID_SequenceAmmoMode.text = "Ammo Mode: " + this.SequenceDef.AmmoMode.ToString();
      this.UID_SequenceWaveCount.text = "Waves: " + (object) this.SequenceDef.Waves.Count;
      this.UID_SequenceImage.sprite = this.Library.Themes[(int) this.m_curTheme].Sprite;
      if (this.SequenceDef.FirearmMode == OmniSequencerSequenceDefinition.OmniSequenceFirearmMode.Open)
      {
        this.UID_SequenceAllowedEquipmentList.gameObject.SetActive(false);
      }
      else
      {
        this.UID_SequenceAllowedEquipmentList.gameObject.SetActive(true);
        string str = "Equipment Types Allowed:\n";
        for (int index = 0; index < this.SequenceDef.AllowedCats.Count; ++index)
        {
          if (index > 0)
            str += ", ";
          string subCatName = ItemSpawnerID.SubCatNames[(int) this.SequenceDef.AllowedCats[index]];
          str += subCatName;
        }
        this.UID_SequenceAllowedEquipmentList.text = str;
      }
      this.BeginSequenceButton.SetActive(true);
    }

    public void ClearGlobalHighScoreDisplay()
    {
      for (int index = 0; index < 6; ++index)
        this.UIH_Scores_Global[index].gameObject.SetActive(false);
    }

    public void SetGlobalHighScoreDisplay(List<HighScoreManager.HighScore> scores)
    {
      for (int index = 0; index < 6; ++index)
      {
        if (scores.Count > index)
        {
          this.UIH_Scores_Global[index].gameObject.SetActive(true);
          this.UIH_Scores_Global[index].text = scores[index].rank.ToString() + ". " + (object) scores[index].score + " - " + scores[index].name;
        }
        else
          this.UIH_Scores_Global[index].gameObject.SetActive(false);
      }
    }

    private void RedrawMenu_HighScore()
    {
      this.UIH_SequenceName.text = "HighScores for " + this.SequenceDef.SequenceName;
      List<OmniScore> scoreList = GM.Omni.OmniFlags.GetScoreList(this.SequenceDef.SequenceID);
      for (int index = 0; index < scoreList.Count; ++index)
      {
        this.UIH_Scores_Local[index].gameObject.SetActive(true);
        this.UIH_Scores_Local[index].text = (index + 1).ToString() + ": " + (object) scoreList[index].Score + " - " + scoreList[index].Name;
      }
      for (int count = scoreList.Count; count < this.UIH_Scores_Local.Length; ++count)
        this.UIH_Scores_Local[count].gameObject.SetActive(false);
      if (this.m_isLatestScoreARankIncrease)
      {
        this.m_isLatestScoreARankIncrease = false;
        this.UIH_NewRank.gameObject.SetActive(true);
        this.UIH_NewRank.text = this.m_youAreNowRankMessage;
      }
      else
        this.UIH_NewRank.gameObject.SetActive(false);
      if (this.m_shouldDisplayLatestCurrencyEarned)
      {
        this.m_shouldDisplayLatestCurrencyEarned = false;
        this.UIH_CurrencyMessage.gameObject.SetActive(true);
        this.UIH_CurrencyMessage.text = this.m_currencyGainMessage;
      }
      else
        this.UIH_CurrencyMessage.gameObject.SetActive(false);
      int rank = GM.Omni.OmniFlags.GetRank(this.SequenceDef.SequenceID);
      for (int index = 0; index < this.UIH_WeinerTrophies.Length; ++index)
        this.UIH_WeinerTrophies[index].color = this.UIH_WeinerTrophy_Dark;
      if (rank >= 3 || rank < 0)
        return;
      this.UIH_WeinerTrophies[rank].color = this.UIH_WeinerTrophy_Light;
    }

    public void SubmitScoreAndGoToBoard()
    {
      int rank1 = GM.Omni.OmniFlags.GetRank(this.SequenceDef.SequenceID);
      GM.Omni.OmniFlags.StoredPlayerName = this.LocalPlayerNameDisplay.text;
      GM.Omni.OmniFlags.AddScore(this.SequenceDef, this.m_score);
      GM.Omni.SaveToFile();
      int rank2 = GM.Omni.OmniFlags.GetRank(this.SequenceDef.SequenceID);
      int currencyForRank = this.SequenceDef.CurrencyForRanks[this.SequenceDef.GetRankForScore(this.m_score)];
      int num1 = (int) Random.Range((float) currencyForRank * 0.8f, (float) currencyForRank * 1.1f);
      if (rank2 < rank1)
        this.m_isLatestScoreARankIncrease = true;
      int num2 = 0;
      if (this.m_isLatestScoreARankIncrease)
        num2 = this.SequenceDef.CurrencyForRanks[4] * Mathf.Abs(rank2 - rank1);
      GM.Omni.OmniUnlocks.GainCurrency(num1 + num2);
      this.InitiatePacketRain(num1 + num2);
      GM.Omni.SaveUnlocksToFile();
      if (this.m_isLatestScoreARankIncrease)
        this.m_currencyGainMessage = "You Earned\n" + (object) num1 + " + " + (object) num2 + "\nS.A.U.C.E.\nPackets!";
      else
        this.m_currencyGainMessage = "You Earned\n" + (object) num1 + "\nS.A.U.C.E.\nPackets!";
      if (this.m_isLatestScoreARankIncrease)
      {
        switch (rank2)
        {
          case 0:
            this.m_youAreNowRankMessage = "You Are Now Ranked\nGold Weenie!";
            break;
          case 1:
            this.m_youAreNowRankMessage = "You Are Now Ranked\nSilver Weenie!";
            break;
          case 2:
            this.m_youAreNowRankMessage = "You Are Now Ranked\nBronze Weenie!";
            break;
          case 3:
            this.m_youAreNowRankMessage = string.Empty;
            break;
        }
      }
      this.m_shouldDisplayLatestCurrencyEarned = true;
      if (this.m_usingSteamworks)
        this.ScoreManager.ProcessHighScore(this.m_score);
      this.m_score = 0;
      this.GameMenu.SetActive(false);
      this.Menu.SetActive(true);
      this.SetMenuCanvas(OmniSequencer.OmniMenuCanvas.HighScore);
    }

    public void BackToRoot()
    {
      this.SetAmmoMode(OmniSequencerSequenceDefinition.OmniSequenceAmmoMode.Infinite);
      this.SetMenuCanvas(OmniSequencer.OmniMenuCanvas.Root);
    }

    public void BackToSequenceList()
    {
      this.SetAmmoMode(OmniSequencerSequenceDefinition.OmniSequenceAmmoMode.Infinite);
      this.SequenceDef = (OmniSequencerSequenceDefinition) null;
      this.UpdateIllegalEquipmentCheck();
      this.SetMenuCanvas(OmniSequencer.OmniMenuCanvas.List);
    }

    public void BackToSequenceDetails()
    {
      this.UpdateIllegalEquipmentCheck();
      this.SetMenuCanvas(OmniSequencer.OmniMenuCanvas.Details);
    }

    public void SelectTheme(int i)
    {
      this.m_curTheme = (OmniSequencerSequenceDefinition.OmniSequenceTheme) i;
      this.m_sequencePage = 0;
      this.m_maxSequencePage = Mathf.FloorToInt((float) ((this.Library.Themes[(int) this.m_curTheme].SequenceList.Length - 1) / 8));
      this.SetAmmoMode(OmniSequencerSequenceDefinition.OmniSequenceAmmoMode.Infinite);
      this.SetMenuCanvas(OmniSequencer.OmniMenuCanvas.List);
    }

    public void SelectSequence(int i)
    {
      int index = this.m_sequencePage * 8 + i;
      if (index >= this.Library.Themes[(int) this.m_curTheme].SequenceList.Length)
        return;
      this.SequenceDef = this.Library.Themes[(int) this.m_curTheme].SequenceList[index];
      if (this.m_usingSteamworks)
        this.ScoreManager.SwitchToSequenceID(this.SequenceDef.SequenceID);
      this.SetAmmoMode(this.SequenceDef.AmmoMode);
      this.SetMenuCanvas(OmniSequencer.OmniMenuCanvas.Details);
      this.UpdateIllegalEquipmentCheck();
    }

    public void ViewHighScorePage() => this.SetMenuCanvas(OmniSequencer.OmniMenuCanvas.HighScore);

    public void NextSequencePage()
    {
      if (this.m_sequencePage >= this.m_maxSequencePage)
        return;
      ++this.m_sequencePage;
      this.RedrawMenu_List();
    }

    public void PrevSequencePage()
    {
      if (this.m_sequencePage <= 0)
        return;
      --this.m_sequencePage;
      this.RedrawMenu_List();
    }

    private void SetAmmoMode(
      OmniSequencerSequenceDefinition.OmniSequenceAmmoMode mode)
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
      float num1 = Mathf.Clamp((float) amount * 0.25f, 50f, 400f);
      float num2 = Mathf.Clamp((float) amount / num1, 0.1f, 4f);
      this.m_packetRainEmissionRate = num1;
      this.m_packetRainTimeTilShutOff = num2;
      this.m_isPacketRaining = true;
      this.UIH_PacketRain.emission.rateOverTime = (ParticleSystem.MinMaxCurve) Random.Range(this.m_packetRainEmissionRate, this.m_packetRainEmissionRate);
      this.UIH_PacketRainAudio.PlayOneShot(this.UIH_PacketDingClip, 1f);
      this.UIH_PacketRainAudio.Play();
    }

    private void UpdatePacketRain()
    {
      if (!this.m_isPacketRaining)
        return;
      this.m_packetRainTimeTilShutOff -= Time.deltaTime;
      if ((double) this.m_packetRainTimeTilShutOff > 0.0)
        return;
      this.m_isPacketRaining = false;
      this.UIH_PacketRain.emission.rateOverTime = (ParticleSystem.MinMaxCurve) 0.0f;
      this.UIH_PacketRainAudio.Stop();
    }

    private void EquipmentCheckLoop()
    {
      if ((Object) this.SequenceDef == (Object) null || this.SequenceDef.FirearmMode == OmniSequencerSequenceDefinition.OmniSequenceFirearmMode.Open)
      {
        if (this.IllegalEquipmentLabel.activeSelf)
          this.IllegalEquipmentLabel.SetActive(false);
        if (!this.UID_BeginSequenceButton.activeSelf)
          this.UID_BeginSequenceButton.SetActive(true);
      }
      else if (this.m_isIllegalEquipmentHeld)
      {
        if (!this.IllegalEquipmentLabel.activeSelf)
          this.IllegalEquipmentLabel.SetActive(true);
        if (this.UID_BeginSequenceButton.activeSelf)
          this.UID_BeginSequenceButton.SetActive(false);
      }
      else
      {
        if (this.IllegalEquipmentLabel.activeSelf)
          this.IllegalEquipmentLabel.SetActive(false);
        if (!this.UID_BeginSequenceButton.activeSelf)
          this.UID_BeginSequenceButton.SetActive(true);
      }
      if (this.m_curMenuCanvas != OmniSequencer.OmniMenuCanvas.Details)
        return;
      if ((double) this.m_equipmentCheckTick > 0.0)
      {
        this.m_equipmentCheckTick -= Time.deltaTime;
      }
      else
      {
        this.m_equipmentCheckTick = 0.5f;
        this.UpdateIllegalEquipmentCheck();
      }
    }

    private bool IsEquipmentIllegal(ItemSpawnerID id) => !((Object) this.SequenceDef == (Object) null) && !((Object) id == (Object) null) && (id.Category != ItemSpawnerID.EItemCategory.Magazine && id.SubCategory != ItemSpawnerID.ESubCategory.None) && (id.SubCategory != ItemSpawnerID.ESubCategory.RailAdapter && id.SubCategory != ItemSpawnerID.ESubCategory.Suppressor) && !this.SequenceDef.AllowedCats.Contains(id.SubCategory);

    public void UpdateIllegalEquipmentCheck()
    {
      if ((Object) this.SequenceDef == (Object) null || this.SequenceDef.FirearmMode == OmniSequencerSequenceDefinition.OmniSequenceFirearmMode.Open)
      {
        this.m_isIllegalEquipmentHeld = false;
      }
      else
      {
        FVRPhysicalObject[] objectsOfType = Object.FindObjectsOfType<FVRPhysicalObject>();
        for (int index1 = 0; index1 < objectsOfType.Length; ++index1)
        {
          if (!((Object) objectsOfType[index1].IDSpawnedFrom == (Object) null) && (objectsOfType[index1].IsHeld || (Object) objectsOfType[index1].QuickbeltSlot != (Object) null))
          {
            if (this.IsEquipmentIllegal(objectsOfType[index1].IDSpawnedFrom))
            {
              this.m_isIllegalEquipmentHeld = true;
              return;
            }
            if (objectsOfType[index1].Attachments.Count > 0)
            {
              for (int index2 = 0; index2 < objectsOfType[index1].Attachments.Count; ++index2)
              {
                if ((Object) objectsOfType[index1].Attachments[index2].IDSpawnedFrom != (Object) null && this.IsEquipmentIllegal(objectsOfType[index1].Attachments[index2].IDSpawnedFrom))
                {
                  this.m_isIllegalEquipmentHeld = true;
                  return;
                }
              }
            }
          }
        }
        this.m_isIllegalEquipmentHeld = false;
      }
    }

    public void DisableIllegalEquipment()
    {
      if (this.SequenceDef.FirearmMode != OmniSequencerSequenceDefinition.OmniSequenceFirearmMode.Category)
        return;
      FVRPhysicalObject[] objectsOfType = Object.FindObjectsOfType<FVRPhysicalObject>();
      for (int index1 = 0; index1 < objectsOfType.Length; ++index1)
      {
        if ((Object) objectsOfType[index1].IDSpawnedFrom != (Object) null && this.IsEquipmentIllegal(objectsOfType[index1].IDSpawnedFrom))
          objectsOfType[index1].IsPickUpLocked = true;
        else if (objectsOfType[index1].Attachments.Count > 0)
        {
          for (int index2 = 0; index2 < objectsOfType[index1].Attachments.Count; ++index2)
          {
            if ((Object) objectsOfType[index1].Attachments[index2].IDSpawnedFrom != (Object) null && (Object) objectsOfType[index1].Attachments[index2].IDSpawnedFrom != (Object) null && this.IsEquipmentIllegal(objectsOfType[index1].Attachments[index2].IDSpawnedFrom))
            {
              objectsOfType[index1].Attachments[index2].IsPickUpLocked = true;
              objectsOfType[index1].IsPickUpLocked = true;
              break;
            }
          }
        }
      }
    }

    public void EnableAllEquipment()
    {
      foreach (FVRPhysicalObject fvrPhysicalObject in Object.FindObjectsOfType<FVRPhysicalObject>())
        fvrPhysicalObject.IsPickUpLocked = false;
    }

    public float RangeToScoreMultiplier(OmniWaveEngagementRange range)
    {
      switch (range)
      {
        case OmniWaveEngagementRange.m5:
          return 0.0f;
        case OmniWaveEngagementRange.m10:
          return 0.1f;
        case OmniWaveEngagementRange.m15:
          return 0.25f;
        case OmniWaveEngagementRange.m20:
          return 0.5f;
        case OmniWaveEngagementRange.m25:
          return 1f;
        case OmniWaveEngagementRange.m50:
          return 2f;
        case OmniWaveEngagementRange.m100:
          return 3f;
        case OmniWaveEngagementRange.m150:
          return 4f;
        case OmniWaveEngagementRange.m200:
          return 5f;
        default:
          return 1f;
      }
    }

    public string FloatToTime(float toConvert, string format)
    {
      if (format != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (OmniSequencer.\u003C\u003Ef__switch\u0024map6 == null)
        {
          // ISSUE: reference to a compiler-generated field
          OmniSequencer.\u003C\u003Ef__switch\u0024map6 = new Dictionary<string, int>(13)
          {
            {
              "00.0",
              0
            },
            {
              "#0.0",
              1
            },
            {
              "00.00",
              2
            },
            {
              "00.000",
              3
            },
            {
              "#00.000",
              4
            },
            {
              "#0:00",
              5
            },
            {
              "#00:00",
              6
            },
            {
              "0:00.0",
              7
            },
            {
              "#0:00.0",
              8
            },
            {
              "0:00.00",
              9
            },
            {
              "#0:00.00",
              10
            },
            {
              "0:00.000",
              11
            },
            {
              "#0:00.000",
              12
            }
          };
        }
        int num;
        // ISSUE: reference to a compiler-generated field
        if (OmniSequencer.\u003C\u003Ef__switch\u0024map6.TryGetValue(format, out num))
        {
          switch (num)
          {
            case 0:
              return string.Format("{0:00}:{1:0}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 1:
              return string.Format("{0:#0}:{1:0}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 2:
              return string.Format("{0:00}:{1:00}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 3:
              return string.Format("{0:00}:{1:000}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 4:
              return string.Format("{0:#00}:{1:000}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 5:
              return string.Format("{0:#0}:{1:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0));
            case 6:
              return string.Format("{0:#00}:{1:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0));
            case 7:
              return string.Format("{0:0}:{1:00}.{2:0}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 8:
              return string.Format("{0:#0}:{1:00}.{2:0}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 9:
              return string.Format("{0:0}:{1:00}.{2:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 10:
              return string.Format("{0:#0}:{1:00}.{2:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 11:
              return string.Format("{0:0}:{1:00}.{2:000}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 12:
              return string.Format("{0:#0}:{1:00}.{2:000}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
          }
        }
      }
      return "error";
    }

    public enum State
    {
      ReadyToStart = 1,
      InWarmUp = 2,
      InWave = 3,
      InCleanup = 4,
      SequenceCompleted = 5,
    }

    public enum OmniMenuCanvas
    {
      Root,
      List,
      Details,
      HighScore,
    }
  }
}
