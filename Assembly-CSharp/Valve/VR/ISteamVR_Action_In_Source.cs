// Decompiled with JetBrains decompiler
// Type: Valve.VR.ISteamVR_Action_In_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace Valve.VR
{
  public interface ISteamVR_Action_In_Source : ISteamVR_Action_Source
  {
    bool changed { get; }

    bool lastChanged { get; }

    float changedTime { get; }

    float updateTime { get; }

    ulong activeOrigin { get; }

    ulong lastActiveOrigin { get; }

    SteamVR_Input_Sources activeDevice { get; }

    uint trackedDeviceIndex { get; }

    string renderModelComponentName { get; }

    string localizedOriginName { get; }
  }
}
