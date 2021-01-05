// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.ColorGrading
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [PostProcess(typeof (ColorGradingRenderer), "Unity/Color Grading", true)]
  [Serializable]
  public sealed class ColorGrading : PostProcessEffectSettings
  {
    [DisplayName("Mode")]
    [Tooltip("Select a color grading mode that fits your dynamic range and workflow. Use HDR if your camera is set to render in HDR and your target platform supports it. Use LDR for low-end mobiles or devices that don't support HDR. Use External if you prefer authoring a Log LUT in external softwares.")]
    public GradingModeParameter gradingMode;
    [DisplayName("Lookup Texture")]
    [Tooltip("")]
    public TextureParameter externalLut;
    [DisplayName("Mode")]
    [Tooltip("Select a tonemapping algorithm to use at the end of the color grading process.")]
    public TonemapperParameter tonemapper;
    [DisplayName("Toe Strength")]
    [Range(0.0f, 1f)]
    [Tooltip("Affects the transition between the toe and the mid section of the curve. A value of 0 means no toe, a value of 1 means a very hard transition.")]
    public FloatParameter toneCurveToeStrength;
    [DisplayName("Toe Length")]
    [Range(0.0f, 1f)]
    [Tooltip("Affects how much of the dynamic range is in the toe. With a small value, the toe will be very short and quickly transition into the linear section, and with a longer value having a longer toe.")]
    public FloatParameter toneCurveToeLength;
    [DisplayName("Shoulder Strength")]
    [Range(0.0f, 1f)]
    [Tooltip("Affects the transition between the mid section and the shoulder of the curve. A value of 0 means no shoulder, a value of 1 means a very hard transition.")]
    public FloatParameter toneCurveShoulderStrength;
    [DisplayName("Shoulder Length")]
    [Min(0.0f)]
    [Tooltip("Affects how many F-stops (EV) to add to the dynamic range of the curve.")]
    public FloatParameter toneCurveShoulderLength;
    [DisplayName("Shoulder Angle")]
    [Range(0.0f, 1f)]
    [Tooltip("Affects how much overshoot to add to the shoulder.")]
    public FloatParameter toneCurveShoulderAngle;
    [DisplayName("Gamma")]
    [Min(0.001f)]
    [Tooltip("")]
    public FloatParameter toneCurveGamma;
    [DisplayName("Lookup Texture")]
    [Tooltip("Custom log-space lookup texture (strip format, e.g. 1024x32). EXR format is highly recommended or precision will be heavily degraded. Refer to the documentation for more information about how to create such a Lut.")]
    public TextureParameter logLut;
    [DisplayName("Lookup Texture")]
    [Tooltip("Custom lookup texture (strip format, e.g. 256x16) to apply before the rest of the color grading operators. If none is provided, a neutral one will be generated internally.")]
    public TextureParameter ldrLut;
    [DisplayName("Temperature")]
    [Range(-100f, 100f)]
    [Tooltip("Sets the white balance to a custom color temperature.")]
    public FloatParameter temperature;
    [DisplayName("Tint")]
    [Range(-100f, 100f)]
    [Tooltip("Sets the white balance to compensate for a green or magenta tint.")]
    public FloatParameter tint;
    [DisplayName("Color Filter")]
    [ColorUsage(false, true, 0.0f, 8f, 0.125f, 3f)]
    [Tooltip("Tint the render by multiplying a color.")]
    public ColorParameter colorFilter;
    [DisplayName("Hue Shift")]
    [Range(-180f, 180f)]
    [Tooltip("Shift the hue of all colors.")]
    public FloatParameter hueShift;
    [DisplayName("Saturation")]
    [Range(-100f, 100f)]
    [Tooltip("Pushes the intensity of all colors.")]
    public FloatParameter saturation;
    [DisplayName("Brightness")]
    [Range(-100f, 100f)]
    [Tooltip("Makes the image brighter or darker.")]
    public FloatParameter brightness;
    [DisplayName("Post-exposure (EV)")]
    [Tooltip("Adjusts the overall exposure of the scene in EV units. This is applied after HDR effect and right before tonemapping so it won't affect previous effects in the chain.")]
    public FloatParameter postExposure;
    [DisplayName("Contrast")]
    [Range(-100f, 100f)]
    [Tooltip("Expands or shrinks the overall range of tonal values.")]
    public FloatParameter contrast;
    [DisplayName("Red")]
    [Range(-200f, 200f)]
    [Tooltip("Modify influence of the red channel in the overall mix.")]
    public FloatParameter mixerRedOutRedIn;
    [DisplayName("Green")]
    [Range(-200f, 200f)]
    [Tooltip("Modify influence of the green channel in the overall mix.")]
    public FloatParameter mixerRedOutGreenIn;
    [DisplayName("Blue")]
    [Range(-200f, 200f)]
    [Tooltip("Modify influence of the blue channel in the overall mix.")]
    public FloatParameter mixerRedOutBlueIn;
    [DisplayName("Red")]
    [Range(-200f, 200f)]
    [Tooltip("Modify influence of the red channel in the overall mix.")]
    public FloatParameter mixerGreenOutRedIn;
    [DisplayName("Green")]
    [Range(-200f, 200f)]
    [Tooltip("Modify influence of the green channel in the overall mix.")]
    public FloatParameter mixerGreenOutGreenIn;
    [DisplayName("Blue")]
    [Range(-200f, 200f)]
    [Tooltip("Modify influence of the blue channel in the overall mix.")]
    public FloatParameter mixerGreenOutBlueIn;
    [DisplayName("Red")]
    [Range(-200f, 200f)]
    [Tooltip("Modify influence of the red channel in the overall mix.")]
    public FloatParameter mixerBlueOutRedIn;
    [DisplayName("Green")]
    [Range(-200f, 200f)]
    [Tooltip("Modify influence of the green channel in the overall mix.")]
    public FloatParameter mixerBlueOutGreenIn;
    [DisplayName("Blue")]
    [Range(-200f, 200f)]
    [Tooltip("Modify influence of the blue channel in the overall mix.")]
    public FloatParameter mixerBlueOutBlueIn;
    [DisplayName("Lift")]
    [Tooltip("Controls the darkest portions of the render.")]
    [Trackball(TrackballAttribute.Mode.Lift)]
    public Vector4Parameter lift;
    [DisplayName("Gamma")]
    [Tooltip("Power function that controls midrange tones.")]
    [Trackball(TrackballAttribute.Mode.Gamma)]
    public Vector4Parameter gamma;
    [DisplayName("Gain")]
    [Tooltip("Controls the lightest portions of the render.")]
    [Trackball(TrackballAttribute.Mode.Gain)]
    public Vector4Parameter gain;
    public SplineParameter masterCurve;
    public SplineParameter redCurve;
    public SplineParameter greenCurve;
    public SplineParameter blueCurve;
    public SplineParameter hueVsHueCurve;
    public SplineParameter hueVsSatCurve;
    public SplineParameter satVsSatCurve;
    public SplineParameter lumVsSatCurve;

    public ColorGrading()
    {
      GradingModeParameter gradingModeParameter = new GradingModeParameter();
      gradingModeParameter.value = GradingMode.HighDefinitionRange;
      this.gradingMode = gradingModeParameter;
      TextureParameter textureParameter1 = new TextureParameter();
      textureParameter1.value = (Texture) null;
      this.externalLut = textureParameter1;
      TonemapperParameter tonemapperParameter = new TonemapperParameter();
      tonemapperParameter.value = Tonemapper.None;
      this.tonemapper = tonemapperParameter;
      FloatParameter floatParameter1 = new FloatParameter();
      floatParameter1.value = 0.0f;
      this.toneCurveToeStrength = floatParameter1;
      FloatParameter floatParameter2 = new FloatParameter();
      floatParameter2.value = 0.5f;
      this.toneCurveToeLength = floatParameter2;
      FloatParameter floatParameter3 = new FloatParameter();
      floatParameter3.value = 0.0f;
      this.toneCurveShoulderStrength = floatParameter3;
      FloatParameter floatParameter4 = new FloatParameter();
      floatParameter4.value = 0.5f;
      this.toneCurveShoulderLength = floatParameter4;
      FloatParameter floatParameter5 = new FloatParameter();
      floatParameter5.value = 0.0f;
      this.toneCurveShoulderAngle = floatParameter5;
      FloatParameter floatParameter6 = new FloatParameter();
      floatParameter6.value = 1f;
      this.toneCurveGamma = floatParameter6;
      TextureParameter textureParameter2 = new TextureParameter();
      textureParameter2.value = (Texture) null;
      this.logLut = textureParameter2;
      TextureParameter textureParameter3 = new TextureParameter();
      textureParameter3.value = (Texture) null;
      this.ldrLut = textureParameter3;
      FloatParameter floatParameter7 = new FloatParameter();
      floatParameter7.value = 0.0f;
      this.temperature = floatParameter7;
      FloatParameter floatParameter8 = new FloatParameter();
      floatParameter8.value = 0.0f;
      this.tint = floatParameter8;
      ColorParameter colorParameter = new ColorParameter();
      colorParameter.value = Color.white;
      this.colorFilter = colorParameter;
      FloatParameter floatParameter9 = new FloatParameter();
      floatParameter9.value = 0.0f;
      this.hueShift = floatParameter9;
      FloatParameter floatParameter10 = new FloatParameter();
      floatParameter10.value = 0.0f;
      this.saturation = floatParameter10;
      FloatParameter floatParameter11 = new FloatParameter();
      floatParameter11.value = 0.0f;
      this.brightness = floatParameter11;
      FloatParameter floatParameter12 = new FloatParameter();
      floatParameter12.value = 0.0f;
      this.postExposure = floatParameter12;
      FloatParameter floatParameter13 = new FloatParameter();
      floatParameter13.value = 0.0f;
      this.contrast = floatParameter13;
      FloatParameter floatParameter14 = new FloatParameter();
      floatParameter14.value = 100f;
      this.mixerRedOutRedIn = floatParameter14;
      FloatParameter floatParameter15 = new FloatParameter();
      floatParameter15.value = 0.0f;
      this.mixerRedOutGreenIn = floatParameter15;
      FloatParameter floatParameter16 = new FloatParameter();
      floatParameter16.value = 0.0f;
      this.mixerRedOutBlueIn = floatParameter16;
      FloatParameter floatParameter17 = new FloatParameter();
      floatParameter17.value = 0.0f;
      this.mixerGreenOutRedIn = floatParameter17;
      FloatParameter floatParameter18 = new FloatParameter();
      floatParameter18.value = 100f;
      this.mixerGreenOutGreenIn = floatParameter18;
      FloatParameter floatParameter19 = new FloatParameter();
      floatParameter19.value = 0.0f;
      this.mixerGreenOutBlueIn = floatParameter19;
      FloatParameter floatParameter20 = new FloatParameter();
      floatParameter20.value = 0.0f;
      this.mixerBlueOutRedIn = floatParameter20;
      FloatParameter floatParameter21 = new FloatParameter();
      floatParameter21.value = 0.0f;
      this.mixerBlueOutGreenIn = floatParameter21;
      FloatParameter floatParameter22 = new FloatParameter();
      floatParameter22.value = 100f;
      this.mixerBlueOutBlueIn = floatParameter22;
      Vector4Parameter vector4Parameter1 = new Vector4Parameter();
      vector4Parameter1.value = new Vector4(1f, 1f, 1f, 0.0f);
      this.lift = vector4Parameter1;
      Vector4Parameter vector4Parameter2 = new Vector4Parameter();
      vector4Parameter2.value = new Vector4(1f, 1f, 1f, 0.0f);
      this.gamma = vector4Parameter2;
      Vector4Parameter vector4Parameter3 = new Vector4Parameter();
      vector4Parameter3.value = new Vector4(1f, 1f, 1f, 0.0f);
      this.gain = vector4Parameter3;
      SplineParameter splineParameter1 = new SplineParameter();
      splineParameter1.value = new Spline(new AnimationCurve(new Keyframe[2]
      {
        new Keyframe(0.0f, 0.0f, 1f, 1f),
        new Keyframe(1f, 1f, 1f, 1f)
      }), 0.0f, false, new Vector2(0.0f, 1f));
      this.masterCurve = splineParameter1;
      SplineParameter splineParameter2 = new SplineParameter();
      splineParameter2.value = new Spline(new AnimationCurve(new Keyframe[2]
      {
        new Keyframe(0.0f, 0.0f, 1f, 1f),
        new Keyframe(1f, 1f, 1f, 1f)
      }), 0.0f, false, new Vector2(0.0f, 1f));
      this.redCurve = splineParameter2;
      SplineParameter splineParameter3 = new SplineParameter();
      splineParameter3.value = new Spline(new AnimationCurve(new Keyframe[2]
      {
        new Keyframe(0.0f, 0.0f, 1f, 1f),
        new Keyframe(1f, 1f, 1f, 1f)
      }), 0.0f, false, new Vector2(0.0f, 1f));
      this.greenCurve = splineParameter3;
      SplineParameter splineParameter4 = new SplineParameter();
      splineParameter4.value = new Spline(new AnimationCurve(new Keyframe[2]
      {
        new Keyframe(0.0f, 0.0f, 1f, 1f),
        new Keyframe(1f, 1f, 1f, 1f)
      }), 0.0f, false, new Vector2(0.0f, 1f));
      this.blueCurve = splineParameter4;
      SplineParameter splineParameter5 = new SplineParameter();
      splineParameter5.value = new Spline(new AnimationCurve(), 0.5f, true, new Vector2(0.0f, 1f));
      this.hueVsHueCurve = splineParameter5;
      SplineParameter splineParameter6 = new SplineParameter();
      splineParameter6.value = new Spline(new AnimationCurve(), 0.5f, true, new Vector2(0.0f, 1f));
      this.hueVsSatCurve = splineParameter6;
      SplineParameter splineParameter7 = new SplineParameter();
      splineParameter7.value = new Spline(new AnimationCurve(), 0.5f, false, new Vector2(0.0f, 1f));
      this.satVsSatCurve = splineParameter7;
      SplineParameter splineParameter8 = new SplineParameter();
      splineParameter8.value = new Spline(new AnimationCurve(), 0.5f, false, new Vector2(0.0f, 1f));
      this.lumVsSatCurve = splineParameter8;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public override bool IsEnabledAndSupported(PostProcessRenderContext context) => (this.gradingMode.value != GradingMode.External || SystemInfo.supports3DRenderTextures && SystemInfo.supportsComputeShaders) && this.enabled.value;
  }
}
