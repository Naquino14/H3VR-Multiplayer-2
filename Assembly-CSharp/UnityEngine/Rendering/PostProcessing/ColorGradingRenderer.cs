// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.ColorGradingRenderer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class ColorGradingRenderer : PostProcessEffectRenderer<ColorGrading>
  {
    private Texture2D m_GradingCurves;
    private readonly Color[] m_Pixels = new Color[256];
    private RenderTexture m_InternalLdrLut;
    private RenderTexture m_InternalLogLut;
    private const int k_Lut2DSize = 32;
    private const int k_Lut3DSize = 33;
    private readonly HableCurve m_HableCurve = new HableCurve();

    public override void Render(PostProcessRenderContext context)
    {
      GradingMode gradingMode = this.settings.gradingMode.value;
      bool flag = SystemInfo.supports3DRenderTextures && SystemInfo.supportsComputeShaders;
      if (gradingMode == GradingMode.External)
        this.RenderExternalPipeline3D(context);
      else if (gradingMode == GradingMode.HighDefinitionRange && flag)
        this.RenderHDRPipeline3D(context);
      else if (gradingMode == GradingMode.HighDefinitionRange)
        this.RenderHDRPipeline2D(context);
      else
        this.RenderLDRPipeline2D(context);
    }

    private void RenderExternalPipeline3D(PostProcessRenderContext context)
    {
      Texture texture = this.settings.externalLut.value;
      if ((Object) texture == (Object) null)
        return;
      PropertySheet uberSheet = context.uberSheet;
      uberSheet.EnableKeyword("COLOR_GRADING_HDR_3D");
      uberSheet.properties.SetTexture(ShaderIDs.Lut3D, texture);
      uberSheet.properties.SetVector(ShaderIDs.Lut3D_Params, (Vector4) new Vector2(1f / (float) texture.width, (float) texture.width - 1f));
      uberSheet.properties.SetFloat(ShaderIDs.PostExposure, RuntimeUtilities.Exp2(this.settings.postExposure.value));
      context.logLut = texture;
    }

    private void RenderHDRPipeline3D(PostProcessRenderContext context)
    {
      this.CheckInternalLogLut();
      ComputeShader lut3Dbaker = context.resources.computeShaders.lut3DBaker;
      int kernelIndex = 0;
      switch (this.settings.tonemapper.value)
      {
        case Tonemapper.None:
          kernelIndex = lut3Dbaker.FindKernel("KGenLut3D_NoTonemap");
          break;
        case Tonemapper.Neutral:
          kernelIndex = lut3Dbaker.FindKernel("KGenLut3D_NeutralTonemap");
          break;
        case Tonemapper.ACES:
          kernelIndex = lut3Dbaker.FindKernel("KGenLut3D_AcesTonemap");
          break;
        case Tonemapper.Custom:
          kernelIndex = lut3Dbaker.FindKernel("KGenLut3D_CustomTonemap");
          break;
      }
      int num = Mathf.CeilToInt(4.125f);
      int threadGroupsZ = Mathf.CeilToInt((float) (33.0 / (!RuntimeUtilities.isAndroidOpenGL ? 8.0 : 2.0)));
      CommandBuffer command = context.command;
      command.SetComputeTextureParam(lut3Dbaker, kernelIndex, "_Output", (RenderTargetIdentifier) (Texture) this.m_InternalLogLut);
      command.SetComputeVectorParam(lut3Dbaker, "_Size", new Vector4(33f, 1f / 32f, 0.0f, 0.0f));
      Vector3 colorBalance = ColorUtilities.ComputeColorBalance(this.settings.temperature.value, this.settings.tint.value);
      command.SetComputeVectorParam(lut3Dbaker, "_ColorBalance", (Vector4) colorBalance);
      command.SetComputeVectorParam(lut3Dbaker, "_ColorFilter", (Vector4) this.settings.colorFilter.value);
      float x = this.settings.hueShift.value / 360f;
      float y = (float) ((double) this.settings.saturation.value / 100.0 + 1.0);
      float z = (float) ((double) this.settings.contrast.value / 100.0 + 1.0);
      command.SetComputeVectorParam(lut3Dbaker, "_HueSatCon", new Vector4(x, y, z, 0.0f));
      Vector4 vector4_1 = new Vector4((float) (ParameterOverride<float>) this.settings.mixerRedOutRedIn, (float) (ParameterOverride<float>) this.settings.mixerRedOutGreenIn, (float) (ParameterOverride<float>) this.settings.mixerRedOutBlueIn, 0.0f);
      Vector4 vector4_2 = new Vector4((float) (ParameterOverride<float>) this.settings.mixerGreenOutRedIn, (float) (ParameterOverride<float>) this.settings.mixerGreenOutGreenIn, (float) (ParameterOverride<float>) this.settings.mixerGreenOutBlueIn, 0.0f);
      Vector4 vector4_3 = new Vector4((float) (ParameterOverride<float>) this.settings.mixerBlueOutRedIn, (float) (ParameterOverride<float>) this.settings.mixerBlueOutGreenIn, (float) (ParameterOverride<float>) this.settings.mixerBlueOutBlueIn, 0.0f);
      command.SetComputeVectorParam(lut3Dbaker, "_ChannelMixerRed", vector4_1 / 100f);
      command.SetComputeVectorParam(lut3Dbaker, "_ChannelMixerGreen", vector4_2 / 100f);
      command.SetComputeVectorParam(lut3Dbaker, "_ChannelMixerBlue", vector4_3 / 100f);
      Vector3 lift = ColorUtilities.ColorToLift(this.settings.lift.value * 0.2f);
      Vector3 gain = ColorUtilities.ColorToGain(this.settings.gain.value * 0.8f);
      Vector3 inverseGamma = ColorUtilities.ColorToInverseGamma(this.settings.gamma.value * 0.8f);
      command.SetComputeVectorParam(lut3Dbaker, "_Lift", new Vector4(lift.x, lift.y, lift.z, 0.0f));
      command.SetComputeVectorParam(lut3Dbaker, "_InvGamma", new Vector4(inverseGamma.x, inverseGamma.y, inverseGamma.z, 0.0f));
      command.SetComputeVectorParam(lut3Dbaker, "_Gain", new Vector4(gain.x, gain.y, gain.z, 0.0f));
      command.SetComputeTextureParam(lut3Dbaker, kernelIndex, "_Curves", (RenderTargetIdentifier) (Texture) this.GetCurveTexture(true));
      if (this.settings.tonemapper.value == Tonemapper.Custom)
      {
        this.m_HableCurve.Init(this.settings.toneCurveToeStrength.value, this.settings.toneCurveToeLength.value, this.settings.toneCurveShoulderStrength.value, this.settings.toneCurveShoulderLength.value, this.settings.toneCurveShoulderAngle.value, this.settings.toneCurveGamma.value);
        command.SetComputeVectorParam(lut3Dbaker, "_CustomToneCurve", this.m_HableCurve.uniforms.curve);
        command.SetComputeVectorParam(lut3Dbaker, "_ToeSegmentA", this.m_HableCurve.uniforms.toeSegmentA);
        command.SetComputeVectorParam(lut3Dbaker, "_ToeSegmentB", this.m_HableCurve.uniforms.toeSegmentB);
        command.SetComputeVectorParam(lut3Dbaker, "_MidSegmentA", this.m_HableCurve.uniforms.midSegmentA);
        command.SetComputeVectorParam(lut3Dbaker, "_MidSegmentB", this.m_HableCurve.uniforms.midSegmentB);
        command.SetComputeVectorParam(lut3Dbaker, "_ShoSegmentA", this.m_HableCurve.uniforms.shoSegmentA);
        command.SetComputeVectorParam(lut3Dbaker, "_ShoSegmentB", this.m_HableCurve.uniforms.shoSegmentB);
      }
      context.command.BeginSample("HdrColorGradingLut3D");
      command.DispatchCompute(lut3Dbaker, kernelIndex, num, num, threadGroupsZ);
      context.command.EndSample("HdrColorGradingLut3D");
      RenderTexture internalLogLut = this.m_InternalLogLut;
      PropertySheet uberSheet = context.uberSheet;
      uberSheet.EnableKeyword("COLOR_GRADING_HDR_3D");
      uberSheet.properties.SetTexture(ShaderIDs.Lut3D, (Texture) internalLogLut);
      uberSheet.properties.SetVector(ShaderIDs.Lut3D_Params, (Vector4) new Vector2(1f / (float) internalLogLut.width, (float) internalLogLut.width - 1f));
      uberSheet.properties.SetFloat(ShaderIDs.PostExposure, RuntimeUtilities.Exp2(this.settings.postExposure.value));
      context.logLut = (Texture) internalLogLut;
    }

    private void RenderHDRPipeline2D(PostProcessRenderContext context)
    {
      this.CheckInternalStripLut();
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.lut2DBaker);
      propertySheet.ClearKeywords();
      propertySheet.properties.SetVector(ShaderIDs.Lut2D_Params, new Vector4(32f, 0.0004882813f, 1f / 64f, 1.032258f));
      Vector3 colorBalance = ColorUtilities.ComputeColorBalance(this.settings.temperature.value, this.settings.tint.value);
      propertySheet.properties.SetVector(ShaderIDs.ColorBalance, (Vector4) colorBalance);
      propertySheet.properties.SetVector(ShaderIDs.ColorFilter, (Vector4) this.settings.colorFilter.value);
      float x = this.settings.hueShift.value / 360f;
      float y = (float) ((double) this.settings.saturation.value / 100.0 + 1.0);
      float z = (float) ((double) this.settings.contrast.value / 100.0 + 1.0);
      propertySheet.properties.SetVector(ShaderIDs.HueSatCon, (Vector4) new Vector3(x, y, z));
      Vector3 vector3_1 = new Vector3((float) (ParameterOverride<float>) this.settings.mixerRedOutRedIn, (float) (ParameterOverride<float>) this.settings.mixerRedOutGreenIn, (float) (ParameterOverride<float>) this.settings.mixerRedOutBlueIn);
      Vector3 vector3_2 = new Vector3((float) (ParameterOverride<float>) this.settings.mixerGreenOutRedIn, (float) (ParameterOverride<float>) this.settings.mixerGreenOutGreenIn, (float) (ParameterOverride<float>) this.settings.mixerGreenOutBlueIn);
      Vector3 vector3_3 = new Vector3((float) (ParameterOverride<float>) this.settings.mixerBlueOutRedIn, (float) (ParameterOverride<float>) this.settings.mixerBlueOutGreenIn, (float) (ParameterOverride<float>) this.settings.mixerBlueOutBlueIn);
      propertySheet.properties.SetVector(ShaderIDs.ChannelMixerRed, (Vector4) (vector3_1 / 100f));
      propertySheet.properties.SetVector(ShaderIDs.ChannelMixerGreen, (Vector4) (vector3_2 / 100f));
      propertySheet.properties.SetVector(ShaderIDs.ChannelMixerBlue, (Vector4) (vector3_3 / 100f));
      Vector3 lift = ColorUtilities.ColorToLift(this.settings.lift.value * 0.2f);
      Vector3 gain = ColorUtilities.ColorToGain(this.settings.gain.value * 0.8f);
      Vector3 inverseGamma = ColorUtilities.ColorToInverseGamma(this.settings.gamma.value * 0.8f);
      propertySheet.properties.SetVector(ShaderIDs.Lift, (Vector4) lift);
      propertySheet.properties.SetVector(ShaderIDs.InvGamma, (Vector4) inverseGamma);
      propertySheet.properties.SetVector(ShaderIDs.Gain, (Vector4) gain);
      propertySheet.properties.SetTexture(ShaderIDs.Curves, (Texture) this.GetCurveTexture(false));
      switch (this.settings.tonemapper.value)
      {
        case Tonemapper.Neutral:
          propertySheet.EnableKeyword("TONEMAPPING_NEUTRAL");
          break;
        case Tonemapper.ACES:
          propertySheet.EnableKeyword("TONEMAPPING_ACES");
          break;
        case Tonemapper.Custom:
          propertySheet.EnableKeyword("TONEMAPPING_CUSTOM");
          this.m_HableCurve.Init(this.settings.toneCurveToeStrength.value, this.settings.toneCurveToeLength.value, this.settings.toneCurveShoulderStrength.value, this.settings.toneCurveShoulderLength.value, this.settings.toneCurveShoulderAngle.value, this.settings.toneCurveGamma.value);
          propertySheet.properties.SetVector(ShaderIDs.CustomToneCurve, this.m_HableCurve.uniforms.curve);
          propertySheet.properties.SetVector(ShaderIDs.ToeSegmentA, this.m_HableCurve.uniforms.toeSegmentA);
          propertySheet.properties.SetVector(ShaderIDs.ToeSegmentB, this.m_HableCurve.uniforms.toeSegmentB);
          propertySheet.properties.SetVector(ShaderIDs.MidSegmentA, this.m_HableCurve.uniforms.midSegmentA);
          propertySheet.properties.SetVector(ShaderIDs.MidSegmentB, this.m_HableCurve.uniforms.midSegmentB);
          propertySheet.properties.SetVector(ShaderIDs.ShoSegmentA, this.m_HableCurve.uniforms.shoSegmentA);
          propertySheet.properties.SetVector(ShaderIDs.ShoSegmentB, this.m_HableCurve.uniforms.shoSegmentB);
          break;
      }
      context.command.BeginSample("HdrColorGradingLut2D");
      context.command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) (Texture) this.m_InternalLdrLut, propertySheet, 2);
      context.command.EndSample("HdrColorGradingLut2D");
      RenderTexture internalLdrLut = this.m_InternalLdrLut;
      PropertySheet uberSheet = context.uberSheet;
      uberSheet.EnableKeyword("COLOR_GRADING_HDR_2D");
      uberSheet.properties.SetVector(ShaderIDs.Lut2D_Params, (Vector4) new Vector3(1f / (float) internalLdrLut.width, 1f / (float) internalLdrLut.height, (float) internalLdrLut.height - 1f));
      uberSheet.properties.SetTexture(ShaderIDs.Lut2D, (Texture) internalLdrLut);
      uberSheet.properties.SetFloat(ShaderIDs.PostExposure, RuntimeUtilities.Exp2(this.settings.postExposure.value));
    }

    private void RenderLDRPipeline2D(PostProcessRenderContext context)
    {
      this.CheckInternalStripLut();
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.lut2DBaker);
      propertySheet.ClearKeywords();
      propertySheet.properties.SetVector(ShaderIDs.Lut2D_Params, new Vector4(32f, 0.0004882813f, 1f / 64f, 1.032258f));
      Vector3 colorBalance = ColorUtilities.ComputeColorBalance(this.settings.temperature.value, this.settings.tint.value);
      propertySheet.properties.SetVector(ShaderIDs.ColorBalance, (Vector4) colorBalance);
      propertySheet.properties.SetVector(ShaderIDs.ColorFilter, (Vector4) this.settings.colorFilter.value);
      float x = this.settings.hueShift.value / 360f;
      float y = (float) ((double) this.settings.saturation.value / 100.0 + 1.0);
      float z = (float) ((double) this.settings.contrast.value / 100.0 + 1.0);
      propertySheet.properties.SetVector(ShaderIDs.HueSatCon, (Vector4) new Vector3(x, y, z));
      Vector3 vector3_1 = new Vector3((float) (ParameterOverride<float>) this.settings.mixerRedOutRedIn, (float) (ParameterOverride<float>) this.settings.mixerRedOutGreenIn, (float) (ParameterOverride<float>) this.settings.mixerRedOutBlueIn);
      Vector3 vector3_2 = new Vector3((float) (ParameterOverride<float>) this.settings.mixerGreenOutRedIn, (float) (ParameterOverride<float>) this.settings.mixerGreenOutGreenIn, (float) (ParameterOverride<float>) this.settings.mixerGreenOutBlueIn);
      Vector3 vector3_3 = new Vector3((float) (ParameterOverride<float>) this.settings.mixerBlueOutRedIn, (float) (ParameterOverride<float>) this.settings.mixerBlueOutGreenIn, (float) (ParameterOverride<float>) this.settings.mixerBlueOutBlueIn);
      propertySheet.properties.SetVector(ShaderIDs.ChannelMixerRed, (Vector4) (vector3_1 / 100f));
      propertySheet.properties.SetVector(ShaderIDs.ChannelMixerGreen, (Vector4) (vector3_2 / 100f));
      propertySheet.properties.SetVector(ShaderIDs.ChannelMixerBlue, (Vector4) (vector3_3 / 100f));
      Vector3 lift = ColorUtilities.ColorToLift(this.settings.lift.value);
      Vector3 gain = ColorUtilities.ColorToGain(this.settings.gain.value);
      Vector3 inverseGamma = ColorUtilities.ColorToInverseGamma(this.settings.gamma.value);
      propertySheet.properties.SetVector(ShaderIDs.Lift, (Vector4) lift);
      propertySheet.properties.SetVector(ShaderIDs.InvGamma, (Vector4) inverseGamma);
      propertySheet.properties.SetVector(ShaderIDs.Gain, (Vector4) gain);
      propertySheet.properties.SetFloat(ShaderIDs.Brightness, (float) (((double) this.settings.brightness.value + 100.0) / 100.0));
      propertySheet.properties.SetTexture(ShaderIDs.Curves, (Texture) this.GetCurveTexture(false));
      context.command.BeginSample("LdrColorGradingLut2D");
      Texture texture = this.settings.ldrLut.value;
      if ((Object) texture == (Object) null)
        context.command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) (Texture) this.m_InternalLdrLut, propertySheet, 0);
      else
        context.command.BlitFullscreenTriangle((RenderTargetIdentifier) texture, (RenderTargetIdentifier) (Texture) this.m_InternalLdrLut, propertySheet, 1);
      context.command.EndSample("LdrColorGradingLut2D");
      RenderTexture internalLdrLut = this.m_InternalLdrLut;
      PropertySheet uberSheet = context.uberSheet;
      uberSheet.EnableKeyword("COLOR_GRADING_LDR_2D");
      uberSheet.properties.SetVector(ShaderIDs.Lut2D_Params, (Vector4) new Vector3(1f / (float) internalLdrLut.width, 1f / (float) internalLdrLut.height, (float) internalLdrLut.height - 1f));
      uberSheet.properties.SetTexture(ShaderIDs.Lut2D, (Texture) internalLdrLut);
    }

    private void CheckInternalLogLut()
    {
      if (!((Object) this.m_InternalLogLut == (Object) null) && this.m_InternalLogLut.IsCreated())
        return;
      RuntimeUtilities.Destroy((Object) this.m_InternalLogLut);
      RenderTexture renderTexture = new RenderTexture(33, 33, 0, ColorGradingRenderer.GetLutFormat(), RenderTextureReadWrite.Linear);
      renderTexture.name = "Color Grading Log Lut";
      renderTexture.hideFlags = HideFlags.DontSave;
      renderTexture.filterMode = FilterMode.Bilinear;
      renderTexture.wrapMode = TextureWrapMode.Clamp;
      renderTexture.anisoLevel = 0;
      renderTexture.enableRandomWrite = true;
      renderTexture.volumeDepth = 33;
      renderTexture.dimension = TextureDimension.Tex3D;
      renderTexture.autoGenerateMips = false;
      renderTexture.useMipMap = false;
      this.m_InternalLogLut = renderTexture;
      this.m_InternalLogLut.Create();
    }

    private void CheckInternalStripLut()
    {
      if (!((Object) this.m_InternalLdrLut == (Object) null) && this.m_InternalLdrLut.IsCreated())
        return;
      RuntimeUtilities.Destroy((Object) this.m_InternalLdrLut);
      RenderTexture renderTexture = new RenderTexture(1024, 32, 0, ColorGradingRenderer.GetLutFormat(), RenderTextureReadWrite.Linear);
      renderTexture.name = "Color Grading Strip Lut";
      renderTexture.hideFlags = HideFlags.DontSave;
      renderTexture.filterMode = FilterMode.Bilinear;
      renderTexture.wrapMode = TextureWrapMode.Clamp;
      renderTexture.anisoLevel = 0;
      renderTexture.autoGenerateMips = false;
      renderTexture.useMipMap = false;
      this.m_InternalLdrLut = renderTexture;
      this.m_InternalLdrLut.Create();
    }

    private Texture2D GetCurveTexture(bool hdr)
    {
      if ((Object) this.m_GradingCurves == (Object) null)
      {
        Texture2D texture2D = new Texture2D(128, 2, ColorGradingRenderer.GetCurveFormat(), false, true);
        texture2D.name = "Internal Curves Texture";
        texture2D.hideFlags = HideFlags.DontSave;
        texture2D.anisoLevel = 0;
        texture2D.wrapMode = TextureWrapMode.Clamp;
        texture2D.filterMode = FilterMode.Bilinear;
        this.m_GradingCurves = texture2D;
      }
      Spline spline1 = this.settings.hueVsHueCurve.value;
      Spline spline2 = this.settings.hueVsSatCurve.value;
      Spline spline3 = this.settings.satVsSatCurve.value;
      Spline spline4 = this.settings.lumVsSatCurve.value;
      Spline spline5 = this.settings.masterCurve.value;
      Spline spline6 = this.settings.redCurve.value;
      Spline spline7 = this.settings.greenCurve.value;
      Spline spline8 = this.settings.blueCurve.value;
      Color[] pixels = this.m_Pixels;
      for (int index = 0; index < 128; ++index)
      {
        float r1 = spline1.cachedData[index];
        float g1 = spline2.cachedData[index];
        float b1 = spline3.cachedData[index];
        float a1 = spline4.cachedData[index];
        pixels[index] = new Color(r1, g1, b1, a1);
        if (!hdr)
        {
          float a2 = spline5.cachedData[index];
          float r2 = spline6.cachedData[index];
          float g2 = spline7.cachedData[index];
          float b2 = spline8.cachedData[index];
          pixels[index + 128] = new Color(r2, g2, b2, a2);
        }
      }
      this.m_GradingCurves.SetPixels(pixels);
      this.m_GradingCurves.Apply(false, false);
      return this.m_GradingCurves;
    }

    private static RenderTextureFormat GetLutFormat()
    {
      RenderTextureFormat format = RenderTextureFormat.ARGBHalf;
      if (!SystemInfo.SupportsRenderTextureFormat(format))
      {
        format = RenderTextureFormat.ARGB2101010;
        if (!SystemInfo.SupportsRenderTextureFormat(format))
          format = RenderTextureFormat.ARGB32;
      }
      return format;
    }

    private static TextureFormat GetCurveFormat()
    {
      TextureFormat format = TextureFormat.RGBAHalf;
      if (!SystemInfo.SupportsTextureFormat(format))
        format = TextureFormat.ARGB32;
      return format;
    }

    public override void Release()
    {
      RuntimeUtilities.Destroy((Object) this.m_InternalLdrLut);
      this.m_InternalLdrLut = (RenderTexture) null;
      RuntimeUtilities.Destroy((Object) this.m_InternalLogLut);
      this.m_InternalLogLut = (RenderTexture) null;
      RuntimeUtilities.Destroy((Object) this.m_GradingCurves);
      this.m_GradingCurves = (Texture2D) null;
    }

    private enum Pass
    {
      LutGenLDRFromScratch,
      LutGenLDR,
      LutGenHDR2D,
    }
  }
}
