﻿// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.Vector2Parameter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class Vector2Parameter : ParameterOverride<Vector2>
  {
    public override void Interp(Vector2 from, Vector2 to, float t)
    {
      this.value.x = from.x + (to.x - from.x) * t;
      this.value.y = from.y + (to.y - from.y) * t;
    }
  }
}
