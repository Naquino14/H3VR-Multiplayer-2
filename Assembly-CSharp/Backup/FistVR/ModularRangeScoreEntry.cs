// Decompiled with JetBrains decompiler
// Type: FistVR.ModularRangeScoreEntry
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ModularRangeScoreEntry : MonoBehaviour
  {
    public ModularRangeMaster Master;
    public Text ScorePanel;
    public Text NamePanel;
    private int score;
    public string ScoreNameEntry = string.Empty;
    private bool hasBeenEntered;
    public int MaxCharacters = 20;
    private AudioSource aud;

    public void Awake() => this.aud = this.GetComponent<AudioSource>();

    public void KeyEntry(string s)
    {
      this.aud.PlayOneShot(this.aud.clip);
      if (s == "Del")
      {
        if (this.ScoreNameEntry.Length > 0)
          this.ScoreNameEntry = this.ScoreNameEntry.Substring(0, this.ScoreNameEntry.Length - 1);
      }
      else if (s == "Enter")
      {
        if (this.ScoreNameEntry.Length > 0)
        {
          this.LogScore();
          this.Master.GoToSequenceSelector();
        }
      }
      else if (this.ScoreNameEntry.Length < this.MaxCharacters)
        this.ScoreNameEntry += s;
      this.NamePanel.text = this.ScoreNameEntry;
    }

    public void SetScoreDisplay(int i)
    {
      this.score = i;
      this.ScorePanel.text = this.score.ToString();
    }

    private void LogScore()
    {
      int selectedSequenceIndex = this.Master.SelectedSequenceIndex;
      string scoreNameEntry = this.ScoreNameEntry;
      int score = this.score;
      SortedList<string, object> fields = new SortedList<string, object>();
      string endPointName = this.Master.m_currentSequenceDefinition.MetaData.EndPointName;
      fields.Add("PlayerName", (object) scoreNameEntry);
      fields.Add("PlayerScore", (object) score);
      Assets.Rust.Lodestone.Lodestone.Log(endPointName, fields);
    }
  }
}
