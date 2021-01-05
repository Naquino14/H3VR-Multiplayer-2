using UnityEngine;

namespace FistVR
{
	public class BangerSwitch : FVRInteractiveObject
	{
		public Banger Banger;

		public Transform SwitchPiece;

		public Vector3 Eulers_Off;

		public Vector3 Eulers_On;

		private bool m_isOn;

		public AudioEvent AudEven_Switch;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			ToggleSwitch();
		}

		public void ToggleSwitch()
		{
			m_isOn = !m_isOn;
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEven_Switch, base.transform.position);
			if (m_isOn)
			{
				Banger.Arm();
				SwitchPiece.localEulerAngles = Eulers_On;
			}
			else
			{
				Banger.DeArm();
				SwitchPiece.localEulerAngles = Eulers_Off;
			}
		}
	}
}
