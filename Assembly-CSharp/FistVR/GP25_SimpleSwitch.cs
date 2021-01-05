// Decompiled with JetBrains decompiler
// Type: FistVR.GP25_SimpleSwitch
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class GP25_SimpleSwitch : FVRInteractiveObject
  {
    public GP25 Launcher;
    public GP25_SimpleSwitch.GP25SwitchType Type;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      if (this.Type == GP25_SimpleSwitch.GP25SwitchType.Safety)
        this.Launcher.ToggleSafety();
      else if (this.Type == GP25_SimpleSwitch.GP25SwitchType.Ejector)
        this.Launcher.SafeEject();
      base.SimpleInteraction(hand);
    }

    public enum GP25SwitchType
    {
      Safety,
      Ejector,
    }
  }
}
