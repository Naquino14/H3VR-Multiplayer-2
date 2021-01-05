// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigEndingManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

namespace FistVR
{
  public class ZosigEndingManager : MonoBehaviour
  {
    public ZosigSpawnManager SpawnManager;
    public ZosigEndingManager.RW_EndPhase EndPhase;
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
    public ZosigEndingManager.EndingCharacter EndCh_Roarnart;
    public ZosigEndingManager.EndingCharacter EndCh_Bobert;
    public ZosigEndingManager.EndingCharacter EndCh_Dustin;
    public ZosigEndingManager.EndingCharacter EndCh_Josh;
    public ZosigEndingManager.EndingCharacter EndCh_Alister;
    public ZosigEndingManager.EndingCharacter EndCh_Ralph;
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
      this.m_isEnding = true;
      this.PickClips();
      this.InitPhase(ZosigEndingManager.RW_EndPhase.InitPlayer);
    }

    private void PickClips()
    {
      int flagValue1 = GM.ZMaster.FlagM.GetFlagValue("s_t");
      int flagValue2 = GM.ZMaster.FlagM.GetFlagValue("s_c");
      int flagValue3 = GM.ZMaster.FlagM.GetFlagValue("s_l");
      int flagValue4 = GM.ZMaster.FlagM.GetFlagValue("s_m");
      int flagValue5 = GM.ZMaster.FlagM.GetFlagValue("s_g");
      this.b_clip = flagValue1 <= 50 || GM.ZMaster.FlagM.GetFlagValue("quest06_state") < 3 ? (flagValue1 < 10 || GM.ZMaster.FlagM.GetFlagValue("quest06_state") < 2 ? 2 : 1) : 0;
      this.d_clip = flagValue2 <= 20 || GM.ZMaster.FlagM.GetFlagValue("quest03_state") < 3 ? (flagValue2 < 5 || GM.ZMaster.FlagM.GetFlagValue("quest03_state") < 2 ? 2 : 1) : 0;
      this.j_clip = flagValue3 <= 20 || GM.ZMaster.FlagM.GetFlagValue("quest17_state") < 3 ? (flagValue3 < 8 || GM.ZMaster.FlagM.GetFlagValue("quest17_state") < 2 ? 2 : 1) : 0;
      this.a_clip = flagValue4 <= 200 || GM.ZMaster.FlagM.GetFlagValue("quest10_state") < 2 ? (flagValue4 < 50 || GM.ZMaster.FlagM.GetFlagValue("quest10_state") < 2 ? 2 : 1) : 0;
      this.r_clip = flagValue5 <= 200 || GM.ZMaster.FlagM.GetFlagValue("quest18_state") < 3 ? (flagValue5 < 50 || GM.ZMaster.FlagM.GetFlagValue("quest18_state") < 2 ? 2 : 1) : 0;
      if (this.b_clip == 2 || this.d_clip == 2 || (this.j_clip == 2 || this.a_clip == 2) || this.r_clip == 2)
        this.n_clip = 3;
      if (GM.ZMaster.FlagM.GetFlagValue("g_c") <= 0)
        return;
      this.n_clip = 4;
    }

