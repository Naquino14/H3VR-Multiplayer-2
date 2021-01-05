using UnityEngine;

namespace FistVR
{
	public class MG_DestroyableWithSpawn : FVRDestroyableObject
	{
		private MeatGrinderMaster m_master;

		public Transform SpawnPos;

		public float baseThreshold = 0.8f;

		public void SetMGMaster(MeatGrinderMaster master)
		{
			m_master = master;
		}

		public override void DestroyEvent()
		{
			if (!m_isDestroyed)
			{
				SpawnRandomLoot();
			}
			base.DestroyEvent();
		}

		private void SpawnRandomLoot()
		{
			float num = Random.Range(0f, 1f);
			if (num >= baseThreshold)
			{
				float num2 = Random.Range(0f, 1f);
				if (num2 > 0.95f)
				{
					GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Handgun1, SpawnPos);
				}
				else if (num2 > 0.9f)
				{
					GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Handgun2, SpawnPos);
				}
				else if (num2 > 0.85f)
				{
					GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Handgun3, SpawnPos);
				}
				else if (num2 > 0.8f)
				{
					GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Shotgun1, SpawnPos);
				}
				else if (num2 > 0.75f)
				{
					GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Shotgun2, SpawnPos);
				}
				else if (num2 > 0.7f)
				{
					GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Shotgun3, SpawnPos);
				}
				else if (num2 > 0.65f)
				{
					GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_RareGun1, SpawnPos);
				}
				else if (num2 > 0.6f)
				{
					GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_RareGun2, SpawnPos);
				}
				else if (num2 > 0.55f)
				{
					GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_RareGun3, SpawnPos);
				}
				else if (num2 > 0.3f)
				{
					GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LT_Melee.GetRandomObject(), SpawnPos);
				}
				else if (num2 > 0.1f)
				{
					GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LT_Attachments.GetRandomObject(), SpawnPos);
				}
				else
				{
					GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LT_Junk.GetRandomObject(), SpawnPos);
				}
			}
		}
	}
}
