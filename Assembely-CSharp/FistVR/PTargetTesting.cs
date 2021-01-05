using UnityEngine;

namespace FistVR
{
	public class PTargetTesting : MonoBehaviour
	{
		public PTargetRailJoint RJ;

		public PTarget Targ;

		public AudioSource Aud;

		public PTargetProfile[] Profile_Strong;

		public PTargetProfile[] Profile_Weak;

		public void GoTo(int i)
		{
			Aud.PlayOneShot(Aud.clip, 0.25f);
			RJ.GoToDistance(i);
		}

		public void ResetStrong(int i)
		{
			Aud.PlayOneShot(Aud.clip, 0.25f);
			Targ.ResetTarget(Profile_Strong[i]);
		}

		public void ResetWeak(int i)
		{
			Aud.PlayOneShot(Aud.clip, 0.25f);
			Targ.ResetTarget(Profile_Weak[i]);
		}

		public void ResetNoShatter(int i)
		{
			Aud.PlayOneShot(Aud.clip, 0.25f);
			Targ.ResetTarget(Profile_Strong[i], 1f, decalsOnly: true);
		}
	}
}
