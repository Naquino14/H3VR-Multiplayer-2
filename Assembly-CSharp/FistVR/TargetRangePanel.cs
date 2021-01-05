// Decompiled with JetBrains decompiler
// Type: FistVR.TargetRangePanel
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class TargetRangePanel : MonoBehaviour
  {
    private int m_unitType;
    private string m_stringInput = string.Empty;
    public float MaxMeters;
    public Text DistanceLabel;
    public OptionsPanel_ButtonSet OBS_UnitType;
    private float m_storedDistanceInMeters;
    private int m_maxMeters;
    private int m_maxYards;
    private int m_maxFeet;
    public PaperTarget PaperTarget;
    public Text ScoreSheet;
    private TargetRangePanel.ScoreSet CurrentScoreSet = new TargetRangePanel.ScoreSet(0.0f, 0, 0);
    public List<TargetRangePanel.ScoreSet> PastScoreSets = new List<TargetRangePanel.ScoreSet>();

    private void Start()
    {
      this.OBS_UnitType.SetSelectedButton(0);
      this.DistanceLabel.text = "0 Meters";
      this.m_maxMeters = Mathf.FloorToInt(this.MaxMeters);
      this.m_maxYards = Mathf.FloorToInt(this.MaxMeters * 1.09361f);
      this.m_maxFeet = Mathf.FloorToInt(this.MaxMeters * 3.28084f);
    }

    public float GetDesiredDistance() => this.m_storedDistanceInMeters;

    public void InputNumeral(string s)
    {
      this.m_stringInput += s;
      this.UpdateCalc();
      this.UpdateDisplay();
      this.UpdateStoredDesiredDistance();
    }

    public void Clear(string s)
    {
      this.m_stringInput = string.Empty;
      this.UpdateDisplay();
      this.UpdateStoredDesiredDistance();
    }

    public void SetUnits(int i)
    {
      this.m_unitType = i;
      this.UpdateDisplay();
      this.UpdateStoredDesiredDistance();
      this.UpdatePaperSheet();
    }

    private void UpdateCalc()
    {
      int num = int.Parse(this.m_stringInput);
      switch (this.m_unitType)
      {
        case 0:
          num = Mathf.Clamp(num, 0, this.m_maxMeters);
          break;
        case 1:
          num = Mathf.Clamp(num, 0, this.m_maxYards);
          break;
        case 2:
          num = Mathf.Clamp(num, 0, this.m_maxFeet);
          break;
      }
      this.m_stringInput = num.ToString();
    }

    private void UpdateDisplay()
    {
      if (this.m_stringInput == string.Empty)
      {
        this.m_storedDistanceInMeters = 0.0f;
        this.DistanceLabel.text = 0.ToString() + " " + this.GetUnitDisplayName();
      }
      else
        this.DistanceLabel.text = this.m_stringInput + " " + this.GetUnitDisplayName();
    }

    private void UpdateStoredDesiredDistance()
    {
      if (this.m_stringInput == string.Empty)
      {
        this.m_storedDistanceInMeters = 0.0f;
      }
      else
      {
        switch (this.m_unitType)
        {
          case 0:
            this.m_storedDistanceInMeters = (float) int.Parse(this.m_stringInput);
            break;
          case 1:
            this.m_storedDistanceInMeters = (float) int.Parse(this.m_stringInput) * 0.9144f;
            break;
          case 2:
            this.m_storedDistanceInMeters = (float) int.Parse(this.m_stringInput) * 0.3048f;
            break;
        }
      }
    }

    private string GetUnitDisplayName()
    {
      switch (this.m_unitType)
      {
        case 0:
          return "Meters";
        case 1:
          return "Yards";
        case 2:
          return "Feet";
        default:
          return "Meters";
      }
    }

    public void AdvanceToNextSet()
    {
      if (this.CurrentScoreSet.Shots <= 0)
        return;
      TargetRangePanel.ScoreSet scoreSet = new TargetRangePanel.ScoreSet(0.0f, 0, 0);
      this.PastScoreSets.Add(this.CurrentScoreSet);
      if (this.PastScoreSets.Count > 5)
        this.PastScoreSets.RemoveAt(0);
      this.CurrentScoreSet.Score = 0;
      this.CurrentScoreSet.MetersDistance = 0.0f;
      this.CurrentScoreSet.Shots = 0;
      this.PaperTarget.ClearHoles();
      this.UpdatePaperSheet();
    }

    public void UpdatePaperSheet()
    {
      string str1 = "Score List\n(Distance Shown is Closest In Set)\n\nTap Sheet For New Set\n\n";
      Vector3 currentScoring = this.PaperTarget.GetCurrentScoring();
      this.CurrentScoreSet.Shots = Mathf.FloorToInt(currentScoring.x);
      this.CurrentScoreSet.Score = Mathf.FloorToInt(currentScoring.y);
      this.CurrentScoreSet.MetersDistance = currentScoring.z;
      string str2 = str1 + "Current Distance: " + this.GetValueInCurrentUnits(this.CurrentScoreSet.MetersDistance).ToString() + " " + this.GetUnitDisplayName() + "\n" + "- Shots: " + this.CurrentScoreSet.Shots.ToString() + "   - Points: " + (object) this.CurrentScoreSet.Score + "\n\n";
      if (this.PastScoreSets.Count > 0)
      {
        for (int index = this.PastScoreSets.Count - 1; index >= 0; --index)
          str2 = str2 + "Previous Set Distance: " + this.GetValueInCurrentUnits(this.PastScoreSets[index].MetersDistance).ToString() + " " + this.GetUnitDisplayName() + "\n" + "- Shots: " + this.PastScoreSets[index].Shots.ToString() + "   - Points: " + (object) this.PastScoreSets[index].Score + "\n\n";
      }
      this.ScoreSheet.text = str2;
    }

    private int GetValueInCurrentUnits(float meters)
    {
      switch (this.m_unitType)
      {
        case 0:
          return Mathf.FloorToInt(meters);
        case 1:
          return Mathf.FloorToInt(meters * 1.09361f);
        case 2:
          return Mathf.FloorToInt(meters * 3.28084f);
        default:
          return Mathf.FloorToInt(meters);
      }
    }

    public struct ScoreSet
    {
      public float MetersDistance;
      public int Shots;
      public int Score;

      public ScoreSet(float m, int sh, int sc)
      {
        this.MetersDistance = m;
        this.Shots = sh;
        this.Score = sc;
      }
    }
  }
}
