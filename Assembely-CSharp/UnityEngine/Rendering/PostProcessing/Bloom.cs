using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	[PostProcess(typeof(BloomRenderer), "Unity/Bloom", true)]
	public sealed class Bloom : PostProcessEffectSettings
	{
		[Min(0f)]
		[Tooltip("Strength of the bloom filter. Values higher than 1 will make bloom contribute more energy to the final render. Keep this under or equal to 1 if you want energy conservation.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		[Min(0f)]
		[Tooltip("Filters out pixels under this level of brightness. Value is in gamma-space.")]
		public FloatParameter threshold = new FloatParameter
		{
			value = 1f
		};

		[Range(0f, 1f)]
		[Tooltip("Makes transition between under/over-threshold gradual (0 = hard threshold, 1 = soft threshold).")]
		public FloatParameter softKnee = new FloatParameter
		{
			value = 0.5f
		};

		[Range(1f, 10f)]
		[Tooltip("Changes the extent of veiling effects. For maximum quality stick to integer values. Because this value changes the internal iteration count, animating it isn't recommended as it may introduce small hiccups in the perceived radius.")]
		public FloatParameter diffusion = new FloatParameter
		{
			value = 7f
		};

		[Range(-1f, 1f)]
		[Tooltip("Distorts the bloom to give an anamorphic look. Negative values distort vertically, positive values distort horizontally.")]
		public FloatParameter anamorphicRatio = new FloatParameter
		{
			value = 0f
		};

		[ColorUsage(false, true, 0f, 8f, 0.125f, 3f)]
		[Tooltip("Global tint of the bloom filter.")]
		public ColorParameter color = new ColorParameter
		{
			value = Color.white
		};

		[Tooltip("Boost performances by lowering the effect quality. This settings is meant to be used on mobile and other low-end platforms.")]
		public BoolParameter mobileOptimized = new BoolParameter
		{
			value = false
		};

		[Tooltip("Dirtiness texture to add smudges or dust to the bloom effect.")]
		[DisplayName("Texture")]
		public TextureParameter dirtTexture = new TextureParameter
		{
			value = null
		};

		[Min(0f)]
		[Tooltip("Amount of dirtiness.")]
		[DisplayName("Intensity")]
		public FloatParameter dirtIntensity = new FloatParameter
		{
			value = 0f
		};

		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return enabled.value && intensity.value > 0f;
		}
	}
}
