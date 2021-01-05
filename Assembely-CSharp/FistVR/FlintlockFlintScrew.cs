using UnityEngine;

namespace FistVR
{
	public class FlintlockFlintScrew : FVRInteractiveObject
	{
		public enum ScrewState
		{
			Screwed,
			Screwing,
			Unscrewed,
			Unscrewing
		}

		public FlintlockWeapon Weapon;

		public Transform Screw;

		public ScrewState SState;

		private float lerp;

		public Vector2 Heights = new Vector2(0.05455612f, 0.06048f);

		public AudioEvent AudEvent_Screw;

		public AudioEvent AudEvent_Unscrew;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			ToggleScrewState();
		}

		public override bool IsInteractable()
		{
			if (SState == ScrewState.Screwing || SState == ScrewState.Unscrewing)
			{
				return false;
			}
			return base.IsInteractable();
		}

		private void ToggleScrewState()
		{
			if (SState == ScrewState.Screwed)
			{
				Weapon.PlayAudioAsHandling(AudEvent_Unscrew, base.transform.position);
				SState = ScrewState.Unscrewing;
				lerp = 0f;
			}
			else if (SState == ScrewState.Unscrewed)
			{
				Weapon.PlayAudioAsHandling(AudEvent_Screw, base.transform.position);
				SState = ScrewState.Screwing;
				lerp = 0f;
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (SState == ScrewState.Screwing)
			{
				lerp += Time.deltaTime * 1.4f;
				Screw.localPosition = new Vector3(Screw.localPosition.x, Mathf.Lerp(Heights.y, Heights.x, lerp), Screw.localPosition.z);
				Screw.localEulerAngles = new Vector3(90f, Mathf.Lerp(0f, 720f, lerp), 0f);
				if (lerp >= 1f)
				{
					SState = ScrewState.Screwed;
				}
			}
			else if (SState == ScrewState.Unscrewing)
			{
				lerp += Time.deltaTime * 1.4f;
				Screw.localPosition = new Vector3(Screw.localPosition.x, Mathf.Lerp(Heights.x, Heights.y, lerp), Screw.localPosition.z);
				Screw.localEulerAngles = new Vector3(90f, Mathf.Lerp(720f, 0f, lerp), 0f);
				if (lerp >= 1f)
				{
					SState = ScrewState.Unscrewed;
				}
			}
		}
	}
}
