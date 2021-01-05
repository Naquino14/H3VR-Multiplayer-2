// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.MinMaxAttribute
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
  public sealed class MinMaxAttribute : Attribute
  {
    public readonly float min;
    public readonly float max;

    public MinMaxAttribute(float min, float max)
    {
      this.min = min;
      this.max = max;
    }
  }
}
