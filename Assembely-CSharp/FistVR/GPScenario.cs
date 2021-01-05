using System;
using System.Collections.Generic;

namespace FistVR
{
	[Serializable]
	public class GPScenario
	{
		public string levelname;

		public int versionNumber;

		public string description;

		public SerializableStringDictionary SceneFlags;

		public List<GPSavedPlaceable> SavedPlaceables;
	}
}
