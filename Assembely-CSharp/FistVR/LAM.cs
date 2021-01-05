using UnityEngine;

namespace FistVR
{
	public class LAM : FVRFireArmAttachmentInterface
	{
		public enum LAMState
		{
			Off,
			Laser,
			Light,
			LaserLight
		}

		public LAMState LState;

		public AudioEvent AudEvent_LAMON;

		public AudioEvent AudEvent_LAMOFF;

		[Header("LaserPart")]
		public GameObject BeamEffect;

		public GameObject BeamHitPoint;

		public Transform Aperture;

		public LayerMask LM;

		private RaycastHit m_hit;

		[Header("LightPart")]
		public GameObject LightParts;

		public AlloyAreaLight FlashlightLight;

		private void CycleMode()
		{
			int lState = (int)LState;
			lState++;
			if (lState > 3)
			{
				lState = 0;
			}
			LState = (LAMState)lState;
			if (LState == LAMState.Off)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_LAMOFF, base.transform.position);
			}
			else
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_LAMON, base.transform.position);
			}
			if (LState == LAMState.Laser || LState == LAMState.LaserLight)
			{
				BeamHitPoint.SetActive(value: true);
				BeamEffect.SetActive(value: true);
			}
			else
			{
				BeamHitPoint.SetActive(value: false);
				BeamEffect.SetActive(value: false);
			}
			if (LState == LAMState.Light || LState == LAMState.LaserLight)
			{
				LightParts.SetActive(value: true);
				if (GM.CurrentSceneSettings.IsSceneLowLight)
				{
					FlashlightLight.Intensity = 2f;
				}
				else
				{
					FlashlightLight.Intensity = 0.5f;
				}
			}
			else
			{
				LightParts.SetActive(value: false);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					CycleMode();
				}
			}
			else if (hand.Input.TouchpadDown && touchpadAxes.magnitude > 0.25f && Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
			{
				CycleMode();
			}
			base.UpdateInteraction(hand);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (LState == LAMState.Laser || LState == LAMState.LaserLight)
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
			LState = LAMState.Off;
			BeamHitPoint.SetActive(value: false);
			BeamEffect.SetActive(value: false);
		}
	}
}
