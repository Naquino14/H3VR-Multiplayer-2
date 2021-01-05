using System.Collections.Generic;
using Assets.Rust.Lodestone;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class ModularRangeJankyHighScoreBoard : MonoBehaviour, IRequester
	{
		public Text[] NameLists;

		public Text[] ScoreLists;

		private float m_update_tick = 1f;

		private int m_list_updating_index;

		public void Update()
		{
			if (m_update_tick > 0f)
			{
				m_update_tick -= Time.deltaTime;
			}
			else
			{
				RefreshScoreBoard();
			}
		}

		private void RefreshScoreBoard()
		{
			string endpointName = "ModularRange_Sequence" + m_list_updating_index;
			if (Lodestone.GetLogs(this, endpointName, "PlayerScore", 10, asc: false) >= 0f)
			{
				m_list_updating_index++;
				if (m_list_updating_index >= NameLists.Length)
				{
					m_list_updating_index = 0;
				}
				m_update_tick = 1f;
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
			int num = 0;
			switch (endPointName)
			{
			case "ModularRange_Sequence0":
				num = 0;
				break;
			case "ModularRange_Sequence1":
				num = 1;
				break;
			case "ModularRange_Sequence2":
				num = 2;
				break;
			case "ModularRange_Sequence3":
				num = 3;
				break;
			}
			NameLists[num].text = text;
			ScoreLists[num].text = text2;
		}
	}
}
