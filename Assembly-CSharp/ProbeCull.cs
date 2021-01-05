// Decompiled with JetBrains decompiler
// Type: ProbeCull
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class ProbeCull : MonoBehaviour
{
  private ProbeCullTrack m_track;
  public float RadiusScale = 1f;
  private ReflectionProbe m_probe;

  private void OnEnable()
  {
    this.m_track.Probe = this.GetComponent<ReflectionProbe>();
    this.m_track.BoundingSphere = new BoundingSphere(this.transform.position, this.m_track.Probe.bounds.extents.magnitude * this.RadiusScale);
    this.m_track.Enabled = this.m_track.Probe.enabled;
    Tracker<ProbeCullTrack>.Register(this.m_track);
  }

  private void OnDisable() => Tracker<ProbeCullTrack>.Deregister(this.m_track);
}
