using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigNPCInterface : MonoBehaviour
	{
		public enum NPCConvoState
		{
			NotInConvo,
			Speaking,
			WaitingForInput
		}

		public Sosig S;

		public NPCConvoState State;

		private bool m_isAbleToSpeak = true;

		private bool m_isInSpeakingRange;

		public ZosigNPCProfile Profile;

		public AudioSource AudSource_Voice;

		[Header("Talk Icons")]
		public ZosigNPCSpeechInput Icon_Howdy;

		public List<ZosigNPCSpeechInput> SpeechInputs;

		public ZosigGameManager M;

		private ZosigNPCProfile.NPCLine m_lineSpeaking;

		private float m_startSize;

		private float pulse;

		private float m_distCheckTick = 1f;

		private float HeadJitterTick = 0.1f;

		private float m_timeTilDoneSpeaking;

		public void SetAbleToSpeak(bool b)
		{
			m_isAbleToSpeak = b;
		}

		private void Start()
		{
			M = GM.ZMaster;
			Icon_Howdy.SetLine(Profile.Lines[0]);
			for (int i = 0; i < SpeechInputs.Count; i++)
			{
				SpeechInputs[i].gameObject.SetActive(value: false);
			}
			m_startSize = Icon_Howdy.transform.localScale.x;
		}

		private void SetState(NPCConvoState s)
		{
			if (s == NPCConvoState.NotInConvo && State != 0)
			{
				M.SetMusic_Gameplay();
			}
			else if (s != 0 && State == NPCConvoState.NotInConvo)
			{
				M.SetMusic_Speaking();
			}
			State = s;
		}

		private void Update()
		{
			UpdateState();
		}

		private void UpdateState()
		{
			pulse += Time.deltaTime;
			pulse = Mathf.Repeat(pulse, 1f);
			if (m_isInSpeakingRange && m_isAbleToSpeak)
			{
				Vector3 vector = GM.CurrentPlayerBody.Head.transform.position - base.transform.position;
				vector.y = 0f;
				base.transform.rotation = Quaternion.LookRotation(vector, Vector3.up);
				S.SetDominantGuardDirection(vector);
			}
			if (S.BodyState != 0 || S.IsStunned || S.CurrentOrder != Sosig.SosigOrder.GuardPoint)
			{
				TerminateSpeech();
				SetHowdyVisibility(b: false);
				SetState(NPCConvoState.NotInConvo);
				SetAbleToSpeak(b: false);
				m_isInSpeakingRange = false;
				m_distCheckTick = 5f;
				m_timeTilDoneSpeaking = 0f;
			}
			else
			{
				SetAbleToSpeak(b: true);
			}
			FollowHead();
			switch (State)
			{
			case NPCConvoState.NotInConvo:
				UpdateState_NotInConvo();
				break;
			case NPCConvoState.Speaking:
				UpdateState_Speaking();
				break;
			case NPCConvoState.WaitingForInput:
				UpdateState_WaitingForInput();
				break;
			}
		}

		private void FollowHead()
		{
			if (S.BodyState == Sosig.SosigBodyState.InControl && S.Links[0] != null)
			{
				base.transform.position = S.Links[0].transform.position;
			}
		}

		private void UpdateState_NotInConvo()
		{
			if (m_distCheckTick > 0f)
			{
				m_distCheckTick -= Time.deltaTime;
			}
			else
			{
				m_distCheckTick = 1f;
				CheckIsWithinSpeakingRange();
			}
			if (m_isInSpeakingRange && m_isAbleToSpeak)
			{
				SetHowdyVisibility(b: true);
				float num = Mathf.Abs(Mathf.Sin(pulse * (float)Math.PI * 2f) * 0.1f) + m_startSize;
				Icon_Howdy.transform.localScale = new Vector3(num, num, num);
			}
			else
			{
				SetHowdyVisibility(b: false);
			}
		}

		private void UpdateState_Speaking()
		{
			if (!m_isInSpeakingRange || !m_isAbleToSpeak)
			{
				TerminateSpeech();
				return;
			}
			CheckIsWithinSpeakingRange();
			if (HeadJitterTick > 0f)
			{
				HeadJitterTick -= Time.deltaTime;
			}
			else
			{
				HeadJitterTick = UnityEngine.Random.Range(Profile.SpeakJitterRange.x, Profile.SpeakJitterRange.y);
				if (S != null && S.Links[0] != null)
				{
					S.Links[0].R.AddForceAtPosition(UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(Profile.SpeakPowerRange.x, Profile.SpeakPowerRange.y), S.Links[0].transform.position + Vector3.up * 0.3f, ForceMode.Impulse);
				}
			}
			m_timeTilDoneSpeaking -= Time.deltaTime;
			if (m_timeTilDoneSpeaking <= 0f)
			{
				M.FlagM.SetFlagMaxBlend(m_lineSpeaking.FlagOnLineSpoken, m_lineSpeaking.FlagValueOnLineSpoken);
				PopulateChoices();
			}
		}

		private void UpdateState_WaitingForInput()
		{
			if (!m_isInSpeakingRange || !m_isAbleToSpeak)
			{
				TerminateSpeech();
				return;
			}
			if (m_distCheckTick > 0f)
			{
				m_distCheckTick -= Time.deltaTime;
				return;
			}
			m_distCheckTick = 0.5f;
			PopulateChoices();
			CheckIsWithinSpeakingRange();
		}

		private void SetHowdyVisibility(bool b)
		{
			if (Icon_Howdy.gameObject.activeSelf != b)
			{
				Icon_Howdy.gameObject.SetActive(b);
			}
		}

		private void PopulateChoices()
		{
			SetState(NPCConvoState.WaitingForInput);
			int num = 0;
			for (int i = 1; i < Profile.Lines.Count; i++)
			{
				ZosigNPCProfile.NPCLine nPCLine = Profile.Lines[i];
				if (nPCLine.Type != 0 && M.FlagM.GetFlagValue(nPCLine.FlagRequiredToPlay) == nPCLine.FlagValueRequiredToPlay)
				{
					SpeechInputs[num].gameObject.SetActive(value: true);
					SpeechInputs[num].SetLine(nPCLine);
					num++;
					if (num > 5)
					{
						break;
					}
				}
			}
			for (int j = num + 1; j < SpeechInputs.Count; j++)
			{
				SpeechInputs[j].Line = null;
				SpeechInputs[j].gameObject.SetActive(value: false);
			}
		}

		private void HideAndClearChoices()
		{
			for (int i = 0; i < SpeechInputs.Count; i++)
			{
				SpeechInputs[i].ClearLine();
				SpeechInputs[i].gameObject.SetActive(value: false);
			}
		}

		public void SpeakLine(ZosigNPCProfile.NPCLine line)
		{
			S.KillSpeech();
			S.IsAllowedToSpeak = false;
			SetHowdyVisibility(b: false);
			HideAndClearChoices();
			SetState(NPCConvoState.Speaking);
			m_lineSpeaking = line;
			AudioClip audioClip = line.Clips[UnityEngine.Random.Range(0, line.Clips.Count)];
			m_timeTilDoneSpeaking = audioClip.length + 0.25f;
			AudSource_Voice.clip = audioClip;
			AudSource_Voice.Play();
		}

		private void CheckIsWithinSpeakingRange()
		{
			float num = Vector3.Distance(GM.CurrentPlayerBody.transform.position, base.transform.position);
			if (num < 4f)
			{
				m_isInSpeakingRange = true;
			}
			else
			{
				m_isInSpeakingRange = false;
			}
		}

		public void TerminateSpeech()
		{
			if (State != 0)
			{
				m_lineSpeaking = null;
				HideAndClearChoices();
				SetState(NPCConvoState.NotInConvo);
				AudSource_Voice.Stop();
				S.IsAllowedToSpeak = true;
			}
		}
	}
}
