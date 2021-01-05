using UnityEngine;

namespace FistVR
{
	public class LaserPointer : FVRFireArmAttachmentInterface
	{
		private bool IsOn;

		public GameObject BeamEffect;

		public GameObject BeamHitPoint;

		public Transform Aperture;

		public LayerMask LM;

		private RaycastHit m_hit;

		public AudioEvent AudEvent_LaserOnClip;

		public AudioEvent AudEvent_LaserOffClip;

		protected override void Awake()
		{
			base.Awake();
			BeamHitPoint.transform.SetParent(null);
		}

		public override void OnDestroy()
		{
			Object.Destroy(BeamHitPoint);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					ToggleOn();
				}
			}
			else if (hand.Input.TouchpadDown && touchpadAxes.magnitude > 0.25f && Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
			{
				ToggleOn();
			}
			base.UpdateInteraction(hand);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (IsOn)
			{
				Vector3 position = Aperture.position + Aperture.forward * 100f;
				float num = 100f;
				if (Physics.Raycast(Aperture.position, Aperture.forward, out m_hit, 100f, LM, QueryTriggerInteraction.Ignore))
				{
					position = m_hit.point;
					num = m_hit.distance;
				}
				float t = num * 0.01f;
				float num2 = Mathf.Lerp(0.01f, 0.2f, t);
				BeamHitPoint.transform.position = position;
				BeamHitPoint.transform.localScale = new Vector3(num2, num2, num2);
			}
		}

		public override void OnDetach()
		{
			base.OnDetach();
			IsOn = false;
			BeamHitPoint.SetActive(IsOn);
			BeamEffect.SetActive(IsOn);
		}

		private void ToggleOn()
		{
			IsOn = !IsOn;
			BeamHitPoint.SetActive(IsOn);
			BeamEffect.SetActive(IsOn);
			if (IsOn)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_LaserOnClip, base.transform.position);
			}
			else
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_LaserOffClip, base.transform.position);
			}
		}
	}
}
