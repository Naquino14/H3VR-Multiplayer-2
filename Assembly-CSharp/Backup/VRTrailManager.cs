// Decompiled with JetBrains decompiler
// Type: VRTrailManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VRTrailManager : MonoBehaviour
{
  public static VRTrailManager Instance;
  public Shader TrailShader;
  private List<VRTrail> m_trails = new List<VRTrail>();
  private HashSet<Camera> m_setCameras = new HashSet<Camera>();
  private CommandBuffer m_cmdBuffer;
  private int m_colId;

  private void Awake()
  {
    this.m_colId = Shader.PropertyToID("_Color");
    VRTrailManager.Instance = this;
    this.m_cmdBuffer = new CommandBuffer();
    Camera.onPreRender += new Camera.CameraCallback(this.OnCam);
  }

  private void OnDestroy() => Camera.onPreRender -= new Camera.CameraCallback(this.OnCam);

  public void Register(VRTrail trail) => this.m_trails.Add(trail);

  public void Deregister(VRTrail trail)
  {
    int index = this.m_trails.IndexOf(trail);
    if (index == -1)
      return;
    this.m_trails[index] = this.m_trails[this.m_trails.Count - 1];
    this.m_trails.RemoveAt(this.m_trails.Count - 1);
  }

  private void Update()
  {
    Matrix4x4 identity = Matrix4x4.identity;
    this.m_cmdBuffer.Clear();
    for (int index = 0; index < this.m_trails.Count; ++index)
    {
      VRTrail trail = this.m_trails[index];
      if (trail.NumPositions > 1)
      {
        trail.SetData();
        trail.Material.SetColor(this.m_colId, trail.Color);
        this.m_cmdBuffer.DrawProcedural(identity, trail.Material, 0, MeshTopology.LineStrip, trail.NumPositions - 1, 1);
      }
    }
  }

  private void OnCam(Camera cam)
  {
    if (!this.m_setCameras.Add(cam))
      return;
    cam.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, this.m_cmdBuffer);
  }
}
