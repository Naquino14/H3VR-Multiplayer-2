using UnityEngine;

namespace FistVR
{
	public class wwAmmoTrigger : FVRInteractiveObject
	{
		public FVRObject AmmoToSpawn;

		public Transform Spinner;

		private float spin;

		private bool m_hasSpinner;

		public new void Start()
		{
			if (Spinner != null)
			{
				m_hasSpinner = true;
			}
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			GameObject gameObject = Object.Instantiate(AmmoToSpawn.GetGameObject(), base.transform.position, base.transform.rotation);
			FVRPhysicalObject component = gameObject.GetComponent<FVRPhysicalObject>();
			hand.ForceSetInteractable(component);
			component.BeginInteraction(hand);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_hasSpinner)
			{
				spin = Mathf.Repeat(spin + Time.deltaTime * 180f, 360f);
				Spinner.transform.localEulerAngles = new Vector3(0f, spin, 0f);
			}
		}
	}
}
