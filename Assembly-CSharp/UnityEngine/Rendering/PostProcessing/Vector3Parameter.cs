// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.Vector3Parameter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class Vector3Parameter : ParameterOverride<Vector3>
  {
    public override void Interp(Vector3 from, Vector3 to, float t)
    {
      this.value.x = from.x + (to.x - from.x) * t;
      this.value.y = from.y + (to.y - from.y) * t;
      this.value.z = from.z + (to.z - from.z) * t;
    }
  }
}
