using System.Collections.Generic;
using Assets.Rust.Lodestone;
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

		public void Awake()
		{
			aud = GetComponent<AudioSource>();
		}

		public void KeyEntry(string s)
		{
			aud.PlayOneShot(aud.clip);
			if (s == "Del")
			{
				if (ScoreNameEntry.Length > 0)
				{
					ScoreNameEntry = ScoreNameEntry.Substring(0, ScoreNameEntry.Length - 1);
				}
			}
			else if (s == "Enter")
			{
				if (ScoreNameEntry.Length > 0)
				{
					LogScore();
					Master.GoToSequenceSelector();
				}
			}
			else if (ScoreNameEntry.Length < MaxCharacters)
			{
				ScoreNameEntry += s;
			}
			NamePanel.text = ScoreNameEntry;
		}

		public void SetScoreDisplay(int i)
		{
			score = i;
			ScorePanel.text = score.ToString();
		}

		private void LogScore()
		{
			int selectedSequenceIndex = Master.SelectedSequenceIndex;
			string scoreNameEntry = ScoreNameEntry;
			int num = score;
			SortedList<string, object> sortedList = new SortedList<string, object>();
			string endPointName = Master.m_currentSequenceDefinition.MetaData.EndPointName;
			sortedList.Add("PlayerName", scoreNameEntry);
			sortedList.Add("PlayerScore", num);
			Lodestone.Log(endPointName, sortedList);
		}
	}
}
