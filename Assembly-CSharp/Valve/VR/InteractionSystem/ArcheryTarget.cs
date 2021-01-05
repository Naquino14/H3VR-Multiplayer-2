// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.ArcheryTarget
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
  public class ArcheryTarget : MonoBehaviour
  {
    public UnityEvent onTakeDamage;
    public bool onceOnly;
    public Transform targetCenter;
    public Transform baseTransform;
    public Transform fallenDownTransform;
    public float fallTime = 0.5f;
    private const float targetRadius = 0.25f;
    private bool targetEnabled = true;

    private void ApplyDamage() => this.OnDamageTaken();

    private void FireExposure() => this.OnDamageTaken();

    private void OnDamageTaken()
    {
      if (!this.targetEnabled)
        return;
      this.onTakeDamage.Invoke();
      this.StartCoroutine(this.FallDown());
      if (!this.onceOnly)
        return;
      this.targetEnabled = false;
    }

    [DebuggerHidden]
    private IEnumerator FallDown() => (IEnumerator) new ArcheryTarget.\u003CFallDown\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
