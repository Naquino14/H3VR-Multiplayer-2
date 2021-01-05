// Decompiled with JetBrains decompiler
// Type: FistVR.M203
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class M203 : AttachableFirearm
  {
    [Header("M203")]
    public FVRFireArmChamber Chamber;
    public M203_Fore Fore;
    public Transform Trigger;
    public Vector2 TriggerRange;

    public override void ProcessInput(FVRViveHand hand, bool fromInterface, FVRInteractiveObject o)
    {
      if (o.m_hasTriggeredUpSinceBegin)
        this.Attachment.SetAnimatedComponent(this.Trigger, Mathf.Lerp(this.TriggerRange.x, this.TriggerRange.y, hand.Input.TriggerFloat), FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
      if (!hand.Input.TriggerDown || this.Fore.CurPos != M203_Fore.ForePos.Rearward || !o.m_hasTriggeredUpSinceBegin)
        return;
      this.Fire(fromInterface);
    }

    public void Fire(bool firedFromInterface)
    {
      if (!this.Chamber.Fire())
        return;
      this.FireMuzzleSmoke();
      if (firedFromInterface)
      {
        FVRFireArm fa = this.Attachment.curMount.MyObject as FVRFireArm;
        if ((Object) fa != (Object) null)
        {
          fa.Recoil(fa.IsTwoHandStabilized(), fa.IsForegripStabilized(), fa.IsShoulderStabilized(), this.RecoilProfile);
          this.Fire(this.Chamber, this.MuzzlePos, true, fa);
        }
        else
          this.Fire(this.Chamber, this.MuzzlePos, true, (FVRFireArm) null);
      }
      else
      {
        this.Recoil(false, false, false);
        this.Fire(this.Chamber, this.MuzzlePos, true, (FVRFireArm) null);
      }
      this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
    }

    private void Update() => this.Fore.UpdateSlide();
  }
}
