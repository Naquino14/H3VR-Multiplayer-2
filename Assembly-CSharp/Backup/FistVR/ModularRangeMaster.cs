// Decompiled with JetBrains decompiler
// Type: FistVR.ModularRangeMaster
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ModularRangeMaster : MonoBehaviour
  {
    public ModularRangeSequencer m_currentSequencer;
    public ModularRangeScoreEntry ScoreEntry;
    public ModularRangeHighScoreBoard ScoreBoard;
    public GameObject SequenceSelectorGO;
    public GameObject MainGameDisplayGO;
    public GameObject ScoreEntryGO;
    public AudioSource AudioSource_Event;
    public Text InGameDisplay;
    public Text ScoreDisplay;
    private float m_displayTimer;
    private ModularRangeSequenceDefinition.SequenceCategory m_selectedSequenceCategory;
    private int m_selectedSequenceIndex;
    public ModularRangeSequenceDefinition m_currentSequenceDefinition;
    public ModularRangeMaster.ModularRangeSequenceCategory[] SequenceCategories;
    public Dictionary<ModularRangeSequenceDefinition.SequenceCategory, ModularRangeMaster.ModularRangeSequenceCategory> SequenceDic = new Dictionary<ModularRangeSequenceDefinition.SequenceCategory, ModularRangeMaster.ModularRangeSequenceCategory>();
    public Image[] SequenceCategoryImages;
    public Image[] SequenceIndexImages;
    public Text SequenceDetailsText;
    public Color SequenceSelected;
    public Color SequenceUnSelected;
    private ModularRangeMaster.RangeMasterState m_state;
    private AudioSource aud;

    public ModularRangeSequenceDefinition.SequenceCategory SelectedSequenceCategory => this.m_selectedSequenceCategory;

    public int SelectedSequenceIndex => this.m_selectedSequenceIndex;

    public void Awake()
    {
      this.aud = this.GetComponent<AudioSource>();
      this.GoToSequenceSelector();
      for (int index = 0; index < this.SequenceCategories.Length; ++index)
      {
        if (!this.SequenceDic.ContainsKey(this.SequenceCategories[index].CategoryType))
          this.SequenceDic.Add(this.SequenceCategories[index].CategoryType, this.SequenceCategories[index]);
      }
      for (int index = 0; index < this.SequenceCategoryImages.Length; ++index)
        this.SequenceCategoryImages[index].gameObject.SetActive(false);
      if (this.SequenceDic[ModularRangeSequenceDefinition.SequenceCategory.Reflex].Sequences.Length > 0)
        this.SequenceCategoryImages[0].gameObject.SetActive(true);
      if (this.SequenceDic[ModularRangeSequenceDefinition.SequenceCategory.Recognition].Sequences.Length > 0)
        this.SequenceCategoryImages[1].gameObject.SetActive(true);
      if (this.SequenceDic[ModularRangeSequenceDefinition.SequenceCategory.Precision].Sequences.Length > 0)
        this.SequenceCategoryImages[2].gameObject.SetActive(true);
      if (this.SequenceDic[ModularRangeSequenceDefinition.SequenceCategory.Cognition].Sequences.Length > 0)
        this.SequenceCategoryImages[3].gameObject.SetActive(true);
      this.SetSequenceCategory("Reflex");
    }

    private void BeginSequence(string s)
    {
      this.m_currentSequenceDefinition = this.SequenceDic[this.m_selectedSequenceCategory].Sequences[this.m_selectedSequenceIndex];
      if (this.m_state != ModularRangeMaster.RangeMasterState.WaitingForCommand)
        return;
      this.m_currentSequencer.BeginSequence(this, this.m_currentSequenceDefinition);
      this.SetState(ModularRangeMaster.RangeMasterState.InSequence);
    }

    public void GoToHighScoreBoard()
    {
      this.ScoreEntry.SetScoreDisplay(this.m_currentSequencer.GetScore());
      this.SetState(ModularRangeMaster.RangeMasterState.HighScoreEntry);
    }

    public void GoToSequenceSelector() => this.SetState(ModularRangeMaster.RangeMasterState.WaitingForCommand);

    public void SetState(ModularRangeMaster.RangeMasterState state)
    {
      this.m_state = state;
      switch (this.m_state)
      {
        case ModularRangeMaster.RangeMasterState.WaitingForCommand:
          this.SequenceSelectorGO.SetActive(true);
          this.MainGameDisplayGO.SetActive(false);
          this.ScoreEntryGO.SetActive(false);
          break;
        case ModularRangeMaster.RangeMasterState.InSequence:
          this.SequenceSelectorGO.SetActive(false);
          this.MainGameDisplayGO.SetActive(true);
          this.ScoreEntryGO.SetActive(false);
          break;
        case ModularRangeMaster.RangeMasterState.HighScoreEntry:
          this.SequenceSelectorGO.SetActive(false);
          this.MainGameDisplayGO.SetActive(false);
          this.ScoreEntryGO.SetActive(true);
          break;
      }
    }

    public void SetSequenceCategory(string c)
    {
      for (int index = 0; index < this.SequenceCategoryImages.Length; ++index)
        this.SequenceCategoryImages[index].color = this.SequenceUnSelected;
      for (int index = 0; index < this.SequenceIndexImages.Length; ++index)
        this.SequenceIndexImages[index].gameObject.SetActive(false);
      switch (c)
      {
        case "Reflex":
          this.m_selectedSequenceCategory = ModularRangeSequenceDefinition.SequenceCategory.Reflex;
          this.SequenceCategoryImages[0].color = this.SequenceSelected;
          break;
        case "Recognition":
          this.m_selectedSequenceCategory = ModularRangeSequenceDefinition.SequenceCategory.Recognition;
          this.SequenceCategoryImages[1].color = this.SequenceSelected;
          break;
        case "Precision":
          this.m_selectedSequenceCategory = ModularRangeSequenceDefinition.SequenceCategory.Precision;
          this.SequenceCategoryImages[2].color = this.SequenceSelected;
          break;
        case "Cognition":
          this.m_selectedSequenceCategory = ModularRangeSequenceDefinition.SequenceCategory.Cognition;
          this.SequenceCategoryImages[3].color = this.SequenceSelected;
          break;
      }
      for (int index = 0; index < this.SequenceDic[this.m_selectedSequenceCategory].Sequences.Length; ++index)
        this.SequenceIndexImages[index].gameObject.SetActive(true);
      this.SetSequenceIndex("1");
    }

    public void SetSequenceIndex(string i)
    {
      this.aud.PlayOneShot(this.aud.clip, 0.3f);
      int index1 = int.Parse(i) - 1;
      this.m_selectedSequenceIndex = index1;
      for (int index2 = 0; index2 < this.SequenceIndexImages.Length; ++index2)
        this.SequenceIndexImages[index2].color = this.SequenceUnSelected;
      this.SequenceIndexImages[this.m_selectedSequenceIndex].color = this.SequenceSelected;
      this.m_currentSequenceDefinition = this.SequenceDic[this.m_selectedSequenceCategory].Sequences[this.m_selectedSequenceIndex];
      this.ScoreBoard.SetScoreBoardSequence(this.m_selectedSequenceCategory, index1);
      this.SequenceDetailsText.text = string.Empty + this.m_currentSequenceDefinition.MetaData.DisplayName + "\n" + "Category - " + this.m_currentSequenceDefinition.MetaData.Category.ToString() + "\n" + "Difficulty - " + this.m_currentSequenceDefinition.MetaData.Difficulty.ToString() + "\n" + "Capacity - " + this.m_currentSequenceDefinition.MetaData.Capacity + "\n" + "Waves - " + this.m_currentSequenceDefinition.MetaData.WaveCount.ToString() + "\n" + "Range - " + this.m_currentSequenceDefinition.MetaData.Range.ToString();
    }

    public void PlayAudioEvent(AudioClip clip, float volume) => this.AudioSource_Event.PlayOneShot(clip, volume);

    [Serializable]
    public class ModularRangeSequenceCategory
    {
      public ModularRangeSequenceDefinition.SequenceCategory CategoryType;
      public ModularRangeSequenceDefinition[] Sequences;
    }

    public enum RangeMasterState
    {
      WaitingForCommand,
      InSequence,
      HighScoreEntry,
    }
  }
}
