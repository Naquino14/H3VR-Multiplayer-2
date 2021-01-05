using UnityEngine;

namespace Valve.VR
{
	[RequireComponent(typeof(AudioListener))]
	public class SteamVR_Ears : MonoBehaviour
	{
		public SteamVR_Camera vrcam;

		private bool usingSpeakers;

		private Quaternion offset;

		private void OnNewPosesApplied()
		{
			Transform origin = vrcam.origin;
			Quaternion quaternion = ((!(origin != null)) ? Quaternion.identity : origin.rotation);
			base.transform.rotation = quaternion * offset;
		}

		private void OnEnable()
		{
			usingSpeakers = false;
			CVRSettings settings = OpenVR.Settings;
			if (settings != null)
			{
				EVRSettingsError peError = EVRSettingsError.None;
				if (settings.GetBool("steamvr", "usingSpeakers", ref peError))
				{
					usingSpeakers = true;
					float @float = settings.GetFloat("steamvr", "speakersForwardYawOffsetDegrees", ref peError);
					offset = Quaternion.Euler(0f, @float, 0f);
				}
			}
			if (usingSpeakers)
			{
				SteamVR_Events.NewPosesApplied.Listen(OnNewPosesApplied);
			}
		}

		private void OnDisable()
		{
			if (usingSpeakers)
			{
				SteamVR_Events.NewPosesApplied.Remove(OnNewPosesApplied);
			}
		}
	}
}
