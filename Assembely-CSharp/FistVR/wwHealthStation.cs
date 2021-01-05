using UnityEngine;

namespace FistVR
{
	public class wwHealthStation : MonoBehaviour
	{
		public ParticleSystem PSystem;

		private void OnTriggerEnter()
		{
			if (GM.GetPlayerHealth() < 1f)
			{
				Heal();
			}
		}

		public void Heal()
		{
			PSystem.Emit(200);
			ManagerSingleton<GM>.Instance.ResetPlayer();
		}
	}
}
