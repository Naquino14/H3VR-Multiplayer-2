using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	public sealed class VectorscopeMonitor : Monitor
	{
		public int size = 256;

		public float exposure = 0.12f;

		private ComputeBuffer m_Data;

		private int m_ThreadGroupSizeX;

		private int m_ThreadGroupSizeY;

		internal override void OnEnable()
		{
			m_ThreadGroupSizeX = 16;
			m_ThreadGroupSizeY = ((!RuntimeUtilities.isAndroidOpenGL) ? 16 : 8);
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
			CheckOutput(size, size);
			exposure = Mathf.Max(0f, exposure);
			int num = size * size;
			if (m_Data == null)
			{
				m_Data = new ComputeBuffer(num, 4);
			}
			else if (m_Data.count != num)
			{
				m_Data.Release();
				m_Data = new ComputeBuffer(num, 4);
			}
			ComputeShader vectorscope = context.resources.computeShaders.vectorscope;
			CommandBuffer command = context.command;
			command.BeginSample("Vectorscope");
			Vector4 val = new Vector4(context.width / 2, context.height / 2, size, RuntimeUtilities.isLinearColorSpace ? 1 : 0);
			int kernelIndex = vectorscope.FindKernel("KVectorscopeClear");
			command.SetComputeBufferParam(vectorscope, kernelIndex, "_VectorscopeBuffer", m_Data);
			command.SetComputeVectorParam(vectorscope, "_Params", val);
			command.DispatchCompute(vectorscope, kernelIndex, Mathf.CeilToInt((float)size / (float)m_ThreadGroupSizeX), Mathf.CeilToInt((float)size / (float)m_ThreadGroupSizeY), 1);
			kernelIndex = vectorscope.FindKernel("KVectorscopeGather");
			command.SetComputeBufferParam(vectorscope, kernelIndex, "_VectorscopeBuffer", m_Data);
			command.SetComputeTextureParam(vectorscope, kernelIndex, "_Source", ShaderIDs.HalfResFinalCopy);
			command.DispatchCompute(vectorscope, kernelIndex, Mathf.CeilToInt(val.x / (float)m_ThreadGroupSizeX), Mathf.CeilToInt(val.y / (float)m_ThreadGroupSizeY), 1);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.vectorscope);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(size, size, exposure, 0f));
			propertySheet.properties.SetBuffer(ShaderIDs.VectorscopeBuffer, m_Data);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0);
			command.EndSample("Vectorscope");
		}
	}
}
