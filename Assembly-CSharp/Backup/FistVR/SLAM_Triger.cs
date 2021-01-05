// Decompiled with JetBrains decompiler
// Type: FistVR.SLAM_Triger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class SLAM_Triger : FVRInteractiveObject
  {
    public SLAM S;
    public bool IsFlipTrigger;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      this.S.TriggerFlipped(this.IsFlipTrigger);
      base.SimpleInteraction(hand);
    }
  }
}
