// Decompiled with JetBrains decompiler
// Type: HolographicSight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
public class HolographicSight : MonoBehaviour
{
  public Transform VirtualQuad;
  public float Scale = 1f;
  public bool SizeCompensation = true;
  public Camera Camera;
  private MaterialPropertyBlock m_block;

  private void OnEnable()
  {
    this.m_block = new MaterialPropertyBlock();
    this.GetComponent<Renderer>().SetPropertyBlock(this.m_block);
  }

  private void OnWillRenderObject()
  {
    this.m_block.SetVector("_Offset", (Vector4) this.transform.InverseTransformPoint(this.VirtualQuad.transform.position));
    this.m_block.SetFloat("_Scale", this.Scale);
    this.m_block.SetFloat("_SizeCompensation", !this.SizeCompensation ? 0.0f : 1f);
    this.GetComponent<Renderer>().SetPropertyBlock(this.m_block);
  }
}
