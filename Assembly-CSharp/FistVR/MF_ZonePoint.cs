// Decompiled with JetBrains decompiler
// Type: FistVR.MF_ZonePoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MF_ZonePoint : MonoBehaviour
  {
    public MF_Zone Z;
    public MF_ZonePoint.ZoneType T;

    private void OnDrawGizmos()
    {
      if (!this.Z.DrawGiz)
        return;
      switch (this.T)
      {
        case MF_ZonePoint.ZoneType.None:
          Gizmos.color = new Color(1f, 1f, 1f, 1f);
          Gizmos.DrawSphere(this.transform.position, 0.15f);
          Gizmos.DrawWireSphere(this.transform.position, 0.15f);
          break;
        case MF_ZonePoint.ZoneType.Assault:
          Gizmos.color = new Color(1f, 0.3f, 0.3f, 1f);
          Gizmos.DrawSphere(this.transform.position, 0.15f);
          Gizmos.DrawWireSphere(this.transform.position, 0.15f);
          break;
        case MF_ZonePoint.ZoneType.Support:
          Gizmos.color = new Color(0.3f, 1f, 0.3f, 1f);
          Gizmos.DrawSphere(this.transform.position, 0.15f);
          Gizmos.DrawWireSphere(this.transform.position, 0.15f);
          break;
        case MF_ZonePoint.ZoneType.Sniping:
          Gizmos.color = new Color(0.3f, 0.3f, 1f, 1f);
          Gizmos.DrawSphere(this.transform.position, 0.15f);
          Gizmos.DrawWireSphere(this.transform.position, 0.15f);
          break;
      }
    }

    public enum ZoneType
    {
      None,
      Assault,
      Support,
      Sniping,
    }
  }
}
