using UnityEngine;

namespace FistVR
{
	public class SparklerTrigger : MonoBehaviour
	{
		public FVRSparkler Sparkler;

		public Collider collider;

		private void OnTriggerEnter(Collider col)
		{
			collider.enabled = false;
			collider.gameObject.layer = LayerMask.NameToLayer("NoCol");
			Sparkler.Ignite();
		}
	}
}
