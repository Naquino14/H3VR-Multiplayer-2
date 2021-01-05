// Decompiled with JetBrains decompiler
// Type: WFX_LightFlicker
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof (Light))]
public class WFX_LightFlicker : MonoBehaviour
{
  public float time = 0.05f;
  private float timer;

  private void Start()
  {
    this.timer = this.time;
    this.StartCoroutine("Flicker");
  }

  [DebuggerHidden]
  private IEnumerator Flicker() => (IEnumerator) new WFX_LightFlicker.\u003CFlicker\u003Ec__Iterator0()
  {
    \u0024this = this
  };
}
