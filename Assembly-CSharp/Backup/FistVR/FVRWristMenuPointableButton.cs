// Decompiled with JetBrains decompiler
// Type: FistVR.FVRWristMenuPointableButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class FVRWristMenuPointableButton : FVRPointable
  {
    public int ButtonIndex;
    public FVRWristMenu WristMenu;

    public override void BeginHoverDisplay()
    {
      base.BeginHoverDisplay();
      this.WristMenu.SetSelectedButton(this.ButtonIndex);
    }

    public override void OnPoint(FVRViveHand hand)
    {
      base.OnPoint(hand);
      if (!hand.Input.TriggerDown)
        return;
      this.WristMenu.InvokeButton(this.ButtonIndex);
    }
  }
}
