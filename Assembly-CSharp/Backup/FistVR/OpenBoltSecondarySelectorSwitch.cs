// Decompiled with JetBrains decompiler
// Type: FistVR.OpenBoltSecondarySelectorSwitch
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OpenBoltSecondarySelectorSwitch : FVRInteractiveObject
  {
    public OpenBoltReceiver Gun;
    public OpenBoltSecondarySelectorSwitch.InterpStyle FireSelector_InterpStyle = OpenBoltSecondarySelectorSwitch.InterpStyle.Rotation;
    public OpenBoltSecondarySelectorSwitch.Axis FireSelector_Axis;
    public int ModeIndexToSub;
    public OpenBoltReceiver.FireSelectorMode[] Modes;
    public int CurModeIndex;
    public Transform Selector;

    public new void Awake() => this.UpdateBaseGunSelector(this.CurModeIndex);

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      this.CycleMode();
    }

    private void CycleMode()
    {
      ++this.CurModeIndex;
      if (this.CurModeIndex >= this.Modes.Length)
        this.CurModeIndex = 0;
      this.UpdateBaseGunSelector(this.CurModeIndex);
      this.Gun.SecondaryFireSelectorClicked();
    }

    private void UpdateBaseGunSelector(int i)
    {
      this.CurModeIndex = i;
      this.Gun.FireSelector_Modes[this.ModeIndexToSub].ModeType = this.Modes[this.CurModeIndex].ModeType;
      switch (this.FireSelector_InterpStyle)
      {
        case OpenBoltSecondarySelectorSwitch.InterpStyle.Translate:
          Vector3 zero1 = Vector3.zero;
          switch (this.FireSelector_Axis)
          {
            case OpenBoltSecondarySelectorSwitch.Axis.X:
              zero1.x = this.Modes[this.CurModeIndex].SelectorPosition;
              break;
            case OpenBoltSecondarySelectorSwitch.Axis.Y:
              zero1.y = this.Modes[this.CurModeIndex].SelectorPosition;
              break;
            case OpenBoltSecondarySelectorSwitch.Axis.Z:
              zero1.z = this.Modes[this.CurModeIndex].SelectorPosition;
              break;
          }
          this.Selector.localPosition = zero1;
          break;
        case OpenBoltSecondarySelectorSwitch.InterpStyle.Rotation:
          Vector3 zero2 = Vector3.zero;
          switch (this.FireSelector_Axis)
          {
            case OpenBoltSecondarySelectorSwitch.Axis.X:
              zero2.x = this.Modes[this.CurModeIndex].SelectorPosition;
              break;
            case OpenBoltSecondarySelectorSwitch.Axis.Y:
              zero2.y = this.Modes[this.CurModeIndex].SelectorPosition;
              break;
            case OpenBoltSecondarySelectorSwitch.Axis.Z:
              zero2.z = this.Modes[this.CurModeIndex].SelectorPosition;
              break;
          }
          this.Selector.localEulerAngles = zero2;
          break;
      }
    }

    public enum InterpStyle
    {
      Translate,
      Rotation,
    }

    public enum Axis
    {
      X,
      Y,
      Z,
    }
  }
}
