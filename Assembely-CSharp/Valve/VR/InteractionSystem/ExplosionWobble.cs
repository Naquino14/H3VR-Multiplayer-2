using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class ExplosionWobble : MonoBehaviour
	{
		public void ExplosionEvent(Vector3 explosionPos)
		{
			Rigidbody component = GetComponent<Rigidbody>();
			if ((bool)component)
			{
				component.AddExplosionForce(2000f, explosionPos, 10f);
			}
		}
	}
}
