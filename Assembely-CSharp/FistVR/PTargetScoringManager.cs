using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class PTargetScoringManager : MonoBehaviour
	{
		public enum SetMode
		{
			Open,
			Timed
		}

		public enum TimerCountdown
		{
			Sec5,
			Sec10,
			Sec5To10
		}

		public enum TimerClockMode
		{
			Standard,
			MuzzleDown,
			EmptyHands
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
				Scores.Add(score);
				Times.Add(time);
				UV.Add(uv);
				NumShotsRegistered++;
				TotalScore += score;
				FinalTime = Mathf.Max(FinalTime, time);
				return true;
			}
		}

		public List<ScoreSet> Sets = new List<ScoreSet>();

		private int m_curSet = -1;

		private SetMode m_selectedMode;

		private TimerCountdown m_selectedCountDown;

		private TimerClockMode m_selectedClockMode;

		private SetMode m_mode;

		private TimerCountdown m_countDown;

		private TimerClockMode m_clockMode;

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

		private void Start()
		{
			CreateNewSetAndAdvancedToIt();
		}

		public void ProcessHit(int score, Vector2 uv)
		{
			if ((m_mode == SetMode.Open || m_tickDownToSet <= 0f) && Sets[m_curSet].RegisterScore(score, m_timer, uv))
			{
				UpdateScoreListDisplay();
				UpdateTargetSheet();
			}
		}

		private void ShotClock()
		{
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_ShotClock, base.transform.position);
		}

		private void Beep()
		{
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Beep, base.transform.position);
		}

		private void Boop()
		{
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Boop, base.transform.position);
		}

		private void Update()
		{
			if (m_mode != SetMode.Timed)
			{
				return;
			}
			if (m_tickDownToSet > 0f)
			{
				if (m_clockMode == TimerClockMode.Standard)
				{
					m_tickDownToSet -= Time.deltaTime;
				}
				else if (m_clockMode == TimerClockMode.MuzzleDown)
				{
					bool flag = true;
					for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
					{
						if (GM.CurrentMovementManager.Hands[i].CurrentInteractable != null && GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArm)
						{
							Vector3 forward = (GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArm).GetMuzzle().forward;
							if (Vector3.Angle(forward, -Vector3.up) > 45f)
							{
								flag = false;
							}
						}
					}
					if (flag)
					{
						m_tickDownToSet -= Time.deltaTime;
					}
				}
				else if (m_clockMode == TimerClockMode.EmptyHands && GM.CurrentMovementManager.Hands[0].CurrentInteractable == null && GM.CurrentMovementManager.Hands[1].CurrentInteractable == null)
				{
					m_tickDownToSet -= Time.deltaTime;
				}
				if (m_tickDownToSet <= 0f)
				{
					ShotClock();
				}
			}
			else
			{
				m_timer += Time.deltaTime;
			}
		}

		public void BTNPress_SetMode(int i)
		{
			m_selectedMode = (SetMode)i;
			Beep();
		}

		public void BTNPress_SetCountdown(int i)
		{
			m_selectedCountDown = (TimerCountdown)i;
			Beep();
		}

		public void BTNPress_SetClockMode(int i)
		{
			m_selectedClockMode = (TimerClockMode)i;
			Beep();
		}

		public void BTNPress_BeginNewSet()
		{
			CreateNewSetAndAdvancedToIt();
			Boop();
		}

		public void BTNPress_SetToTimings()
		{
			TargetTimingsDisplay.SetActive(value: true);
			TargetSheetDisplay.SetActive(value: false);
			Beep();
			UpdateScoreListDisplay();
			UpdateTargetSheet();
		}

		public void BTNPress_SetToTargetSheet()
		{
			TargetTimingsDisplay.SetActive(value: false);
			TargetSheetDisplay.SetActive(value: true);
			Beep();
			UpdateScoreListDisplay();
			UpdateTargetSheet();
		}

		public void BTNPress_SetToNextSet()
		{
			m_curSetDisplay++;
			if (m_curSetDisplay >= Sets.Count)
			{
				m_curSetDisplay = Sets.Count - 1;
			}
			Beep();
			m_curPageDisplay = 0;
			UpdateScoreListDisplay();
			UpdateTargetSheet();
		}

		public void BTNPress_SetToPrevSet()
		{
			m_curSetDisplay--;
			if (m_curSetDisplay < 0)
			{
				m_curSetDisplay = 0;
			}
			Beep();
			m_curPageDisplay = 0;
			UpdateScoreListDisplay();
			UpdateTargetSheet();
		}

		public void BTNPress_SetToNextShots()
		{
			int max = Mathf.FloorToInt(Sets[m_curSet].NumShotsRegistered / 15);
			m_curPageDisplay++;
			Beep();
			m_curPageDisplay = Mathf.Clamp(m_curPageDisplay, 0, max);
			UpdateScoreListDisplay();
			UpdateTargetSheet();
		}

		public void BTNPress_SetToPrevShots()
		{
			m_curPageDisplay--;
			Beep();
			if (m_curPageDisplay < 0)
			{
				m_curPageDisplay = 0;
			}
			UpdateScoreListDisplay();
			UpdateTargetSheet();
		}

		private void CreateNewSetAndAdvancedToIt()
		{
			ScoreSet item = new ScoreSet();
			Sets.Add(item);
			m_curSet = Sets.Count - 1;
			m_curSetDisplay = m_curSet;
			m_mode = m_selectedMode;
			m_countDown = m_selectedCountDown;
			m_clockMode = m_selectedClockMode;
			m_timer = 0f;
			if (m_mode == SetMode.Timed)
			{
				if (m_countDown == TimerCountdown.Sec5)
				{
					m_tickDownToSet = 5f;
				}
				if (m_countDown == TimerCountdown.Sec10)
				{
					m_tickDownToSet = 10f;
				}
				if (m_countDown == TimerCountdown.Sec5To10)
				{
					m_tickDownToSet = UnityEngine.Random.Range(5f, 10f);
				}
			}
			else
			{
				m_tickDownToSet = -0.01f;
			}
			UpdateScoreListDisplay();
			UpdateTargetSheet();
		}

		private void UpdateScoreListDisplay()
		{
			SetNumeral.text = m_curSetDisplay.ToString();
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			ScoreSet scoreSet = Sets[m_curSetDisplay];
			int num = m_curPageDisplay * 15;
			int a = m_curPageDisplay * 15 + 15;
			a = Mathf.Min(a, scoreSet.NumShotsRegistered);
			for (int i = num; i < a; i++)
			{
				text = text + (i + 1) + ":\n";
				text2 = text2 + scoreSet.Scores[i] + "\n";
				if (m_mode == SetMode.Timed)
				{
					text3 = text3 + FloatToTime(scoreSet.Times[i], "#0:00.000") + "\n";
					float toConvert = scoreSet.Times[i];
					if (i > 0)
					{
						toConvert = scoreSet.Times[i] - scoreSet.Times[i - 1];
					}
					text4 = text4 + FloatToTime(toConvert, "#0:00.000") + "\n";
				}
			}
			SetShotNumbers.text = text;
			SetScores.text = text2;
			SetTimes.text = text3;
			SetSplits.text = text4;
			SetTotalShots.text = "Set Total (" + scoreSet.NumShotsRegistered + "):";
			SetTotalScore.text = scoreSet.TotalScore + " Pts";
			if (scoreSet.NumShotsRegistered == 0)
			{
				SetTotalAVG.text = string.Empty;
			}
			else
			{
				SetTotalAVG.text = Math.Round((double)scoreSet.TotalScore / (double)scoreSet.NumShotsRegistered, 2) + " Avg";
			}
		}

		private void UpdateTargetSheet()
		{
			ScoreSet scoreSet = Sets[m_curSetDisplay];
			if (scoreSet.NumShotsRegistered == 0)
			{
				LastHitIndicator.gameObject.SetActive(value: false);
			}
			int num = 0;
			for (int i = m_curPageDisplay * 15; i < scoreSet.NumShotsRegistered; i++)
			{
				if (num >= HitIndicators.Length)
				{
					break;
				}
				Vector3 position = Vector3.Lerp(UL.position, UR.position, scoreSet.UV[i].x);
				position.y = Mathf.Lerp(LL.position.y, UL.position.y, scoreSet.UV[i].y);
				HitIndicators[num].gameObject.SetActive(value: true);
				HitIndicators[num].position = position;
				if (i == scoreSet.NumShotsRegistered - 1)
				{
					LastHitIndicator.gameObject.SetActive(value: true);
					LastHitIndicator.position = position;
				}
				num++;
			}
			for (int j = num; j < 15; j++)
			{
				if (j < HitIndicators.Length)
				{
					HitIndicators[j].gameObject.SetActive(value: false);
				}
			}
		}

		public string FloatToTime(float toConvert, string format)
		{
			return format switch
			{
				"00.0" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"#0.0" => $"{Mathf.Floor(toConvert) % 60f:#0}:{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"00.00" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"00.000" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#00.000" => $"{Mathf.Floor(toConvert) % 60f:#00}:{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#0:00" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}", 
				"#00:00" => $"{Mathf.Floor(toConvert / 60f):#00}:{Mathf.Floor(toConvert) % 60f:00}", 
				"0:00.0" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"#0:00.0" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"0:00.00" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"#0:00.00" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"0:00.000" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#0:00.000" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				_ => "error", 
			};
		}
	}
}
