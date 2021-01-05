using UnityEngine;

namespace FistVR
{
	public class HG_Gizmo : MonoBehaviour
	{
		public enum HGGizmosType
		{
			PlayerSpawn,
			SpawnPoint_Offense,
			SpawnPoint_Defense,
			TargetPoint,
			SpawnPoint_Civvie
		}

		public HG_Zone Zone;

		public HGGizmosType Type;

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying && Zone != null && Zone.DebugView)
			{
				Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.localScale);
				Matrix4x4 matrix = Gizmos.matrix;
				switch (Type)
				{
				case HGGizmosType.PlayerSpawn:
					Gizmos.matrix *= matrix4x;
					Gizmos.color = new Color(0.1f, 1f, 1f);
					Gizmos.DrawCube(Vector3.zero, new Vector3(0.35f, 0.2f, 0.35f));
					Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.35f, 0.2f, 0.35f));
					Gizmos.matrix = matrix;
					break;
				case HGGizmosType.SpawnPoint_Offense:
					Gizmos.color = new Color(1f, 0.1f, 0.1f);
					Gizmos.DrawCube(base.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
					Gizmos.DrawWireCube(base.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
					break;
				case HGGizmosType.SpawnPoint_Defense:
					Gizmos.color = new Color(0.1f, 1f, 0.1f);
					Gizmos.DrawCube(base.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
					Gizmos.DrawWireCube(base.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
					break;
				case HGGizmosType.TargetPoint:
					Gizmos.color = new Color(0.1f, 0.1f, 1f);
					Gizmos.DrawCube(base.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
					Gizmos.DrawWireCube(base.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
					break;
				case HGGizmosType.SpawnPoint_Civvie:
					Gizmos.color = new Color(1f, 1f, 0.1f);
					Gizmos.DrawCube(base.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
					Gizmos.DrawWireCube(base.transform.position + Vector3.up, new Vector3(0.35f, 2f, 0.35f));
					break;
				}
			}
		}
	}
}
