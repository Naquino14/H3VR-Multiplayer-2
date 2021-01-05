using System.Collections.Generic;
using Assets.Rust.Lodestone;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class ModularRangeHighScoreBoard : MonoBehaviour, IRequester
	{
		public Text HighScoreBoardTitle;

		public Text[] NameLists;

		public Text[] ScoreLists;

		private float m_update_tick = 0.1f;

		private int m_list_updating_index;

		public ModularRangeMaster Master;

		private bool IsUpdatingScore;

		private ModularRangeSequenceDefinition.SequenceCategory Category;

		private int SequenceIndex;

		public void Update()
		{
			if (m_update_tick > 0f)
			{
				m_update_tick -= Time.deltaTime;
			}
			else if (!IsUpdatingScore)
			{
				RefreshScoreBoard();
			}
		}

		private void ClearScoreBoard()
		{
			string text = "Loading...";
			string empty = string.Empty;
			for (int i = 0; i < NameLists.Length; i++)
			{
				NameLists[i].text = text;
				ScoreLists[i].text = empty;
			}
		}

		public void SetScoreBoardSequence(ModularRangeSequenceDefinition.SequenceCategory cat, int index)
		{
			Category = cat;
			SequenceIndex = index;
			ClearScoreBoard();
		}

		private void RefreshScoreBoard()
		{
			string text = Master.SequenceDic[Category].Sequences[SequenceIndex].MetaData.Category.ToString();
			text += " - ";
			text += Master.SequenceDic[Category].Sequences[SequenceIndex].MetaData.DisplayName;
			HighScoreBoardTitle.text = text;
			string endPointName = Master.SequenceDic[Category].Sequences[SequenceIndex].MetaData.EndPointName;
			float num = -1f;
			switch (m_list_updating_index)
			{
			case 0:
				num = Lodestone.GetLogs(this, endPointName, "PlayerScore", 10, asc: false);
				break;
			case 1:
				num = Lodestone.GetLogs(this, endPointName, "PlayerScore", 10, asc: false, overridePreviousRequest: false, new SortedList<string, string>
				{
					{
						"PlayerName",
						"unique"
					}
				});
				break;
			case 2:
				num = Lodestone.GetLogs(this, endPointName, "PlayerScore", 10, asc: false, overridePreviousRequest: false, null, currentPlayerOnly: true);
				break;
			}
			if (num >= 0f)
			{
				IsUpdatingScore = true;
			}
		}

		public void HandleResponse(string endPointName, float startTime, List<KeyValuePair<string, string>> fieldTypes, List<Dictionary<string, object>> fieldValues)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			for (int i = 0; i < fieldValues.Count; i++)
			{
				string text3 = text;
				text = text3 + (i + 1).ToString("##") + ". " + (string)fieldValues[i]["PlayerName"] + "\n";
				text2 = text2 + (int)fieldValues[i]["PlayerScore"] + "\n";
			}
			NameLists[m_list_updating_index].text = text;
			ScoreLists[m_list_updating_index].text = text2;
			IsUpdatingScore = false;
			m_update_tick = 0.1f;
			m_list_updating_index++;
			if (m_list_updating_index >= NameLists.Length)
			{
				m_list_updating_index = 0;
			}
		}
	}
}
