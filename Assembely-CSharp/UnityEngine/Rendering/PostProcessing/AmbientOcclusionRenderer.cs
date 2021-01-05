namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class AmbientOcclusionRenderer : PostProcessEffectRenderer<AmbientOcclusion>
	{
		private IAmbientOcclusionMethod[] m_Methods;

		public override void Init()
		{
			if (m_Methods == null)
			{
				m_Methods = new IAmbientOcclusionMethod[2]
				{
					new ScalableAO(base.settings),
					new MultiScaleVO(base.settings)
				};
			}
		}

		public bool IsAmbientOnly(PostProcessRenderContext context)
		{
			Camera camera = context.camera;
			return base.settings.ambientOnly.value && camera.actualRenderingPath == RenderingPath.DeferredShading && camera.allowHDR;
		}

		public IAmbientOcclusionMethod Get()
		{
			return m_Methods[(int)base.settings.mode.value];
		}

		public override DepthTextureMode GetCameraFlags()
		{
			return Get().GetCameraFlags();
		}

		public override void Release()
		{
			IAmbientOcclusionMethod[] methods = m_Methods;
			foreach (IAmbientOcclusionMethod ambientOcclusionMethod in methods)
			{
				ambientOcclusionMethod.Release();
			}
		}

		public override void Render(PostProcessRenderContext context)
		{
		}
	}
}
