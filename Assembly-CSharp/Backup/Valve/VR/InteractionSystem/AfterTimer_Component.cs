// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.AfterTimer_Component
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  [Serializable]
  public class AfterTimer_Component : MonoBehaviour
  {
    private System.Action callback;
    private float triggerTime;
    private bool timerActive;
    private bool triggerOnEarlyDestroy;

    public void Init(float _time, System.Action _callback, bool earlydestroy)
    {
      this.triggerTime = _time;
      this.callback = _callback;
      this.triggerOnEarlyDestroy = earlydestroy;
      this.timerActive = true;
      this.StartCoroutine(this.Wait());
    }

    [DebuggerHidden]
    private IEnumerator Wait() => (IEnumerator) new AfterTimer_Component.\u003CWait\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    private void OnDestroy()
    {
      if (!this.timerActive)
        return;
      this.StopCoroutine(this.Wait());
      this.timerActive = false;
      if (!this.triggerOnEarlyDestroy)
        return;
      this.callback();
    }
  }
}
