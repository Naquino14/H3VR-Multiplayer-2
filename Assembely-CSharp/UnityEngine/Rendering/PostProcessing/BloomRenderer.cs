namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class BloomRenderer : PostProcessEffectRenderer<Bloom>
	{
		private enum Pass
		{
			Prefilter13,
			Prefilter4,
			Downsample13,
			Downsample4,
			UpsampleTent,
			UpsampleBox,
			DebugOverlayThreshold,
			DebugOverlayTent,
			DebugOverlayBox
		}

		private struct Level
		{
			internal int down;

			internal int up;
		}

		private Level[] m_Pyramid;

		private const int k_MaxPyramidSize = 16;

		public override void Init()
		{
			m_Pyramid = new Level[16];
			for (int i = 0; i < 16; i++)
			{
				ref Level reference = ref m_Pyramid[i];
				reference = new Level
				{
					down = Shader.PropertyToID("_BloomMipDown" + i),
					up = Shader.PropertyToID("_BloomMipUp" + i)
				};
			}
		}

		public override void Render(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("BloomPyramid");
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.bloom);
			propertySheet.properties.SetTexture(ShaderIDs.AutoExposureTex, context.autoExposureTexture);
			float num = Mathf.Clamp(base.settings.anamorphicRatio, -1f, 1f);
			float num2 = ((!(num < 0f)) ? 0f : (0f - num));
			float num3 = ((!(num > 0f)) ? 0f : num);
			int num4 = Mathf.FloorToInt((float)context.width / (2f - num2));
			int num5 = Mathf.FloorToInt((float)context.height / (2f - num3));
			int a = num4 / ((!RuntimeUtilities.isSinglePassStereoEnabled) ? 1 : 2);
			int num6 = Mathf.Max(a, num5);
			float num7 = Mathf.Log(num6, 2f) + Mathf.Min(base.settings.diffusion.value, 10f) - 10f;
			int num8 = Mathf.FloorToInt(num7);
			int num9 = Mathf.Clamp(num8, 1, 16);
			float num10 = 0.5f + num7 - (float)num8;
			propertySheet.properties.SetFloat(ShaderIDs.SampleScale, num10);
			float num11 = Mathf.GammaToLinearSpace(base.settings.threshold.value);
			float num12 = num11 * base.settings.softKnee.value + 1E-05f;
			Vector4 value = new Vector4(num11, num11 - num12, num12 * 2f, 0.25f / num12);
			propertySheet.properties.SetVector(ShaderIDs.Threshold, value);
			int num13 = (base.settings.mobileOptimized ? 1 : 0);
			RenderTargetIdentifier source = context.source;
			for (int i = 0; i < num9; i++)
			{
				int down = m_Pyramid[i].down;
				int up = m_Pyramid[i].up;
				int pass = ((i != 0) ? (2 + num13) : num13);
				command.GetTemporaryRT(down, num4, num5, 0, FilterMode.Bilinear, context.sourceFormat);
				command.GetTemporaryRT(up, num4, num5, 0, FilterMode.Bilinear, context.sourceFormat);
				command.BlitFullscreenTriangle(source, down, propertySheet, pass);
				source = down;
				num4 = Mathf.Max(num4 / 2, 1);
				num5 = Mathf.Max(num5 / 2, 1);
			}
			source = m_Pyramid[num9 - 1].down;
			for (int num14 = num9 - 2; num14 >= 0; num14--)
			{
				int down2 = m_Pyramid[num14].down;
				int up2 = m_Pyramid[num14].up;
				command.SetGlobalTexture(ShaderIDs.BloomTex, down2);
				command.BlitFullscreenTriangle(source, up2, propertySheet, 4 + num13);
				source = up2;
			}
			Color linear = base.settings.color.value.linear;
			float num15 = RuntimeUtilities.Exp2(base.settings.intensity.value / 10f) - 1f;
			Vector4 value2 = new Vector4(num10, num15, base.settings.dirtIntensity.value, num9);
			if (context.IsDebugOverlayEnabled(DebugOverlay.BloomThreshold))
			{
				context.PushDebugOverlay(command, context.source, propertySheet, 6);
			}
			else if (context.IsDebugOverlayEnabled(DebugOverlay.BloomBuffer))
			{
				propertySheet.properties.SetVector(ShaderIDs.ColorIntensity, new Vector4(linear.r, linear.g, linear.b, num15));
				context.PushDebugOverlay(command, m_Pyramid[0].up, propertySheet, 7 + num13);
			}
			Texture texture = ((!(base.settings.dirtTexture.value == null)) ? base.settings.dirtTexture.value : RuntimeUtilities.blackTexture);
			float num16 = (float)texture.width / (float)texture.height;
			float num17 = (float)context.width / (float)context.height;
			Vector4 value3 = new Vector4(1f, 1f, 0f, 0f);
			if (num16 > num17)
			{
				value3.x = num17 / num16;
				value3.z = (1f - value3.x) * 0.5f;
			}
			else if (num17 > num16)
			{
				value3.y = num16 / num17;
				value3.w = (1f - value3.y) * 0.5f;
			}
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword("BLOOM");
			uberSheet.properties.SetVector(ShaderIDs.Bloom_DirtTileOffset, value3);
			uberSheet.properties.SetVector(ShaderIDs.Bloom_Settings, value2);
			uberSheet.properties.SetColor(ShaderIDs.Bloom_Color, linear);
			uberSheet.properties.SetTexture(ShaderIDs.Bloom_DirtTex, texture);
			command.SetGlobalTexture(ShaderIDs.BloomTex, m_Pyramid[0].up);
			for (int j = 0; j < num9; j++)
			{
				command.ReleaseTemporaryRT(m_Pyramid[j].down);
				command.ReleaseTemporaryRT(m_Pyramid[j].up);
			}
			command.EndSample("BloomPyramid");
		}
	}
}
