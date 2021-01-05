// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessDebug
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  [ExecuteInEditMode]
  public sealed class PostProcessDebug : MonoBehaviour
  {
    public PostProcessLayer postProcessLayer;
    private PostProcessLayer m_PreviousPostProcessLayer;
    public bool lightMeter;
    public bool histogram;
    public bool waveform;
    public bool vectorscope;
    public DebugOverlay debugOverlay;
    private Camera m_CurrentCamera;
    private CommandBuffer m_CmdAfterEverything;

    private void OnEnable() => this.m_CmdAfterEverything = new CommandBuffer()
    {
      name = "Post-processing Debug Overlay"
    };

    private void OnDisable()
    {
      if ((Object) this.m_CurrentCamera != (Object) null)
        this.m_CurrentCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, this.m_CmdAfterEverything);
      this.m_CurrentCamera = (Camera) null;
      this.m_PreviousPostProcessLayer = (PostProcessLayer) null;
    }

    private void Update() => this.UpdateStates();

    private void Reset() => this.postProcessLayer = this.GetComponent<PostProcessLayer>();

    private void UpdateStates()
    {
      if ((Object) this.m_PreviousPostProcessLayer != (Object) this.postProcessLayer)
      {
        if ((Object) this.m_CurrentCamera != (Object) null)
        {
          this.m_CurrentCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, this.m_CmdAfterEverything);
          this.m_CurrentCamera = (Camera) null;
        }
        this.m_PreviousPostProcessLayer = this.postProcessLayer;
        if ((Object) this.postProcessLayer != (Object) null)
        {
          this.m_CurrentCamera = this.postProcessLayer.GetComponent<Camera>();
          this.m_CurrentCamera.AddCommandBuffer(CameraEvent.AfterImageEffects, this.m_CmdAfterEverything);
        }
      }
      if ((Object) this.postProcessLayer == (Object) null || !this.postProcessLayer.enabled)
        return;
      if (this.lightMeter)
        this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.LightMeter);
      if (this.histogram)
        this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Histogram);
      if (this.waveform)
        this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Waveform);
      if (this.vectorscope)
        this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Vectorscope);
      this.postProcessLayer.debugLayer.RequestDebugOverlay(this.debugOverlay);
    }

    private void OnPostRender()
    {
      this.m_CmdAfterEverything.Clear();
      if ((Object) this.postProcessLayer == (Object) null || !this.postProcessLayer.enabled || !this.postProcessLayer.debugLayer.debugOverlayActive)
        return;
      this.m_CmdAfterEverything.Blit((Texture) this.postProcessLayer.debugLayer.debugOverlayTarget, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget);
    }

    private void OnGUI()
    {
      if ((Object) this.postProcessLayer == (Object) null || !this.postProcessLayer.enabled)
        return;
      Rect rect = new Rect(5f, 5f, 0.0f, 0.0f);
      PostProcessDebugLayer debugLayer = this.postProcessLayer.debugLayer;
      this.DrawMonitor(ref rect, (Monitor) debugLayer.lightMeter, this.lightMeter);
      this.DrawMonitor(ref rect, (Monitor) debugLayer.histogram, this.histogram);
      this.DrawMonitor(ref rect, (Monitor) debugLayer.waveform, this.waveform);
      this.DrawMonitor(ref rect, (Monitor) debugLayer.vectorscope, this.vectorscope);
    }

    private void DrawMonitor(ref Rect rect, Monitor monitor, bool enabled)
    {
      if (!enabled || (Object) monitor.output == (Object) null)
        return;
      rect.width = (float) monitor.output.width;
      rect.height = (float) monitor.output.height;
      GUI.DrawTexture(rect, (Texture) monitor.output);
      rect.x += (float) monitor.output.width + 5f;
    }
  }
}
