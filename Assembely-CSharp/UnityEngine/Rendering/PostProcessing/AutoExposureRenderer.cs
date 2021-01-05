namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class AutoExposureRenderer : PostProcessEffectRenderer<AutoExposure>
	{
		private readonly RenderTexture[] m_AutoExposurePool = new RenderTexture[2];

		private int m_AutoExposurePingPong;

		private RenderTexture m_CurrentAutoExposure;

		private void CheckTexture(int id)
		{
			if (m_AutoExposurePool[id] == null || !m_AutoExposurePool[id].IsCreated())
			{
				m_AutoExposurePool[id] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
				m_AutoExposurePool[id].Create();
			}
		}

		public override void Render(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("AutoExposureLookup");
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.autoExposure);
			propertySheet.ClearKeywords();
			CheckTexture(0);
			CheckTexture(1);
			float x = base.settings.filtering.value.x;
			float y = base.settings.filtering.value.y;
			y = Mathf.Clamp(y, 1.01f, 99f);
			x = Mathf.Clamp(x, 1f, y - 0.01f);
			float value = base.settings.minLuminance.value;
			float value2 = base.settings.maxLuminance.value;
			base.settings.minLuminance.value = Mathf.Min(value, value2);
			base.settings.maxLuminance.value = Mathf.Max(value, value2);
			propertySheet.properties.SetBuffer(ShaderIDs.HistogramBuffer, context.logHistogram.data);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(x * 0.01f, y * 0.01f, RuntimeUtilities.Exp2(base.settings.minLuminance.value), RuntimeUtilities.Exp2(base.settings.maxLuminance.value)));
			propertySheet.properties.SetVector(ShaderIDs.Speed, new Vector2(base.settings.speedDown.value, base.settings.speedUp.value));
			propertySheet.properties.SetVector(ShaderIDs.ScaleOffsetRes, context.logHistogram.GetHistogramScaleOffsetRes(context));
			propertySheet.properties.SetFloat(ShaderIDs.ExposureCompensation, base.settings.keyValue.value);
			if (m_ResetHistory || !Application.isPlaying)
			{
				m_CurrentAutoExposure = m_AutoExposurePool[0];
				command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, m_CurrentAutoExposure, propertySheet, 1);
				RuntimeUtilities.CopyTexture(command, m_AutoExposurePool[0], m_AutoExposurePool[1]);
				m_ResetHistory = false;
			}
			else
			{
				int autoExposurePingPong = m_AutoExposurePingPong;
				RenderTexture renderTexture = m_AutoExposurePool[++autoExposurePingPong % 2];
				RenderTexture renderTexture2 = m_AutoExposurePool[++autoExposurePingPong % 2];
				command.BlitFullscreenTriangle(renderTexture, renderTexture2, propertySheet, (int)base.settings.eyeAdaptation.value);
				m_AutoExposurePingPong = ++autoExposurePingPong % 2;
				m_CurrentAutoExposure = renderTexture2;
			}
			command.EndSample("AutoExposureLookup");
			context.autoExposureTexture = m_CurrentAutoExposure;
			context.autoExposure = base.settings;
		}

		public override void Release()
		{
			RenderTexture[] autoExposurePool = m_AutoExposurePool;
			foreach (RenderTexture obj in autoExposurePool)
			{
				RuntimeUtilities.Destroy(obj);
			}
		}
	}
}
