// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessDebugLayer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class PostProcessDebugLayer
  {
    public LightMeterMonitor lightMeter;
    public HistogramMonitor histogram;
    public WaveformMonitor waveform;
    public VectorscopeMonitor vectorscope;
    private Dictionary<MonitorType, Monitor> m_Monitors;
    private int frameWidth;
    private int frameHeight;
    public PostProcessDebugLayer.OverlaySettings overlaySettings;

    public RenderTexture debugOverlayTarget { get; private set; }

    public bool debugOverlayActive { get; private set; }

    public DebugOverlay debugOverlay { get; private set; }

    internal void OnEnable()
    {
      RuntimeUtilities.CreateIfNull<LightMeterMonitor>(ref this.lightMeter);
      RuntimeUtilities.CreateIfNull<HistogramMonitor>(ref this.histogram);
      RuntimeUtilities.CreateIfNull<WaveformMonitor>(ref this.waveform);
      RuntimeUtilities.CreateIfNull<VectorscopeMonitor>(ref this.vectorscope);
      RuntimeUtilities.CreateIfNull<PostProcessDebugLayer.OverlaySettings>(ref this.overlaySettings);
      this.m_Monitors = new Dictionary<MonitorType, Monitor>()
      {
        {
          MonitorType.LightMeter,
          (Monitor) this.lightMeter
        },
        {
          MonitorType.Histogram,
          (Monitor) this.histogram
        },
        {
          MonitorType.Waveform,
          (Monitor) this.waveform
        },
        {
          MonitorType.Vectorscope,
          (Monitor) this.vectorscope
        }
      };
      foreach (KeyValuePair<MonitorType, Monitor> monitor in this.m_Monitors)
        monitor.Value.OnEnable();
    }

    internal void OnDisable()
    {
      foreach (KeyValuePair<MonitorType, Monitor> monitor in this.m_Monitors)
        monitor.Value.OnDisable();
      this.DestroyDebugOverlayTarget();
    }

    private void DestroyDebugOverlayTarget()
    {
      RuntimeUtilities.Destroy((UnityEngine.Object) this.debugOverlayTarget);
      this.debugOverlayTarget = (RenderTexture) null;
    }

    public void RequestMonitorPass(MonitorType monitor) => this.m_Monitors[monitor].requested = true;

    public void RequestDebugOverlay(DebugOverlay mode) => this.debugOverlay = mode;

    internal void SetFrameSize(int width, int height)
    {
      this.frameWidth = width;
      this.frameHeight = height;
      this.debugOverlayActive = false;
    }

    public void PushDebugOverlay(
      CommandBuffer cmd,
      RenderTargetIdentifier source,
      PropertySheet sheet,
      int pass)
    {
      if ((UnityEngine.Object) this.debugOverlayTarget == (UnityEngine.Object) null || !this.debugOverlayTarget.IsCreated() || (this.debugOverlayTarget.width != this.frameWidth || this.debugOverlayTarget.height != this.frameHeight))
      {
        RuntimeUtilities.Destroy((UnityEngine.Object) this.debugOverlayTarget);
        RenderTexture renderTexture = new RenderTexture(this.frameWidth, this.frameHeight, 0, RenderTextureFormat.ARGB32);
        renderTexture.name = "Debug Overlay Target";
        renderTexture.anisoLevel = 1;
        renderTexture.filterMode = FilterMode.Bilinear;
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.hideFlags = HideFlags.HideAndDontSave;
        this.debugOverlayTarget = renderTexture;
        this.debugOverlayTarget.Create();
      }
      cmd.BlitFullscreenTriangle(source, (RenderTargetIdentifier) (Texture) this.debugOverlayTarget, sheet, pass);
      this.debugOverlayActive = true;
    }

    internal DepthTextureMode GetCameraFlags()
    {
      if (this.debugOverlay == DebugOverlay.Depth)
        return DepthTextureMode.Depth;
      if (this.debugOverlay == DebugOverlay.Normals)
        return DepthTextureMode.DepthNormals;
      return this.debugOverlay == DebugOverlay.MotionVectors ? DepthTextureMode.Depth | DepthTextureMode.MotionVectors : DepthTextureMode.None;
    }

    internal void RenderMonitors(PostProcessRenderContext context)
    {
      bool flag1 = false;
      bool flag2 = false;
      foreach (KeyValuePair<MonitorType, Monitor> monitor in this.m_Monitors)
      {
        bool flag3 = monitor.Value.IsRequestedAndSupported();
        flag1 |= flag3;
        flag2 = ((flag2 ? 1 : 0) | (!flag3 ? 0 : (monitor.Value.NeedsHalfRes() ? 1 : 0))) != 0;
      }
      if (!flag1)
        return;
      CommandBuffer command = context.command;
      command.BeginSample("Monitors");
      if (flag2)
      {
        command.GetTemporaryRT(ShaderIDs.HalfResFinalCopy, context.width / 2, context.height / 2, 0, FilterMode.Bilinear, context.sourceFormat);
        command.Blit(context.destination, (RenderTargetIdentifier) ShaderIDs.HalfResFinalCopy);
      }
      foreach (KeyValuePair<MonitorType, Monitor> monitor1 in this.m_Monitors)
      {
        Monitor monitor2 = monitor1.Value;
        if (monitor2.requested)
          monitor2.Render(context);
      }
      if (flag2)
        command.ReleaseTemporaryRT(ShaderIDs.HalfResFinalCopy);
      command.EndSample("Monitors");
    }

    internal void RenderSpecialOverlays(PostProcessRenderContext context)
    {
      if (this.debugOverlay == DebugOverlay.Depth)
      {
        PropertySheet sheet = context.propertySheets.Get(context.resources.shaders.debugOverlays);
        this.PushDebugOverlay(context.command, (RenderTargetIdentifier) BuiltinRenderTextureType.None, sheet, 0);
      }
      else if (this.debugOverlay == DebugOverlay.Normals)
      {
        PropertySheet sheet = context.propertySheets.Get(context.resources.shaders.debugOverlays);
        sheet.ClearKeywords();
        if (context.camera.actualRenderingPath == RenderingPath.DeferredLighting)
          sheet.EnableKeyword("SOURCE_GBUFFER");
        this.PushDebugOverlay(context.command, (RenderTargetIdentifier) BuiltinRenderTextureType.None, sheet, 1);
      }
      else if (this.debugOverlay == DebugOverlay.MotionVectors)
      {
        PropertySheet sheet = context.propertySheets.Get(context.resources.shaders.debugOverlays);
        sheet.properties.SetVector(ShaderIDs.Params, new Vector4(this.overlaySettings.motionColorIntensity, (float) this.overlaySettings.motionGridSize, 0.0f, 0.0f));
        this.PushDebugOverlay(context.command, context.source, sheet, 2);
      }
      else if (this.debugOverlay == DebugOverlay.NANTracker)
      {
        PropertySheet sheet = context.propertySheets.Get(context.resources.shaders.debugOverlays);
        this.PushDebugOverlay(context.command, context.source, sheet, 3);
      }
      else
      {
        if (this.debugOverlay != DebugOverlay.ColorBlindnessSimulation)
          return;
        PropertySheet sheet = context.propertySheets.Get(context.resources.shaders.debugOverlays);
        sheet.properties.SetVector(ShaderIDs.Params, new Vector4(this.overlaySettings.colorBlindnessStrength, 0.0f, 0.0f, 0.0f));
        this.PushDebugOverlay(context.command, context.source, sheet, (int) (4 + this.overlaySettings.colorBlindnessType));
      }
    }

    internal void EndFrame()
    {
      foreach (KeyValuePair<MonitorType, Monitor> monitor in this.m_Monitors)
        monitor.Value.requested = false;
      if (!this.debugOverlayActive)
        this.DestroyDebugOverlayTarget();
      this.debugOverlay = DebugOverlay.None;
    }

    [Serializable]
    public class OverlaySettings
    {
      [Range(0.0f, 16f)]
      public float motionColorIntensity = 4f;
      [Range(4f, 128f)]
      public int motionGridSize = 64;
      public ColorBlindnessType colorBlindnessType;
      [Range(0.0f, 1f)]
      public float colorBlindnessStrength = 1f;
    }
  }
}
