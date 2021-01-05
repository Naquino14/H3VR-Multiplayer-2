using UnityEngine;

namespace FistVR
{
	public class FlintlockFlintHolder : FVRInteractiveObject
	{
		public FlintlockFlintScrew Screw;

		public GameObject FlintPrefab;

		public AudioEvent AudEvent_Remove;

		public AudioEvent AudEvent_Replace;

		public Transform FlintPos;

		private float TimeTilFlintReplace = 1f;

		public override bool IsInteractable()
		{
			if (Screw.SState == FlintlockFlintScrew.ScrewState.Unscrewed)
			{
				return true;
			}
			return false;
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			if (Screw.Weapon.HasFlint())
			{
				Vector3 u = Screw.Weapon.RemoveFlint();
				ExtractFlint(hand, u);
				Screw.Weapon.PlayAudioAsHandling(AudEvent_Remove, base.transform.position);
			}
			base.SimpleInteraction(hand);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (TimeTilFlintReplace > 0f)
			{
				TimeTilFlintReplace -= Time.deltaTime;
			}
		}

		private void ExtractFlint(FVRViveHand h, Vector3 u)
		{
			TimeTilFlintReplace = 1f;
			GameObject gameObject = Object.Instantiate(FlintPrefab, FlintPos.position, FlintPos.rotation);
			FlintlockFlint component = gameObject.GetComponent<FlintlockFlint>();
			component.m_flintUses = u;
			component.UpdateState();
			h.ForceSetInteractable(component);
			component.BeginInteraction(h);
		}

		public void OnTriggerEnter(Collider other)
		{
			if (!(TimeTilFlintReplace > 0f) && !Screw.Weapon.HasFlint() && !(other.attachedRigidbody == null))
			{
				GameObject gameObject = other.attachedRigidbody.gameObject;
				if (gameObject.CompareTag("flintlock_flint"))
				{
					FlintlockFlint component = gameObject.GetComponent<FlintlockFlint>();
					Screw.Weapon.AddFlint(component.m_flintUses);
					Screw.Weapon.PlayAudioAsHandling(AudEvent_Replace, base.transform.position);
					Object.Destroy(other.gameObject);
				}
			}
		}
	}
}
