using UnityEngine;

namespace FistVR
{
	public class TacticalFlashlight : FVRFireArmAttachmentInterface
	{
		private bool IsOn;

		public GameObject LightParts;

		public AudioEvent AudEvent_LaserOnClip;

		public AudioEvent AudEvent_LaserOffClip;

		public AlloyAreaLight FlashlightLight;

		public AudioSource BackUpAudio;

		public override void OnAttach()
		{
			base.OnAttach();
		}

		public override void OnDetach()
		{
			base.OnDetach();
			IsOn = false;
			LightParts.SetActive(IsOn);
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

		private void ToggleOn()
		{
			IsOn = !IsOn;
			LightParts.SetActive(IsOn);
			if (IsOn)
			{
				if (GM.CurrentSceneSettings.IsSceneLowLight)
				{
					FlashlightLight.Intensity = 2f;
				}
				else
				{
					FlashlightLight.Intensity = 0.5f;
				}
			}
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
