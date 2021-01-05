// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_Sources
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.ComponentModel;

namespace Valve.VR
{
  public enum SteamVR_Input_Sources
  {
    [Description("/unrestricted")] Any,
    [Description("/user/hand/left")] LeftHand,
    [Description("/user/hand/right")] RightHand,
    [Description("/user/foot/left")] LeftFoot,
    [Description("/user/foot/right")] RightFoot,
    [Description("/user/shoulder/left")] LeftShoulder,
    [Description("/user/shoulder/right")] RightShoulder,
    [Description("/user/waist")] Waist,
    [Description("/user/chest")] Chest,
    [Description("/user/head")] Head,
    [Description("/user/gamepad")] Gamepad,
    [Description("/user/camera")] Camera,
    [Description("/user/keyboard")] Keyboard,
  }
}
