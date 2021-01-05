using UnityEngine;

namespace FistVR
{
	public class HealthPickUp : MonoBehaviour
	{
		public GameObject Spawn;

		public GameObject Root;

		public bool m_isPartialHeal;

		public float m_partialHealAmount = 0.1f;

		private bool m_hasDone;

		private void Update()
		{
			float a = Vector3.Distance(GM.CurrentPlayerBody.LeftHand.position, base.transform.position);
			a = Mathf.Min(a, Vector3.Distance(GM.CurrentPlayerBody.RightHand.position, base.transform.position));
			if (a < 0.25f)
			{
				Boom();
			}
		}

		private void Boom()
		{
			if (m_hasDone)
			{
				return;
			}
			m_hasDone = true;
			if (GM.GetPlayerHealth() < 1f)
			{
				if (m_isPartialHeal)
				{
					GM.CurrentPlayerBody.HealPercent(m_partialHealAmount);
				}
				else
				{
					GM.CurrentPlayerBody.ResetHealth();
				}
				Object.Instantiate(Spawn, base.transform.position, base.transform.rotation);
				Object.Destroy(Root);
			}
		}
	}
}
