using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	public sealed class MultiScaleVO : IAmbientOcclusionMethod
	{
		internal enum MipLevel
		{
			Original,
			L1,
			L2,
			L3,
			L4,
			L5,
			L6
		}

		internal enum TextureType
		{
			Fixed,
			Half,
			Float,
			FixedUAV,
			HalfUAV,
			FloatUAV,
			FixedTiledUAV,
			HalfTiledUAV,
			FloatTiledUAV
		}

		private enum Pass
		{
			DepthCopy,
			CompositionDeferred,
			CompositionForward,
			DebugOverlay
		}

		internal class RTHandle
		{
			private static int s_BaseWidth;

			private static int s_BaseHeight;

			private RenderTexture m_RT;

			private TextureType m_Type;

			private MipLevel m_Level;

			public int nameID
			{
				get;
				private set;
			}

			public int width
			{
				get;
				private set;
			}

			public int height
			{
				get;
				private set;
			}

			public int depth => (!isTiled) ? 1 : 16;

			public bool isTiled => m_Type > TextureType.FloatUAV;

			public bool hasUAV => m_Type > TextureType.Float;

			public RenderTargetIdentifier id => (!(m_RT != null)) ? new RenderTargetIdentifier(nameID) : new RenderTargetIdentifier(m_RT);

			public Vector2 inverseDimensions => new Vector2(1f / (float)width, 1f / (float)height);

			private RenderTextureFormat renderTextureFormat => ((int)m_Type % 3) switch
			{
				0 => RenderTextureFormat.R8, 
				1 => RenderTextureFormat.RHalf, 
				_ => RenderTextureFormat.RFloat, 
			};

			public RTHandle(string name, TextureType type, MipLevel level)
			{
				nameID = Shader.PropertyToID(name);
				m_Type = type;
				m_Level = level;
			}

			public static void SetBaseDimensions(int w, int h)
			{
				s_BaseWidth = w;
				s_BaseHeight = h;
			}

			public void AllocateNow()
			{
				CalculateDimensions();
				bool flag = false;
				if (m_RT == null || !m_RT.IsCreated())
				{
					m_RT = new RenderTexture(width, height, 0, renderTextureFormat, RenderTextureReadWrite.Linear)
					{
						hideFlags = HideFlags.DontSave
					};
					flag = true;
				}
				else if (m_RT.width != width || m_RT.height != height)
				{
					m_RT.Release();
					m_RT.width = width;
					m_RT.height = height;
					m_RT.format = renderTextureFormat;
					flag = true;
				}
				if (flag)
				{
					m_RT.filterMode = FilterMode.Point;
					m_RT.enableRandomWrite = hasUAV;
					if (isTiled)
					{
						m_RT.dimension = TextureDimension.Tex2DArray;
						m_RT.volumeDepth = depth;
					}
					m_RT.Create();
				}
			}

			public void PushAllocationCommand(CommandBuffer cmd)
			{
				CalculateDimensions();
				cmd.GetTemporaryRT(nameID, width, height, 0, FilterMode.Point, renderTextureFormat, RenderTextureReadWrite.Linear, 1, hasUAV);
			}

			public void Destroy()
			{
				RuntimeUtilities.Destroy(m_RT);
				m_RT = null;
			}

			private void CalculateDimensions()
			{
				int num = 1 << (int)m_Level;
				width = (s_BaseWidth + (num - 1)) / num;
				height = (s_BaseHeight + (num - 1)) / num;
			}
		}

		private AmbientOcclusion m_Settings;

		private PropertySheet m_PropertySheet;

		private RTHandle m_DepthCopy;

		private RTHandle m_LinearDepth;

		private RTHandle m_LowDepth1;

		private RTHandle m_LowDepth2;

		private RTHandle m_LowDepth3;

		private RTHandle m_LowDepth4;

		private RTHandle m_TiledDepth1;

		private RTHandle m_TiledDepth2;

		private RTHandle m_TiledDepth3;

		private RTHandle m_TiledDepth4;

		private RTHandle m_Occlusion1;

		private RTHandle m_Occlusion2;

		private RTHandle m_Occlusion3;

		private RTHandle m_Occlusion4;

		private RTHandle m_Combined1;

		private RTHandle m_Combined2;

		private RTHandle m_Combined3;

		private RTHandle m_Result;

		private readonly float[] m_SampleThickness = new float[12]
		{
			Mathf.Sqrt(0.96f),
			Mathf.Sqrt(0.84f),
			Mathf.Sqrt(0.64f),
			Mathf.Sqrt(0.359999985f),
			Mathf.Sqrt(0.92f),
			Mathf.Sqrt(0.8f),
			Mathf.Sqrt(0.599999964f),
			Mathf.Sqrt(0.32f),
			Mathf.Sqrt(0.68f),
			Mathf.Sqrt(0.479999959f),
			Mathf.Sqrt(0.199999973f),
			Mathf.Sqrt(0.279999942f)
		};

		private readonly float[] m_InvThicknessTable = new float[12];

		private readonly float[] m_SampleWeightTable = new float[12];

		private readonly RenderTargetIdentifier[] m_MRT = new RenderTargetIdentifier[2]
		{
			BuiltinRenderTextureType.GBuffer0,
			BuiltinRenderTextureType.CameraTarget
		};

		public MultiScaleVO(AmbientOcclusion settings)
		{
			m_Settings = settings;
		}

		public DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		private void DoLazyInitialization(PostProcessRenderContext context)
		{
			Shader multiScaleAO = context.resources.shaders.multiScaleAO;
			m_PropertySheet = context.propertySheets.Get(multiScaleAO);
			m_PropertySheet.ClearKeywords();
			if (m_Result == null)
			{
				m_DepthCopy = new RTHandle("DepthCopy", TextureType.Float, MipLevel.Original);
				m_LinearDepth = new RTHandle("LinearDepth", TextureType.HalfUAV, MipLevel.Original);
				m_LowDepth1 = new RTHandle("LowDepth1", TextureType.FloatUAV, MipLevel.L1);
				m_LowDepth2 = new RTHandle("LowDepth2", TextureType.FloatUAV, MipLevel.L2);
				m_LowDepth3 = new RTHandle("LowDepth3", TextureType.FloatUAV, MipLevel.L3);
				m_LowDepth4 = new RTHandle("LowDepth4", TextureType.FloatUAV, MipLevel.L4);
				m_TiledDepth1 = new RTHandle("TiledDepth1", TextureType.HalfTiledUAV, MipLevel.L3);
				m_TiledDepth2 = new RTHandle("TiledDepth2", TextureType.HalfTiledUAV, MipLevel.L4);
				m_TiledDepth3 = new RTHandle("TiledDepth3", TextureType.HalfTiledUAV, MipLevel.L5);
				m_TiledDepth4 = new RTHandle("TiledDepth4", TextureType.HalfTiledUAV, MipLevel.L6);
				m_Occlusion1 = new RTHandle("Occlusion1", TextureType.FixedUAV, MipLevel.L1);
				m_Occlusion2 = new RTHandle("Occlusion2", TextureType.FixedUAV, MipLevel.L2);
				m_Occlusion3 = new RTHandle("Occlusion3", TextureType.FixedUAV, MipLevel.L3);
				m_Occlusion4 = new RTHandle("Occlusion4", TextureType.FixedUAV, MipLevel.L4);
				m_Combined1 = new RTHandle("Combined1", TextureType.FixedUAV, MipLevel.L1);
				m_Combined2 = new RTHandle("Combined2", TextureType.FixedUAV, MipLevel.L2);
				m_Combined3 = new RTHandle("Combined3", TextureType.FixedUAV, MipLevel.L3);
				m_Result = new RTHandle("AmbientOcclusion", TextureType.FixedUAV, MipLevel.Original);
			}
		}

		private void RebuildCommandBuffers(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			RTHandle.SetBaseDimensions(context.width, context.height);
			m_PropertySheet.properties.SetVector(ShaderIDs.AOColor, Color.white - m_Settings.color.value);
			m_TiledDepth1.AllocateNow();
			m_TiledDepth2.AllocateNow();
			m_TiledDepth3.AllocateNow();
			m_TiledDepth4.AllocateNow();
			m_Result.AllocateNow();
			PushDownsampleCommands(context, command);
			m_Occlusion1.PushAllocationCommand(command);
			m_Occlusion2.PushAllocationCommand(command);
			m_Occlusion3.PushAllocationCommand(command);
			m_Occlusion4.PushAllocationCommand(command);
			float tanHalfFovH = CalculateTanHalfFovHeight(context);
			PushRenderCommands(context, command, m_TiledDepth1, m_Occlusion1, tanHalfFovH);
			PushRenderCommands(context, command, m_TiledDepth2, m_Occlusion2, tanHalfFovH);
			PushRenderCommands(context, command, m_TiledDepth3, m_Occlusion3, tanHalfFovH);
			PushRenderCommands(context, command, m_TiledDepth4, m_Occlusion4, tanHalfFovH);
			m_Combined1.PushAllocationCommand(command);
			m_Combined2.PushAllocationCommand(command);
			m_Combined3.PushAllocationCommand(command);
			PushUpsampleCommands(context, command, m_LowDepth4, m_Occlusion4, m_LowDepth3, m_Occlusion3, m_Combined3);
			PushUpsampleCommands(context, command, m_LowDepth3, m_Combined3, m_LowDepth2, m_Occlusion2, m_Combined2);
			PushUpsampleCommands(context, command, m_LowDepth2, m_Combined2, m_LowDepth1, m_Occlusion1, m_Combined1);
			PushUpsampleCommands(context, command, m_LowDepth1, m_Combined1, m_LinearDepth, null, m_Result);
			if (context.IsDebugOverlayEnabled(DebugOverlay.AmbientOcclusion))
			{
				context.PushDebugOverlay(command, m_Result.id, m_PropertySheet, 3);
			}
		}

		private Vector4 CalculateZBufferParams(Camera camera)
		{
			float num = camera.farClipPlane / camera.nearClipPlane;
			if (SystemInfo.usesReversedZBuffer)
			{
				return new Vector4(num - 1f, 1f, 0f, 0f);
			}
			return new Vector4(1f - num, num, 0f, 0f);
		}

		private float CalculateTanHalfFovHeight(PostProcessRenderContext context)
		{
			return 1f / context.camera.projectionMatrix[0, 0];
		}

		private void PushDownsampleCommands(PostProcessRenderContext context, CommandBuffer cmd)
		{
			bool flag = !RuntimeUtilities.IsResolvedDepthAvailable(context.camera);
			if (flag)
			{
				m_DepthCopy.PushAllocationCommand(cmd);
				cmd.BlitFullscreenTriangle(BuiltinRenderTextureType.None, m_DepthCopy.id, m_PropertySheet, 0);
			}
			m_LinearDepth.PushAllocationCommand(cmd);
			m_LowDepth1.PushAllocationCommand(cmd);
			m_LowDepth2.PushAllocationCommand(cmd);
			m_LowDepth3.PushAllocationCommand(cmd);
			m_LowDepth4.PushAllocationCommand(cmd);
			ComputeShader multiScaleAODownsample = context.resources.computeShaders.multiScaleAODownsample1;
			int kernelIndex = multiScaleAODownsample.FindKernel("main");
			cmd.SetComputeTextureParam(multiScaleAODownsample, kernelIndex, "LinearZ", m_LinearDepth.id);
			cmd.SetComputeTextureParam(multiScaleAODownsample, kernelIndex, "DS2x", m_LowDepth1.id);
			cmd.SetComputeTextureParam(multiScaleAODownsample, kernelIndex, "DS4x", m_LowDepth2.id);
			cmd.SetComputeTextureParam(multiScaleAODownsample, kernelIndex, "DS2xAtlas", m_TiledDepth1.id);
			cmd.SetComputeTextureParam(multiScaleAODownsample, kernelIndex, "DS4xAtlas", m_TiledDepth2.id);
			cmd.SetComputeVectorParam(multiScaleAODownsample, "ZBufferParams", CalculateZBufferParams(context.camera));
			cmd.SetComputeTextureParam(multiScaleAODownsample, kernelIndex, "Depth", (!flag) ? ((RenderTargetIdentifier)BuiltinRenderTextureType.ResolvedDepth) : m_DepthCopy.id);
			cmd.DispatchCompute(multiScaleAODownsample, kernelIndex, m_TiledDepth2.width, m_TiledDepth2.height, 1);
			if (flag)
			{
				cmd.ReleaseTemporaryRT(m_DepthCopy.nameID);
			}
			multiScaleAODownsample = context.resources.computeShaders.multiScaleAODownsample2;
			kernelIndex = multiScaleAODownsample.FindKernel("main");
			cmd.SetComputeTextureParam(multiScaleAODownsample, kernelIndex, "DS4x", m_LowDepth2.id);
			cmd.SetComputeTextureParam(multiScaleAODownsample, kernelIndex, "DS8x", m_LowDepth3.id);
			cmd.SetComputeTextureParam(multiScaleAODownsample, kernelIndex, "DS16x", m_LowDepth4.id);
			cmd.SetComputeTextureParam(multiScaleAODownsample, kernelIndex, "DS8xAtlas", m_TiledDepth3.id);
			cmd.SetComputeTextureParam(multiScaleAODownsample, kernelIndex, "DS16xAtlas", m_TiledDepth4.id);
			cmd.DispatchCompute(multiScaleAODownsample, kernelIndex, m_TiledDepth4.width, m_TiledDepth4.height, 1);
		}

		private void PushRenderCommands(PostProcessRenderContext context, CommandBuffer cmd, RTHandle source, RTHandle dest, float tanHalfFovH)
		{
			float num = 2f * tanHalfFovH * 10f / (float)source.width;
			if (!source.isTiled)
			{
				num *= 2f;
			}
			if (RuntimeUtilities.isSinglePassStereoEnabled)
			{
				num *= 2f;
			}
			float num2 = 1f / num;
			for (int i = 0; i < 12; i++)
			{
				m_InvThicknessTable[i] = num2 / m_SampleThickness[i];
			}
			m_SampleWeightTable[0] = 4f * m_SampleThickness[0];
			m_SampleWeightTable[1] = 4f * m_SampleThickness[1];
			m_SampleWeightTable[2] = 4f * m_SampleThickness[2];
			m_SampleWeightTable[3] = 4f * m_SampleThickness[3];
			m_SampleWeightTable[4] = 4f * m_SampleThickness[4];
			m_SampleWeightTable[5] = 8f * m_SampleThickness[5];
			m_SampleWeightTable[6] = 8f * m_SampleThickness[6];
			m_SampleWeightTable[7] = 8f * m_SampleThickness[7];
			m_SampleWeightTable[8] = 4f * m_SampleThickness[8];
			m_SampleWeightTable[9] = 8f * m_SampleThickness[9];
			m_SampleWeightTable[10] = 8f * m_SampleThickness[10];
			m_SampleWeightTable[11] = 4f * m_SampleThickness[11];
			m_SampleWeightTable[0] = 0f;
			m_SampleWeightTable[2] = 0f;
			m_SampleWeightTable[5] = 0f;
			m_SampleWeightTable[7] = 0f;
			m_SampleWeightTable[9] = 0f;
			float num3 = 0f;
			float[] sampleWeightTable = m_SampleWeightTable;
			foreach (float num4 in sampleWeightTable)
			{
				num3 += num4;
			}
			for (int k = 0; k < m_SampleWeightTable.Length; k++)
			{
				m_SampleWeightTable[k] /= num3;
			}
			ComputeShader multiScaleAORender = context.resources.computeShaders.multiScaleAORender;
			int kernelIndex = multiScaleAORender.FindKernel("main_interleaved");
			cmd.SetComputeFloatParams(multiScaleAORender, "gInvThicknessTable", m_InvThicknessTable);
			cmd.SetComputeFloatParams(multiScaleAORender, "gSampleWeightTable", m_SampleWeightTable);
			cmd.SetComputeVectorParam(multiScaleAORender, "gInvSliceDimension", source.inverseDimensions);
			cmd.SetComputeVectorParam(multiScaleAORender, "AdditionalParams", new Vector2(-1f / m_Settings.thicknessModifier.value, m_Settings.intensity.value));
			cmd.SetComputeTextureParam(multiScaleAORender, kernelIndex, "DepthTex", source.id);
			cmd.SetComputeTextureParam(multiScaleAORender, kernelIndex, "Occlusion", dest.id);
			multiScaleAORender.GetKernelThreadGroupSizes(kernelIndex, out var x, out var y, out var z);
			cmd.DispatchCompute(multiScaleAORender, kernelIndex, (source.width + (int)x - 1) / (int)x, (source.height + (int)y - 1) / (int)y, (source.depth + (int)z - 1) / (int)z);
		}

		private void PushUpsampleCommands(PostProcessRenderContext context, CommandBuffer cmd, RTHandle lowResDepth, RTHandle interleavedAO, RTHandle highResDepth, RTHandle highResAO, RTHandle dest)
		{
			ComputeShader multiScaleAOUpsample = context.resources.computeShaders.multiScaleAOUpsample;
			int kernelIndex = multiScaleAOUpsample.FindKernel((highResAO != null) ? "main_blendout" : "main");
			float num = 1920f / (float)lowResDepth.width;
			float num2 = 1f - Mathf.Pow(10f, m_Settings.blurTolerance.value) * num;
			num2 *= num2;
			float num3 = Mathf.Pow(10f, m_Settings.upsampleTolerance.value);
			float x = 1f / (Mathf.Pow(10f, m_Settings.noiseFilterTolerance.value) + num3);
			cmd.SetComputeVectorParam(multiScaleAOUpsample, "InvLowResolution", lowResDepth.inverseDimensions);
			cmd.SetComputeVectorParam(multiScaleAOUpsample, "InvHighResolution", highResDepth.inverseDimensions);
			cmd.SetComputeVectorParam(multiScaleAOUpsample, "AdditionalParams", new Vector4(x, num, num2, num3));
			cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "LoResDB", lowResDepth.id);
			cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "HiResDB", highResDepth.id);
			cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "LoResAO1", interleavedAO.id);
			if (highResAO != null)
			{
				cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "HiResAO", highResAO.id);
			}
			cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "AoResult", dest.id);
			int threadGroupsX = (highResDepth.width + 17) / 16;
			int threadGroupsY = (highResDepth.height + 17) / 16;
			cmd.DispatchCompute(multiScaleAOUpsample, kernelIndex, threadGroupsX, threadGroupsY, 1);
		}

		public void RenderAfterOpaque(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion");
			DoLazyInitialization(context);
			PropertySheet propertySheet = m_PropertySheet;
			if (context.camera.actualRenderingPath == RenderingPath.Forward && RenderSettings.fog)
			{
				propertySheet.EnableKeyword("APPLY_FORWARD_FOG");
				propertySheet.properties.SetVector(ShaderIDs.FogParams, new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
			}
			RebuildCommandBuffers(context);
			command.SetGlobalTexture(ShaderIDs.MSVOcclusionTexture, m_Result.id);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, BuiltinRenderTextureType.CameraTarget, m_PropertySheet, 2);
			command.EndSample("Ambient Occlusion");
		}

		public void RenderAmbientOnly(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion Render");
			DoLazyInitialization(context);
			RebuildCommandBuffers(context);
			command.EndSample("Ambient Occlusion Render");
		}

		public void CompositeAmbientOnly(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion Composite");
			command.SetGlobalTexture(ShaderIDs.MSVOcclusionTexture, m_Result.id);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, m_MRT, BuiltinRenderTextureType.CameraTarget, m_PropertySheet, 1);
			command.EndSample("Ambient Occlusion Composite");
		}

		public void Release()
		{
			if (m_Result != null)
			{
				m_TiledDepth1.Destroy();
				m_TiledDepth2.Destroy();
				m_TiledDepth3.Destroy();
				m_TiledDepth4.Destroy();
				m_Result.Destroy();
			}
			m_TiledDepth1 = null;
			m_TiledDepth2 = null;
			m_TiledDepth3 = null;
			m_TiledDepth4 = null;
			m_Result = null;
		}
	}
}
