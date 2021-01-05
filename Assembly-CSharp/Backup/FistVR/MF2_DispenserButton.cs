// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_DispenserButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class MF2_DispenserButton : FVRInteractiveObject
  {
    public MF2_Dispenser Dispenser;
    public MF2_DispenserButton.DispenserButtonType ButtonType;
    public MF2_DispenserButton.DispenserSide Side;

    public override void Poke(FVRViveHand hand)
    {
      base.Poke(hand);
      this.Dispenser.ButtonPressed(this.ButtonType, this.Side);
    }

    public enum DispenserButtonType
    {
      Heal,
      Reload,
      Reset,
      Turret,
      Magazine,
    }

    public enum DispenserSide
    {
      Front,
      Rear,
    }
  }
}
