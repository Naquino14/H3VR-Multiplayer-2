using System.Collections.Generic;
using RUST.Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class HG_UIManager : MonoBehaviour
	{
		[Header("Main Screen")]
		public Text LBL_SelectedMode;

		public Text LBL_ModeRules;

		public Text[] UIH_Scores_Global;

		public Text[] UIH_Scores_Local;

		public Text[] ScoringReadOuts;

		public void SetSelectedModeText(HG_ModeProfile p)
		{
			LBL_SelectedMode.text = "Selected Game: " + p.Title;
			LBL_ModeRules.text = p.DescriptionText;
		}

		public void SetScoringReadouts(List<string> readout)
		{
			if (readout == null)
			{
				return;
			}
			for (int i = 0; i < ScoringReadOuts.Length; i++)
			{
				if (i < readout.Count)
				{
					ScoringReadOuts[i].gameObject.SetActive(value: true);
					ScoringReadOuts[i].text = readout[i];
				}
				else
				{
					ScoringReadOuts[i].gameObject.SetActive(value: false);
				}
			}
		}

		public void ClearGlobalHighScoreDisplay()
		{
			for (int i = 0; i < 6; i++)
			{
				UIH_Scores_Global[i].gameObject.SetActive(value: false);
			}
		}

		public void SetGlobalHighScoreDisplay(List<HighScoreManager.HighScore> scores)
		{
			for (int i = 0; i < 6; i++)
			{
				if (scores.Count > i)
				{
					UIH_Scores_Global[i].gameObject.SetActive(value: true);
					UIH_Scores_Global[i].text = scores[i].rank.ToString() + ". " + scores[i].score + " - " + scores[i].name;
				}
				else
				{
					UIH_Scores_Global[i].gameObject.SetActive(value: false);
				}
			}
		}

		public void RedrawHighScoreDisplay(string SequenceID)
		{
			List<OmniScore> scoreList = GM.Omni.OmniFlags.GetScoreList(SequenceID);
			for (int i = 0; i < scoreList.Count; i++)
			{
				UIH_Scores_Local[i].gameObject.SetActive(value: true);
				UIH_Scores_Local[i].text = (i + 1).ToString() + ": " + scoreList[i].Score;
			}
			for (int j = scoreList.Count; j < UIH_Scores_Local.Length; j++)
			{
				UIH_Scores_Local[j].gameObject.SetActive(value: false);
			}
		}
	}
}
