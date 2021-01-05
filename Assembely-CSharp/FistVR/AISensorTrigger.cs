using UnityEngine;

namespace FistVR
{
	public class AISensorTrigger : MonoBehaviour
	{
		public AISensor MySensor;

		private void OnTriggerStay(Collider other)
		{
			MySensor.TriggerProxy(other);
		}
	}
}
