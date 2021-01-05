// Decompiled with JetBrains decompiler
// Type: FistVR.FirearmAudioEventType
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public enum FirearmAudioEventType
  {
    BoltSlideForward = 0,
    BoltSlideBack = 1,
    BoltSlideForwardHeld = 2,
    BoltSlideBackHeld = 3,
    BoltSlideBackLocked = 4,
    CatchOnSear = 5,
    HammerHit = 6,
    Prefire = 7,
    BoltRelease = 8,
    HandleGrab = 9,
    HandleBack = 10, // 0x0000000A
    HandleForward = 11, // 0x0000000B
    HandleUp = 12, // 0x0000000C
    HandleDown = 13, // 0x0000000D
    Safety = 14, // 0x0000000E
    FireSelector = 15, // 0x0000000F
    TriggerReset = 16, // 0x00000010
    BreachOpen = 17, // 0x00000011
    BreachClose = 18, // 0x00000012
    MagazineIn = 20, // 0x00000014
    MagazineOut = 21, // 0x00000015
    TopCoverRelease = 22, // 0x00000016
    TopCoverUp = 23, // 0x00000017
    TopCoverDown = 24, // 0x00000018
    MagazineInsertRound = 28, // 0x0000001C
    MagazineEjectRound = 29, // 0x0000001D
    StockOpen = 30, // 0x0000001E
    StockClosed = 31, // 0x0000001F
    BipodOpen = 32, // 0x00000020
    BipodClosed = 33, // 0x00000021
    HandleForwardEmpty = 40, // 0x00000028
    HandleBackEmpty = 41, // 0x00000029
    ChamberManual = 42, // 0x0000002A
    BeltGrab = 50, // 0x00000032
    BeltRelease = 51, // 0x00000033
    BeltSeat = 52, // 0x00000034
    BeltSettle = 53, // 0x00000035
    Shots_Main = 100, // 0x00000064
    Shots_Suppressed = 101, // 0x00000065
    Shots_LowPressure = 102, // 0x00000066
  }
}
