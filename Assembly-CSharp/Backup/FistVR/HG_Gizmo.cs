// Decompiled with JetBrains decompiler
// Type: FistVR.HG_Gizmo
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class HG_Gizmo : MonoBehaviour
  {
    public HG_Zone Zone;
    public HG_Gizmo.HGGizmosType Type;

    private void OnDrawGizmos()
    {
      if (Application.isPlaying || !((Object) this.Zone != (Object) null) || !this.Zone.DebugView)
        return;
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(this.transform.position, this.transform.rotation, this.transform.localScale);
      Matrix4x4 matrix = Gizmos.matrix;
      switch (this.Type)
      {
        case HG_Gizmo.HGGizmosType.PlayerSpawn:
          Gizmos.matrix *= matrix4x4;
          Gizmos.color = new Color(0.1f, 1f, 1f);
          Gizmos.DrawCube(Vector3.zero, new Vector3(0.35f, 0.2f, 0.35f));
          Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.35f, 0.2f, 0.35f));
          Gizmos.matrix = matrix;
          break;
        case HG_Gizmo.HGGizmosType.SpawnPoint_Offense:
          Gizmos.color = new Color(1f, 0.1f, 0.1f);
          Gizmos.DrawCube(this.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
          Gizmos.DrawWireCube(this.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
          break;
        case HG_Gizmo.HGGizmosType.SpawnPoint_Defense:
          Gizmos.color = new Color(0.1f, 1f, 0.1f);
          Gizmos.DrawCube(this.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
          Gizmos.DrawWireCube(this.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
          break;
        case HG_Gizmo.HGGizmosType.TargetPoint:
          Gizmos.color = new Color(0.1f, 0.1f, 1f);
          Gizmos.DrawCube(this.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
          Gizmos.DrawWireCube(this.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
          break;
        case HG_Gizmo.HGGizmosType.SpawnPoint_Civvie:
          Gizmos.color = new Color(1f, 1f, 0.1f);
          Gizmos.DrawCube(this.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
          Gizmos.DrawWireCube(this.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
          break;
      }
    }

    public enum HGGizmosType
    {
      PlayerSpawn,
      SpawnPoint_Offense,
      SpawnPoint_Defense,
      TargetPoint,
      SpawnPoint_Civvie,
    }
  }
}
