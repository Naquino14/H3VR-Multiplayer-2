// Decompiled with JetBrains decompiler
// Type: FistVR.TubeFedShotgunMagazineSwitcher
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TubeFedShotgunMagazineSwitcher : FVRInteractiveObject
  {
    public TubeFedShotgun Shotgun;
    public FVRFireArmMagazineReloadTrigger Trig;
    public Transform Switch;
    public List<TubeFedShotgunMagazineSwitcher.FauxMag> FauxMags;
    public TubeFedShotgunMagazineSwitcher.MagState CurState;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      if (!this.Shotgun.CanCycleMagState())
        return;
      this.CycleSwitch();
      this.Shotgun.PlayAudioAsHandling(this.Shotgun.AudioClipSet.Safety, this.transform.position);
    }

    private void CycleSwitch()
    {
      if (this.CurState == TubeFedShotgunMagazineSwitcher.MagState.LeftMag)
        this.CurState = TubeFedShotgunMagazineSwitcher.MagState.NoMag;
      else if (this.CurState == TubeFedShotgunMagazineSwitcher.MagState.NoMag)
        this.CurState = TubeFedShotgunMagazineSwitcher.MagState.RightMag;
      else if (this.CurState == TubeFedShotgunMagazineSwitcher.MagState.RightMag)
        this.CurState = TubeFedShotgunMagazineSwitcher.MagState.LeftMag;
      this.UpdateState();
    }

    private void UpdateState()
    {
      if (this.CurState == TubeFedShotgunMagazineSwitcher.MagState.LeftMag)
      {
        this.Shotgun.Magazine = this.FauxMags[0].Mag;
        this.Trig.Magazine = this.FauxMags[0].Mag;
        this.Trig.gameObject.SetActive(true);
        this.Shotgun.RoundPos_LowerPath_Forward.position = this.FauxMags[0].LowerPathForward.position;
        this.Shotgun.RoundPos_LowerPath_Rearward.position = this.FauxMags[0].LowerPathRearward.position;
        this.Trig.transform.position = this.FauxMags[0].TrigPoint.position;
        this.Switch.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -16f);
      }
      else if (this.CurState == TubeFedShotgunMagazineSwitcher.MagState.NoMag)
      {
        this.Shotgun.Magazine = (FVRFireArmMagazine) null;
        this.Trig.gameObject.SetActive(false);
        this.Switch.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      }
      else
      {
        if (this.CurState != TubeFedShotgunMagazineSwitcher.MagState.RightMag)
          return;
        this.Shotgun.Magazine = this.FauxMags[1].Mag;
        this.Trig.Magazine = this.FauxMags[1].Mag;
        this.Trig.gameObject.SetActive(true);
        this.Shotgun.RoundPos_LowerPath_Forward.position = this.FauxMags[1].LowerPathForward.position;
        this.Shotgun.RoundPos_LowerPath_Rearward.position = this.FauxMags[1].LowerPathRearward.position;
        this.Trig.transform.position = this.FauxMags[1].TrigPoint.position;
        this.Switch.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 16f);
      }
    }

    public enum MagState
    {
      LeftMag,
      NoMag,
      RightMag,
    }

    [Serializable]
    public class FauxMag
    {
      public FVRFireArmMagazine Mag;
      public Transform LowerPathForward;
      public Transform LowerPathRearward;
      public Transform TrigPoint;
    }
  }
}
