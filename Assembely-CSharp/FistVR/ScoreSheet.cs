using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class ScoreSheet : FVRInteractiveObject
	{
		public PaperTarget PaperTarget;

		public Text ScoreTabulation;

		public override void Poke(FVRViveHand hand)
		{
			base.Poke(hand);
			if (PaperTarget.CurrentShots.Count > 0)
			{
				Vector3 vector = PaperTarget.ClearHolesAndReportScore();
				string text = "\n" + (int)vector.x + " Shots - " + (int)(vector.z * 3.28084f) + " Feet - " + (int)vector.y + " Points";
				ScoreTabulation.text += text;
			}
		}
	}
}
