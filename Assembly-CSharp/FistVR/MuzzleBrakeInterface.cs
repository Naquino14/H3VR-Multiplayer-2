// Decompiled with JetBrains decompiler
// Type: FistVR.MuzzleBrakeInterface
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class MuzzleBrakeInterface : MuzzleDeviceInterface
  {
    private MuzzleBrake tempBrake;

    protected override void Awake()
    {
      base.Awake();
      this.tempBrake = this.Attachment as MuzzleBrake;
    }

    public override void OnAttach()
    {
      this.tempBrake = this.Attachment as MuzzleBrake;
      if (this.Attachment.curMount.GetRootMount().Parent is FVRFireArm)
        (this.Attachment.curMount.GetRootMount().Parent as FVRFireArm).RegisterMuzzleBrake(this.tempBrake);
      base.OnAttach();
    }

    public override void OnDetach()
    {
      if (this.Attachment.curMount.GetRootMount().Parent is FVRFireArm)
        (this.Attachment.curMount.GetRootMount().Parent as FVRFireArm).RegisterMuzzleBrake((MuzzleBrake) null);
      base.OnDetach();
    }
  }
}
