// Decompiled with JetBrains decompiler
// Type: FistVR.BreakActionModeSwitchButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BreakActionModeSwitchButton : FVRInteractiveObject
  {
    public BreakActionWeapon Weapon;
    public Transform Switch;
    public Vector2 RotSet;
    private bool m_isSwitched;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      this.Weapon.FireAllBarrels = !this.Weapon.FireAllBarrels;
      this.Weapon.PlayAudioEvent(FirearmAudioEventType.FireSelector);
      this.m_isSwitched = !this.m_isSwitched;
      if (this.m_isSwitched)
        this.Weapon.SetAnimatedComponent(this.Switch, this.RotSet.y, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
      else
        this.Weapon.SetAnimatedComponent(this.Switch, this.RotSet.x, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
    }
  }
}
