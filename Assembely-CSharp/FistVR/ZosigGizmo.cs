using UnityEngine;

namespace FistVR
{
	public class ZosigGizmo : MonoBehaviour
	{
		public enum ZosigGizmosType
		{
			None = 1,
			CivvieWiener = 0,
			Herb_Katchup = 10,
			Herb_Mustard = 11,
			Herb_Pickle = 12,
			Herb_Blue = 13,
			Herb_Eggplant = 14,
			Volume_BringItemQuest = 20,
			Volume_MoodChange = 21,
			Volume_ZosigSpawn = 22,
			Volume_MusicChange = 23,
			Volume_Ambient = 24,
			Point_Spawn_BareItem = 30,
			Point_Spawn_BareItemReward = 0x1F,
			Point_Spawn_Test = 0x20,
			Point_Spawn_GunCase = 33,
			Point_Spawn_DestroyableBox = 34,
			Point_Spawn_Locker = 35,
			Point_Spawn_BigWoodCrate = 36,
			Point_Spawn_Cooler = 37,
			Point_Spawn_Buybuddy = 38,
			ZosigSpawn_Basic = 40,
			ZosigSpawn_Blut = 41,
			ZosigSpawn_Spitter = 42,
			ZosigSpawn_Exploding = 43,
			ZosigSpawn_Runner = 44,
			ZosigSpawn_Armored = 45
		}

		[InspectorButton("Align")]
		public bool DoAlign;

		public ZosigGizmosType Type;

		public BoxCollider Collider;

