// Decompiled with JetBrains decompiler
// Type: FistVR.wwBotWurstLaserDot
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwBotWurstLaserDot : MonoBehaviour
  {
    public Renderer Beam;
    public float MaxCastDist;
    public LayerMask LM_Collide;
    private RaycastHit m_hit;
    public float width = 0.005f;

    private void Start()
    {
    }

    private void Update()
    {
      float z = this.MaxCastDist;
      if (Physics.Raycast(this.transform.position, this.transform.forward, out this.m_hit, this.MaxCastDist, (int) this.LM_Collide, QueryTriggerInteraction.Ignore))
        z = this.m_hit.distance;
      this.transform.localScale = new Vector3(this.width, this.width, z);
    }
  }
}
