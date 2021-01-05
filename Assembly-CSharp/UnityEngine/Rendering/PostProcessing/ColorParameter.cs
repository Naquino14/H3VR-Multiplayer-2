// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.ColorParameter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class ColorParameter : ParameterOverride<Color>
  {
    public override void Interp(Color from, Color to, float t)
    {
      this.value.r = from.r + (to.r - from.r) * t;
      this.value.g = from.g + (to.g - from.g) * t;
      this.value.b = from.b + (to.b - from.b) * t;
      this.value.a = from.a + (to.a - from.a) * t;
    }
  }
}
