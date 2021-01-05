// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigGastroCyclerPointZone
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class ZosigGastroCyclerPointZone : FVRPointable
  {
    public ZosigGastroCycler Cycler;
    public int PointIndex;
    public bool IsEnabled;

    public override void OnPoint(FVRViveHand hand)
    {
      base.OnPoint(hand);
      if (!this.IsEnabled)
        return;
      this.Cycler.InitiatePoint(this.PointIndex);
      if (!hand.Input.TriggerDown)
        return;
      this.Cycler.ClickPoint(this.PointIndex);
    }

    public override void EndPoint(FVRViveHand hand)
    {
      base.OnPoint(hand);
      this.Cycler.EndPoint(this.PointIndex);
    }
  }
}
