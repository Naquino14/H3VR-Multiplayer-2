using UnityEngine;

namespace FistVR
{
	public class wwFinaleLightTrigger : MonoBehaviour
	{
		public wwFinaleManager Manager;

		public int LightTriggerIndex;

		public AudioSource Aud;

		public bool TriggersNormalPA;

		public bool TriggersSuppresedPA;

		public bool TriggersSilentPA;

		public void Awake()
		{
			Aud = GetComponent<AudioSource>();
		}

		public void OnTriggerEnter(Collider col)
		{
			if (LightTriggerIndex >= 0)
			{
				Manager.SwitchToFinaleLight(LightTriggerIndex);
			}
			else
			{
				Manager.DisableAllFinaleLights();
				Manager.EnableOutdoorLight();
			}
			if (TriggersNormalPA)
			{
				Manager.ParkManager.PASystem.EngageStandardMode();
			}
			else if (TriggersSuppresedPA)
			{
				Manager.ParkManager.PASystem.EngageSuppressedMode();
			}
			else if (TriggersSilentPA)
			{
				Manager.ParkManager.PASystem.EngageSilentMode();
			}
		}
	}
}
