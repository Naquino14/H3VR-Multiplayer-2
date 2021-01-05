using UnityEngine;

namespace FistVR
{
	public class MinigunBox : FVRFireArmMagazine
	{
		[Header("MinigunBox Config")]
		public FireArmRoundClass RoundClass;

		public GameObject ProjectilePrefab;

		public int NumBulletsLeft = 2000;

		public bool HasAmmo()
		{
			if (NumBulletsLeft > 0)
			{
				return true;
			}
			return false;
		}

		public new void RemoveRound()
		{
			if (!GM.CurrentSceneSettings.IsAmmoInfinite && !GM.CurrentPlayerBody.IsInfiniteAmmo)
			{
				NumBulletsLeft--;
			}
		}
	}
}
