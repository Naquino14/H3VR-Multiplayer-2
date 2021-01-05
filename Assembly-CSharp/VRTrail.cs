// Decompiled with JetBrains decompiler
// Type: VRTrail
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

public class VRTrail : MonoBehaviour
{
  [ColorUsage(true, true, 0.0f, 50f, 0.125f, 3f)]
  public Color Color = Color.white;
  private Vector3[] m_positions;
  private int m_numPositions;
  public ComputeBuffer Buffer;
  private bool m_dirty;
  [NonSerialized]
  public Material Material;

  public int NumPositions => this.m_numPositions;

  private void Awake()
  {
    this.m_positions = new Vector3[256];
    this.AllocBuffer();
    VRTrailManager.Instance.Register(this);
    this.Material = new Material(VRTrailManager.Instance.TrailShader);
    this.Material.SetBuffer("_PosBuffer", this.Buffer);
  }

  private void AllocBuffer() => this.Buffer = new ComputeBuffer(this.m_positions.Length, 12);

  private void OnDestroy()
  {
    this.Buffer.Dispose();
    VRTrailManager.Instance.Deregister(this);
  }

  public void AddPosition(Vector3 pos)
  {
    if (this.m_numPositions >= this.m_positions.Length)
    {
      Array.Resize<Vector3>(ref this.m_positions, this.m_numPositions + 256);
      this.Buffer.Dispose();
      this.AllocBuffer();
      this.SetData();
    }
    this.m_positions[this.m_numPositions] = pos;
    ++this.m_numPositions;
    this.m_dirty = true;
  }

  public void SetData()
  {
    if (!this.m_dirty)
      return;
    this.Buffer.SetData((Array) this.m_positions);
    this.m_dirty = false;
  }
}
