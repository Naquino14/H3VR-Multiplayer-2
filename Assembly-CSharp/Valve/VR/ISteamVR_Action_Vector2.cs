﻿// Decompiled with JetBrains decompiler
// Type: Valve.VR.ISteamVR_Action_Vector2
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public interface ISteamVR_Action_Vector2 : ISteamVR_Action_In_Source, ISteamVR_Action_Source
  {
    Vector2 axis { get; }

    Vector2 lastAxis { get; }

    Vector2 delta { get; }

    Vector2 lastDelta { get; }
  }
}