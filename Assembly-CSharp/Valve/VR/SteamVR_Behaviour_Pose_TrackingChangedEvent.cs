// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Behaviour_Pose_TrackingChangedEvent
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine.Events;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Behaviour_Pose_TrackingChangedEvent : UnityEvent<SteamVR_Behaviour_Pose, SteamVR_Input_Sources, ETrackingResult>
  {
  }
}
