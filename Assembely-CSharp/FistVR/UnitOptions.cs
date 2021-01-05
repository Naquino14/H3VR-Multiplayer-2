using System;

namespace FistVR
{
	[Serializable]
	public class UnitOptions
	{
		public enum UnitType
		{
			Imperial,
			Metric,
			Zorgborgs
		}

		public UnitType Units;

		public float FloorHeightOffset;

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("Units"))
			{
				Units = reader.Read<UnitType>("Units");
			}
			if (reader.TagExists("FloorHeightOffset"))
			{
				FloorHeightOffset = reader.Read<float>("FloorHeightOffset");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(Units, "Units");
			writer.Write(FloorHeightOffset, "FloorHeightOffset");
		}
	}
}
