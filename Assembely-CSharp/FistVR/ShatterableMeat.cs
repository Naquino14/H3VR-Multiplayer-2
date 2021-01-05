using UnityEngine;

namespace FistVR
{
	public class ShatterableMeat : FVRPhysicalObject
	{
		public GameObject Splosion;

		public bool DoesDamage;

		public bool Explode()
		{
			if (m_hand != null)
			{
				m_hand.EndInteractionIfHeld(this);
			}
			Object.Instantiate(Splosion, base.transform.position, base.transform.rotation);
			Object.Destroy(base.gameObject);
			return DoesDamage;
		}
	}
}