		public Mesh GunCaseBase;

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
			{
				Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.localScale);
				Matrix4x4 matrix = Gizmos.matrix;
				switch (Type)
				{
				case ZosigGizmosType.CivvieWiener:
					Gizmos.color = new Color(1f, 0.8f, 0.5f);
					Gizmos.DrawCube(base.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
					break;
				case ZosigGizmosType.Herb_Katchup:
					Gizmos.color = new Color(1f, 0.7f, 0.7f);
					Gizmos.DrawSphere(base.transform.position + Vector3.up * 0.2f, 0.4f);
					Gizmos.DrawWireSphere(base.transform.position + Vector3.up * 0.2f, 0.4f);
					break;
				case ZosigGizmosType.Herb_Mustard:
					Gizmos.color = new Color(1f, 1f, 0.7f);
					Gizmos.DrawSphere(base.transform.position + Vector3.up * 0.2f, 0.4f);
					Gizmos.DrawWireSphere(base.transform.position + Vector3.up * 0.2f, 0.4f);
					break;
				case ZosigGizmosType.Herb_Pickle:
					Gizmos.color = new Color(0.7f, 1f, 0.7f);
					Gizmos.DrawSphere(base.transform.position + Vector3.up * 0.2f, 0.4f);
					Gizmos.DrawWireSphere(base.transform.position + Vector3.up * 0.2f, 0.4f);
					break;
				case ZosigGizmosType.Herb_Blue:
					Gizmos.color = new Color(0.7f, 0.7f, 1f);
					Gizmos.DrawSphere(base.transform.position + Vector3.up * 0.2f, 0.4f);
					Gizmos.DrawWireSphere(base.transform.position + Vector3.up * 0.2f, 0.4f);
					break;
				case ZosigGizmosType.Herb_Eggplant:
					Gizmos.color = new Color(1f, 0.7f, 1f);
					Gizmos.DrawSphere(base.transform.position + Vector3.up * 0.2f, 0.4f);
					Gizmos.DrawWireSphere(base.transform.position + Vector3.up * 0.2f, 0.4f);
					break;
				case ZosigGizmosType.Volume_BringItemQuest:
					Gizmos.color = Color.yellow;
					Gizmos.matrix *= matrix4x;
					Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
					Gizmos.matrix = matrix;
					break;
				case ZosigGizmosType.Volume_MoodChange:
					Gizmos.color = new Color(0.85f, 0.85f, 1f, 0.6f);
					Gizmos.matrix *= matrix4x;
					Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
					Gizmos.matrix = matrix;
					break;
				case ZosigGizmosType.Volume_ZosigSpawn:
					Gizmos.color = new Color(1f, 0.25f, 0.25f, 0.6f);
					Gizmos.matrix *= matrix4x;
					Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
					Gizmos.matrix = matrix;
					break;
				case ZosigGizmosType.Volume_MusicChange:
					Gizmos.color = new Color(0.25f, 1f, 0.25f, 0.6f);
					Gizmos.matrix *= matrix4x;
					Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
					Gizmos.matrix = matrix;
					break;
				case ZosigGizmosType.Volume_Ambient:
					Gizmos.color = new Color(0.25f, 0.25f, 1f, 0.6f);
					Gizmos.matrix *= matrix4x;
					Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
					Gizmos.matrix = matrix;
					break;
				case ZosigGizmosType.Point_Spawn_BareItem:
					Gizmos.color = Color.magenta;
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.2f, base.transform.position - Vector3.up * 0.2f);
					Gizmos.DrawLine(base.transform.position + Vector3.right * 0.2f, base.transform.position - Vector3.right * 0.2f);
					Gizmos.DrawLine(base.transform.position + Vector3.forward * 0.2f, base.transform.position - Vector3.forward * 0.2f);
					break;
				case ZosigGizmosType.Point_Spawn_BareItemReward:
					Gizmos.color = new Color(0f, 0.35f, 1f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.2f, base.transform.position - Vector3.up * 0.2f);
					Gizmos.DrawLine(base.transform.position + Vector3.right * 0.2f, base.transform.position - Vector3.right * 0.2f);
					Gizmos.DrawLine(base.transform.position + Vector3.forward * 0.2f, base.transform.position - Vector3.forward * 0.2f);
					break;
				case ZosigGizmosType.Point_Spawn_GunCase:
					Gizmos.color = new Color(1f, 0.6f, 0f);
					Gizmos.DrawMesh(GunCaseBase, base.transform.position, base.transform.rotation);
					break;
				case ZosigGizmosType.Point_Spawn_DestroyableBox:
					Gizmos.color = new Color(0.6f, 1f, 0f);
					Gizmos.DrawMesh(GunCaseBase, base.transform.position, base.transform.rotation);
					break;
				case ZosigGizmosType.Point_Spawn_Locker:
					Gizmos.color = new Color(1f, 0.75f, 0.2f, 1f);
					Gizmos.matrix *= matrix4x;
					Gizmos.DrawCube(Vector3.up, new Vector3(0.43f, 2f, 0.53f));
					Gizmos.DrawWireCube(Vector3.up, new Vector3(0.43f, 2f, 0.53f));
					Gizmos.matrix = matrix;
					break;
				case ZosigGizmosType.Point_Spawn_BigWoodCrate:
					Gizmos.color = new Color(0.5f, 1f, 0.2f, 1f);
					Gizmos.matrix *= matrix4x;
					Gizmos.DrawCube(Vector3.up * 0.405f, new Vector3(2f, 0.81f, 1.14f));
					Gizmos.DrawWireCube(Vector3.up * 0.405f, new Vector3(2f, 0.81f, 1.14f));
					Gizmos.matrix = matrix;
					break;
				case ZosigGizmosType.Point_Spawn_Cooler:
					Gizmos.color = new Color(1f, 0.2f, 0.2f, 1f);
					Gizmos.matrix *= matrix4x;
					Gizmos.DrawCube(Vector3.up * 0.215f, new Vector3(0.5f, 0.43f, 0.34f));
					Gizmos.DrawWireCube(Vector3.up * 0.215f, new Vector3(0.5f, 0.43f, 0.34f));
					Gizmos.matrix = matrix;
					break;
				case ZosigGizmosType.Point_Spawn_Buybuddy:
					Gizmos.color = new Color(0.2f, 1f, 0.4f, 1f);
					Gizmos.matrix *= matrix4x;
					Gizmos.DrawCube(Vector3.up * 0.15f, new Vector3(0.3f, 0.3f, 0.3f));
					Gizmos.DrawWireCube(Vector3.up * 0.15f, new Vector3(0.3f, 0.3f, 0.3f));
					Gizmos.matrix = matrix;
					break;
				case ZosigGizmosType.ZosigSpawn_Basic:
					Gizmos.color = new Color(0.1f, 1f, 0.1f, 1f);
					Gizmos.DrawCube(base.transform.position + Vector3.up, new Vector3(0.15f, 2f, 0.15f));
					Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.up * 2f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.5f + Vector3.right * 0.2f, base.transform.position + Vector3.up * 0.5f - Vector3.right * 0.2f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.5f + Vector3.forward * 0.2f, base.transform.position + Vector3.up * 0.5f - Vector3.forward * 0.2f);
					break;
				case ZosigGizmosType.ZosigSpawn_Blut:
					Gizmos.color = new Color(1f, 0.3f, 0.3f, 1f);
					Gizmos.DrawCube(base.transform.position + Vector3.up, new Vector3(0.15f, 2f, 0.15f));
					Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.up * 2f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.5f + Vector3.right * 0.2f, base.transform.position + Vector3.up * 0.5f - Vector3.right * 0.2f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.5f + Vector3.forward * 0.2f, base.transform.position + Vector3.up * 0.5f - Vector3.forward * 0.2f);
					break;
				case ZosigGizmosType.ZosigSpawn_Spitter:
					Gizmos.color = new Color(1f, 1f, 0.1f, 1f);
					Gizmos.DrawCube(base.transform.position + Vector3.up, new Vector3(0.15f, 2f, 0.15f));
					Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.up * 2f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.5f + Vector3.right * 0.2f, base.transform.position + Vector3.up * 0.5f - Vector3.right * 0.2f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.5f + Vector3.forward * 0.2f, base.transform.position + Vector3.up * 0.5f - Vector3.forward * 0.2f);
					break;
				case ZosigGizmosType.ZosigSpawn_Exploding:
					Gizmos.color = new Color(1f, 0.1f, 1f, 1f);
					Gizmos.DrawCube(base.transform.position + Vector3.up, new Vector3(0.15f, 2f, 0.15f));
					Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.up * 2f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.5f + Vector3.right * 0.2f, base.transform.position + Vector3.up * 0.5f - Vector3.right * 0.2f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.5f + Vector3.forward * 0.2f, base.transform.position + Vector3.up * 0.5f - Vector3.forward * 0.2f);
					break;
				case ZosigGizmosType.ZosigSpawn_Runner:
					Gizmos.color = new Color(0.1f, 0.7f, 1f, 1f);
					Gizmos.DrawCube(base.transform.position + Vector3.up, new Vector3(0.15f, 2f, 0.15f));
					Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.up * 2f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.5f + Vector3.right * 0.2f, base.transform.position + Vector3.up * 0.5f - Vector3.right * 0.2f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.5f + Vector3.forward * 0.2f, base.transform.position + Vector3.up * 0.5f - Vector3.forward * 0.2f);
					break;
				case ZosigGizmosType.ZosigSpawn_Armored:
					Gizmos.color = new Color(1f, 0f, 0f, 1f);
					Gizmos.DrawCube(base.transform.position + Vector3.up, new Vector3(0.15f, 2f, 0.15f));
					Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.up * 2f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.5f + Vector3.right * 0.2f, base.transform.position + Vector3.up * 0.5f - Vector3.right * 0.2f);
					Gizmos.DrawLine(base.transform.position + Vector3.up * 0.5f + Vector3.forward * 0.2f, base.transform.position + Vector3.up * 0.5f - Vector3.forward * 0.2f);
					break;
				}
			}
		}

		[ContextMenu("Align")]
		public void Align()
		{
			if (Physics.Raycast(base.transform.position + Vector3.up, -Vector3.up, out var hitInfo))
			{
				Vector3 forward = Vector3.ProjectOnPlane(base.transform.forward, hitInfo.normal);
				base.transform.rotation = Quaternion.LookRotation(forward, hitInfo.normal);
				base.transform.position = hitInfo.point;
			}
		}
	}
}
