using System.Collections.Generic;
using UnityEngine.VR;

namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class PostProcessRenderContext
	{
		private struct CachedCameraSettings
		{
			public float RenderScale;

			public float ViewportScale;

			public int Width;

			public int Height;
		}

		private CachedCameraSettings m_camCache;

		private Camera m_Camera;

		internal PropertySheet uberSheet;

		internal Texture autoExposureTexture;

		internal LogHistogram logHistogram;

		internal Texture logLut;

		internal AutoExposure autoExposure;

		public Camera camera
		{
			get
			{
				return m_Camera;
			}
			set
			{
				m_Camera = value;
				if (VRSettings.isDeviceActive)
				{
					if (RuntimeUtilities.isSinglePassStereoEnabled)
					{
						if (m_Camera.pixelWidth != m_camCache.Width || m_Camera.pixelHeight != m_camCache.Height || VRSettings.renderScale != m_camCache.RenderScale || VRSettings.renderViewportScale != m_camCache.ViewportScale)
						{
							GameObject gameObject = new GameObject("TempCamp", typeof(Camera));
							Camera component = gameObject.GetComponent<Camera>();
							component.forceIntoRenderTexture = true;
							component.stereoTargetEye = StereoTargetEyeMask.Both;
							component.cullingMask = 0;
							GetTextureSizeHack getTextureSizeHack = component.gameObject.AddComponent<GetTextureSizeHack>();
							component.Render();
							width = getTextureSizeHack.Width;
							height = getTextureSizeHack.Height;
							Object.DestroyImmediate(gameObject);
							m_camCache.Width = m_Camera.pixelWidth;
							m_camCache.Height = m_Camera.pixelHeight;
							m_camCache.RenderScale = VRSettings.renderScale;
							m_camCache.ViewportScale = VRSettings.renderViewportScale;
						}
					}
					else
					{
						width = VRSettings.eyeTextureWidth;
						height = VRSettings.eyeTextureHeight;
					}
					if (camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)
					{
						xrActiveEye = 1;
					}
					xrSingleEyeWidth = VRSettings.eyeTextureWidth;
					xrSingleEyeHeight = VRSettings.eyeTextureHeight;
				}
				else
				{
					width = m_Camera.pixelWidth;
					height = m_Camera.pixelHeight;
					xrSingleEyeWidth = width;
				}
			}
		}

		public CommandBuffer command
		{
			get;
			set;
		}

		public RenderTargetIdentifier source
		{
			get;
			set;
		}

		public RenderTargetIdentifier destination
		{
			get;
			set;
		}

		public RenderTextureFormat sourceFormat
		{
			get;
			set;
		}

		public bool flip
		{
			get;
			set;
		}

		public PostProcessResources resources
		{
			get;
			internal set;
		}

		public PropertySheetFactory propertySheets
		{
			get;
			internal set;
		}

		public Dictionary<string, object> userData
		{
			get;
			private set;
		}

		public PostProcessDebugLayer debugLayer
		{
			get;
			internal set;
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

		public int xrActiveEye
		{
			get;
			private set;
		}

		public int xrSingleEyeWidth
		{
			get;
			private set;
		}

		public int xrSingleEyeHeight
		{
			get;
			private set;
		}

		public bool isSceneView
		{
			get;
			internal set;
		}

		public PostProcessLayer.Antialiasing antialiasing
		{
			get;
			internal set;
		}

		public TemporalAntialiasing temporalAntialiasing
		{
			get;
			internal set;
		}

		public void Reset()
		{
			m_Camera = null;
			xrActiveEye = 0;
			xrSingleEyeWidth = 0;
			xrSingleEyeHeight = 0;
			command = null;
			source = 0;
			destination = 0;
			sourceFormat = RenderTextureFormat.ARGB32;
			flip = false;
			resources = null;
			propertySheets = null;
			userData = null;
			debugLayer = null;
			isSceneView = false;
			antialiasing = PostProcessLayer.Antialiasing.None;
			temporalAntialiasing = null;
			uberSheet = null;
			autoExposureTexture = null;
			logLut = null;
			autoExposure = null;
			if (userData == null)
			{
				userData = new Dictionary<string, object>();
			}
			userData.Clear();
		}

		public bool IsTemporalAntialiasingActive()
		{
			return antialiasing == PostProcessLayer.Antialiasing.TemporalAntialiasing && !isSceneView && temporalAntialiasing.IsSupported();
		}

		public bool IsDebugOverlayEnabled(DebugOverlay overlay)
		{
			return debugLayer.debugOverlay == overlay;
		}

		public void PushDebugOverlay(CommandBuffer cmd, RenderTargetIdentifier source, PropertySheet sheet, int pass)
		{
			debugLayer.PushDebugOverlay(cmd, source, sheet, pass);
		}
	}
}
