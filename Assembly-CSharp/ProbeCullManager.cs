// Decompiled with JetBrains decompiler
// Type: ProbeCullManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ProbeCullManager : MonoBehaviour
{
  public Camera Camera;
  private CullingGroup m_cullGroup;
  private BoundingSphere[] m_bounds = new BoundingSphere[64];

  private void Awake()
  {
    this.Camera = Camera.main;
    this.m_cullGroup = new CullingGroup();
    this.m_cullGroup.SetBoundingSphereCount(0);
    this.m_cullGroup.SetBoundingSpheres(this.m_bounds);
    this.m_cullGroup.targetCamera = this.Camera;
  }

  private void Update()
  {
    while (this.m_bounds.Length < Tracker<ProbeCullTrack>.Count)
    {
      Array.Resize<BoundingSphere>(ref this.m_bounds, this.m_bounds.Length * 2);
      this.m_cullGroup.SetBoundingSpheres(this.m_bounds);
    }
    this.m_cullGroup.SetBoundingSphereCount(Tracker<ProbeCullTrack>.Count);
    for (int index = 0; index < Tracker<ProbeCullTrack>.Count; ++index)
      this.m_bounds[index] = Tracker<ProbeCullTrack>.All[index].BoundingSphere;
  }

  private void LateUpdate()
  {
    for (int index = 0; index < Tracker<ProbeCullTrack>.Count; ++index)
    {
      bool flag = this.m_cullGroup.IsVisible(index);
      ProbeCullTrack probeCullTrack = Tracker<ProbeCullTrack>.All[index];
      if (probeCullTrack.Enabled != flag)
      {
        probeCullTrack.Probe.enabled = flag;
        Tracker<ProbeCullTrack>.All[index].Enabled = flag;
      }
    }
  }

  private void OnDestroy() => this.m_cullGroup.Dispose();
}
