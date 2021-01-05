// Decompiled with JetBrains decompiler
// Type: FistVR.ClosedBoltSecondarySwitch
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ClosedBoltSecondarySwitch : FVRInteractiveObject
  {
    public ClosedBoltWeapon Weapon;
    public int CurModeIndex;
    public int ModeIndexToSub = 1;
    public Transform SelctorSwitch;
    public FVRPhysicalObject.Axis Axis;
    public FVRPhysicalObject.InterpStyle InterpStyle;
    public ClosedBoltWeapon.FireSelectorMode[] Modes;

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
      this.Weapon.PlayAudioEvent(FirearmAudioEventType.FireSelector);
    }

    private void UpdateBaseGunSelector(int i)
    {
      ClosedBoltWeapon.FireSelectorMode mode = this.Modes[i];
      this.Weapon.SetAnimatedComponent(this.SelctorSwitch, mode.SelectorPosition, this.InterpStyle, this.Axis);
      ClosedBoltWeapon.FireSelectorMode fireSelectorMode = this.Weapon.FireSelector_Modes[this.ModeIndexToSub];
      fireSelectorMode.ModeType = mode.ModeType;
      fireSelectorMode.BurstAmount = mode.BurstAmount;
      this.Weapon.ResetCamBurst();
    }
  }
}
