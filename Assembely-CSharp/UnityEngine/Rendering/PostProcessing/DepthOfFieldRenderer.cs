namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class DepthOfFieldRenderer : PostProcessEffectRenderer<DepthOfField>
	{
		private enum Pass
		{
			CoCCalculation,
			CoCTemporalFilter,
			DownsampleAndPrefilter,
			BokehSmallKernel,
			BokehMediumKernel,
			BokehLargeKernel,
			BokehVeryLargeKernel,
			PostFilter,
			Combine,
			DebugOverlay
		}

		private readonly RenderTexture[] m_CoCHistoryTextures = new RenderTexture[2];

		private int m_HistoryPingPong;

		private const float k_FilmHeight = 0.024f;

		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		private RenderTextureFormat SelectFormat(RenderTextureFormat primary, RenderTextureFormat secondary)
		{
			if (SystemInfo.SupportsRenderTextureFormat(primary))
			{
				return primary;
			}
			if (SystemInfo.SupportsRenderTextureFormat(secondary))
			{
				return secondary;
			}
			return RenderTextureFormat.Default;
		}

		private float CalculateMaxCoCRadius(int screenHeight)
		{
			float num = (float)base.settings.kernelSize.value * 4f + 6f;
			return Mathf.Min(0.05f, num / (float)screenHeight);
		}

		private RenderTexture CheckHistory(int id, int width, int height, RenderTextureFormat format)
		{
			RenderTexture renderTexture = m_CoCHistoryTextures[id];
			if (m_ResetHistory || renderTexture == null || !renderTexture.IsCreated() || renderTexture.width != width || renderTexture.height != height)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = RenderTexture.GetTemporary(width, height, 0, format);
				renderTexture.name = "CoC History";
				renderTexture.filterMode = FilterMode.Bilinear;
				renderTexture.Create();
				m_CoCHistoryTextures[id] = renderTexture;
			}
			return renderTexture;
		}

		public override void Render(PostProcessRenderContext context)
		{
			RenderTextureFormat format = RenderTextureFormat.DefaultHDR;
			RenderTextureFormat format2 = SelectFormat(RenderTextureFormat.R8, RenderTextureFormat.RHalf);
			float num = base.settings.focalLength.value / 1000f;
			float num2 = Mathf.Max(base.settings.focusDistance.value, num);
			float num3 = (float)context.width / (float)context.height;
			float value = num * num / (base.settings.aperture.value * (num2 - num) * 0.024f * 2f);
			float num4 = CalculateMaxCoCRadius(context.height);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.depthOfField);
			propertySheet.properties.Clear();
			propertySheet.properties.SetFloat(ShaderIDs.Distance, num2);
			propertySheet.properties.SetFloat(ShaderIDs.LensCoeff, value);
			propertySheet.properties.SetFloat(ShaderIDs.MaxCoC, num4);
			propertySheet.properties.SetFloat(ShaderIDs.RcpMaxCoC, 1f / num4);
			propertySheet.properties.SetFloat(ShaderIDs.RcpAspect, 1f / num3);
			CommandBuffer command = context.command;
			command.BeginSample("DepthOfField");
			command.GetTemporaryRT(ShaderIDs.CoCTex, context.width, context.height, 0, FilterMode.Bilinear, format2, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, ShaderIDs.CoCTex, propertySheet, 0);
			if (context.IsTemporalAntialiasingActive())
			{
				float motionBlending = context.temporalAntialiasing.motionBlending;
				float z = ((!m_ResetHistory) ? motionBlending : 0f);
				Vector2 jitter = context.temporalAntialiasing.jitter;
				propertySheet.properties.SetVector(ShaderIDs.TaaParams, new Vector3(jitter.x, jitter.y, z));
				int historyPingPong = m_HistoryPingPong;
				RenderTexture renderTexture = CheckHistory(++historyPingPong % 2, context.width, context.height, format2);
				RenderTexture renderTexture2 = CheckHistory(++historyPingPong % 2, context.width, context.height, format2);
				m_HistoryPingPong = ++historyPingPong % 2;
				command.BlitFullscreenTriangle(renderTexture, renderTexture2, propertySheet, 1);
				command.ReleaseTemporaryRT(ShaderIDs.CoCTex);
				command.SetGlobalTexture(ShaderIDs.CoCTex, renderTexture2);
			}
			command.GetTemporaryRT(ShaderIDs.DepthOfFieldTex, context.width / 2, context.height / 2, 0, FilterMode.Bilinear, format);
			command.BlitFullscreenTriangle(context.source, ShaderIDs.DepthOfFieldTex, propertySheet, 2);
			command.GetTemporaryRT(ShaderIDs.DepthOfFieldTemp, context.width / 2, context.height / 2, 0, FilterMode.Bilinear, format);
			command.BlitFullscreenTriangle(ShaderIDs.DepthOfFieldTex, ShaderIDs.DepthOfFieldTemp, propertySheet, (int)(3 + base.settings.kernelSize.value));
			command.BlitFullscreenTriangle(ShaderIDs.DepthOfFieldTemp, ShaderIDs.DepthOfFieldTex, propertySheet, 7);
			command.ReleaseTemporaryRT(ShaderIDs.DepthOfFieldTemp);
			if (context.IsDebugOverlayEnabled(DebugOverlay.DepthOfField))
			{
				context.PushDebugOverlay(command, context.source, propertySheet, 9);
			}
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 8);
			command.ReleaseTemporaryRT(ShaderIDs.DepthOfFieldTex);
			if (!context.IsTemporalAntialiasingActive())
			{
				command.ReleaseTemporaryRT(ShaderIDs.CoCTex);
			}
			command.EndSample("DepthOfField");
			m_ResetHistory = false;
		}

		public override void Release()
		{
			for (int i = 0; i < m_CoCHistoryTextures.Length; i++)
			{
				RenderTexture.ReleaseTemporary(m_CoCHistoryTextures[i]);
				m_CoCHistoryTextures[i] = null;
			}
			m_HistoryPingPong = 0;
			ResetHistory();
		}
	}
}
