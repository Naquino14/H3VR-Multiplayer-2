using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class TargetRangePanel : MonoBehaviour
	{
		public struct ScoreSet
		{
			public float MetersDistance;

			public int Shots;

			public int Score;

			public ScoreSet(float m, int sh, int sc)
			{
				MetersDistance = m;
				Shots = sh;
				Score = sc;
			}
		}

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

		private ScoreSet CurrentScoreSet = new ScoreSet(0f, 0, 0);

		public List<ScoreSet> PastScoreSets = new List<ScoreSet>();

		private void Start()
		{
			OBS_UnitType.SetSelectedButton(0);
			DistanceLabel.text = "0 Meters";
			m_maxMeters = Mathf.FloorToInt(MaxMeters);
			m_maxYards = Mathf.FloorToInt(MaxMeters * 1.09361f);
			m_maxFeet = Mathf.FloorToInt(MaxMeters * 3.28084f);
		}

		public float GetDesiredDistance()
		{
			return m_storedDistanceInMeters;
		}

		public void InputNumeral(string s)
		{
			m_stringInput += s;
			UpdateCalc();
			UpdateDisplay();
			UpdateStoredDesiredDistance();
		}

		public void Clear(string s)
		{
			m_stringInput = string.Empty;
			UpdateDisplay();
			UpdateStoredDesiredDistance();
		}

		public void SetUnits(int i)
		{
			m_unitType = i;
			UpdateDisplay();
			UpdateStoredDesiredDistance();
			UpdatePaperSheet();
		}

		private void UpdateCalc()
		{
			int value = int.Parse(m_stringInput);
			switch (m_unitType)
			{
			case 0:
				value = Mathf.Clamp(value, 0, m_maxMeters);
				break;
			case 1:
				value = Mathf.Clamp(value, 0, m_maxYards);
				break;
			case 2:
				value = Mathf.Clamp(value, 0, m_maxFeet);
				break;
			}
			m_stringInput = value.ToString();
		}

		private void UpdateDisplay()
		{
			if (m_stringInput == string.Empty)
			{
				m_storedDistanceInMeters = 0f;
				DistanceLabel.text = 0 + " " + GetUnitDisplayName();
			}
			else
			{
				DistanceLabel.text = m_stringInput + " " + GetUnitDisplayName();
			}
		}

		private void UpdateStoredDesiredDistance()
		{
			if (m_stringInput == string.Empty)
			{
				m_storedDistanceInMeters = 0f;
				return;
			}
			switch (m_unitType)
			{
			case 0:
				m_storedDistanceInMeters = int.Parse(m_stringInput);
				break;
			case 1:
				m_storedDistanceInMeters = (float)int.Parse(m_stringInput) * 0.9144f;
				break;
			case 2:
				m_storedDistanceInMeters = (float)int.Parse(m_stringInput) * 0.3048f;
				break;
			}
		}

		private string GetUnitDisplayName()
		{
			return m_unitType switch
			{
				0 => "Meters", 
				1 => "Yards", 
				2 => "Feet", 
				_ => "Meters", 
			};
		}

		public void AdvanceToNextSet()
		{
			if (CurrentScoreSet.Shots > 0)
			{
				ScoreSet scoreSet = new ScoreSet(0f, 0, 0);
				scoreSet = CurrentScoreSet;
				PastScoreSets.Add(scoreSet);
				if (PastScoreSets.Count > 5)
				{
					PastScoreSets.RemoveAt(0);
				}
				CurrentScoreSet.Score = 0;
				CurrentScoreSet.MetersDistance = 0f;
				CurrentScoreSet.Shots = 0;
				PaperTarget.ClearHoles();
				UpdatePaperSheet();
			}
		}

		public void UpdatePaperSheet()
		{
			string text = "Score List\n(Distance Shown is Closest In Set)\n\nTap Sheet For New Set\n\n";
			Vector3 currentScoring = PaperTarget.GetCurrentScoring();
			CurrentScoreSet.Shots = Mathf.FloorToInt(currentScoring.x);
			CurrentScoreSet.Score = Mathf.FloorToInt(currentScoring.y);
			CurrentScoreSet.MetersDistance = currentScoring.z;
			string text2 = text;
			text = text2 + "Current Distance: " + GetValueInCurrentUnits(CurrentScoreSet.MetersDistance) + " " + GetUnitDisplayName() + "\n";
			text2 = text;
			text = text2 + "- Shots: " + CurrentScoreSet.Shots.ToString() + "   - Points: " + CurrentScoreSet.Score + "\n\n";
			if (PastScoreSets.Count > 0)
			{
				for (int num = PastScoreSets.Count - 1; num >= 0; num--)
				{
					text2 = text;
					text = text2 + "Previous Set Distance: " + GetValueInCurrentUnits(PastScoreSets[num].MetersDistance) + " " + GetUnitDisplayName() + "\n";
					text2 = text;
					text = text2 + "- Shots: " + PastScoreSets[num].Shots.ToString() + "   - Points: " + PastScoreSets[num].Score + "\n\n";
				}
			}
			ScoreSheet.text = text;
		}

		private int GetValueInCurrentUnits(float meters)
		{
			return m_unitType switch
			{
				0 => Mathf.FloorToInt(meters), 
				1 => Mathf.FloorToInt(meters * 1.09361f), 
				2 => Mathf.FloorToInt(meters * 3.28084f), 
				_ => Mathf.FloorToInt(meters), 
			};
		}
	}
}
