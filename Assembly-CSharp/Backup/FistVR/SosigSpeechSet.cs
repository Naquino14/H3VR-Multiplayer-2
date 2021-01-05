// Decompiled with JetBrains decompiler
// Type: FistVR.SosigSpeechSet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New SosigSpeechSet", menuName = "Sosig/SosigSpeechSet", order = 0)]
  public class SosigSpeechSet : ScriptableObject
  {
    public float BasePitch = 1f;
    public float BaseVolume = 0.4f;
    public bool ForceDeathSpeech;
    public bool UseAltDeathOnHeadExplode;
    public bool LessTalkativeSkirmish;
    [Header("Pain Speech")]
    public List<AudioClip> OnJointBreak;
    public List<AudioClip> OnJointSlice;
    public List<AudioClip> OnJointSever;
    public List<AudioClip> OnDeath;
    public List<AudioClip> OnBackBreak;
    public List<AudioClip> OnNeckBreak;
    public List<AudioClip> OnPain;
    public List<AudioClip> OnConfusion;
    public List<AudioClip> OnDeathAlt;
    [Header("State Speech")]
    public List<AudioClip> OnWander;
    public List<AudioClip> OnSkirmish;
    public List<AudioClip> OnInvestigate;
    public List<AudioClip> OnSearchingForGuns;
    public List<AudioClip> OnTakingCover;
    public List<AudioClip> OnBeingAimedAt;
    public List<AudioClip> OnAssault;
    public List<AudioClip> OnReloading;
    public List<AudioClip> OnMedic;
    [Header("CallAndResponse")]
    public List<AudioClip> OnCall_Skirmish;
    public List<AudioClip> OnRespond_Skirmish;
    public List<AudioClip> OnCall_Assistance;
    public List<AudioClip> OnRespond_Assistance;
    public List<AudioClip> Test;

    [ContextMenu("dist")]
    public void dist()
    {
      for (int index = 0; index < this.Test.Count; ++index)
      {
        if (this.Test[index].name.Contains("pain"))
        {
          this.OnJointBreak.Add(this.Test[index]);
          this.OnBackBreak.Add(this.Test[index]);
          this.OnNeckBreak.Add(this.Test[index]);
          this.OnPain.Add(this.Test[index]);
        }
        else if (this.Test[index].name.Contains("death"))
        {
          this.OnJointSlice.Add(this.Test[index]);
          this.OnJointSever.Add(this.Test[index]);
          this.OnDeath.Add(this.Test[index]);
        }
        else if (this.Test[index].name.Contains("panic"))
          this.OnSearchingForGuns.Add(this.Test[index]);
        else if (this.Test[index].name.Contains("combat"))
          this.OnSkirmish.Add(this.Test[index]);
        else if (this.Test[index].name.Contains("invest"))
          this.OnInvestigate.Add(this.Test[index]);
        else if (this.Test[index].name.Contains("aim"))
          this.OnBeingAimedAt.Add(this.Test[index]);
      }
    }
  }
}
