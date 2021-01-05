using UnityEngine;

namespace FistVR
{
	public class MR_DamageOnEnter : MonoBehaviour
	{
		public int DamageP = 2;

		public bool m_isCheaterDamage;

		private void OnTriggerEnter(Collider col)
		{
			FVRPlayerHitbox component = col.GetComponent<FVRPlayerHitbox>();
			if (component != null)
			{
				Damage damage = new Damage();
				damage.strikeDir = -Vector3.up;
				damage.hitNormal = Vector3.up;
				damage.point = base.transform.position;
				damage.Dam_Piercing = DamageP;
				damage.Dam_TotalKinetic = DamageP;
				damage.Class = Damage.DamageClass.Abstract;
				component.Damage(damage);
				if (m_isCheaterDamage)
				{
					GM.MGMaster.Narrator.PlayDiedCheating();
				}
			}
		}
	}
}
