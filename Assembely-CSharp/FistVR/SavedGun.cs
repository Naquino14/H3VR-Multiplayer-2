using System;
using System.Collections.Generic;

namespace FistVR
{
	[Serializable]
	public class SavedGun
	{
		public string FileName;

		public List<SavedGunComponent> Components = new List<SavedGunComponent>();

		public List<FireArmRoundClass> LoadedRoundsInMag = new List<FireArmRoundClass>();

		public List<FireArmRoundClass> LoadedRoundsInChambers = new List<FireArmRoundClass>();

		public List<string> SavedFlags = new List<string>();

		public DateTime DateMade = default(DateTime);
	}
}
