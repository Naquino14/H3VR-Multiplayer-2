// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.LinearAnimator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class LinearAnimator : MonoBehaviour
  {
    public LinearMapping linearMapping;
    public Animator animator;
    private float currentLinearMapping = float.NaN;
    private int framesUnchanged;

    private void Awake()
    {
      if ((Object) this.animator == (Object) null)
        this.animator = this.GetComponent<Animator>();
      this.animator.speed = 0.0f;
      if (!((Object) this.linearMapping == (Object) null))
        return;
      this.linearMapping = this.GetComponent<LinearMapping>();
    }

    private void Update()
    {
      if ((double) this.currentLinearMapping != (double) this.linearMapping.value)
      {
        this.currentLinearMapping = this.linearMapping.value;
        this.animator.enabled = true;
        this.animator.Play(0, 0, this.currentLinearMapping);
        this.framesUnchanged = 0;
      }
      else
      {
        ++this.framesUnchanged;
        if (this.framesUnchanged <= 2)
          return;
        this.animator.enabled = false;
      }
    }
  }
}
