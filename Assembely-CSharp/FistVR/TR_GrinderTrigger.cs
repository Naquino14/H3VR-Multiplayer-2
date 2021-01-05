using UnityEngine;

namespace FistVR
{
	public class TR_GrinderTrigger : MonoBehaviour
	{
		private void OnTriggerStay(Collider col)
		{
			FVRPlayerHitbox component = col.GetComponent<FVRPlayerHitbox>();
			if (component != null)
			{
				DamageDealt dam = default(DamageDealt);
				dam.force = Vector3.zero;
				dam.PointsDamage = 2000f;
				dam.hitNormal = Vector3.zero;
				dam.IsInside = false;
				dam.MPa = 1f;
				dam.MPaRootMeter = 1f;
				dam.point = base.transform.position;
				dam.ShotOrigin = null;
				dam.strikeDir = Vector3.zero;
				dam.uvCoords = Vector2.zero;
				dam.IsInitialContact = true;
				component.Damage(dam);
			}
		}
	}
}
