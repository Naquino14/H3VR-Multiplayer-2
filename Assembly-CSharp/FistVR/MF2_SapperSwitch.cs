// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_SapperSwitch
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class MF2_SapperSwitch : FVRInteractiveObject
  {
    public MF2_Sapper Sapper;
    public int WhichSwitch;
    public AudioEvent AudEvent_Switch;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      this.Sapper.ToggleSwitch(this.WhichSwitch);
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Switch, this.transform.position);
    }
  }
}
