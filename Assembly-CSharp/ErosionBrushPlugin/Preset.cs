// Decompiled with JetBrains decompiler
// Type: ErosionBrushPlugin.Preset
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace ErosionBrushPlugin
{
  [Serializable]
  public class Preset
  {
    public float brushSize = 50f;
    public float brushFallof = 0.6f;
    public float brushSpacing = 0.15f;
    public int downscale = 1;
    public float blur = 0.1f;
    public bool preserveDetail;
    public bool isErosion;
    public int noise_seed = 12345;
    public float noise_amount = 20f;
    public float noise_size = 200f;
    public float noise_detail = 0.55f;
    public float noise_uplift = 0.8f;
    public float noise_ruffle = 1f;
    public int erosion_iterations = 3;
    public float erosion_durability = 0.9f;
    public int erosion_fluidityIterations = 3;
    public float erosion_amount = 1f;
    public float sediment_amount = 0.8f;
    public float wind_amount = 0.75f;
    public float erosion_smooth = 0.15f;
    public float ruffle = 0.1f;
    public Preset.SplatPreset foreground = new Preset.SplatPreset()
    {
      opacity = 1f
    };
    public Preset.SplatPreset background = new Preset.SplatPreset()
    {
      opacity = 1f
    };
    public string name;
    public bool saveBrushSize;
    public bool saveBrushParams;
    public bool saveErosionNoiseParams;
    public bool saveSplatParams;

    public bool isNoise
    {
      get => !this.isErosion;
      set => this.isErosion = !value;
    }

    public bool paintSplat
    {
      get
      {
        if (this.foreground.apply && (double) this.foreground.opacity > 0.00999999977648258)
          return true;
        return this.background.apply && (double) this.background.opacity > 0.00999999977648258;
      }
    }

    public Preset Copy() => (Preset) this.MemberwiseClone();

    [Serializable]
    public struct SplatPreset
    {
      public bool apply;
      public float opacity;
      public int num;
    }
  }
}
