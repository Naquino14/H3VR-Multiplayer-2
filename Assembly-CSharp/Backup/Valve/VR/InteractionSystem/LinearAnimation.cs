// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.LinearAnimation
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class LinearAnimation : MonoBehaviour
  {
    public LinearMapping linearMapping;
    public Animation animation;
    private AnimationState animState;
    private float animLength;
    private float lastValue;

    private void Awake()
    {
      if ((Object) this.animation == (Object) null)
        this.animation = this.GetComponent<Animation>();
      if ((Object) this.linearMapping == (Object) null)
        this.linearMapping = this.GetComponent<LinearMapping>();
      this.animation.playAutomatically = true;
      this.animState = this.animation[this.animation.clip.name];
      this.animState.wrapMode = WrapMode.PingPong;
      this.animState.speed = 0.0f;
      this.animLength = this.animState.length;
    }

    private void Update()
    {
      float num = this.linearMapping.value;
      if ((double) num != (double) this.lastValue)
        this.animState.time = num * this.animLength;
      this.lastValue = num;
    }
  }
}
