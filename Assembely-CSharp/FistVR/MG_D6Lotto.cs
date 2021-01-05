using UnityEngine;

namespace FistVR
{
	public class MG_D6Lotto : FVRPhysicalObject
	{
		public RedRoom m_room;

		private bool m_hasBeenPickedUp;

		private bool m_hasBeenReleased;

		private bool m_hasBeenRolled;

		public Transform[] Facings;

		public Rigidbody[] Shards;

		public GameObject[] Spawns;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			m_hasBeenPickedUp = true;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			m_hasBeenReleased = true;
			base.RootRigidbody.angularVelocity = Random.onUnitSphere * 5f;
		}

		public override bool IsInteractable()
		{
			if (m_hasBeenReleased)
			{
				return false;
			}
			return base.IsInteractable();
		}

		public override bool IsDistantGrabbable()
		{
			return false;
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (m_hasBeenReleased && base.RootRigidbody.velocity.magnitude < 0.1f && base.RootRigidbody.angularVelocity.magnitude < 0.1f)
			{
				base.RootRigidbody.isKinematic = true;
				CheckFacingAndRoll();
			}
		}

		private void CheckFacingAndRoll()
		{
			int facing = 0;
			float num = 180f;
			for (int i = 1; i < Facings.Length; i++)
			{
				float num2 = Vector3.Angle(Facings[i].forward, Vector3.up);
				if (num2 < num)
				{
					facing = i;
					num = num2;
				}
			}
			SpawnBasedOnFacing(facing);
		}

		private void SpawnBasedOnFacing(int facing)
		{
			m_hasBeenRolled = true;
			for (int i = 0; i < Shards.Length; i++)
			{
				Shards[i].transform.SetParent(null);
				Shards[i].gameObject.SetActive(value: true);
			}
			FVRFireArm fVRFireArm = null;
			FVRFireArmMagazine fVRFireArmMagazine = null;
			FVRPhysicalObject fVRPhysicalObject = null;
			switch (facing)
			{
			case 4:
				GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LTEntry_Handgun3, base.transform.position + Vector3.up * 0.3f, Random.rotation);
				GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Handgun3, base.transform.position + Vector3.up * 0.5f, Random.rotation);
				break;
			case 5:
				GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LTEntry_Shotgun3, base.transform.position + Vector3.up * 0.3f, Random.rotation);
				GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Shotgun3, base.transform.position + Vector3.up * 0.5f, Random.rotation);
				break;
			case 6:
				GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LTEntry_RareGun3, base.transform.position + Vector3.up * 0.3f, Random.rotation);
				GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_RareGun3, base.transform.position + Vector3.up * 0.5f, Random.rotation);
				break;
			}
			if (facing == 1)
			{
				Object.Instantiate(GM.MGMaster.FlyingHotDogSwarmPrefab, base.transform.position, Quaternion.identity);
			}
			for (int j = 0; j < Spawns.Length; j++)
			{
				Object.Instantiate(Spawns[j], base.transform.position, base.transform.rotation);
			}
			m_room.OpenDoors(playSound: true);
			Object.Destroy(base.gameObject);
		}
	}
}
