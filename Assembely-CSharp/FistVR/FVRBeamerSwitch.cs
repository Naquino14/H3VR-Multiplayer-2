using UnityEngine;

namespace FistVR
{
	public class FVRBeamerSwitch : FVRInteractiveObject
	{
		[Header("Beamer Switch Config")]
		public FVRBeamer Beamer;

		public GBeamer GBeamer;

		public int SwitchIndex;

		private bool m_switchedOn;

		public AudioEvent AudEvent_Switch;

		private AudioSource aud;

		protected override void Awake()
		{
			base.Awake();
			m_switchedOn = false;
			aud = GetComponent<AudioSource>();
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			ToggleSwitch(isHandInteraction: true);
		}

		public void ToggleSwitch(bool isHandInteraction)
		{
			m_switchedOn = !m_switchedOn;
			if (aud == null)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Switch, base.transform.position);
			}
			else
			{
				aud.Play();
			}
			if (m_switchedOn)
			{
				base.transform.localEulerAngles = new Vector3(0f, 0f, 20f);
				if (Beamer != null)
				{
					Beamer.SetSwitchState(SwitchIndex, b: true);
				}
				if (GBeamer != null)
				{
					GBeamer.SetSwitchState(SwitchIndex, b: true);
				}
			}
			else
			{
				base.transform.localEulerAngles = new Vector3(0f, 0f, -20f);
				if (Beamer != null)
				{
					Beamer.SetSwitchState(SwitchIndex, b: false);
				}
				if (GBeamer != null)
				{
					GBeamer.SetSwitchState(SwitchIndex, b: false);
				}
			}
		}
	}
}
