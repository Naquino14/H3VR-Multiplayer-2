using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class BalloonHapticBump : MonoBehaviour
	{
		public GameObject physParent;

		private void OnCollisionEnter(Collision other)
		{
			Balloon componentInParent = other.collider.GetComponentInParent<Balloon>();
			if (componentInParent != null)
			{
				Hand componentInParent2 = physParent.GetComponentInParent<Hand>();
				if (componentInParent2 != null)
				{
					componentInParent2.TriggerHapticPulse(500);
				}
			}
		}
	}
}