    private void InitPhase(ZosigEndingManager.RW_EndPhase p)
    {
      this.EndPhase = p;
      this.m_phaseTimer = 0.0f;
      switch (this.EndPhase)
      {
        case ZosigEndingManager.RW_EndPhase.InitPlayer:
          GM.CurrentMovementManager.Head.forward.y = 0.0f;
          double point = (double) GM.CurrentMovementManager.TeleportToPoint(this.TeleportPlace.position, false, GM.CurrentPlayerBody.transform.forward);
          if ((UnityEngine.Object) GM.CurrentMovementManager.Hands[0].CurrentInteractable != (UnityEngine.Object) null)
            GM.CurrentMovementManager.Hands[0].CurrentInteractable.ForceBreakInteraction();
          if ((UnityEngine.Object) GM.CurrentMovementManager.Hands[1].CurrentInteractable != (UnityEngine.Object) null)
            GM.CurrentMovementManager.Hands[1].CurrentInteractable.ForceBreakInteraction();
          GM.CurrentMovementManager.CleanupFlagsForModeSwitch();
          GM.CurrentMovementManager.Hands[0].gameObject.SetActive(false);
          GM.CurrentMovementManager.Hands[1].gameObject.SetActive(false);
          GM.CurrentPlayerBody.HealthBar.SetActive(false);
          this.m_phaseTimer = 2f;
          break;
        case ZosigEndingManager.RW_EndPhase.RoarnartTeleportsIn:
          this.EndCh_Roarnart.Spawn();
          SM.PlayGenericSound(this.AudEvent_Teleport, this.EndCh_Roarnart.SpawnInPoint.position);
          UnityEngine.Object.Instantiate<GameObject>(this.TeleportEffect, this.EndCh_Roarnart.SpawnInPoint.position, this.EndCh_Roarnart.SpawnInPoint.rotation);
          this.m_phaseTimer = 1f;
          break;
        case ZosigEndingManager.RW_EndPhase.RoarnartSpeaks:
          GM.ZMaster.SetMusic_Speaking();
          this.EndCh_Roarnart.AudSource.clip = this.Lines_Narrator[0];
          this.m_phaseTimer = this.EndCh_Roarnart.BeginSpeaking();
          break;
        case ZosigEndingManager.RW_EndPhase.Explodes1:
          this.EndCh_Roarnart.Explode();
          this.m_phaseTimer = 0.7f;
          break;
        case ZosigEndingManager.RW_EndPhase.RoarnartContinues:
          this.m_isScreenOn = true;
          this.m_isScreenTalking = true;
          this.AudSource_Narrator_Reverbee.clip = this.Lines_Narrator[1];
          this.AudSource_Narrator_Reverbee.Play();
          this.m_phaseTimer = this.AudSource_Narrator_Reverbee.clip.length + 0.5f;
          break;
        case ZosigEndingManager.RW_EndPhase.TeleportIn1:
          GM.ZMaster.SetMusic_Gameplay();
          this.EndCh_Roarnart.ClearSosig();
          this.EndCh_Bobert.Spawn();
          SM.PlayGenericSound(this.AudEvent_Teleport, this.EndCh_Bobert.SpawnInPoint.position);
          UnityEngine.Object.Instantiate<GameObject>(this.TeleportEffect, this.EndCh_Bobert.SpawnInPoint.position, this.EndCh_Bobert.SpawnInPoint.rotation);
          this.m_phaseTimer = 1f;
          break;
        case ZosigEndingManager.RW_EndPhase.TeleportIn2:
          this.EndCh_Dustin.Spawn();
          SM.PlayGenericSound(this.AudEvent_Teleport, this.EndCh_Dustin.SpawnInPoint.position);
          UnityEngine.Object.Instantiate<GameObject>(this.TeleportEffect, this.EndCh_Dustin.SpawnInPoint.position, this.EndCh_Dustin.SpawnInPoint.rotation);
          this.m_phaseTimer = 1f;
          break;
        case ZosigEndingManager.RW_EndPhase.TeleportIn3:
          this.EndCh_Josh.Spawn();
          SM.PlayGenericSound(this.AudEvent_Teleport, this.EndCh_Josh.SpawnInPoint.position);
          UnityEngine.Object.Instantiate<GameObject>(this.TeleportEffect, this.EndCh_Josh.SpawnInPoint.position, this.EndCh_Josh.SpawnInPoint.rotation);
          this.m_phaseTimer = 1f;
          break;
        case ZosigEndingManager.RW_EndPhase.TeleportIn4:
          this.EndCh_Alister.Spawn();
          SM.PlayGenericSound(this.AudEvent_Teleport, this.EndCh_Alister.SpawnInPoint.position);
          UnityEngine.Object.Instantiate<GameObject>(this.TeleportEffect, this.EndCh_Alister.SpawnInPoint.position, this.EndCh_Alister.SpawnInPoint.rotation);
          this.m_phaseTimer = 1f;
          break;
        case ZosigEndingManager.RW_EndPhase.TeleportIn5:
          this.EndCh_Ralph.Spawn();
          SM.PlayGenericSound(this.AudEvent_Teleport, this.EndCh_Ralph.SpawnInPoint.position);
          UnityEngine.Object.Instantiate<GameObject>(this.TeleportEffect, this.EndCh_Ralph.SpawnInPoint.position, this.EndCh_Ralph.SpawnInPoint.rotation);
          this.m_phaseTimer = 2f;
          break;
        case ZosigEndingManager.RW_EndPhase.Bobert:
          GM.ZMaster.SetMusic_Speaking();
          this.EndCh_Bobert.AudSource.clip = this.Lines_Bobert[this.b_clip];
          this.m_phaseTimer = this.EndCh_Bobert.BeginSpeaking() + 0.5f;
          break;
        case ZosigEndingManager.RW_EndPhase.Dustin:
          this.EndCh_Dustin.AudSource.clip = this.Lines_Dustin[this.d_clip];
          this.m_phaseTimer = this.EndCh_Dustin.BeginSpeaking() + 0.5f;
          break;
        case ZosigEndingManager.RW_EndPhase.Josh:
          this.EndCh_Josh.AudSource.clip = this.Lines_Josh[this.j_clip];
          this.m_phaseTimer = this.EndCh_Josh.BeginSpeaking() + 0.5f;
          break;
        case ZosigEndingManager.RW_EndPhase.Alister:
          this.EndCh_Alister.AudSource.clip = this.Lines_Alister[this.a_clip];
          this.m_phaseTimer = this.EndCh_Alister.BeginSpeaking() + 0.5f;
          break;
        case ZosigEndingManager.RW_EndPhase.Ralph:
          this.EndCh_Ralph.AudSource.clip = this.Lines_Ralph[this.r_clip];
          this.m_phaseTimer = this.EndCh_Ralph.BeginSpeaking() + 0.5f;
          break;
        case ZosigEndingManager.RW_EndPhase.FinalLine:
          this.m_isScreenOn = true;
          this.m_isScreenTalking = true;
          this.AudSource_Narrator_Reverbee.clip = this.Lines_Narrator[this.n_clip];
          this.AudSource_Narrator_Reverbee.Play();
          this.m_phaseTimer = this.AudSource_Narrator_Reverbee.clip.length + 3f;
          break;
        case ZosigEndingManager.RW_EndPhase.TeleportOut1:
          GM.ZMaster.SetMusic_Gameplay();
          SM.PlayGenericSound(this.AudEvent_Teleport, this.EndCh_Bobert.SpawnInPoint.position);
          UnityEngine.Object.Instantiate<GameObject>(this.TeleportEffect, this.EndCh_Bobert.SpawnInPoint.position, this.EndCh_Bobert.SpawnInPoint.rotation);
          this.EndCh_Bobert.ClearSosig();
          this.m_phaseTimer = 1f;
          break;
        case ZosigEndingManager.RW_EndPhase.TeleportOut2:
          SM.PlayGenericSound(this.AudEvent_Teleport, this.EndCh_Dustin.SpawnInPoint.position);
          UnityEngine.Object.Instantiate<GameObject>(this.TeleportEffect, this.EndCh_Dustin.SpawnInPoint.position, this.EndCh_Dustin.SpawnInPoint.rotation);
          this.EndCh_Dustin.ClearSosig();
          this.m_phaseTimer = 1f;
          break;
        case ZosigEndingManager.RW_EndPhase.TeleportOut3:
          SM.PlayGenericSound(this.AudEvent_Teleport, this.EndCh_Josh.SpawnInPoint.position);
          UnityEngine.Object.Instantiate<GameObject>(this.TeleportEffect, this.EndCh_Josh.SpawnInPoint.position, this.EndCh_Josh.SpawnInPoint.rotation);
          this.EndCh_Josh.ClearSosig();
          this.m_phaseTimer = 1f;
          break;
        case ZosigEndingManager.RW_EndPhase.TeleportOut4:
          SM.PlayGenericSound(this.AudEvent_Teleport, this.EndCh_Alister.SpawnInPoint.position);
          UnityEngine.Object.Instantiate<GameObject>(this.TeleportEffect, this.EndCh_Alister.SpawnInPoint.position, this.EndCh_Alister.SpawnInPoint.rotation);
          this.EndCh_Alister.ClearSosig();
          this.m_phaseTimer = 1f;
          break;
        case ZosigEndingManager.RW_EndPhase.TeleportOut5:
          SM.PlayGenericSound(this.AudEvent_Teleport, this.EndCh_Ralph.SpawnInPoint.position);
          UnityEngine.Object.Instantiate<GameObject>(this.TeleportEffect, this.EndCh_Ralph.SpawnInPoint.position, this.EndCh_Ralph.SpawnInPoint.rotation);
          this.EndCh_Ralph.ClearSosig();
          this.m_phaseTimer = 1f;
          break;
        case ZosigEndingManager.RW_EndPhase.ScreenOff:
          this.FinalScreen.material.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.0f, 1f));
          this.m_phaseTimer = 1f;
          break;
        case ZosigEndingManager.RW_EndPhase.Unlocks:
          this.DisplayUnlocks(true);
          this.m_phaseTimer = 15f;
          break;
        case ZosigEndingManager.RW_EndPhase.Credits0:
          this.DisplayUnlocks(false);
          this.DisplayCredits(0);
          this.m_phaseTimer = 15f;
          break;
        case ZosigEndingManager.RW_EndPhase.Credits1:
          this.DisplayCredits(1);
          this.m_phaseTimer = 15f;
          break;
        case ZosigEndingManager.RW_EndPhase.Credits2:
          this.DisplayCredits(2);
          this.m_phaseTimer = 15f;
          break;
        case ZosigEndingManager.RW_EndPhase.Credits3:
          this.DisplayCredits(3);
          this.m_phaseTimer = 15f;
          break;
        case ZosigEndingManager.RW_EndPhase.Credits4:
          this.DisplayCredits(4);
          this.m_phaseTimer = 15f;
          break;
        case ZosigEndingManager.RW_EndPhase.FadeOut:
          SteamVR_Fade.Start(Color.black, 2.5f);
          this.m_phaseTimer = 3f;
          break;
        case ZosigEndingManager.RW_EndPhase.LevelLoad:
          GM.CurrentMovementManager.Hands[0].gameObject.SetActive(true);
          GM.CurrentMovementManager.Hands[1].gameObject.SetActive(true);
          SteamVR_LoadLevel.Begin("RotWienersStagingScene");
          break;
      }
    }

    private void DisplayCredits(int page)
    {
      if (page < 0)
      {
        this.CreditsCanvas.SetActive(false);
      }
      else
      {
        this.CreditsCanvas.SetActive(true);
        for (int index = 0; index < this.CreditsPages.Count; ++index)
        {
          if (index == page)
            this.CreditsPages[index].SetActive(true);
          else
            this.CreditsPages[index].SetActive(false);
        }
      }
    }

    private void DisplayUnlocks(bool b)
    {
      string str = "New Unlocks Earned:";
      if (GM.ZMaster.FlagM.GetFlagValue("flag_Difficulty") == 0)
      {
        GM.ROTRWSaves.HBC = true;
        for (int index = 0; index < this.RW_Classic.Count; ++index)
        {
          if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(this.RW_Classic[index]))
          {
            GM.Rewards.RewardUnlocks.UnlockReward(this.RW_Classic[index]);
            str = str + "\nUnlocked: " + this.RW_Classic[index].DisplayName + "!";
          }
        }
      }
      else if (GM.ZMaster.FlagM.GetFlagValue("flag_Difficulty") == 1)
      {
        GM.ROTRWSaves.HBA = true;
        for (int index = 0; index < this.RW_Arcade.Count; ++index)
        {
          if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(this.RW_Arcade[index]))
          {
            GM.Rewards.RewardUnlocks.UnlockReward(this.RW_Arcade[index]);
            str = str + "\nUnlocked: " + this.RW_Arcade[index].DisplayName + "!";
          }
        }
      }
      else if (GM.ZMaster.FlagM.GetFlagValue("flag_Difficulty") == 2)
      {
        GM.ROTRWSaves.HBH = true;
        for (int index = 0; index < this.RW_Hardcore.Count; ++index)
        {
          if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(this.RW_Hardcore[index]))
          {
            GM.Rewards.RewardUnlocks.UnlockReward(this.RW_Hardcore[index]);
            str = str + "\nUnlocked: " + this.RW_Hardcore[index].DisplayName + "!";
          }
        }
      }
      if (this.r_clip == 0)
      {
        for (int index = 0; index < this.RW_Ralph.Count; ++index)
        {
          if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(this.RW_Ralph[index]))
          {
            GM.Rewards.RewardUnlocks.UnlockReward(this.RW_Ralph[index]);
            str = str + "\nUnlocked: " + this.RW_Ralph[index].DisplayName + "!";
          }
        }
      }
      if (this.a_clip == 0)
      {
        for (int index = 0; index < this.RW_Alister.Count; ++index)
        {
          if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(this.RW_Alister[index]))
          {
            GM.Rewards.RewardUnlocks.UnlockReward(this.RW_Alister[index]);
            str = str + "\nUnlocked: " + this.RW_Alister[index].DisplayName + "!";
          }
        }
      }
      if (this.b_clip == 0)
      {
        for (int index = 0; index < this.RW_Bobert.Count; ++index)
        {
          if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(this.RW_Bobert[index]))
          {
            GM.Rewards.RewardUnlocks.UnlockReward(this.RW_Bobert[index]);
            str = str + "\nUnlocked: " + this.RW_Bobert[index].DisplayName + "!";
          }
        }
      }
      if (this.d_clip == 0)
      {
        for (int index = 0; index < this.RW_Dustin.Count; ++index)
        {
          if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(this.RW_Dustin[index]))
          {
            GM.Rewards.RewardUnlocks.UnlockReward(this.RW_Dustin[index]);
            str = str + "\nUnlocked: " + this.RW_Dustin[index].DisplayName + "!";
          }
        }
      }
      if (this.j_clip == 0)
      {
        for (int index = 0; index < this.RW_Josh.Count; ++index)
        {
          if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(this.RW_Josh[index]))
          {
            GM.Rewards.RewardUnlocks.UnlockReward(this.RW_Josh[index]);
            str = str + "\nUnlocked: " + this.RW_Josh[index].DisplayName + "!";
          }
        }
      }
      if (this.n_clip == 4)
      {
        for (int index = 0; index < this.RW_Nar.Count; ++index)
        {
          if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(this.RW_Nar[index]))
          {
            GM.Rewards.RewardUnlocks.UnlockReward(this.RW_Nar[index]);
            str = str + "\nUnlocked: " + this.RW_Nar[index].DisplayName + "!";
          }
        }
      }
      this.UnlockLabel.text = str;
      if (b)
      {
        this.UnlocksCanvas.SetActive(true);
        if (GM.ZMaster.FlagM.GetFlagValue("flag_Difficulty") == 0)
          GM.ROTRWSaves.HBC = true;
        else if (GM.ZMaster.FlagM.GetFlagValue("flag_Difficulty") == 1)
          GM.ROTRWSaves.HBA = true;
        else if (GM.ZMaster.FlagM.GetFlagValue("flag_Difficulty") == 2)
          GM.ROTRWSaves.HBH = true;
      }
      else
        this.UnlocksCanvas.SetActive(false);
      GM.ZMaster.FlagM.Save();
      GM.ROTRWSaves.SaveToFile();
      GM.Rewards.SaveToFile();
    }

    private void Update()
    {
      if (!this.m_isEnding)
        return;
      switch (this.EndPhase)
      {
        case ZosigEndingManager.RW_EndPhase.RoarnartSpeaks:
          this.EndCh_Roarnart.Update(Time.deltaTime);
          break;
        case ZosigEndingManager.RW_EndPhase.RoarnartContinues:
          if (!this.AudSource_Narrator_Reverbee.isPlaying)
          {
            this.m_isScreenOn = true;
            this.m_isScreenTalking = false;
            this.m_screenIntensity = 1f;
          }
          else
          {
            this.m_screenIntensityTick -= Time.deltaTime;
            if ((double) this.m_screenIntensityTick <= 0.0)
            {
              this.m_screenIntensityTick = UnityEngine.Random.Range(0.05f, 0.2f);
              this.m_screenIntensity = UnityEngine.Random.Range(1f, 5f);
            }
          }
          this.FinalScreen.material.SetColor("_EmissionColor", new Color(this.m_screenIntensity, this.m_screenIntensity, this.m_screenIntensity, 1f));
          break;
        case ZosigEndingManager.RW_EndPhase.Bobert:
          this.EndCh_Bobert.Update(Time.deltaTime);
          break;
        case ZosigEndingManager.RW_EndPhase.Dustin:
          this.EndCh_Dustin.Update(Time.deltaTime);
          break;
        case ZosigEndingManager.RW_EndPhase.Josh:
          this.EndCh_Josh.Update(Time.deltaTime);
          break;
        case ZosigEndingManager.RW_EndPhase.Alister:
          this.EndCh_Alister.Update(Time.deltaTime);
          break;
        case ZosigEndingManager.RW_EndPhase.Ralph:
          this.EndCh_Ralph.Update(Time.deltaTime);
          break;
        case ZosigEndingManager.RW_EndPhase.FinalLine:
          if (!this.AudSource_Narrator_Reverbee.isPlaying)
          {
            this.m_isScreenOn = true;
            this.m_isScreenTalking = false;
            this.m_screenIntensity = 1f;
          }
          else
          {
            this.m_screenIntensityTick -= Time.deltaTime;
            if ((double) this.m_screenIntensityTick <= 0.0)
            {
              this.m_screenIntensityTick = UnityEngine.Random.Range(0.03f, 0.08f);
              this.m_screenIntensity = UnityEngine.Random.Range(1f, 5f);
            }
          }
          this.FinalScreen.material.SetColor("_EmissionColor", new Color(this.m_screenIntensity, this.m_screenIntensity, this.m_screenIntensity, 1f));
          break;
      }
      this.m_phaseTimer -= Time.deltaTime;
      if ((double) this.m_phaseTimer > 0.0 || this.EndPhase == ZosigEndingManager.RW_EndPhase.LevelLoad)
        return;
      this.InitPhase(this.EndPhase + 1);
    }

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
      LevelLoad,
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
        this.m_isSpeaking = true;
        this.AudSource.Play();
        return this.AudSource.clip.length;
      }

      public void Spawn() => this.S = this.SpawnManager.SpawnNPCToPoint(this.Template, this.Index, this.SpawnInPoint);

      public void Explode()
      {
        if (!((UnityEngine.Object) this.S != (UnityEngine.Object) null))
          return;
        this.S.ClearSosig();
      }

      public void ClearSosig()
      {
        if (!((UnityEngine.Object) this.S != (UnityEngine.Object) null))
          return;
        this.S.DeSpawnSosig();
      }

      public void Update(float t)
      {
        if (!this.AudSource.isPlaying)
          this.m_isSpeaking = false;
        if (!this.m_isSpeaking)
          return;
        if ((double) this.HeadJitterTick > 0.0)
        {
          this.HeadJitterTick -= Time.deltaTime;
        }
        else
        {
          this.HeadJitterTick = UnityEngine.Random.Range(this.Profile.SpeakJitterRange.x, this.Profile.SpeakJitterRange.y);
          if (!((UnityEngine.Object) this.S != (UnityEngine.Object) null) || !((UnityEngine.Object) this.S.Links[0] != (UnityEngine.Object) null))
            return;
          this.S.Links[0].R.AddForceAtPosition(UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(this.Profile.SpeakPowerRange.x, this.Profile.SpeakPowerRange.y), this.S.Links[0].transform.position + Vector3.up * 0.3f, ForceMode.Impulse);
        }
      }
    }
  }
}
