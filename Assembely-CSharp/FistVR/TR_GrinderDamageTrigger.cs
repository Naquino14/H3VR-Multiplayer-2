using UnityEngine;

namespace FistVR
{
	public class TR_GrinderDamageTrigger : MonoBehaviour
	{
		public TR_GrinderContoller GController;

		public TR_Grinder Grinder;

		private void OnTriggerEnter(Collider col)
		{
			if (!(col.attachedRigidbody != null))
			{
				return;
			}
			ShatterableMeat component = col.gameObject.GetComponent<ShatterableMeat>();
			if (component != null)
			{
				if (component.Explode())
				{
					Grinder.EmitEvent(col.transform.position);
					GController.DamageGrinder();
				}
			}
			else
			{
				col.attachedRigidbody.AddForce(-base.transform.right * 5f, ForceMode.Impulse);
			}
		}
	}
}
