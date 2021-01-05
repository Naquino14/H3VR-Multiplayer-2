using UnityEngine;

namespace FistVR
{
	public class MM_Payout : MonoBehaviour
	{
		public float Tick = 3f;

		public int Reward;

		private void Start()
		{
			Invoke("Dead", Tick);
		}

		private void Dead()
		{
			GM.Omni.OmniUnlocks.GainCurrency(Reward);
			GM.Omni.SaveUnlocksToFile();
			Object.Destroy(base.gameObject);
		}
	}
}
