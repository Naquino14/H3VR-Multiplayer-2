using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigLemonManager : MonoBehaviour
	{
		public List<FVRObject> ObjectWrappers;

		public List<ZosigSpawnFromTable> InitialTTSpawns;

		public List<ZosigSpawnFromTable> FinishedTTSpawns;

		public List<ZosigFlagOnItemDetect> BringTTHere;

		public List<ZosigFlagOnItemDetect> DONTBringTTHere;

		public List<Renderer> ScreamInstructions;

		public List<Material> ScreamMats;

		private List<int> indicies = new List<int>
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12
		};

		public void InitLemons()
		{
			indicies.Shuffle();
			indicies.Shuffle();
			indicies.Shuffle();
			indicies.Shuffle();
			for (int i = 0; i < InitialTTSpawns.Count; i++)
			{
				int index = indicies[i];
				InitialTTSpawns[i].Item = ObjectWrappers[index];
			}
			for (int j = 0; j < 6; j++)
			{
				int index2 = indicies[j];
				FinishedTTSpawns[j].Item = ObjectWrappers[index2];
				BringTTHere[j].ObjectsToBeDetected[0] = ObjectWrappers[index2];
				DONTBringTTHere[j].ObjectsToBeDetected.Clear();
				for (int k = 0; k < ObjectWrappers.Count; k++)
				{
					DONTBringTTHere[j].ObjectsToBeDetected.Add(ObjectWrappers[k]);
				}
				DONTBringTTHere[j].ObjectsToBeDetected.Remove(BringTTHere[j].ObjectsToBeDetected[0]);
				DONTBringTTHere[j].ObjectsToBeDetected.TrimExcess();
				BringTTHere[j].RefreshFlagCache();
				DONTBringTTHere[j].RefreshFlagCache();
				ScreamInstructions[j].material = ScreamMats[index2];
			}
		}
	}
}
