using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RotrwCharcoalGrillChart : MonoBehaviour
	{
		public List<GameObject> HerbRecipeLines;

		public List<GameObject> HerbRecipeLines_Inverted;

		public List<GameObject> CoreNameLabels;

		public List<GameObject> CoreIntensityLines;

		public List<GameObject> CoreDurationLines;

		public List<GameObject> CoreSpecialLines;

		public RotrwMeatCore rw;

		public void RevealChartElements(RW_Powerup p)
		{
			if (!p.isInverted)
			{
				HerbRecipeLines[(int)p.PowerupType].SetActive(value: true);
			}
			else
			{
				HerbRecipeLines_Inverted[(int)p.PowerupType].SetActive(value: true);
			}
			RotrwMeatCore.CoreType mCMadeWith = p.GetMCMadeWith();
			CoreSpecialLines[(int)mCMadeWith].SetActive(value: true);
			CoreNameLabels[(int)mCMadeWith].SetActive(value: true);
			switch (p.PowerupType)
			{
			case PowerupType.Health:
				CoreIntensityLines[(int)mCMadeWith].SetActive(value: true);
				break;
			case PowerupType.QuadDamage:
				CoreIntensityLines[(int)mCMadeWith].SetActive(value: true);
				CoreDurationLines[(int)mCMadeWith].SetActive(value: true);
				break;
			case PowerupType.InfiniteAmmo:
				CoreDurationLines[(int)mCMadeWith].SetActive(value: true);
				break;
			case PowerupType.Invincibility:
				CoreIntensityLines[(int)mCMadeWith].SetActive(value: true);
				CoreDurationLines[(int)mCMadeWith].SetActive(value: true);
				break;
			case PowerupType.Ghosted:
				CoreDurationLines[(int)mCMadeWith].SetActive(value: true);
				break;
			case PowerupType.FarOutMeat:
				CoreDurationLines[(int)mCMadeWith].SetActive(value: true);
				break;
			case PowerupType.MuscleMeat:
				CoreIntensityLines[(int)mCMadeWith].SetActive(value: true);
				CoreDurationLines[(int)mCMadeWith].SetActive(value: true);
				break;
			case PowerupType.HomeTown:
				break;
			case PowerupType.SnakeEye:
				CoreDurationLines[(int)mCMadeWith].SetActive(value: true);
				break;
			case PowerupType.Blort:
				CoreDurationLines[(int)mCMadeWith].SetActive(value: true);
				break;
			case PowerupType.Regen:
				CoreIntensityLines[(int)mCMadeWith].SetActive(value: true);
				CoreDurationLines[(int)mCMadeWith].SetActive(value: true);
				break;
			case PowerupType.Cyclops:
				CoreDurationLines[(int)mCMadeWith].SetActive(value: true);
				break;
			case PowerupType.WheredIGo:
				break;
			}
		}
	}
}
