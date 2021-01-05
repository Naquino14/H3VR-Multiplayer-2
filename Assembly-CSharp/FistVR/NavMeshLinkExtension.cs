// Decompiled with JetBrains decompiler
// Type: FistVR.NavMeshLinkExtension
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class NavMeshLinkExtension : MonoBehaviour
  {
    public NavMeshLinkExtension.NavMeshLinkType Type;
    public float TimeToClear = 1f;
    private float m_xySpeed;
    public OffMeshLink Link;

    public float GetXYSpeed() => this.m_xySpeed;

    private void Start()
    {
      this.Link = this.gameObject.GetComponent<OffMeshLink>();
      this.m_xySpeed = (this.Link.startTransform.position - this.Link.endTransform.position).magnitude / this.TimeToClear;
    }

    public enum NavMeshLinkType
    {
      LateralJump,
      Climb,
      Drop,
    }
  }
}
