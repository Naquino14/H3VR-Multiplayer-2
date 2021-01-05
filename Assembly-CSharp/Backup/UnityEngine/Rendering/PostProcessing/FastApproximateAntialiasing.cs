// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.FastApproximateAntialiasing
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class FastApproximateAntialiasing
  {
    [Tooltip("Boost performances by lowering the effect quality. This settings is meant to be used on mobile and other low-end platforms.")]
    public bool mobileOptimized;
    [Tooltip("Keep alpha channel. This will slightly lower the effect quality but allows rendering against a transparent background.")]
    public bool keepAlpha;
  }
}
