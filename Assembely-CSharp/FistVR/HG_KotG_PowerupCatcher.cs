using UnityEngine;

namespace FistVR
{
	public class HG_KotG_PowerupCatcher : MonoBehaviour
	{
		public HG_ModeManager_KingOfTheGrill M;

		public GameObject SpawnOnPowerup;

		private void OnTriggerEnter(Collider col)
		{
			TestCollider(col);
		}

		private void TestCollider(Collider col)
		{
			if (!(col.attachedRigidbody == null))
			{
				RW_Powerup component = col.attachedRigidbody.GetComponent<RW_Powerup>();
				if (!(component == null) && !(component.QuickbeltSlot != null) && component.Cooked)
				{
					M.DepositPowerUp(col.attachedRigidbody.GetComponent<RW_Powerup>().PowerupType);
					Object.Instantiate(SpawnOnPowerup, base.transform.position, base.transform.rotation);
					Object.Destroy(col.attachedRigidbody.gameObject);
				}
			}
		}
	}
}
