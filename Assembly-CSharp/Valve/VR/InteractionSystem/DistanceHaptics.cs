// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.DistanceHaptics
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class DistanceHaptics : MonoBehaviour
  {
    public Transform firstTransform;
    public Transform secondTransform;
    public AnimationCurve distanceIntensityCurve = AnimationCurve.Linear(0.0f, 800f, 1f, 800f);
    public AnimationCurve pulseIntervalCurve = AnimationCurve.Linear(0.0f, 0.01f, 1f, 0.0f);

    [DebuggerHidden]
    private IEnumerator Start() => (IEnumerator) new DistanceHaptics.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
