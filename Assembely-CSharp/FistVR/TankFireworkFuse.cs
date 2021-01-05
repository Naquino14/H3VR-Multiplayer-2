using UnityEngine;

namespace FistVR
{
	public class TankFireworkFuse : MonoBehaviour
	{
		public TankFirework Firework;

		private void OnTriggerEnter()
		{
			Firework.Ignite();
		}
	}
}
