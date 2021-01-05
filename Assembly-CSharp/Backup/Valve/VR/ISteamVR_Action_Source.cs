// Decompiled with JetBrains decompiler
// Type: Valve.VR.ISteamVR_Action_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace Valve.VR
{
  public interface ISteamVR_Action_Source
  {
    bool active { get; }

    bool activeBinding { get; }

    bool lastActive { get; }

    bool lastActiveBinding { get; }

    string fullPath { get; }

    ulong handle { get; }

    SteamVR_ActionSet actionSet { get; }

    SteamVR_ActionDirections direction { get; }
  }
}
