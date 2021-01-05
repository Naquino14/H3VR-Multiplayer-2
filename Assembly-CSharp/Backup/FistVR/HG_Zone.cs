// Decompiled with JetBrains decompiler
// Type: FistVR.HG_Zone
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class HG_Zone : MonoBehaviour
  {
    public bool DebugView;
    public Transform PlayerSpawnPoint;
    public List<Transform> SpawnPoints_Offense;
    public List<Transform> SpawnPoints_Defense;
    public List<Transform> TargetPoints;
    public Transform Indicator;
  }
}
