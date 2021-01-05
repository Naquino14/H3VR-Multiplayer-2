using UnityEngine;

namespace FistVR
{
	public class Flashlight : FVRPhysicalObject
	{
		public bool IsOn;

		public GameObject LightParts;

		public AudioSource Aud;

		public AudioClip LaserOnClip;

		public AudioClip LaserOffClip;

		public AlloyAreaLight FlashlightLight;

		public Transform[] Poses;

		private int m_curPose;

		protected override void Awake()
		{
			base.Awake();
			LightParts.SetActive(IsOn);
			if (IsOn)
			{
				if (GM.CurrentSceneSettings.IsSceneLowLight)
				{
					FlashlightLight.Intensity = 0.9f;
				}
				else
				{
					FlashlightLight.Intensity = 0.5f;
				}
			}
			if (IsOn)
			{
				Aud.PlayOneShot(LaserOnClip, 1f);
			}
			else
			{
				Aud.PlayOneShot(LaserOffClip, 1f);
			}
		}

		private void CyclePose()
		{
			m_curPose++;
			if (m_curPose >= Poses.Length)
			{
				m_curPose = 0;
			}
			PoseOverride.localPosition = Poses[m_curPose].localPosition;
			PoseOverride.localRotation = Poses[m_curPose].localRotation;
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
			else if (hand.Input.TouchpadDown)
			{
				ToggleOn();
			}
			if (hand.Input.TriggerDown && m_hasTriggeredUpSinceBegin)
			{
				CyclePose();
			}
			base.UpdateInteraction(hand);
		}

		public void ToggleOn()
		{
			IsOn = !IsOn;
			LightParts.SetActive(IsOn);
			if (IsOn)
			{
				if (GM.CurrentSceneSettings.IsSceneLowLight)
				{
					FlashlightLight.Intensity = 0.9f;
				}
				else
				{
					FlashlightLight.Intensity = 0.5f;
				}
			}
			if (IsOn)
			{
				Aud.PlayOneShot(LaserOnClip, 1f);
			}
			else
			{
				Aud.PlayOneShot(LaserOffClip, 1f);
			}
		}
	}
}
