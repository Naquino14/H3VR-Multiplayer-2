// Decompiled with JetBrains decompiler
// Type: FistVR.BangerSwitch
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BangerSwitch : FVRInteractiveObject
  {
    public Banger Banger;
    public Transform SwitchPiece;
    public Vector3 Eulers_Off;
    public Vector3 Eulers_On;
    private bool m_isOn;
    public AudioEvent AudEven_Switch;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      this.ToggleSwitch();
    }

    public void ToggleSwitch()
    {
      this.m_isOn = !this.m_isOn;
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEven_Switch, this.transform.position);
      if (this.m_isOn)
      {
        this.Banger.Arm();
        this.SwitchPiece.localEulerAngles = this.Eulers_On;
      }
      else
      {
        this.Banger.DeArm();
        this.SwitchPiece.localEulerAngles = this.Eulers_Off;
      }
    }
  }
}
