// Decompiled with JetBrains decompiler
// Type: VrTrailTest
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class VrTrailTest : MonoBehaviour
{
  private int m_render;
  private float m_lisA;
  private float m_lisB;
  private Vector3 m_offset;
  private VRTrail m_trail;

  private void Awake()
  {
    this.m_trail = this.GetComponent<VRTrail>();
    this.m_lisA = Random.value / 10f;
    this.m_lisB = Random.value / 10f;
    this.m_offset = this.transform.position;
  }

  private void Update()
  {
    this.m_trail.AddPosition(this.m_offset + new Vector3(Mathf.Sin(this.m_lisA * (float) this.m_render), Mathf.Sin(this.m_lisB * (float) this.m_render)));
    ++this.m_render;
  }
}
