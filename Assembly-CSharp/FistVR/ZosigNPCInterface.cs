// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigNPCInterface
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigNPCInterface : MonoBehaviour
  {
    public Sosig S;
    public ZosigNPCInterface.NPCConvoState State;
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

    public void SetAbleToSpeak(bool b) => this.m_isAbleToSpeak = b;

    private void Start()
    {
      this.M = GM.ZMaster;
      this.Icon_Howdy.SetLine(this.Profile.Lines[0]);
      for (int index = 0; index < this.SpeechInputs.Count; ++index)
        this.SpeechInputs[index].gameObject.SetActive(false);
      this.m_startSize = this.Icon_Howdy.transform.localScale.x;
    }

    private void SetState(ZosigNPCInterface.NPCConvoState s)
    {
      if (s == ZosigNPCInterface.NPCConvoState.NotInConvo && this.State != ZosigNPCInterface.NPCConvoState.NotInConvo)
        this.M.SetMusic_Gameplay();
      else if (s != ZosigNPCInterface.NPCConvoState.NotInConvo && this.State == ZosigNPCInterface.NPCConvoState.NotInConvo)
        this.M.SetMusic_Speaking();
      this.State = s;
    }

    private void Update() => this.UpdateState();

    private void UpdateState()
    {
      this.pulse += Time.deltaTime;
      this.pulse = Mathf.Repeat(this.pulse, 1f);
      if (this.m_isInSpeakingRange && this.m_isAbleToSpeak)
      {
        Vector3 vector3 = GM.CurrentPlayerBody.Head.transform.position - this.transform.position;
        vector3.y = 0.0f;
        this.transform.rotation = Quaternion.LookRotation(vector3, Vector3.up);
        this.S.SetDominantGuardDirection(vector3);
      }
      if (this.S.BodyState != Sosig.SosigBodyState.InControl || this.S.IsStunned || this.S.CurrentOrder != Sosig.SosigOrder.GuardPoint)
      {
        this.TerminateSpeech();
        this.SetHowdyVisibility(false);
        this.SetState(ZosigNPCInterface.NPCConvoState.NotInConvo);
        this.SetAbleToSpeak(false);
        this.m_isInSpeakingRange = false;
        this.m_distCheckTick = 5f;
        this.m_timeTilDoneSpeaking = 0.0f;
      }
      else
        this.SetAbleToSpeak(true);
      this.FollowHead();
      switch (this.State)
      {
        case ZosigNPCInterface.NPCConvoState.NotInConvo:
          this.UpdateState_NotInConvo();
          break;
        case ZosigNPCInterface.NPCConvoState.Speaking:
          this.UpdateState_Speaking();
          break;
        case ZosigNPCInterface.NPCConvoState.WaitingForInput:
          this.UpdateState_WaitingForInput();
          break;
      }
    }

    private void FollowHead()
    {
      if (this.S.BodyState != Sosig.SosigBodyState.InControl || !((Object) this.S.Links[0] != (Object) null))
        return;
      this.transform.position = this.S.Links[0].transform.position;
    }

    private void UpdateState_NotInConvo()
    {
      if ((double) this.m_distCheckTick > 0.0)
      {
        this.m_distCheckTick -= Time.deltaTime;
      }
      else
      {
        this.m_distCheckTick = 1f;
        this.CheckIsWithinSpeakingRange();
      }
      if (this.m_isInSpeakingRange && this.m_isAbleToSpeak)
      {
        this.SetHowdyVisibility(true);
        float num = Mathf.Abs(Mathf.Sin((float) ((double) this.pulse * 3.14159274101257 * 2.0)) * 0.1f) + this.m_startSize;
        this.Icon_Howdy.transform.localScale = new Vector3(num, num, num);
      }
      else
        this.SetHowdyVisibility(false);
    }

    private void UpdateState_Speaking()
    {
      if (!this.m_isInSpeakingRange || !this.m_isAbleToSpeak)
      {
        this.TerminateSpeech();
      }
      else
      {
        this.CheckIsWithinSpeakingRange();
        if ((double) this.HeadJitterTick > 0.0)
        {
          this.HeadJitterTick -= Time.deltaTime;
        }
        else
        {
          this.HeadJitterTick = Random.Range(this.Profile.SpeakJitterRange.x, this.Profile.SpeakJitterRange.y);
          if ((Object) this.S != (Object) null && (Object) this.S.Links[0] != (Object) null)
            this.S.Links[0].R.AddForceAtPosition(Random.onUnitSphere * Random.Range(this.Profile.SpeakPowerRange.x, this.Profile.SpeakPowerRange.y), this.S.Links[0].transform.position + Vector3.up * 0.3f, ForceMode.Impulse);
        }
        this.m_timeTilDoneSpeaking -= Time.deltaTime;
        if ((double) this.m_timeTilDoneSpeaking > 0.0)
          return;
        this.M.FlagM.SetFlagMaxBlend(this.m_lineSpeaking.FlagOnLineSpoken, this.m_lineSpeaking.FlagValueOnLineSpoken);
        this.PopulateChoices();
      }
    }

    private void UpdateState_WaitingForInput()
    {
      if (!this.m_isInSpeakingRange || !this.m_isAbleToSpeak)
        this.TerminateSpeech();
      else if ((double) this.m_distCheckTick > 0.0)
      {
        this.m_distCheckTick -= Time.deltaTime;
      }
      else
      {
        this.m_distCheckTick = 0.5f;
        this.PopulateChoices();
        this.CheckIsWithinSpeakingRange();
      }
    }

    private void SetHowdyVisibility(bool b)
    {
      if (this.Icon_Howdy.gameObject.activeSelf == b)
        return;
      this.Icon_Howdy.gameObject.SetActive(b);
    }

    private void PopulateChoices()
    {
      this.SetState(ZosigNPCInterface.NPCConvoState.WaitingForInput);
      int index1 = 0;
      for (int index2 = 1; index2 < this.Profile.Lines.Count; ++index2)
      {
        ZosigNPCProfile.NPCLine line = this.Profile.Lines[index2];
        if (line.Type != ZosigNPCProfile.NPCLineType.Howdy && this.M.FlagM.GetFlagValue(line.FlagRequiredToPlay) == line.FlagValueRequiredToPlay)
        {
          this.SpeechInputs[index1].gameObject.SetActive(true);
          this.SpeechInputs[index1].SetLine(line);
          ++index1;
          if (index1 > 5)
            break;
        }
      }
      for (int index2 = index1 + 1; index2 < this.SpeechInputs.Count; ++index2)
      {
        this.SpeechInputs[index2].Line = (ZosigNPCProfile.NPCLine) null;
        this.SpeechInputs[index2].gameObject.SetActive(false);
      }
    }

    private void HideAndClearChoices()
    {
      for (int index = 0; index < this.SpeechInputs.Count; ++index)
      {
        this.SpeechInputs[index].ClearLine();
        this.SpeechInputs[index].gameObject.SetActive(false);
      }
    }

    public void SpeakLine(ZosigNPCProfile.NPCLine line)
    {
      this.S.KillSpeech();
      this.S.IsAllowedToSpeak = false;
      this.SetHowdyVisibility(false);
      this.HideAndClearChoices();
      this.SetState(ZosigNPCInterface.NPCConvoState.Speaking);
      this.m_lineSpeaking = line;
      AudioClip clip = line.Clips[Random.Range(0, line.Clips.Count)];
      this.m_timeTilDoneSpeaking = clip.length + 0.25f;
      this.AudSource_Voice.clip = clip;
      this.AudSource_Voice.Play();
    }

    private void CheckIsWithinSpeakingRange()
    {
      if ((double) Vector3.Distance(GM.CurrentPlayerBody.transform.position, this.transform.position) < 4.0)
        this.m_isInSpeakingRange = true;
      else
        this.m_isInSpeakingRange = false;
    }

    public void TerminateSpeech()
    {
      if (this.State == ZosigNPCInterface.NPCConvoState.NotInConvo)
        return;
      this.m_lineSpeaking = (ZosigNPCProfile.NPCLine) null;
      this.HideAndClearChoices();
      this.SetState(ZosigNPCInterface.NPCConvoState.NotInConvo);
      this.AudSource_Voice.Stop();
      this.S.IsAllowedToSpeak = true;
    }

    public enum NPCConvoState
    {
      NotInConvo,
      Speaking,
      WaitingForInput,
    }
  }
}
