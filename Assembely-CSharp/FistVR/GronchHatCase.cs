using UnityEngine;

namespace FistVR
{
	public class GronchHatCase : FVRPhysicalObject
	{
		public Transform Lid;

		public Transform KeyTarget;

		public Transform SpawnPoint;

		public Transform FXPoint;

		public string HID;

		private bool m_isOpen;

		public Vector2 CaseLidRots = new Vector2(0f, 0f);

		public GameObject SpawnOnUnlock;

		public GameObject SpawnOnAlreadyHave;

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (!m_isOpen && base.IsHeld && m_hand.OtherHand.CurrentInteractable != null && m_hand.OtherHand.CurrentInteractable is GronchHatCaseKey)
			{
				GronchHatCaseKey gronchHatCaseKey = m_hand.OtherHand.CurrentInteractable as GronchHatCaseKey;
				bool flag = true;
				if (Vector3.Distance(gronchHatCaseKey.transform.position, KeyTarget.position) > 0.02f)
				{
					flag = false;
				}
				else if (Vector3.Angle(gronchHatCaseKey.transform.up, KeyTarget.up) > 10f)
				{
					flag = false;
				}
				else if (Vector3.Angle(gronchHatCaseKey.transform.forward, KeyTarget.forward) > 10f)
				{
					flag = false;
				}
				else if (flag)
				{
					Open(gronchHatCaseKey);
				}
			}
		}

		private void Open(GronchHatCaseKey k)
		{
			if (!m_isOpen)
			{
				m_isOpen = true;
				Lid.localEulerAngles = new Vector3(CaseLidRots.y, 0f, 0f);
				GM.MMFlags.AddHat(HID);
				GM.MMFlags.SaveToFile();
				if (GM.Rewards.RewardUnlocks.IsRewardUnlocked(HID))
				{
					Object.Instantiate(SpawnOnAlreadyHave, FXPoint.position, FXPoint.rotation);
				}
				else
				{
					GM.Rewards.RewardUnlocks.UnlockReward(HID);
					Object.Instantiate(SpawnOnAlreadyHave, FXPoint.position, FXPoint.rotation);
				}
				GM.Rewards.SaveToFile();
				if (IM.HasSpawnedID(HID))
				{
					ItemSpawnerID spawnerID = IM.GetSpawnerID(HID);
					FVRObject mainObject = spawnerID.MainObject;
					Object.Instantiate(mainObject.GetGameObject(), SpawnPoint.position, SpawnPoint.rotation);
				}
				SMEME sMEME = Object.FindObjectOfType<SMEME>();
				sMEME.UpdateInventory();
				sMEME.DrawInventory();
				Object.Destroy(k.gameObject);
			}
		}
	}
}
