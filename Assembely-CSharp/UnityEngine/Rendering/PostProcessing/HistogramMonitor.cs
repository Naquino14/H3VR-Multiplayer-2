using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	public sealed class HistogramMonitor : Monitor
	{
		public enum Channel
		{
			Red,
			Green,
			Blue,
			Master
		}

		public int width = 512;

		public int height = 256;

		public Channel channel = Channel.Master;

		private ComputeBuffer m_Data;

		private int m_NumBins;

		private int m_ThreadGroupSizeX;

		private int m_ThreadGroupSizeY;

		internal override void OnEnable()
		{
			m_ThreadGroupSizeX = 16;
			if (RuntimeUtilities.isAndroidOpenGL)
			{
				m_NumBins = 128;
				m_ThreadGroupSizeY = 8;
			}
			else
			{
				m_NumBins = 256;
				m_ThreadGroupSizeY = 16;
			}
		}

		internal override void OnDisable()
		{
			base.OnDisable();
			if (m_Data != null)
			{
				m_Data.Release();
			}
			m_Data = null;
		}

		internal override bool NeedsHalfRes()
		{
			return true;
		}

		internal override void Render(PostProcessRenderContext context)
		{
			CheckOutput(width, height);
			if (m_Data == null)
			{
				m_Data = new ComputeBuffer(m_NumBins, 4);
			}
			ComputeShader gammaHistogram = context.resources.computeShaders.gammaHistogram;
			CommandBuffer command = context.command;
			command.BeginSample("GammaHistogram");
			int kernelIndex = gammaHistogram.FindKernel("KHistogramClear");
			command.SetComputeBufferParam(gammaHistogram, kernelIndex, "_HistogramBuffer", m_Data);
			command.DispatchCompute(gammaHistogram, kernelIndex, Mathf.CeilToInt((float)m_NumBins / (float)m_ThreadGroupSizeX), 1, 1);
			kernelIndex = gammaHistogram.FindKernel("KHistogramGather");
			Vector4 val = new Vector4(context.width / 2, context.height / 2, RuntimeUtilities.isLinearColorSpace ? 1 : 0, (float)channel);
			command.SetComputeVectorParam(gammaHistogram, "_Params", val);
			command.SetComputeTextureParam(gammaHistogram, kernelIndex, "_Source", ShaderIDs.HalfResFinalCopy);
			command.SetComputeBufferParam(gammaHistogram, kernelIndex, "_HistogramBuffer", m_Data);
			command.DispatchCompute(gammaHistogram, kernelIndex, Mathf.CeilToInt(val.x / (float)m_ThreadGroupSizeX), Mathf.CeilToInt(val.y / (float)m_ThreadGroupSizeY), 1);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.gammaHistogram);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(width, height, 0f, 0f));
			propertySheet.properties.SetBuffer(ShaderIDs.HistogramBuffer, m_Data);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0);
			command.EndSample("GammaHistogram");
		}
	}
}
