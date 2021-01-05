using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	[PostProcess(typeof(ChromaticAberrationRenderer), "Unity/Chromatic Aberration", true)]
	public sealed class ChromaticAberration : PostProcessEffectSettings
	{
		[Tooltip("Shift the hue of chromatic aberrations.")]
		public TextureParameter spectralLut = new TextureParameter
		{
			value = null
		};

		[Range(0f, 1f)]
		[Tooltip("Amount of tangential distortion.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		[Tooltip("Boost performances by lowering the effect quality. This settings is meant to be used on mobile and other low-end platforms.")]
		public BoolParameter mobileOptimized = new BoolParameter
		{
			value = false
		};

		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return enabled.value && intensity.value > 0f;
		}
	}
}
