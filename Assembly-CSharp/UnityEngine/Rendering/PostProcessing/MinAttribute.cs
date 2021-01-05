﻿// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.MinAttribute
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
  public sealed class MinAttribute : Attribute
  {
    public readonly float min;

    public MinAttribute(float min) => this.min = min;
  }
}
