// Decompiled with JetBrains decompiler
// Type: FistVR.OmniClericRing
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class OmniClericRing : MonoBehaviour
  {
    public GameObject Map;
    public List<Transform> SpawnPoints;
    public List<Transform> Blockers;
    public List<Renderer> Indicators;
    public float BlockerMinHeight = 0.5f;
    public float BlockerMaxHeight = 1.9f;
  }
}
