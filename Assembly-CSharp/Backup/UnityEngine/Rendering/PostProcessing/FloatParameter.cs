﻿// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.FloatParameter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class FloatParameter : ParameterOverride<float>
  {
    public override void Interp(float from, float to, float t) => this.value = from + (to - from) * t;
  }
}
