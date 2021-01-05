// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessResources
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class PostProcessResources : ScriptableObject
  {
    public Texture2D[] blueNoise64;
    public Texture2D[] blueNoise256;
    public PostProcessResources.SMAALuts smaaLuts;
    public PostProcessResources.Shaders shaders;
    public PostProcessResources.ComputeShaders computeShaders;

    [Serializable]
    public sealed class Shaders
    {
      public Shader autoExposure;
      public Shader bloom;
      public Shader copy;
      public Shader copyStd;
      public Shader discardAlpha;
      public Shader depthOfField;
      public Shader finalPass;
      public Shader grainBaker;
      public Shader motionBlur;
      public Shader temporalAntialiasing;
      public Shader subpixelMorphologicalAntialiasing;
      public Shader texture2dLerp;
      public Shader uber;
      public Shader lut2DBaker;
      public Shader lightMeter;
      public Shader gammaHistogram;
      public Shader waveform;
      public Shader vectorscope;
      public Shader debugOverlays;
      public Shader deferredFog;
      public Shader scalableAO;
      public Shader multiScaleAO;
      public Shader screenSpaceReflections;
    }

    [Serializable]
    public sealed class ComputeShaders
    {
      public ComputeShader exposureHistogram;
      public ComputeShader lut3DBaker;
      public ComputeShader texture3dLerp;
      public ComputeShader gammaHistogram;
      public ComputeShader waveform;
      public ComputeShader vectorscope;
      public ComputeShader multiScaleAODownsample1;
      public ComputeShader multiScaleAODownsample2;
      public ComputeShader multiScaleAORender;
      public ComputeShader multiScaleAOUpsample;
      public ComputeShader gaussianDownsample;
    }

    [Serializable]
    public sealed class SMAALuts
    {
      public Texture2D area;
      public Texture2D search;
    }
  }
}
