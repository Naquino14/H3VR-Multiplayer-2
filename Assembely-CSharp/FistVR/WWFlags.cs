using System;

namespace FistVR
{
	[Serializable]
	public class WWFlags
	{
		public int[] State_TargetPuzzles = new int[9];

		public int[] State_TargetPuzzleSafes = new int[9];

		public int[] State_Bandits = new int[10];

		public int State_NextBanditToSpawn;

		public int[] State_Horseshoes = new int[14];

		public int[] State_EventPuzzles = new int[4];

		public int[] State_Chests = new int[7];

		public int[] State_Keys = new int[7];

		public int[] State_EndDoors = new int[7];

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("State_TargetPuzzles"))
			{
				State_TargetPuzzles = reader.ReadArray<int>("State_TargetPuzzles");
			}
			if (reader.TagExists("State_TargetPuzzleSafes"))
			{
				State_TargetPuzzleSafes = reader.ReadArray<int>("State_TargetPuzzleSafes");
			}
			if (reader.TagExists("State_Bandits"))
			{
				State_Bandits = reader.ReadArray<int>("State_Bandits");
			}
			if (reader.TagExists("State_NextBanditToSpawn"))
			{
				State_NextBanditToSpawn = reader.Read<int>("State_NextBanditToSpawn");
			}
			if (reader.TagExists("State_Horseshoes"))
			{
				State_Horseshoes = reader.ReadArray<int>("State_Horseshoes");
			}
			if (reader.TagExists("State_EventPuzzles"))
			{
				State_EventPuzzles = reader.ReadArray<int>("State_EventPuzzles");
			}
			if (reader.TagExists("State_Chests"))
			{
				State_Chests = reader.ReadArray<int>("State_Chests");
			}
			if (reader.TagExists("State_Keys"))
			{
				State_Keys = reader.ReadArray<int>("State_Keys");
			}
			if (reader.TagExists("State_EndDoors"))
			{
				State_EndDoors = reader.ReadArray<int>("State_EndDoors");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(State_TargetPuzzles, "State_TargetPuzzles");
			writer.Write(State_TargetPuzzleSafes, "State_TargetPuzzleSafes");
			writer.Write(State_Bandits, "State_Bandits");
			writer.Write(State_NextBanditToSpawn, "State_NextBanditToSpawn");
			writer.Write(State_Horseshoes, "State_Horseshoes");
			writer.Write(State_EventPuzzles, "State_EventPuzzles");
			writer.Write(State_Chests, "State_Chests");
			writer.Write(State_Keys, "State_Keys");
			writer.Write(State_EndDoors, "State_EndDoors");
		}
	}
}
