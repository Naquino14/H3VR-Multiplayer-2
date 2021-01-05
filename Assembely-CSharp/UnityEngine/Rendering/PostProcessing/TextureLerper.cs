using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	internal class TextureLerper
	{
		private static TextureLerper m_Instance;

		private CommandBuffer m_Command;

		private PropertySheetFactory m_PropertySheets;

		private PostProcessResources m_Resources;

		private List<RenderTexture> m_Recycled;

		private List<RenderTexture> m_Actives;

		internal static TextureLerper instance
		{
			get
			{
				if (m_Instance == null)
				{
					m_Instance = new TextureLerper();
				}
				return m_Instance;
			}
		}

		private TextureLerper()
		{
			m_Recycled = new List<RenderTexture>();
			m_Actives = new List<RenderTexture>();
		}

		internal void BeginFrame(PostProcessRenderContext context)
		{
			m_Command = context.command;
			m_PropertySheets = context.propertySheets;
			m_Resources = context.resources;
		}

		internal void EndFrame()
		{
			if (m_Recycled.Count > 0)
			{
				foreach (RenderTexture item in m_Recycled)
				{
					RuntimeUtilities.Destroy(item);
				}
				m_Recycled.Clear();
			}
			if (m_Actives.Count <= 0)
			{
				return;
			}
			foreach (RenderTexture active in m_Actives)
			{
				m_Recycled.Add(active);
			}
			m_Actives.Clear();
		}

		private RenderTexture Get(RenderTextureFormat format, int w, int h, int d = 1, bool enableRandomWrite = false)
		{
			RenderTexture renderTexture = null;
			int count = m_Recycled.Count;
			int i;
			for (i = 0; i < count; i++)
			{
				RenderTexture renderTexture2 = m_Recycled[i];
				if (renderTexture2.width == w && renderTexture2.height == h && renderTexture2.volumeDepth == d && renderTexture2.format == format && renderTexture2.enableRandomWrite == enableRandomWrite)
				{
					renderTexture = renderTexture2;
					break;
				}
			}
			if (renderTexture == null)
			{
				TextureDimension dimension = ((d <= 1) ? TextureDimension.Tex2D : TextureDimension.Tex3D);
				RenderTexture renderTexture3 = new RenderTexture(w, h, d, format);
				renderTexture3.filterMode = FilterMode.Bilinear;
				renderTexture3.wrapMode = TextureWrapMode.Clamp;
				renderTexture3.anisoLevel = 0;
				renderTexture3.volumeDepth = d;
				renderTexture3.enableRandomWrite = enableRandomWrite;
				renderTexture3.dimension = dimension;
				renderTexture = renderTexture3;
				renderTexture.Create();
			}
			else
			{
				m_Recycled.RemoveAt(i);
			}
			m_Actives.Add(renderTexture);
			return renderTexture;
		}

		internal Texture Lerp(Texture from, Texture to, float t)
		{
			bool flag = to is Texture3D || (to is RenderTexture && ((RenderTexture)to).volumeDepth > 1);
			RenderTexture renderTexture = null;
			if (flag)
			{
				int width = to.width;
				renderTexture = Get(RenderTextureFormat.ARGBHalf, width, width, width, enableRandomWrite: true);
				ComputeShader texture3dLerp = m_Resources.computeShaders.texture3dLerp;
				int kernelIndex = texture3dLerp.FindKernel("KTexture3DLerp");
				m_Command.SetComputeVectorParam(texture3dLerp, "_Params", new Vector4(t, width, 0f, 0f));
				m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_Output", renderTexture);
				m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_From", from);
				m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_To", to);
				int num = Mathf.CeilToInt((float)width / 8f);
				int threadGroupsZ = Mathf.CeilToInt((float)width / ((!RuntimeUtilities.isAndroidOpenGL) ? 8f : 2f));
				m_Command.DispatchCompute(texture3dLerp, kernelIndex, num, num, threadGroupsZ);
			}
			else
			{
				RenderTextureFormat uncompressedRenderTextureFormat = TextureFormatUtilities.GetUncompressedRenderTextureFormat(to);
				renderTexture = Get(uncompressedRenderTextureFormat, to.width, to.height);
				PropertySheet propertySheet = m_PropertySheets.Get(m_Resources.shaders.texture2dLerp);
				propertySheet.properties.SetTexture(ShaderIDs.To, to);
				propertySheet.properties.SetFloat(ShaderIDs.Interp, t);
				m_Command.BlitFullscreenTriangle(from, renderTexture, propertySheet, 0);
			}
			return renderTexture;
		}

		internal void Clear()
		{
			foreach (RenderTexture active in m_Actives)
			{
				RuntimeUtilities.Destroy(active);
			}
			foreach (RenderTexture item in m_Recycled)
			{
				RuntimeUtilities.Destroy(item);
			}
			m_Actives.Clear();
			m_Recycled.Clear();
		}
	}
}
