using UnityEngine;

namespace FistVR
{
	public class MF_ZonePoint : MonoBehaviour
	{
		public enum ZoneType
		{
			None,
			Assault,
			Support,
			Sniping
		}

		public MF_Zone Z;

		public ZoneType T;

		private void OnDrawGizmos()
		{
			if (Z.DrawGiz)
			{
				switch (T)
				{
				case ZoneType.None:
					Gizmos.color = new Color(1f, 1f, 1f, 1f);
					Gizmos.DrawSphere(base.transform.position, 0.15f);
					Gizmos.DrawWireSphere(base.transform.position, 0.15f);
					break;
				case ZoneType.Assault:
					Gizmos.color = new Color(1f, 0.3f, 0.3f, 1f);
					Gizmos.DrawSphere(base.transform.position, 0.15f);
					Gizmos.DrawWireSphere(base.transform.position, 0.15f);
					break;
				case ZoneType.Support:
					Gizmos.color = new Color(0.3f, 1f, 0.3f, 1f);
					Gizmos.DrawSphere(base.transform.position, 0.15f);
					Gizmos.DrawWireSphere(base.transform.position, 0.15f);
					break;
				case ZoneType.Sniping:
					Gizmos.color = new Color(0.3f, 0.3f, 1f, 1f);
					Gizmos.DrawSphere(base.transform.position, 0.15f);
					Gizmos.DrawWireSphere(base.transform.position, 0.15f);
					break;
				}
			}
		}
	}
}
