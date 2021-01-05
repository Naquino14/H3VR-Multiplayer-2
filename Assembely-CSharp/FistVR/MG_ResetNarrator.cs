using UnityEngine;

namespace FistVR
{
	public class MG_ResetNarrator : MonoBehaviour
	{
		public void Reset()
		{
			GM.Options.MeatGrinderFlags.HasNarratorDoneLongIntro = false;
			GM.Options.MeatGrinderFlags.HasPlayerEverWon = false;
			GM.Options.MeatGrinderFlags.ShortIntroIndex = 0;
			GM.Options.MeatGrinderFlags.SuccessEventVoiceIndex = 0;
		}
	}
}
