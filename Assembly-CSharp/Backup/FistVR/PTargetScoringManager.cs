// Decompiled with JetBrains decompiler
// Type: FistVR.PTargetScoringManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class PTargetScoringManager : MonoBehaviour
  {
    public List<PTargetScoringManager.ScoreSet> Sets = new List<PTargetScoringManager.ScoreSet>();
    private int m_curSet = -1;
    private PTargetScoringManager.SetMode m_selectedMode;
    private PTargetScoringManager.TimerCountdown m_selectedCountDown;
    private PTargetScoringManager.TimerClockMode m_selectedClockMode;
    private PTargetScoringManager.SetMode m_mode;
    private PTargetScoringManager.TimerCountdown m_countDown;
    private PTargetScoringManager.TimerClockMode m_clockMode;
    private float m_tickDownToSet = 5f;
    private float m_timer;
    private int m_curSetDisplay = -1;
    private int m_curPageDisplay;
    [Header("Sheet Display")]
    public GameObject TargetSheetDisplay;
    public GameObject TargetTimingsDisplay;
    public Text SetNumeral;
    public Text SetShotNumbers;
    public Text SetScores;
    public Text SetTimes;
    public Text SetSplits;
    public Text SetTotalShots;
    public Text SetTotalScore;
    public Text SetTotalAVG;
    public Transform[] HitIndicators;
    public Transform LastHitIndicator;
    public Transform UL;
    public Transform UR;
    public Transform LL;
    public Transform LR;
    [Header("Audio")]
    public AudioEvent AudEvent_ShotClock;
    public AudioEvent AudEvent_Beep;
    public AudioEvent AudEvent_Boop;

    private void Start() => this.CreateNewSetAndAdvancedToIt();

    public void ProcessHit(int score, Vector2 uv)
    {
      if (this.m_mode != PTargetScoringManager.SetMode.Open && (double) this.m_tickDownToSet > 0.0 || !this.Sets[this.m_curSet].RegisterScore(score, this.m_timer, uv))
        return;
      this.UpdateScoreListDisplay();
      this.UpdateTargetSheet();
    }

    private void ShotClock() => SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_ShotClock, this.transform.position);

    private void Beep() => SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Beep, this.transform.position);

    private void Boop() => SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Boop, this.transform.position);

    private void Update()
    {
      if (this.m_mode != PTargetScoringManager.SetMode.Timed)
        return;
      if ((double) this.m_tickDownToSet > 0.0)
      {
        if (this.m_clockMode == PTargetScoringManager.TimerClockMode.Standard)
          this.m_tickDownToSet -= Time.deltaTime;
        else if (this.m_clockMode == PTargetScoringManager.TimerClockMode.MuzzleDown)
        {
          bool flag = true;
          for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
          {
            if ((UnityEngine.Object) GM.CurrentMovementManager.Hands[index].CurrentInteractable != (UnityEngine.Object) null && GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArm && (double) Vector3.Angle((GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArm).GetMuzzle().forward, -Vector3.up) > 45.0)
              flag = false;
          }
          if (flag)
            this.m_tickDownToSet -= Time.deltaTime;
        }
        else if (this.m_clockMode == PTargetScoringManager.TimerClockMode.EmptyHands && (UnityEngine.Object) GM.CurrentMovementManager.Hands[0].CurrentInteractable == (UnityEngine.Object) null && (UnityEngine.Object) GM.CurrentMovementManager.Hands[1].CurrentInteractable == (UnityEngine.Object) null)
          this.m_tickDownToSet -= Time.deltaTime;
        if ((double) this.m_tickDownToSet > 0.0)
          return;
        this.ShotClock();
      }
      else
        this.m_timer += Time.deltaTime;
    }

    public void BTNPress_SetMode(int i)
    {
      this.m_selectedMode = (PTargetScoringManager.SetMode) i;
      this.Beep();
    }

    public void BTNPress_SetCountdown(int i)
    {
      this.m_selectedCountDown = (PTargetScoringManager.TimerCountdown) i;
      this.Beep();
    }

    public void BTNPress_SetClockMode(int i)
    {
      this.m_selectedClockMode = (PTargetScoringManager.TimerClockMode) i;
      this.Beep();
    }

    public void BTNPress_BeginNewSet()
    {
      this.CreateNewSetAndAdvancedToIt();
      this.Boop();
    }

    public void BTNPress_SetToTimings()
    {
      this.TargetTimingsDisplay.SetActive(true);
      this.TargetSheetDisplay.SetActive(false);
      this.Beep();
      this.UpdateScoreListDisplay();
      this.UpdateTargetSheet();
    }

    public void BTNPress_SetToTargetSheet()
    {
      this.TargetTimingsDisplay.SetActive(false);
      this.TargetSheetDisplay.SetActive(true);
      this.Beep();
      this.UpdateScoreListDisplay();
      this.UpdateTargetSheet();
    }

    public void BTNPress_SetToNextSet()
    {
      ++this.m_curSetDisplay;
      if (this.m_curSetDisplay >= this.Sets.Count)
        this.m_curSetDisplay = this.Sets.Count - 1;
      this.Beep();
      this.m_curPageDisplay = 0;
      this.UpdateScoreListDisplay();
      this.UpdateTargetSheet();
    }

    public void BTNPress_SetToPrevSet()
    {
      --this.m_curSetDisplay;
      if (this.m_curSetDisplay < 0)
        this.m_curSetDisplay = 0;
      this.Beep();
      this.m_curPageDisplay = 0;
      this.UpdateScoreListDisplay();
      this.UpdateTargetSheet();
    }

    public void BTNPress_SetToNextShots()
    {
      int max = Mathf.FloorToInt((float) (this.Sets[this.m_curSet].NumShotsRegistered / 15));
      ++this.m_curPageDisplay;
      this.Beep();
      this.m_curPageDisplay = Mathf.Clamp(this.m_curPageDisplay, 0, max);
      this.UpdateScoreListDisplay();
      this.UpdateTargetSheet();
    }

    public void BTNPress_SetToPrevShots()
    {
      --this.m_curPageDisplay;
      this.Beep();
      if (this.m_curPageDisplay < 0)
        this.m_curPageDisplay = 0;
      this.UpdateScoreListDisplay();
      this.UpdateTargetSheet();
    }

    private void CreateNewSetAndAdvancedToIt()
    {
      this.Sets.Add(new PTargetScoringManager.ScoreSet());
      this.m_curSet = this.Sets.Count - 1;
      this.m_curSetDisplay = this.m_curSet;
      this.m_mode = this.m_selectedMode;
      this.m_countDown = this.m_selectedCountDown;
      this.m_clockMode = this.m_selectedClockMode;
      this.m_timer = 0.0f;
      if (this.m_mode == PTargetScoringManager.SetMode.Timed)
      {
        if (this.m_countDown == PTargetScoringManager.TimerCountdown.Sec5)
          this.m_tickDownToSet = 5f;
        if (this.m_countDown == PTargetScoringManager.TimerCountdown.Sec10)
          this.m_tickDownToSet = 10f;
        if (this.m_countDown == PTargetScoringManager.TimerCountdown.Sec5To10)
          this.m_tickDownToSet = UnityEngine.Random.Range(5f, 10f);
      }
      else
        this.m_tickDownToSet = -0.01f;
      this.UpdateScoreListDisplay();
      this.UpdateTargetSheet();
    }

    private void UpdateScoreListDisplay()
    {
      this.SetNumeral.text = this.m_curSetDisplay.ToString();
      string str1 = string.Empty;
      string str2 = string.Empty;
      string str3 = string.Empty;
      string str4 = string.Empty;
      PTargetScoringManager.ScoreSet set = this.Sets[this.m_curSetDisplay];
      int num1 = this.m_curPageDisplay * 15;
      int num2 = Mathf.Min(this.m_curPageDisplay * 15 + 15, set.NumShotsRegistered);
      for (int index = num1; index < num2; ++index)
      {
        str1 = str1 + (object) (index + 1) + ":\n";
        str2 = str2 + set.Scores[index].ToString() + "\n";
        if (this.m_mode == PTargetScoringManager.SetMode.Timed)
        {
          str3 = str3 + this.FloatToTime(set.Times[index], "#0:00.000") + "\n";
          float toConvert = set.Times[index];
          if (index > 0)
            toConvert = set.Times[index] - set.Times[index - 1];
          str4 = str4 + this.FloatToTime(toConvert, "#0:00.000") + "\n";
        }
      }
      this.SetShotNumbers.text = str1;
      this.SetScores.text = str2;
      this.SetTimes.text = str3;
      this.SetSplits.text = str4;
      this.SetTotalShots.text = "Set Total (" + set.NumShotsRegistered.ToString() + "):";
      this.SetTotalScore.text = set.TotalScore.ToString() + " Pts";
      if (set.NumShotsRegistered == 0)
        this.SetTotalAVG.text = string.Empty;
      else
        this.SetTotalAVG.text = Math.Round((double) set.TotalScore / (double) set.NumShotsRegistered, 2).ToString() + " Avg";
    }

    private void UpdateTargetSheet()
    {
      PTargetScoringManager.ScoreSet set = this.Sets[this.m_curSetDisplay];
      if (set.NumShotsRegistered == 0)
        this.LastHitIndicator.gameObject.SetActive(false);
      int index1 = 0;
      for (int index2 = this.m_curPageDisplay * 15; index2 < set.NumShotsRegistered && index1 < this.HitIndicators.Length; ++index2)
      {
        Vector3 vector3 = Vector3.Lerp(this.UL.position, this.UR.position, set.UV[index2].x);
        vector3.y = Mathf.Lerp(this.LL.position.y, this.UL.position.y, set.UV[index2].y);
        this.HitIndicators[index1].gameObject.SetActive(true);
        this.HitIndicators[index1].position = vector3;
        if (index2 == set.NumShotsRegistered - 1)
        {
          this.LastHitIndicator.gameObject.SetActive(true);
          this.LastHitIndicator.position = vector3;
        }
        ++index1;
      }
      for (int index2 = index1; index2 < 15; ++index2)
      {
        if (index2 < this.HitIndicators.Length)
          this.HitIndicators[index2].gameObject.SetActive(false);
      }
    }

    public string FloatToTime(float toConvert, string format)
    {
      if (format != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (PTargetScoringManager.\u003C\u003Ef__switch\u0024map9 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PTargetScoringManager.\u003C\u003Ef__switch\u0024map9 = new Dictionary<string, int>(13)
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
        if (PTargetScoringManager.\u003C\u003Ef__switch\u0024map9.TryGetValue(format, out num))
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

    public enum SetMode
    {
      Open,
      Timed,
    }

    public enum TimerCountdown
    {
      Sec5,
      Sec10,
      Sec5To10,
    }

    public enum TimerClockMode
    {
      Standard,
      MuzzleDown,
      EmptyHands,
    }

    public class ScoreSet
    {
      public int NumShotsRegistered;
      public List<int> Scores = new List<int>();
      public List<float> Times = new List<float>();
      public List<Vector2> UV = new List<Vector2>();
      public int TotalScore;
      public float FinalTime;

      public bool RegisterScore(int score, float time, Vector2 uv)
      {
        this.Scores.Add(score);
        this.Times.Add(time);
        this.UV.Add(uv);
        ++this.NumShotsRegistered;
        this.TotalScore += score;
        this.FinalTime = Mathf.Max(this.FinalTime, time);
        return true;
      }
    }
  }
}
