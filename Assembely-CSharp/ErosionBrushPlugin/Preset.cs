using System;

namespace ErosionBrushPlugin
{
	[Serializable]
	public class Preset
	{
		[Serializable]
		public struct SplatPreset
		{
			public bool apply;

			public float opacity;

			public int num;
		}

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

		public SplatPreset foreground = new SplatPreset
		{
			opacity = 1f
		};

		public SplatPreset background = new SplatPreset
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
			get
			{
				return !isErosion;
			}
			set
			{
				isErosion = !value;
			}
		}

		public bool paintSplat => (foreground.apply && foreground.opacity > 0.01f) || (background.apply && background.opacity > 0.01f);

		public Preset Copy()
		{
			return (Preset)MemberwiseClone();
		}
	}
}
