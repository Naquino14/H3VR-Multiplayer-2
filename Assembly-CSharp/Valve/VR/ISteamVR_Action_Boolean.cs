﻿// Decompiled with JetBrains decompiler
// Type: Valve.VR.ISteamVR_Action_Boolean
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace Valve.VR
{
  public interface ISteamVR_Action_Boolean : ISteamVR_Action_In_Source, ISteamVR_Action_Source
  {
    bool state { get; }

    bool stateDown { get; }

    bool stateUp { get; }

    bool lastState { get; }

    bool lastStateDown { get; }

    bool lastStateUp { get; }
  }
}
