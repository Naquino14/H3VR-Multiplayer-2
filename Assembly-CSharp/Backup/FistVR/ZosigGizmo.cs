// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigGizmo
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ZosigGizmo : MonoBehaviour
  {
    [InspectorButton("Align")]
    public bool DoAlign;
    public ZosigGizmo.ZosigGizmosType Type;
    public BoxCollider Collider;
    public Mesh GunCaseBase;

    private void OnDrawGizmos()
    {
      if (Application.isPlaying)
        return;
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(this.transform.position, this.transform.rotation, this.transform.localScale);
      Matrix4x4 matrix = Gizmos.matrix;
      ZosigGizmo.ZosigGizmosType type = this.Type;
      switch (type)
      {
        case ZosigGizmo.ZosigGizmosType.Herb_Katchup:
          Gizmos.color = new Color(1f, 0.7f, 0.7f);
          Gizmos.DrawSphere(this.transform.position + Vector3.up * 0.2f, 0.4f);
          Gizmos.DrawWireSphere(this.transform.position + Vector3.up * 0.2f, 0.4f);
          break;
        case ZosigGizmo.ZosigGizmosType.Herb_Mustard:
          Gizmos.color = new Color(1f, 1f, 0.7f);
          Gizmos.DrawSphere(this.transform.position + Vector3.up * 0.2f, 0.4f);
          Gizmos.DrawWireSphere(this.transform.position + Vector3.up * 0.2f, 0.4f);
          break;
        case ZosigGizmo.ZosigGizmosType.Herb_Pickle:
          Gizmos.color = new Color(0.7f, 1f, 0.7f);
          Gizmos.DrawSphere(this.transform.position + Vector3.up * 0.2f, 0.4f);
          Gizmos.DrawWireSphere(this.transform.position + Vector3.up * 0.2f, 0.4f);
          break;
        case ZosigGizmo.ZosigGizmosType.Herb_Blue:
          Gizmos.color = new Color(0.7f, 0.7f, 1f);
          Gizmos.DrawSphere(this.transform.position + Vector3.up * 0.2f, 0.4f);
          Gizmos.DrawWireSphere(this.transform.position + Vector3.up * 0.2f, 0.4f);
          break;
        case ZosigGizmo.ZosigGizmosType.Herb_Eggplant:
          Gizmos.color = new Color(1f, 0.7f, 1f);
          Gizmos.DrawSphere(this.transform.position + Vector3.up * 0.2f, 0.4f);
          Gizmos.DrawWireSphere(this.transform.position + Vector3.up * 0.2f, 0.4f);
          break;
        case ZosigGizmo.ZosigGizmosType.Volume_BringItemQuest:
          Gizmos.color = Color.yellow;
          Gizmos.matrix *= matrix4x4;
          Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
          Gizmos.matrix = matrix;
          break;
        case ZosigGizmo.ZosigGizmosType.Volume_MoodChange:
          Gizmos.color = new Color(0.85f, 0.85f, 1f, 0.6f);
          Gizmos.matrix *= matrix4x4;
          Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
          Gizmos.matrix = matrix;
          break;
        case ZosigGizmo.ZosigGizmosType.Volume_ZosigSpawn:
          Gizmos.color = new Color(1f, 0.25f, 0.25f, 0.6f);
          Gizmos.matrix *= matrix4x4;
          Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
          Gizmos.matrix = matrix;
          break;
        case ZosigGizmo.ZosigGizmosType.Volume_MusicChange:
          Gizmos.color = new Color(0.25f, 1f, 0.25f, 0.6f);
          Gizmos.matrix *= matrix4x4;
          Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
          Gizmos.matrix = matrix;
          break;
        case ZosigGizmo.ZosigGizmosType.Volume_Ambient:
          Gizmos.color = new Color(0.25f, 0.25f, 1f, 0.6f);
          Gizmos.matrix *= matrix4x4;
          Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
          Gizmos.matrix = matrix;
          break;
        case ZosigGizmo.ZosigGizmosType.Point_Spawn_BareItem:
          Gizmos.color = Color.magenta;
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.2f, this.transform.position - Vector3.up * 0.2f);
          Gizmos.DrawLine(this.transform.position + Vector3.right * 0.2f, this.transform.position - Vector3.right * 0.2f);
          Gizmos.DrawLine(this.transform.position + Vector3.forward * 0.2f, this.transform.position - Vector3.forward * 0.2f);
          break;
        case ZosigGizmo.ZosigGizmosType.Point_Spawn_BareItemReward:
          Gizmos.color = new Color(0.0f, 0.35f, 1f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.2f, this.transform.position - Vector3.up * 0.2f);
          Gizmos.DrawLine(this.transform.position + Vector3.right * 0.2f, this.transform.position - Vector3.right * 0.2f);
          Gizmos.DrawLine(this.transform.position + Vector3.forward * 0.2f, this.transform.position - Vector3.forward * 0.2f);
          break;
        case ZosigGizmo.ZosigGizmosType.Point_Spawn_GunCase:
          Gizmos.color = new Color(1f, 0.6f, 0.0f);
          Gizmos.DrawMesh(this.GunCaseBase, this.transform.position, this.transform.rotation);
          break;
        case ZosigGizmo.ZosigGizmosType.Point_Spawn_DestroyableBox:
          Gizmos.color = new Color(0.6f, 1f, 0.0f);
          Gizmos.DrawMesh(this.GunCaseBase, this.transform.position, this.transform.rotation);
          break;
        case ZosigGizmo.ZosigGizmosType.Point_Spawn_Locker:
          Gizmos.color = new Color(1f, 0.75f, 0.2f, 1f);
          Gizmos.matrix *= matrix4x4;
          Gizmos.DrawCube(Vector3.up, new Vector3(0.43f, 2f, 0.53f));
          Gizmos.DrawWireCube(Vector3.up, new Vector3(0.43f, 2f, 0.53f));
          Gizmos.matrix = matrix;
          break;
        case ZosigGizmo.ZosigGizmosType.Point_Spawn_BigWoodCrate:
          Gizmos.color = new Color(0.5f, 1f, 0.2f, 1f);
          Gizmos.matrix *= matrix4x4;
          Gizmos.DrawCube(Vector3.up * 0.405f, new Vector3(2f, 0.81f, 1.14f));
          Gizmos.DrawWireCube(Vector3.up * 0.405f, new Vector3(2f, 0.81f, 1.14f));
          Gizmos.matrix = matrix;
          break;
        case ZosigGizmo.ZosigGizmosType.Point_Spawn_Cooler:
          Gizmos.color = new Color(1f, 0.2f, 0.2f, 1f);
          Gizmos.matrix *= matrix4x4;
          Gizmos.DrawCube(Vector3.up * 0.215f, new Vector3(0.5f, 0.43f, 0.34f));
          Gizmos.DrawWireCube(Vector3.up * 0.215f, new Vector3(0.5f, 0.43f, 0.34f));
          Gizmos.matrix = matrix;
          break;
        case ZosigGizmo.ZosigGizmosType.Point_Spawn_Buybuddy:
          Gizmos.color = new Color(0.2f, 1f, 0.4f, 1f);
          Gizmos.matrix *= matrix4x4;
          Gizmos.DrawCube(Vector3.up * 0.15f, new Vector3(0.3f, 0.3f, 0.3f));
          Gizmos.DrawWireCube(Vector3.up * 0.15f, new Vector3(0.3f, 0.3f, 0.3f));
          Gizmos.matrix = matrix;
          break;
        case ZosigGizmo.ZosigGizmosType.ZosigSpawn_Basic:
          Gizmos.color = new Color(0.1f, 1f, 0.1f, 1f);
          Gizmos.DrawCube(this.transform.position + Vector3.up, new Vector3(0.15f, 2f, 0.15f));
          Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.up * 2f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.5f + Vector3.right * 0.2f, this.transform.position + Vector3.up * 0.5f - Vector3.right * 0.2f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.5f + Vector3.forward * 0.2f, this.transform.position + Vector3.up * 0.5f - Vector3.forward * 0.2f);
          break;
        case ZosigGizmo.ZosigGizmosType.ZosigSpawn_Blut:
          Gizmos.color = new Color(1f, 0.3f, 0.3f, 1f);
          Gizmos.DrawCube(this.transform.position + Vector3.up, new Vector3(0.15f, 2f, 0.15f));
          Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.up * 2f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.5f + Vector3.right * 0.2f, this.transform.position + Vector3.up * 0.5f - Vector3.right * 0.2f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.5f + Vector3.forward * 0.2f, this.transform.position + Vector3.up * 0.5f - Vector3.forward * 0.2f);
          break;
        case ZosigGizmo.ZosigGizmosType.ZosigSpawn_Spitter:
          Gizmos.color = new Color(1f, 1f, 0.1f, 1f);
          Gizmos.DrawCube(this.transform.position + Vector3.up, new Vector3(0.15f, 2f, 0.15f));
          Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.up * 2f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.5f + Vector3.right * 0.2f, this.transform.position + Vector3.up * 0.5f - Vector3.right * 0.2f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.5f + Vector3.forward * 0.2f, this.transform.position + Vector3.up * 0.5f - Vector3.forward * 0.2f);
          break;
        case ZosigGizmo.ZosigGizmosType.ZosigSpawn_Exploding:
          Gizmos.color = new Color(1f, 0.1f, 1f, 1f);
          Gizmos.DrawCube(this.transform.position + Vector3.up, new Vector3(0.15f, 2f, 0.15f));
          Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.up * 2f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.5f + Vector3.right * 0.2f, this.transform.position + Vector3.up * 0.5f - Vector3.right * 0.2f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.5f + Vector3.forward * 0.2f, this.transform.position + Vector3.up * 0.5f - Vector3.forward * 0.2f);
          break;
        case ZosigGizmo.ZosigGizmosType.ZosigSpawn_Runner:
          Gizmos.color = new Color(0.1f, 0.7f, 1f, 1f);
          Gizmos.DrawCube(this.transform.position + Vector3.up, new Vector3(0.15f, 2f, 0.15f));
          Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.up * 2f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.5f + Vector3.right * 0.2f, this.transform.position + Vector3.up * 0.5f - Vector3.right * 0.2f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.5f + Vector3.forward * 0.2f, this.transform.position + Vector3.up * 0.5f - Vector3.forward * 0.2f);
          break;
        case ZosigGizmo.ZosigGizmosType.ZosigSpawn_Armored:
          Gizmos.color = new Color(1f, 0.0f, 0.0f, 1f);
          Gizmos.DrawCube(this.transform.position + Vector3.up, new Vector3(0.15f, 2f, 0.15f));
          Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.up * 2f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.5f + Vector3.right * 0.2f, this.transform.position + Vector3.up * 0.5f - Vector3.right * 0.2f);
          Gizmos.DrawLine(this.transform.position + Vector3.up * 0.5f + Vector3.forward * 0.2f, this.transform.position + Vector3.up * 0.5f - Vector3.forward * 0.2f);
          break;
        default:
          if (type != ZosigGizmo.ZosigGizmosType.CivvieWiener)
            break;
          Gizmos.color = new Color(1f, 0.8f, 0.5f);
          Gizmos.DrawCube(this.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
          break;
      }
    }

    [ContextMenu("Align")]
    public void Align()
    {
      RaycastHit hitInfo;
      if (!Physics.Raycast(this.transform.position + Vector3.up, -Vector3.up, out hitInfo))
        return;
      this.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(this.transform.forward, hitInfo.normal), hitInfo.normal);
      this.transform.position = hitInfo.point;
    }

    public enum ZosigGizmosType
    {
      CivvieWiener = 0,
      None = 1,
      Herb_Katchup = 10, // 0x0000000A
      Herb_Mustard = 11, // 0x0000000B
      Herb_Pickle = 12, // 0x0000000C
      Herb_Blue = 13, // 0x0000000D
      Herb_Eggplant = 14, // 0x0000000E
      Volume_BringItemQuest = 20, // 0x00000014
      Volume_MoodChange = 21, // 0x00000015
      Volume_ZosigSpawn = 22, // 0x00000016
      Volume_MusicChange = 23, // 0x00000017
      Volume_Ambient = 24, // 0x00000018
      Point_Spawn_BareItem = 30, // 0x0000001E
      Point_Spawn_BareItemReward = 31, // 0x0000001F
      Point_Spawn_Test = 32, // 0x00000020
      Point_Spawn_GunCase = 33, // 0x00000021
      Point_Spawn_DestroyableBox = 34, // 0x00000022
      Point_Spawn_Locker = 35, // 0x00000023
      Point_Spawn_BigWoodCrate = 36, // 0x00000024
      Point_Spawn_Cooler = 37, // 0x00000025
      Point_Spawn_Buybuddy = 38, // 0x00000026
      ZosigSpawn_Basic = 40, // 0x00000028
      ZosigSpawn_Blut = 41, // 0x00000029
      ZosigSpawn_Spitter = 42, // 0x0000002A
      ZosigSpawn_Exploding = 43, // 0x0000002B
      ZosigSpawn_Runner = 44, // 0x0000002C
      ZosigSpawn_Armored = 45, // 0x0000002D
    }
  }
}
