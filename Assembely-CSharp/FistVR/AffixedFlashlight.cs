using UnityEngine;

namespace FistVR
{
	public class AffixedFlashlight : FVRInteractiveObject
	{
		private bool IsOn;

		public GameObject LightParts;

		public AudioEvent AudEvent_LaserOnClip;

		public AudioEvent AudEvent_LaserOffClip;

		public Light FlashlightLight;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			ToggleOn();
		}

		private void ToggleOn()
		{
			IsOn = !IsOn;
			LightParts.SetActive(IsOn);
			if (IsOn)
			{
				if (GM.CurrentSceneSettings.IsSceneLowLight)
				{
					FlashlightLight.intensity = 2f;
				}
				else
				{
					FlashlightLight.intensity = 0.5f;
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
