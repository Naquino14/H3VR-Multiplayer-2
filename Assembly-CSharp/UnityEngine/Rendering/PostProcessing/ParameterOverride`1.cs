// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.ParameterOverride`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public class ParameterOverride<T> : ParameterOverride
  {
    public T value;

    public ParameterOverride()
      : this(default (T), false)
    {
    }

    public ParameterOverride(T value)
      : this(value, false)
    {
    }

    public ParameterOverride(T value, bool overrideState)
    {
      this.value = value;
      this.overrideState = overrideState;
    }

    internal override void Interp(ParameterOverride from, ParameterOverride to, float t) => this.Interp(from.GetValue<T>(), to.GetValue<T>(), t);

    public virtual void Interp(T from, T to, float t) => this.value = (double) t <= 0.0 ? from : to;

    public void Override(T x)
    {
      this.overrideState = true;
      this.value = x;
    }

    public override int GetHash() => (17 * 23 + this.overrideState.GetHashCode()) * 23 + this.value.GetHashCode();

    public static implicit operator T(ParameterOverride<T> prop) => prop.value;
  }
}
