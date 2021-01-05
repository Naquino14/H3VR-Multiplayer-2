// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessRenderContext
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine.VR;

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class PostProcessRenderContext
  {
    private PostProcessRenderContext.CachedCameraSettings m_camCache;
    private Camera m_Camera;
    internal PropertySheet uberSheet;
    internal Texture autoExposureTexture;
    internal LogHistogram logHistogram;
    internal Texture logLut;
    internal AutoExposure autoExposure;

    public Camera camera
    {
      get => this.m_Camera;
      set
      {
        this.m_Camera = value;
        if (VRSettings.isDeviceActive)
        {
          if (RuntimeUtilities.isSinglePassStereoEnabled)
          {
            if (this.m_Camera.pixelWidth != this.m_camCache.Width || this.m_Camera.pixelHeight != this.m_camCache.Height || ((double) VRSettings.renderScale != (double) this.m_camCache.RenderScale || (double) VRSettings.renderViewportScale != (double) this.m_camCache.ViewportScale))
            {
              GameObject gameObject = new GameObject("TempCamp", new System.Type[1]
              {
                typeof (Camera)
              });
              Camera component = gameObject.GetComponent<Camera>();
              component.forceIntoRenderTexture = true;
              component.stereoTargetEye = StereoTargetEyeMask.Both;
              component.cullingMask = 0;
              GetTextureSizeHack getTextureSizeHack = component.gameObject.AddComponent<GetTextureSizeHack>();
              component.Render();
              this.width = getTextureSizeHack.Width;
              this.height = getTextureSizeHack.Height;
              UnityEngine.Object.DestroyImmediate((UnityEngine.Object) gameObject);
              this.m_camCache.Width = this.m_Camera.pixelWidth;
              this.m_camCache.Height = this.m_Camera.pixelHeight;
              this.m_camCache.RenderScale = VRSettings.renderScale;
              this.m_camCache.ViewportScale = VRSettings.renderViewportScale;
            }
          }
          else
          {
            this.width = VRSettings.eyeTextureWidth;
            this.height = VRSettings.eyeTextureHeight;
          }
          if (this.camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)
            this.xrActiveEye = 1;
          this.xrSingleEyeWidth = VRSettings.eyeTextureWidth;
          this.xrSingleEyeHeight = VRSettings.eyeTextureHeight;
        }
        else
        {
          this.width = this.m_Camera.pixelWidth;
          this.height = this.m_Camera.pixelHeight;
          this.xrSingleEyeWidth = this.width;
        }
      }
    }

    public CommandBuffer command { get; set; }

    public RenderTargetIdentifier source { get; set; }

    public RenderTargetIdentifier destination { get; set; }

    public RenderTextureFormat sourceFormat { get; set; }

    public bool flip { get; set; }

    public PostProcessResources resources { get; internal set; }

    public PropertySheetFactory propertySheets { get; internal set; }

    public Dictionary<string, object> userData { get; private set; }

    public PostProcessDebugLayer debugLayer { get; internal set; }

    public int width { get; private set; }

    public int height { get; private set; }

    public int xrActiveEye { get; private set; }

    public int xrSingleEyeWidth { get; private set; }

    public int xrSingleEyeHeight { get; private set; }

    public bool isSceneView { get; internal set; }

    public PostProcessLayer.Antialiasing antialiasing { get; internal set; }

    public TemporalAntialiasing temporalAntialiasing { get; internal set; }

    public void Reset()
    {
      this.m_Camera = (Camera) null;
      this.xrActiveEye = 0;
      this.xrSingleEyeWidth = 0;
      this.xrSingleEyeHeight = 0;
      this.command = (CommandBuffer) null;
      this.source = (RenderTargetIdentifier) 0;
      this.destination = (RenderTargetIdentifier) 0;
      this.sourceFormat = RenderTextureFormat.ARGB32;
      this.flip = false;
      this.resources = (PostProcessResources) null;
      this.propertySheets = (PropertySheetFactory) null;
      this.userData = (Dictionary<string, object>) null;
      this.debugLayer = (PostProcessDebugLayer) null;
      this.isSceneView = false;
      this.antialiasing = PostProcessLayer.Antialiasing.None;
      this.temporalAntialiasing = (TemporalAntialiasing) null;
      this.uberSheet = (PropertySheet) null;
      this.autoExposureTexture = (Texture) null;
      this.logLut = (Texture) null;
      this.autoExposure = (AutoExposure) null;
      if (this.userData == null)
        this.userData = new Dictionary<string, object>();
      this.userData.Clear();
    }

    public bool IsTemporalAntialiasingActive() => this.antialiasing == PostProcessLayer.Antialiasing.TemporalAntialiasing && !this.isSceneView && this.temporalAntialiasing.IsSupported();

    public bool IsDebugOverlayEnabled(DebugOverlay overlay) => this.debugLayer.debugOverlay == overlay;

    public void PushDebugOverlay(
      CommandBuffer cmd,
      RenderTargetIdentifier source,
      PropertySheet sheet,
      int pass)
    {
      this.debugLayer.PushDebugOverlay(cmd, source, sheet, pass);
    }

    private struct CachedCameraSettings
    {
      public float RenderScale;
      public float ViewportScale;
      public int Width;
      public int Height;
    }
  }
}
