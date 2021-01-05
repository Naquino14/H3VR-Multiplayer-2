using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class ModularRangeMaster : MonoBehaviour
	{
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
			HighScoreEntry
		}

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

		public ModularRangeSequenceCategory[] SequenceCategories;

		public Dictionary<ModularRangeSequenceDefinition.SequenceCategory, ModularRangeSequenceCategory> SequenceDic = new Dictionary<ModularRangeSequenceDefinition.SequenceCategory, ModularRangeSequenceCategory>();

		public Image[] SequenceCategoryImages;

		public Image[] SequenceIndexImages;

		public Text SequenceDetailsText;

		public Color SequenceSelected;

		public Color SequenceUnSelected;

		private RangeMasterState m_state;

		private AudioSource aud;

		public ModularRangeSequenceDefinition.SequenceCategory SelectedSequenceCategory => m_selectedSequenceCategory;

		public int SelectedSequenceIndex => m_selectedSequenceIndex;

		public void Awake()
		{
			aud = GetComponent<AudioSource>();
			GoToSequenceSelector();
			for (int i = 0; i < SequenceCategories.Length; i++)
			{
				if (!SequenceDic.ContainsKey(SequenceCategories[i].CategoryType))
				{
					SequenceDic.Add(SequenceCategories[i].CategoryType, SequenceCategories[i]);
				}
			}
			for (int j = 0; j < SequenceCategoryImages.Length; j++)
			{
				SequenceCategoryImages[j].gameObject.SetActive(value: false);
			}
			if (SequenceDic[ModularRangeSequenceDefinition.SequenceCategory.Reflex].Sequences.Length > 0)
			{
				SequenceCategoryImages[0].gameObject.SetActive(value: true);
			}
			if (SequenceDic[ModularRangeSequenceDefinition.SequenceCategory.Recognition].Sequences.Length > 0)
			{
				SequenceCategoryImages[1].gameObject.SetActive(value: true);
			}
			if (SequenceDic[ModularRangeSequenceDefinition.SequenceCategory.Precision].Sequences.Length > 0)
			{
				SequenceCategoryImages[2].gameObject.SetActive(value: true);
			}
			if (SequenceDic[ModularRangeSequenceDefinition.SequenceCategory.Cognition].Sequences.Length > 0)
			{
				SequenceCategoryImages[3].gameObject.SetActive(value: true);
			}
			SetSequenceCategory("Reflex");
		}

		private void BeginSequence(string s)
		{
			m_currentSequenceDefinition = SequenceDic[m_selectedSequenceCategory].Sequences[m_selectedSequenceIndex];
			if (m_state == RangeMasterState.WaitingForCommand)
			{
				m_currentSequencer.BeginSequence(this, m_currentSequenceDefinition);
				SetState(RangeMasterState.InSequence);
			}
		}

		public void GoToHighScoreBoard()
		{
			ScoreEntry.SetScoreDisplay(m_currentSequencer.GetScore());
			SetState(RangeMasterState.HighScoreEntry);
		}

		public void GoToSequenceSelector()
		{
			SetState(RangeMasterState.WaitingForCommand);
		}

		public void SetState(RangeMasterState state)
		{
			m_state = state;
			switch (m_state)
			{
			case RangeMasterState.WaitingForCommand:
				SequenceSelectorGO.SetActive(value: true);
				MainGameDisplayGO.SetActive(value: false);
				ScoreEntryGO.SetActive(value: false);
				break;
			case RangeMasterState.InSequence:
				SequenceSelectorGO.SetActive(value: false);
				MainGameDisplayGO.SetActive(value: true);
				ScoreEntryGO.SetActive(value: false);
				break;
			case RangeMasterState.HighScoreEntry:
				SequenceSelectorGO.SetActive(value: false);
				MainGameDisplayGO.SetActive(value: false);
				ScoreEntryGO.SetActive(value: true);
				break;
			}
		}

		public void SetSequenceCategory(string c)
		{
			for (int i = 0; i < SequenceCategoryImages.Length; i++)
			{
				SequenceCategoryImages[i].color = SequenceUnSelected;
			}
			for (int j = 0; j < SequenceIndexImages.Length; j++)
			{
				SequenceIndexImages[j].gameObject.SetActive(value: false);
			}
			switch (c)
			{
			case "Reflex":
				m_selectedSequenceCategory = ModularRangeSequenceDefinition.SequenceCategory.Reflex;
				SequenceCategoryImages[0].color = SequenceSelected;
				break;
			case "Recognition":
				m_selectedSequenceCategory = ModularRangeSequenceDefinition.SequenceCategory.Recognition;
				SequenceCategoryImages[1].color = SequenceSelected;
				break;
			case "Precision":
				m_selectedSequenceCategory = ModularRangeSequenceDefinition.SequenceCategory.Precision;
				SequenceCategoryImages[2].color = SequenceSelected;
				break;
			case "Cognition":
				m_selectedSequenceCategory = ModularRangeSequenceDefinition.SequenceCategory.Cognition;
				SequenceCategoryImages[3].color = SequenceSelected;
				break;
			}
			for (int k = 0; k < SequenceDic[m_selectedSequenceCategory].Sequences.Length; k++)
			{
				SequenceIndexImages[k].gameObject.SetActive(value: true);
			}
			SetSequenceIndex("1");
		}

		public void SetSequenceIndex(string i)
		{
			aud.PlayOneShot(aud.clip, 0.3f);
			int index = (m_selectedSequenceIndex = int.Parse(i) - 1);
			for (int j = 0; j < SequenceIndexImages.Length; j++)
			{
				SequenceIndexImages[j].color = SequenceUnSelected;
			}
			SequenceIndexImages[m_selectedSequenceIndex].color = SequenceSelected;
			m_currentSequenceDefinition = SequenceDic[m_selectedSequenceCategory].Sequences[m_selectedSequenceIndex];
			ScoreBoard.SetScoreBoardSequence(m_selectedSequenceCategory, index);
			string empty = string.Empty;
			empty = empty + m_currentSequenceDefinition.MetaData.DisplayName + "\n";
			empty = empty + "Category - " + m_currentSequenceDefinition.MetaData.Category.ToString() + "\n";
			empty = empty + "Difficulty - " + m_currentSequenceDefinition.MetaData.Difficulty.ToString() + "\n";
			empty = empty + "Capacity - " + m_currentSequenceDefinition.MetaData.Capacity + "\n";
			empty = empty + "Waves - " + m_currentSequenceDefinition.MetaData.WaveCount + "\n";
			empty = empty + "Range - " + m_currentSequenceDefinition.MetaData.Range;
			SequenceDetailsText.text = empty;
		}

		public void PlayAudioEvent(AudioClip clip, float volume)
		{
			AudioSource_Event.PlayOneShot(clip, volume);
		}
	}
}
